using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DitzelGames.FastIK;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossTarget : MonoBehaviour
{
    [Header("Arrays")]
    public Rigidbody[] RbJoints;
    public FastIKFabric[] IKFabric;


    [Header("Variables")]
    public float Health = 5000;
    public Slider HealthBar;
    public float HealthInterpolation;


    [Header("Actors")]
    private BossController Controller;
    public BossAttacks Attacks;
    public NavMeshAgent Agent;

    public Drill drill;
    

    void Start()
    {
        Controller = GetComponent<BossController>();
    }

    void Update()
    {
        HealthBar.value = Mathf.Lerp(HealthBar.value, Health, HealthInterpolation * Time.deltaTime);
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;

        if (Health <= 0f)
        {
            Die();
            LevelLoader.loaderInstance.LoadNextLevel();
        }
    }

    public void StartKilling()
    {
        Controller.enabled = true;
        Attacks.enabled = true;
        drill.enabled = true;
    }

    private void Die()
    {
        DisableIK();
        Ragdoll();

        Controller.enabled = false;
        Controller.RunSteppers = false;
        Attacks.enabled = false;
        drill.enabled = false;
        Agent.enabled = false;
    }

    private void DisableIK()
    {
        foreach(FastIKFabric i in IKFabric)
        {
            i.enabled = false;
        }
    }

    private void Ragdoll()
    {
        foreach(Rigidbody b in RbJoints)
        {
            b.constraints = RigidbodyConstraints.None;
            b.isKinematic = false;
        }
    }
}
