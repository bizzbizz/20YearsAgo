using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public Transform center;
    public Transform maxReach;
    public Transform[] points;

    SpriteRenderer _renderer;
    public void ShowRenderer(bool val) { _renderer.enabled = val; }

    public bool IsOn { get; private set; }
    public bool IsRecharging { get; private set; }
    /// <summary>
    /// 0 is decharged. 1 is fully charged
    /// </summary>
    public float BatteryRatio { get { return _batteryLevel / MaxBatteryLevel; } }

    float _batteryLevel = 3;
    const float MaxBatteryLevel = 3;
    const float RechargeRate = 1;
    const float ConsumptionRate = 1;
    const float InitialFocusRateX = 12;
    const float InitialFocusRateY = 8;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        IsOn = false;
        IsRecharging = false;
        _batteryLevel = MaxBatteryLevel;
    }

    #region Subroutines
    public void UpdateAim(float holdTime, bool self)
    {
        //inital focusing (aesthetics)
        transform.localScale = new Vector3(Mathf.Clamp(holdTime * InitialFocusRateX, 0, 1), Mathf.Clamp(holdTime * InitialFocusRateY, 0, 1), 1);

        //AIM AT MOUSE POSITION
        if (self)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, -90);
        }
        else
        {
            Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            float rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            if (!SingletonGame.Instance.player.IsFacingRight)
                rotZ += 180;
            transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
        }
    }
    public void UpdateDecharge()
    {
        //WEAPON IS ON
        IsOn = true;
        _batteryLevel -= (Time.deltaTime * ConsumptionRate);

        //check to see if it is FULLY DECHARGED
        if (_batteryLevel <= 0)
        {
            IsRecharging = true;
            _batteryLevel = 0;
        }
    }
    public void UpdateRecharge()
    {
        //WEAPON IS OFF
        IsOn = false;
        _batteryLevel += (Time.deltaTime * RechargeRate);//if IsRechargin can affect rate

        //check to see if it is FULLY RECHARGED
        if (_batteryLevel >= MaxBatteryLevel)
        {
            IsRecharging = false;
            _batteryLevel = MaxBatteryLevel;
        }
    } 
    #endregion

    #region Shader Data
    Vector4 GetVertexCoord(Vector3 pos)
    {
        //var v = cam.WorldToViewportPoint(pos);
        var v = pos;
        return new Vector4(v.x, v.y, 0, 0);
    }
    public Vector4[] GetFlashLightPolygon()
    {
        return new Vector4[]
        {
            GetVertexCoord(points[0].position) + new Vector4(0,0,center.position.x,center.position.y),
            GetVertexCoord(points[1].position) + new Vector4(0,0,GetFlashLightLengthSqrMag(),IsOn?1:0),
            GetVertexCoord(points[2].position),
            GetVertexCoord(points[3].position),
        };
    }
    public float GetFlashLightLengthSqrMag()
    {
        return (center.position - maxReach.position).sqrMagnitude;
    }
    public Vector4 GetFlashLightOrigin()
    {
        return GetVertexCoord(center.position);
    }
    public Vector4 GetFlashLightDirection()
    {
        var deg = Mathf.Clamp(transform.eulerAngles.z, 0, 360);
        var rad = Mathf.Deg2Rad * deg;
        var pos = new Vector4(deg, rad, Mathf.Sin(rad), Mathf.Cos(rad));
        return pos;
    }
    #endregion

}
