using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttacks : MonoBehaviour
{
    [Header("Actors")]
    private BossController Controller;
    private float TimerForAttacks = 5;
    private int AttackMode = 0;

    [Header("Laser")]
    public LineRenderer LaserLine;
    public AudioSource LaserSource;
    public ParticleSystem LaserBeam;
    public LayerMask AttackMask;
    private Transform LaserOrigin;
    public float LaserDamage;
    public float LaserDuration;
    public float LaserCooldown;

    [Header("Explodee Attack")]
    public Transform ExplodeInstantiationPos;
    public AudioSource InstSource;
    public GameObject ExplodeeOrb;
    public ParticleSystem InstantiationParticles;
    public float ExplodeeFireRate;
    public float ExplodeeCooldown;
    private float NextTimeTofire;
    private Vector3 Hitpoint;
    private GameObject CurrentOrb;
    private bool OrbInterp = false;
    

    void Start()
    {
        LaserOrigin = LaserLine.GetComponent<Transform>();
        Controller = GetComponent<BossController>();
    }

    void Update() 
    {
        LaserAttack();
        TimerForAttacks += Time.deltaTime;

        if (TimerForAttacks >= 10)
        {
            SwtichAttacks();
            TimerForAttacks = 0;
        }

        if (AttackMode == 0)
        {
            LaserAttack();
            LaserSource.Play();
        }

        if (AttackMode == 1)
        {
            LaserLine.enabled = false;
            LaserSource.Stop();

            if (Time.time >= NextTimeTofire)
            {
                NextTimeTofire = Time.time + 1f / ExplodeeFireRate;
                ExplodeeAttack();
                InstSource.Play();

                if(OrbInterp) return;
                StartCoroutine(OrbInterpolation());
            }
        }

    }

    private void SwtichAttacks()
    {
        switch(AttackMode)
        {
            case 0:
                AttackMode++;
                
                break;

            case 1:
                
                AttackMode--;

                break;
        }
    }

    public void LaserAttack()
    {
        LaserLine.enabled = true;

        if (Physics.Raycast(LaserOrigin.position, LaserOrigin.forward, out RaycastHit hit, Mathf.Infinity, AttackMask))
        {
            float LaserDistance = Vector3.Distance(LaserOrigin.position, hit.point);
            LaserLine.SetPosition(1, new Vector3(0, 0, LaserDistance));

            // Damage player
            PlayerHealth targetHealth = hit.transform.GetComponent<PlayerHealth>();
            Debug.DrawLine(LaserOrigin.position, hit.point, Color.green);

            if (targetHealth != null)
            {
                PlayerHealth.takingDamage = true;
                targetHealth.health -= LaserDamage;
            }
            else
            {
                PlayerHealth.takingDamage = false;
            }
        }
    }

    public void ExplodeeAttack()
    {
        InstantiationParticles.Play();

        if (Physics.Raycast(ExplodeInstantiationPos.position, ExplodeInstantiationPos.forward, out RaycastHit hit, Mathf.Infinity, AttackMask))
        {
            Hitpoint = hit.point;
            CurrentOrb = Instantiate(ExplodeeOrb, ExplodeInstantiationPos.position, Quaternion.identity);
        }
    }

    IEnumerator OrbInterpolation()
    {
        OrbInterp = true;

        float timeElapsed = 0;

        do
        {
            timeElapsed += Time.deltaTime;

            float normalizedTime = timeElapsed / 0.3f;
            Vector3 direction = Hitpoint - transform.position;

            CurrentOrb.transform.position = Vector3.Lerp(CurrentOrb.transform.position, Hitpoint - (direction.normalized * 3f), normalizedTime);

            yield return null;
        }
        while (timeElapsed < 0.3f);

        OrbInterp = false;
    }
}
