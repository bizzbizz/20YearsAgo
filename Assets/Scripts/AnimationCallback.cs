using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator), typeof(Collider))]
public class AnimationCallback : MonoBehaviour {
    public Animator animator { get; private set; }
    public Collider2D collider2d { get; private set; }

    public event UnityAction AnimationCallbackEvent;
    public event UnityAction TriggerCallbackEvent;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        collider2d = GetComponent<Collider2D>();
    }
    public void CallbackMethod()
    {
        if (AnimationCallbackEvent != null)
            AnimationCallbackEvent();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if (TriggerCallbackEvent != null)
                TriggerCallbackEvent();
        }
    }
}
