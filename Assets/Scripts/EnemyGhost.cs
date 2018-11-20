using UnityEngine;

[RequireComponent(typeof(VisualEffects))]
public class EnemyGhost : TriggerTarget
{
    public enum GhostState
    {
        Dormant, Awakened, Chasing, Physical, Countered
    }
    public GhostState State { get; private set; }

    protected VisualEffects _fx;
    public SpriteRenderer Eyes;
    public EnemyAttention Attention;
    public AnimationCallback Hammer;

    const float MaxMovementSpeed = 1f;
    float _movementSpeed;
    const float MovementAcc = 1;

    const float PhysicalDamage = .22f;
    const float MagicalDamage = .005f;
    const float MagicalDamageInterval = .05f;
    const float PhysicalDamageInterval = 1.5f;
    float _lastMagicalHitTime = 0;
    float _lastPhysicalHitTime = 0;

    const float PhysicalAcquisitionRange = 2.4f;

    private void Start()
    {
        _fx = GetComponent<VisualEffects>();
        ChangeToDormant();

        Hammer.AnimationCallbackEvent += ChangeToChasing;
        Hammer.TriggerCallbackEvent += InflictPhysicalDamage;
        Attention.CallbackEvent += ChangeToAwakened;
    }
    public void ChangeToDormant()
    {
        State = GhostState.Dormant;
        Eyes.enabled = false;
        Hammer.gameObject.SetActive(false);
        Attention.gameObject.SetActive(false);
    }
    public void ChangeToAwakened()
    {
        State = GhostState.Awakened;
        //Eyes.enabled = true;
        //Attention.gameObject.SetActive(false);
    }
    void ChangeToPhysical()
    {
        State = GhostState.Physical;
        Hammer.gameObject.SetActive(true);
        Hammer.animator.SetTrigger("hit");
    }
    void ChangeToChasing()
    {
        State = GhostState.Chasing;
    }

    #region TriggerTarget
    public override void ActivateTrigger(Transform target)
    {
        isActivated = true;

        _target = target;
        _movementSpeed = 0;
        Attention.gameObject.SetActive(true);

        if (_fx != null)
        {
            _fx.Alpha = 0;
            _fx.AlphaRate = .2f;
        }
    }
    public override void DeactivateTrigger()
    {
        isActivated = false;

        _target = null;
        Eyes.enabled = false;

        if (_fx != null)
        {
            _fx.Alpha = 0;
            _fx.AlphaRate = 0;
        }
    }
    #endregion


    private void Update()
    {
        if (_target == null)
            return;

        switch (State)
        {
            case GhostState.Dormant:
                break;
            case GhostState.Awakened:
                MoveTowardTarget(.3f);
                break;
            case GhostState.Chasing:
                MoveTowardTarget(1f);
                break;
            case GhostState.Physical:
                break;
            case GhostState.Countered:
                break;
            default:
                break;
        }
    }
    private void FixedUpdate()
    {
        if (_target == null)
            return;

        switch (State)
        {
            case GhostState.Dormant:
                return;
            case GhostState.Awakened:
                InflictMagicalDamage();
                break;
            case GhostState.Chasing:
                InflictMagicalDamage();
                break;
            case GhostState.Physical:
                break;
            case GhostState.Countered:
                break;
            default:
                break;
        }
    }

    void MoveTowardTarget(float speedMultiplier)
    {
        //change face direction only when move
        var dx = _target.position - transform.position;
        transform.localScale = new Vector3(dx.x > 0 ? -1 : 1, 1, 1);

        if (Mathf.Abs(dx.x) < PhysicalAcquisitionRange)
        {
            ChangeToPhysical();
        }
        else
        {
            _movementSpeed = Mathf.Clamp(_movementSpeed + Time.deltaTime * MovementAcc, 0, MaxMovementSpeed);
            transform.Translate(dx.normalized * Time.deltaTime * _movementSpeed * speedMultiplier);
        }
    }
    void InflictMagicalDamage()
    {
        bool isLookingIntoTheEyes = SingletonGame.Instance.player.IsFacingRight == (_target.position.x < transform.position.x);

        SingletonGame.Instance.SetHaze(isLookingIntoTheEyes);

        if (isLookingIntoTheEyes)
        {
            if (Time.time > _lastMagicalHitTime + MagicalDamageInterval)
            {
                SingletonGame.Instance.player.Hurt(MagicalDamage);
                _lastMagicalHitTime = Time.time;
            }
        }
    }

    void InflictPhysicalDamage()
    {
        if (Time.time > _lastPhysicalHitTime + PhysicalDamageInterval)
        {
            SingletonGame.Instance.player.Hurt(PhysicalDamage);
            SingletonGame.Instance.player.KnockBack(_target.position.x > transform.position.x);
            _lastPhysicalHitTime = Time.time;
        }
    }
}
