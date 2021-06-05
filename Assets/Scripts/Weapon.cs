using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public enum weaponState
    {
        Shoot,
        Gravityorb,
        Boomer,
        SlowMo,
    }

    [Header("Weapon Variables")]
    public weaponState state;
    public float damage;
    public float impactForce;
    public static Weapon weaponInstance;

    [SerializeField] float range;
    [SerializeField] float fireRate;
    [SerializeField] float reloadTime = 1f;
    [SerializeField] int maxAmmo;

    [Header("Actors")]
    [SerializeField] Image crossHair;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] cameraRecoil cameraRecoil;
    [SerializeField] Renderer[] mesh;
    [SerializeField] Material[] mat;
    public GameObject impactHole;

    [Header("Gravity Orb")]
    public GameObject GravityOrb;
    private GameObject CurrentOrb;
    public float OrbInterpolationDuration;
    public float OrbCooldown;
    private float OrbTimeStamp;
    private bool OrbLerp;
    

    [Header("Bullet")]
    [SerializeField] Transform bulletPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float bulletSpeed;

    [Header("Audio")]
    [SerializeField] AudioClip shootClip;
    [SerializeField] AudioClip reloadClip;
    [SerializeField] AudioSource shootSource;
    [SerializeField] AudioSource reloadSource;


    //Privates
    private Animator playerAnims;
    private int currentAmmo;
    private int weaponType = 0;
    private bool isReloading = false;
    private float nextTimeTofire = 0f;
    private RaycastHit hit;
    private proceduralRecoil recoil;

    void Awake() 
    {
        playerAnims = transform.root.GetChild(0).GetComponent<Animator>();
    }

    void Start()
    {
        currentAmmo = maxAmmo;
        weaponInstance = this;

        recoil = GetComponent<proceduralRecoil>();
        StoreAudio();

        playerAnims.SetBool("WeaponSpawn", true);
    }

    void OnEnable() 
    {
        isReloading = false;
    }

    void Update() 
    {
        if (currentAmmo > 0)
            isReloading = false;

        if (isReloading)
            return;

        if (currentAmmo <= 0) 
        {
            StartCoroutine(Reload());
            return;
        }

        /*
        if (weaponType == 0)
        {
            state = weaponState.Shoot;
            SetMaterials(mat[0]);
        }

        if (weaponType == 1)
        {
            state = weaponState.Gravityorb;
            SetMaterials(mat[1]);
        }

        if (weaponType == 2)
        {
            state = weaponState.Boomer;
            SetMaterials(mat[2]);
        }

        if (weaponType == 3)
        {
            state = weaponState.SlowMo;
            SetMaterials(mat[3]);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            weaponType++;

            if (weaponType > 3) weaponType = 0;
        }*/


        if (Input.GetButton("Fire1") && !PlayerHealth.dead)
        {
            /*
            // Gravity orb
            if (state == weaponState.Gravityorb && OrbTimeStamp <= Time.time)
            {
                OrbTimeStamp = Time.time + OrbCooldown;
                GravityPull();

                recoil.Recoil();
                cameraRecoil.camRecoil();
            } */

            // Ordinary shoot
            Debug.Log(Time.time);
            
            if (/*state == weaponState.Shoot &&*/ Time.time >= nextTimeTofire)  //&& OrbTimeStamp/2 <= Time.time)
            {
                nextTimeTofire = Time.time + 1f / fireRate;
                Shoot();
                recoil.Recoil();
                cameraRecoil.camRecoil();
            } 


        }
    }
    
    void OnDisable()
    {
        OrbTimeStamp = 0;
    }

    public void SetMaterials(Material newMat)
    {
        for (int i = 0; i < mesh.Length; ++i)
        {
            Material[] materials = new Material[mesh[i].materials.Length];

            for (int j = 0; j < materials.Length; ++j)
            {
                if (mesh[i].materials[j].name == "Glass (Instance)")
                {
                    Material oldMat = mesh[i].materials[j];
                    materials[j] = oldMat;
                }

                if (mesh[i].materials[j].name == "Black (Instance)")
                {
                    Material oldMat = mesh[i].materials[j];
                    materials[j] = oldMat;
                }

                else if (mesh[i].materials[j].name == "ShootLight (Instance)") materials[j] = newMat;
                else if (mesh[i].materials[j].name == "GravityOrb (Instance)") materials[j] = newMat;
                else if (mesh[i].materials[j].name == "GunTip (Instance)") materials[j] = newMat;
                else if (mesh[i].materials[j].name == "BulletGlow (Instance)") materials[j] = newMat;
            }

            mesh[i].materials = materials;
        }
    }

    private void Shoot() 
    {
        Debug.Log(currentAmmo);
        currentAmmo--;

        muzzleFlash.Play();
        shootSource.Play();

        Ray ray = Camera.main.ScreenPointToRay(crossHair.transform.position);
        
        
        if (Physics.Raycast(ray, out hit, range)) 
        {
            
            Debug.Log(hit.transform.name);

            RobotParts enemyParts = hit.transform.GetComponent<RobotParts>();
            BossParts bossParts = hit.transform.GetComponent<BossParts>();
            MeshDestroy destroyableMesh = hit.transform.GetComponent<MeshDestroy>();

            if (hit.rigidbody != null)
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            
            
            if (enemyParts != null) 
            {
                enemyParts.PartChecks(damage);
            }

            if (bossParts != null)
            {
                bossParts.Manager.TakeDamage(damage);
            }

            if (destroyableMesh != null)
            {
                destroyableMesh.DestroyMesh();
            } 

            

            GameObject impact = Instantiate(impactHole, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, 3.5f);

            /*
            GameObject currentBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
            Rigidbody bulletRb = currentBullet.GetComponentInChildren<Rigidbody>();
            Vector3 bulletDir = hit.point - bulletPos.position;

            bulletRb.AddForce(bulletSpeed * bulletDir * Time.deltaTime, ForceMode.Impulse);*/
        }
    }

    private void GravityPull()
    {
        muzzleFlash.Play();
        shootSource.Play();

        Ray ray = Camera.main.ScreenPointToRay(crossHair.transform.position);

        if (Physics.Raycast(ray, out hit, range))
        {
            GameObject orb = Instantiate(GravityOrb, bulletPos.position, Quaternion.identity);
            CurrentOrb = orb;
        }

        if (CurrentOrb != null)
        {
            StartCoroutine(OrbInterpolation());
        }
    }

    IEnumerator OrbInterpolation()
    {
        OrbLerp = true;

        float timeElapsed = 0;

        do
        {
            timeElapsed += Time.deltaTime;

            float normalizedTime = timeElapsed / OrbInterpolationDuration;
            Vector3 direction = hit.point - transform.position;

            CurrentOrb.transform.position = Vector3.Lerp(CurrentOrb.transform.position, hit.point - (direction.normalized * 3f), normalizedTime);

            yield return null;
        }
        while (timeElapsed < OrbInterpolationDuration);

        OrbLerp = false;
    }

    IEnumerator Reload() 
    {
        isReloading = true;
        Debug.Log("Reloading");

        playerAnims.SetBool("Reloading", true);
        yield return new WaitForSeconds(reloadTime - 0.25f);

        yield return new WaitForSeconds(0.25f);

        playerAnims.SetBool("Reloading", false);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    private void StoreAudio() 
    {
        shootSource.clip = shootClip;
        //reloadSource.clip = reloadClip;
    }
}
