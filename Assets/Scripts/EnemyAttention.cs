using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAttention : MonoBehaviour {

    public event UnityAction CallbackEvent;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if (CallbackEvent != null)
                CallbackEvent();
        }
    }
}
