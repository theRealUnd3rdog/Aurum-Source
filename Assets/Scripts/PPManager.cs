using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PPManager : MonoBehaviour
{
    public static PPManager PPInstance;
    private Volume v;

    // Post processing effects
    [HideInInspector]
    public Bloom b;

    [HideInInspector]
    public Vignette vg;

    [HideInInspector]
    public WhiteBalance wb;

    [HideInInspector]
    public MotionBlur mb;

    void Awake()
    {
        PPInstance = this;
    }

    void Start()
    {
        v = GetComponent<Volume>();
        
        v.profile.TryGet(out b);
        v.profile.TryGet(out vg);
        v.profile.TryGet(out wb);
        v.profile.TryGet(out mb);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
