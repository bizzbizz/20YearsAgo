using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyVase : TriggerTarget
{
    const float Damage = .2f;

    float _warningTime;
    const float MaxWarningTime = 2f;
    const float MaxVibrationX = .1f;

    Vector3 _v;
    const float MaxSpeed = 20;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(this.gameObject);
            SingletonGame.Instance.player.Hurt(Damage);
        }
    }
    public override void ActivateTrigger(Transform target)
    {
        isActivated = true;

        _target = target;
        _warningTime = 0;

        _v = (_target.position - transform.position).normalized;
    }
    public override void DeactivateTrigger()
    {
        isActivated = false;

        _target = null;
    }

    void Start()
    {
    }
    void Update()
    {
        if (_target != null)
        {
            _warningTime += Time.deltaTime;
            if (_warningTime > MaxWarningTime)
            {
                //throw
                transform.Translate(_v * Time.deltaTime * MaxSpeed);
            }
            else
            {
                //warning vibration
                transform.Translate(quake());
            }
        }
    }
    Vector3 quake()
    {
        return new Vector3(
            -(MaxVibrationX * _warningTime / MaxWarningTime) / 2f + Mathf.PingPong(Time.time, MaxVibrationX * _warningTime / MaxWarningTime), 0, 0);
    }

}
