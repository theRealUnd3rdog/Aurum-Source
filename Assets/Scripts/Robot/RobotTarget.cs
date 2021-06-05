using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class RobotTarget : MonoBehaviour
{
    public enum AttackMode
    {
        none,
        ranged,
        melee,
    }

    public float health = 200f;
    public bool die = false;

    [Header("Checks")]
    [SerializeField] RobotController controller;
    [SerializeField] RobotManager ragdoll;
    [SerializeField] NavMeshAgent robotAgent;

    [Header("Robot Attacks")]
    public AttackMode attackMode;
    public LookAtConstraint armAim;
    public LineRenderer laser;
    public AudioSource laserSource;
    public float laserDamage;
    public float armAimDuration;
    public LayerMask attackMask;
    private Transform laserOrigin;
    private ConstraintSource source;
    private float laserDistance;
    private bool aiming;
    private float currentAim = 0;

    [Header("Parts")]
    public RobotParts[] fragileParts;
    public RobotParts[] normalParts;
    public RobotParts[] bodies;

    // Update is called once per frame
    void Start()
    {
        laserOrigin = laser.GetComponent<Transform>();

        //SetAimTarget();
    }

    void Update()
    {
        if (die || GravityOrb.InAir) 
        {
            Die();
        }

        if (controller.playerDetected && !die && health > 0)
        {
            TurnOnLaser();
            LimitLaserLength();

            if (aiming) return;

            if (armAim.weight < 0.9) StartAiming();
            else aiming = false;
        }
        else
        {
            TurnOffLaser();

            if (aiming) return;

            if (armAim.weight != 0) StopAiming();
            else aiming = false;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        controller.running = false;
        controller.enabled = false;

        ragdoll.RemoveHomeRigid();
        ragdoll.steppers = false;
        ragdoll.inverseKinematics = false;
        ragdoll.ragdoll = true;

        TurnOffLaser();
        armAim.enabled = false;

        robotAgent.enabled = false;
    }

    private void SetAimTarget()
    {
        Transform aimTarget = controller.target.GetChild(0);
        source.sourceTransform = aimTarget;
        source.weight = 1;

        armAim.SetSource(0, source);
    }

    private void LimitLaserLength()
    {
        if (Physics.Raycast(laserOrigin.position, laserOrigin.forward, out RaycastHit hit, Mathf.Infinity, attackMask))
        {
            laserDistance = Vector3.Distance(laserOrigin.position, hit.point);

            laser.SetPosition(1, new Vector3(0, 0, laserDistance));

            Debug.DrawLine(laserOrigin.position, hit.point, Color.green);

            PlayerHealth targetHealth = hit.transform.GetComponent<PlayerHealth>();

            if (targetHealth != null)
            {
                PlayerHealth.takingDamage = true;
                targetHealth.health -= laserDamage;
            }
            else
            {
                PlayerHealth.takingDamage = false;
            }
        }
    }

    private void TurnOnLaser()
    {
        laser.enabled = true;
    }

    public void TurnOffLaser()
    {
        PlayerHealth.takingDamage = false;
        laser.enabled = false;
    }

    private void StartAiming()
    {
        StartCoroutine(Aim(0, 1));
        laserSource.Play();
    }

    private void StopAiming()
    {
        StartCoroutine(Aim(currentAim, 0));
        laserSource.Stop();
    }

    IEnumerator Aim(float initialWeight, float finalWeight)
    {
        aiming = true;

        float timeElapsed = 0;

        do
        {
            timeElapsed += Time.deltaTime;

            float normalizedTime = timeElapsed / armAimDuration;
            
            armAim.weight = Mathf.Lerp(initialWeight, finalWeight, normalizedTime);
            currentAim = armAim.weight;

            yield return null;
        }
        while(timeElapsed < armAimDuration);

        aiming = false;
    }
}
