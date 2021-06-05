using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EZCameraShake;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Variables")]
    public float health;
    public float healthRegenRate;
    public float healthDelay;
    public float fallDamage;
    public float fallDamageInAirTimer;
    public static bool takingDamage = false;
    public static bool dead = false;

    public Slider healthbar;

    private float maxHealth;
    private float currentFallDamage = 0;
    private float timeStamp = 0.0f;
    private PlayerMovement movement;
    private Animator playerAnims;
    private Rigidbody rigid;

    // Post processing
    [Header("Post processing")]
    public float pPSpeed;

    // vignette manipulation
    private Color currentColour;
    private float currentIntensity;
    private Vector2 currentOffset;

    // Update is called once per frame
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        movement = GetComponent<PlayerMovement>();
        playerAnims = GetComponentInChildren<Animator>();

        maxHealth = health;
        dead = false;

        //currentColour = ((Color)PPManager.PPInstance.vg.color);
        //currentIntensity = PPManager.PPInstance.vg.intensity.value;

        InvokeRepeating("Regenerate", 0.0f, 1.0f / healthRegenRate);
    }

    void Update()
    {
        if (health <= 0) Die();

        healthbar.value = Mathf.Lerp(healthbar.value, health, 15f * Time.deltaTime);

        if (takingDamage)
        {
            timeStamp = Time.time;
            UpdateEffectsToDamaged();
        }
        else
        {
            UpdateEffectsToRegen();
        }

        FallDamage();

        if (movement.CurrentState == PlayerMovement.PlayerStates.Grounded)
        {
            health = health - currentFallDamage;
            currentFallDamage = 0;
        }
    }
    private void Die()
    {
        dead = true;
        rigid.constraints = RigidbodyConstraints.None;
        movement.enabled = false;
        playerAnims.SetInteger("State", 4);
        playerAnims.enabled = false;

        rigid.AddTorque(transform.right * 15f);

        LevelLoader.loaderInstance.LoadSameLevel();
    }

    private void UpdateEffectsToDamaged()
    {
        
        PPManager.PPInstance.vg.color.Interp(currentColour, Color.red, health/100 * Time.deltaTime * pPSpeed);
        currentColour = (Color)PPManager.PPInstance.vg.color;
        
        PPManager.PPInstance.vg.intensity.Interp(currentIntensity, 0.6f, health/100 * Time.deltaTime * pPSpeed);
        currentIntensity = PPManager.PPInstance.vg.intensity.value;
        

        CameraShaker.Instance.ShakeOnce(0.35f, 0.1f, 1f, 2f);
    }

    private void UpdateEffectsToRegen()
    {
        
        PPManager.PPInstance.vg.color.Interp(currentColour, Color.black, health/100 * Time.deltaTime * pPSpeed);
        currentColour = ((Color)PPManager.PPInstance.vg.color);
        
        PPManager.PPInstance.vg.intensity.Interp(currentIntensity, 0.42f, health/100 * pPSpeed * Time.deltaTime);
        currentIntensity = PPManager.PPInstance.vg.intensity.value;
        
    }

    private void FallDamage()
    {
        if (movement.InAirTimer > fallDamageInAirTimer)
        {
            currentFallDamage = (fallDamage * movement.InAirTimer);
        }
    }

    void Regenerate()
    {
        if (health < maxHealth && Time.time > (timeStamp + healthDelay))
            health += 1.0f;
    }
}
