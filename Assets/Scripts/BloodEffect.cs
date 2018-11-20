using UnityEngine;
public class BloodEffect : MonoBehaviour
{
    SpriteRenderer _rend;
    public float _Value, HoleSmooth;
    float _TimeX;
    public Texture textureSource;

    void Start()
    {
        _rend = GetComponent<SpriteRenderer>();
        UpdatePropertyBlock();
    }

    void Update()
    {
        UpdatePropertyBlock();
    }

    void UpdatePropertyBlock()
    {
        var mp = new MaterialPropertyBlock();
        mp.SetTexture("_MainTex", textureSource);

        _TimeX += Time.deltaTime;
        if (_TimeX > 100) _TimeX = 0;
        mp.SetFloat("_TimeX", _TimeX);
        mp.SetFloat("_Value", _Value);
        mp.SetFloat("_Value2", HoleSmooth);

        //mp.SetVector("_NScale", NoiseScale);
        //mp.SetVector("_NOffset", NoiseOffset);
        _rend.SetPropertyBlock(mp);
    }
}
