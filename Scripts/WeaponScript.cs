using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    [SerializeField] public float reloadTime = 2f;
    [SerializeField] public int ammoCap = 30;
    [SerializeField] public int burstCount = 1;
    [SerializeField] public float burstTime = 1f;
    [SerializeField] public float timeBetweenBurst = 1f;
    [SerializeField] public float recoil = 1f;
    [SerializeField] public float force = 1f;
    [SerializeField] public float spread = 1f;
    [SerializeField] public int weaponType = 0;
    [SerializeField] public float damage = 0;
    [SerializeField] public float damageType = 0;
    [SerializeField] public float bulletSpeed = 50;
    [SerializeField] public int penetration = 1;
    [SerializeField] public float angleBurst = 0f;
    [SerializeField] public int projInShot = 1;
    [SerializeField] public int bounceTimes = 0;
    [SerializeField] public int explosionRadius = 0;
    //[SerializeField] PlayerStats stats = null;
    //[SerializeField] public float rotateSpeed = 1f;
    //ParticleSystem weapon;
    [SerializeField] GameObject Bullet;
    [SerializeField] Transform firePoint;
    [ColorUsageAttribute(true,true)] public Color bulletColor;

    public float cooldown;

    float tickRate = 0.5f;
    public int burstNum;
    public float burstTick;
    public int ammoCount;

    public bool noAmmo;

    public float firingtime;

    void Start()
    {
        //weapon = GetComponent<ParticleSystem>();
        //weapon.GetComponent<ParticleSystem>().Stop();
        cooldown = reloadTime;
        ammoCount = ammoCap;
        burstNum = 0;
        burstTick = 0;
        firePoint = GameObject.FindGameObjectWithTag("firepoint").transform;

    }

    public void Check(bool shooting) {
        if (ammoCount > 0) {
            noAmmo = false;
            if (shooting && cooldown >= timeBetweenBurst) {
                cooldown = 0f;
                Fire();
                shooting = false;
                if (burstCount > 1) burstNum = 1;
            } else if (burstNum >= 1) {
                if (burstTick >= burstNum * burstTime){
                    Fire();
                    burstNum += 1;
                } burstTick += tickRate;
                if (burstTick >= burstCount * burstTime) {
                    burstNum = 0;
                    burstTick = 0;
                }
            } else {
                if (cooldown < timeBetweenBurst) {
                    cooldown += tickRate;
                }
            }
        } else {
            noAmmo = true;
            burstNum = 0;
            burstTick = 0;
            Debug.Log("pls reload");
        }
    }

    /*
    public void Check(bool shooting) {
        if (ammoCount > 0) {
            noAmmo = false;
            if (shooting && cooldown >= timeBetweenBurst) {
                cooldown = 0f;
                Fire();
                shooting = false;
                burstNum = 1;
            } else if (burstNum >= 1) {
                if (burstTick >= burstNum * burstTime){
                    Fire();
                    burstNum += 1;
                } else {
                    weapon.GetComponent<ParticleSystem>().Stop();
                } burstTick += tickRate;
                if (burstTick >= burstCount * burstTime) {
                    burstNum = 0;
                    burstTick = 0;
                }
            } else {
                if (cooldown < timeBetweenBurst) {
                    cooldown += tickRate;
                    weapon.GetComponent<ParticleSystem>().Stop();
                }
            }
        } else {
            noAmmo = true;
            burstNum = 0;
            burstTick = 0;
            Debug.Log("pls reload");
        }
    }*/

    public void Reload() {
        ammoCount = ammoCap;
    }

    public void Fire() {
        //Debug.Log("fire");
        CinemachineShake.Instance.ShakeCamera(recoil, 0.1f);
        for (int i = 1; i <= projInShot; i++) {
            var inc = angleBurst/projInShot;
            var angle = -angleBurst/2f + (inc/2) + (inc*(i-1));


            GameObject bullet = Instantiate(Bullet);

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            bullet.GetComponent<BulletParticle>().SetWeapon(damage, force, penetration, bulletColor, bounceTimes, bulletSpeed);
            Quaternion spreadFire = firePoint.rotation * Quaternion.Euler(0,Random.Range(-5f*spread, 5f*spread),0)*Quaternion.Euler(0,angle,0);

            bullet.transform.position = firePoint.transform.position;
            bullet.transform.rotation = spreadFire;

            rb.AddForce(  bullet.transform.forward* bulletSpeed, ForceMode.Impulse);
            //bullet.GetComponent<BulletParticle>().Fire(Player.instance.rb.velocity + firePoint.forward* bulletSpeed);
        }
        EnemySpawner.instance.Alert(transform.parent.parent.position, 25f);
        ammoCount--;
        Player.instance.Recoil(recoil);
    }

    /*
    public void Fire(Player airship) {
        weapon.GetComponent<ParticleSystem>().Play();
        airship.Recoil(weapon.transform, force);
        CinemachineShake.Instance.ShakeCamera(force/50, .1f);
    }*/
}
