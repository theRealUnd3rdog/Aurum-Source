using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Explode : MonoBehaviour
{   
    public float ExplodeDamage;
    public ParticleSystem ExplodeParticles;
    public float Timer = 4f;
    private bool isCollided;
    private PlayerHealth health;
    private AudioSource source;

    void Start()
    {
        source = ExplodeParticles.GetComponentInChildren<AudioSource>();
    }

    void Update()
    {
        Timer -= Time.deltaTime;

        if (Timer <= 0)
        {
            if (isCollided) health.health -= ExplodeDamage;
            ExplodeParticles.transform.SetParent(null);
            ExplodeParticles.Play();
            source.pitch = Random.Range(0.9f, 1.1f);
            source.Play();

            CameraShaker.Instance.ShakeOnce(4f, 1.2f, 2, 4);

            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Player"))
        {
            isCollided = true;
            health = collisionInfo.transform.GetComponent<PlayerHealth>();
        }
    }

    void OnTriggerExit(Collider collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Player"))
        {
            isCollided = false;
        }
    }
}
