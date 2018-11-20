using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TriggerTarget : MonoBehaviour
{
    protected Transform _target;
    internal bool isActivated;

    public abstract void ActivateTrigger(Transform target);

    public abstract void DeactivateTrigger();
}
