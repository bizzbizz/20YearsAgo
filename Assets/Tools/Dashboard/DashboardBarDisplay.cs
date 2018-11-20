using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashboardBarDisplay : MonoBehaviour
{
    public UnityEngine.UI.Image HPUIObject;
    public UnityEngine.UI.Image BatteryUIObject;
    public GameObject GameoverObject;

    public Gradient _gradient;
    public Color _chargedColor;
    public Color _dechargedColor;

    void Start()
    {
        GameoverObject.SetActive(false);
    }

    void FixedUpdate()
    {
        HPUIObject.fillAmount = SingletonGame.Instance.player.HP;
        HPUIObject.color = _gradient.Evaluate(SingletonGame.Instance.player.HP);

        if (SingletonGame.Instance.player.HP <= 0)
            GameoverObject.SetActive(true);

        BatteryUIObject.fillAmount = SingletonGame.Instance.player.Weapon.BatteryRatio;
        BatteryUIObject.color = SingletonGame.Instance.player.Weapon.IsRecharging ? _dechargedColor : _chargedColor;
    }
}
