using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public Flashlight Weapon;
    public float movementSpeed;
    public float movementAcc;
    public Transform Body;
    public Transform RegularAimPoint;
    public Transform SelfAimPoint;
    public Flashlight WeaponPrefab;

    //private Rigidbody2D _rigidbody;

    float _targetPositionX;
    bool _isMoving;
    bool _isHasting;
    bool _isStopping;
    bool _isMovingRight;
    public bool IsFacingRight { get { return Body.localScale.x > 0; } }

    float _currentAbsVx = 0;
    const float DeccDx = .8f;//dx at which player deccelerates when reaching the target
    const float TurnDx = .6f;//dx at which player only turns to face the target when ordered to move
    const float SelfDx = .25f;//dx at which player only stops and does not turn when ordered to move
    //const float AccTime = 1;//time after which player accelerates when ordered to move 
    bool _isClicked = false;
    float _clickTime;
    const float ClickDetectionTime = 0.33f;
    float _holdTime { get { return Time.time - _clickTime - ClickDetectionTime; } }

    /// <summary>
    /// 0..1
    /// </summary>
    public float HP { get; set; }

    public void Hurt(float damage)
    {
        HP -= damage;
        SingletonGame.Instance.SetBloodiness(1 - HP);
        //if (HP <= 0)
        //{
        //    UnityEngine.Debug.Log("GAME OVER");
        //}
    }

    bool _isKnockBack = false;
    float _knockBack = 0;
    bool _isKnockBackRight = false;
    const float KnockBackMax = 5f;
    const float KnockBackDecayRate = 7f;
    public void KnockBack(bool right)
    {
        _isKnockBack = true;
        _isKnockBackRight = right;
        _knockBack = KnockBackMax;
    }

    void Awake()
    {
        //_rigidbody = GetComponent<Rigidbody2D>();
        Weapon = GameObject.Instantiate<Flashlight>(WeaponPrefab);
        Weapon.transform.parent = Body;
        Weapon.transform.localPosition = RegularAimPoint.localPosition;
        Weapon.ShowRenderer(false);
        HP = 1;
    }
    public GameObject ball;
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Weapon.ShowRenderer(false);
            //CLICK
            _isClicked = true;
            _clickTime = Time.time;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Weapon.ShowRenderer(false);
            if (_isClicked)
            {
                //RELEASE
                if (_holdTime < 0)
                {
                    _targetPositionX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
                    float dx = _targetPositionX - transform.position.x;
                    if (!_isMoving || ((dx > 0) != _isMovingRight))
                    {
                        //reset speed when stationary or moving in opposite direction
                        _currentAbsVx = 0;
                        _isHasting = false;
                    }

                    //order player to turn/stop/move
                    float dist = Mathf.Abs(dx);
                    if (dist < SelfDx)
                    {
                        //stop
                        _isMoving = false;
                        _isHasting = false;
                    }
                    else
                    {
                        //turn and face the target
                        _isMovingRight = dx > 0;
                        Body.localScale = new Vector3(_isMovingRight ? 1 : -1, 1, 1);

                        if (dist < TurnDx)
                        {
                            //(turn and) stop
                            _isMoving = false;
                            _isHasting = false;
                        }
                        else
                        {
                            //(turn and) move
                            _isHasting = _isMoving;
                            _isMoving = true;
                            _isStopping = false;
                        }
                    }
                }
                _isClicked = false;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (_holdTime >= 0)
            {
                Weapon.ShowRenderer(true);

                //HOLD
                if (Weapon.IsRecharging)
                    //WEAPON IS OFF
                    Weapon.UpdateRecharge();
                else
                    //WEAPON IS ON
                    Weapon.UpdateDecharge();

                //MOVE FLASHLIGHT AROUND
                bool self = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).ManhattanSize2D() < 1;
                Weapon.transform.localPosition = self ? SelfAimPoint.localPosition : RegularAimPoint.localPosition;
                //AIM FLASHLIGHT
                Weapon.UpdateAim(_holdTime, self);
            }
            else
                Weapon.ShowRenderer(false);
        }
        else
        {
            Weapon.ShowRenderer(false);

            //WEAPON IS OFF
            Weapon.UpdateRecharge();
        }
    }
    void FixedUpdate()
    {
        if (_isMoving)
        {
            float dx = _targetPositionX - transform.position.x;
            float dist = Mathf.Abs(dx);

            //if (_holdTime < AccTime)
            //{
            //    //Accelerate
            //    _currentVx += Time.fixedDeltaTime * movementAcc;
            //}else
            if (_isStopping || dist < DeccDx)
            {
                //Deccelerate
                if (_isHasting)
                    _currentAbsVx -= (Time.fixedDeltaTime * movementAcc * 2);
                else
                    _currentAbsVx -= (Time.fixedDeltaTime * movementAcc);

                _isStopping = true;
                if (_currentAbsVx <= 0)
                {
                    //STOP
                    _isMoving = false;
                    _isStopping = false;
                }
            }
            else if (_currentAbsVx < movementSpeed)
            {
                //Accelerate
                //move at constant speed
                if (_isHasting)
                    _currentAbsVx += (Time.fixedDeltaTime * movementAcc * 2);
                else
                    _currentAbsVx += (Time.fixedDeltaTime * movementAcc);
            }

            _currentAbsVx = Mathf.Clamp(_currentAbsVx, 0, movementSpeed);

            float v = _currentAbsVx * Time.fixedDeltaTime;
            if (_isHasting) v *= 2;

            if (_isMovingRight)
                //move right
                transform.Translate(new Vector3(v, 0));
            else
                //move left
                transform.Translate(new Vector3(-v, 0));
        }

        if (_isKnockBack)
        {
            transform.Translate(new Vector3(Time.fixedDeltaTime * (_isKnockBackRight ? _knockBack : -_knockBack), 0, 0));
            _knockBack -= KnockBackDecayRate * Time.fixedDeltaTime;
            if (_knockBack < 0)
                _isKnockBack = false;
        }
    }
}
