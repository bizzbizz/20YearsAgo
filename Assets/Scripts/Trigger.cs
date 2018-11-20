using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Trigger : MonoBehaviour
{
    public TriggerTarget target;


    public void ResetTrigger()
    {
        if (target != null)
            if (target.isActivated)
                target.DeactivateTrigger();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!target.isActivated && collision.gameObject.tag == "Player")
        {
            target.ActivateTrigger(collision.transform);
            gameObject.SetActive(false);
        }
    }

    void Start()
    {

    }

    void FixedUpdate()
    {

    }
}
