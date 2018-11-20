using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
    public Trigger[] Triggers { get; private set; }
	void Awake () {
        Triggers = GetComponentsInChildren<Trigger>();
	}
    private void Start()
    {
        foreach (var trig in Triggers)
        {
            trig.ResetTrigger();
        }
    }

    void Update () {

    }
}
