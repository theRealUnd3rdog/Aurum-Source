using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class GravityOrb : MonoBehaviour
{
    public static bool InAir;

    [Header("Attraction variables")]
    public float attractionForce = 1000f;
    public float attractionRange = 200f;
    public float orbDuration = 9f;
    public float orbEffectInterpolation = 3f;
    private VisualEffect orbEffects;
    private float trailSpawnRate;
    private float trailLifeTime;
    private float size;
    private float spawnRate;

    // Update is called once per frame
    void Start()
    {
        orbEffects = GetComponentInChildren<VisualEffect>();

        trailSpawnRate = orbEffects.GetFloat("TrailSpawnRate");
        trailLifeTime = orbEffects.GetFloat("TrailLifeTime");
        size = orbEffects.GetFloat("Size");
        spawnRate = orbEffects.GetFloat("SpawnRate");
    }

    void Update()
    {
        InterpolateEffectsExpansion();
    }
    void FixedUpdate()
    {
        InAir = true;

        foreach(pickUp attractor in pickUp.Attractors)
        {
            Pull(attractor);
        }

        if (pickUp.Attractors == null) return;

        Destroy(this.gameObject, orbDuration);
    }

    void OnDestroy() 
    {
        InAir = false;
    }

    void Pull(pickUp objToAttract)
    {
        Rigidbody rbToAttract = objToAttract.rb;

        Vector3  direction = transform.position - rbToAttract.position;
        float distance = direction.magnitude;
        Vector3 force = direction.normalized * attractionForce;

        if (distance <= attractionRange) rbToAttract.AddForce(force);
    }

    void InterpolateEffectsExpansion()
    {
        orbEffects.SetFloat("TrailSpawnRate", Mathf.Lerp(trailSpawnRate, 100f, orbEffectInterpolation * Time.deltaTime));

        orbEffects.SetFloat("TrailLifeTime", Mathf.Lerp(trailLifeTime, 20f, orbEffectInterpolation * Time.deltaTime));

        orbEffects.SetFloat("SpawnRate", Mathf.Lerp(spawnRate, 3000, orbEffectInterpolation * Time.deltaTime));

        orbEffects.SetFloat("Size", Mathf.Lerp(size, 3f, orbEffectInterpolation * Time.deltaTime));
    }
}
