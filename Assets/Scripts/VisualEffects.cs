using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(SpriteRenderer))]
public class VisualEffects : MonoBehaviour
{
    private SpriteRenderer _rend;

    public Texture textureSource;

    public float AnimSpeed;
    public Vector3 NoiseOffset;
    public Vector4 NoiseScale = new Vector4(1f, .2f, .8f, .6f);

    public bool isGhost;
    public float Alpha { get; set; }//0..1
    public float AlphaRate { get; set; }//rate of change from 0 to 1

    const float AlphaScale = 0.5f;//only affects the shader
    public static bool Haze = false;
    public static bool Mist = true;
    private float _TimeX = 0f;

    void Start()
    {
        _rend = GetComponent<SpriteRenderer>();
        UpdatePropertyBlock();
    }

    void Update()
    {
        Alpha = Mathf.Clamp01(Alpha + AlphaRate * Time.deltaTime);

        _rend.material.SetVectorArray("_Light", SingletonGame.Instance.player.Weapon.GetFlashLightPolygon());

        NoiseOffset.x += (AnimSpeed * Time.deltaTime);
        NoiseOffset.z += (AnimSpeed * Time.deltaTime);
        NoiseOffset = Quaternion.Euler(new Vector3(0.0f, AnimSpeed * Time.deltaTime, 0.0f)) * NoiseOffset;

        UpdatePropertyBlock();
    }

    void UpdatePropertyBlock()
    {
        var mp = new MaterialPropertyBlock();
        mp.SetTexture("_MainTex", textureSource);
        mp.SetFloat("_IsHazy", Haze ? 1f : 0f);
        mp.SetFloat("_Mist", Mist ? 1f : 0f);

        if (!Haze)
            _TimeX = 0f;
        if (isGhost || Haze)
        {
            mp.SetFloat("_IsGhost", isGhost ? 1f : 0f);
            mp.SetFloat("_Alpha", Alpha * AlphaScale);
            _TimeX += Time.deltaTime;
            if (_TimeX > 80) _TimeX = 0f;
            mp.SetFloat("_TimeX", _TimeX);
        }

        mp.SetVector("_NScale", NoiseScale);
        mp.SetVector("_NOffset", NoiseOffset);
        _rend.SetPropertyBlock(mp);
    }
}
