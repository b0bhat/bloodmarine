using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParticle : MonoBehaviour
{
    private float damage = 0f;
    private float force = 0f;
    private int penetration;
    private int bounceTimes;
    private int curBounce;
    private float bulletSpeed;
    //private float piercing = 1f;

    //public float piercecount = 0;

    //public new ParticleSystem particleSystem;
    bool isBounce = false;

    public GameObject hit;
    Rigidbody rb;
    public Vector3 fireDir = Vector3.zero;
    //List<ParticleCollisionEvent> colEvents = new List<ParticleCollisionEvent>();
    public int weaponType;
    public int alreadyPen;
    Color bulletColor;
    [SerializeField] GameObject capsule;
    Vector3 lastVel;

    List<GameObject> alreadyHit = new List<GameObject>();

    void Awake() {
        rb = this.GetComponent<Rigidbody>();
        alreadyPen = 0;
    }

    public void SetWeaponType(int type) {
        weaponType = type;
    }

    public void SetWeapon(float dmg, float bulletForce, int pen, Color color, int bounce, float speed) {
        damage = dmg;
        force = bulletForce;
        penetration = pen;
        bulletColor = color;
        bulletSpeed = speed;

        bounceTimes = bounce;
        curBounce = bounce;

        var originalMat =  capsule.gameObject.GetComponent<MeshRenderer>().material;
        Material mat = Instantiate(originalMat as Material);
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", bulletColor);
        capsule.gameObject.GetComponent<MeshRenderer>().material = mat;

        //piercing = prc;
    }
    void OnCollisionEnter(Collision collision) { // for non-enemy objects
        if (curBounce >= 1 || isBounce) {
            Vector3 refPos = Vector3.Reflect(transform.forward, collision.GetContact(0).normal);
            //Debug.DrawRay(transform.position, refPos, Color.green, 10f);
            var dir = ((refPos.normalized) * bulletSpeed);
            float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
            //fireDir = rb.velocity.normalized;
            //rb.MoveRotation(Quaternion.Euler(0f,angle,0f));
            this.gameObject.transform.eulerAngles = new Vector3(dir.x, 0, dir.z);
            //rb.angularVelocity = Vector3.zero;
            //rb.AddForce(collision.contacts[0].normal * bulletSpeed*10f);

            //var dir = Vector3.Reflect(lastVel.normalized, collision.contacts[0].normal);
            //rb.velocity = dir * Mathf.Max(lastVel.magnitude, 0f);

            isBounce = true;
            Invoke("StopBounce", 0.0f);
            curBounce--;
        } else {
            Instantiate(hit, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    void StopBounce() {
        isBounce = false;
    }

    public void Fire(Vector3 dir) {
        fireDir = dir;
    }

    void FixedUpdate() {
        this.transform.Translate(fireDir);
        this.transform.rotation = Quaternion.LookRotation(rb.velocity.normalized);
        //Debug.DrawRay(transform.position, rb.velocity.normalized, Color.green, 1f);
    }

    void Update() {
        lastVel = rb.velocity;
    }


    void OnTriggerEnter(Collider collider) { // for enemy - nonignore
        Instantiate(hit, transform.position, transform.rotation);
        GameObject other = collider.gameObject.transform.parent.gameObject;
        if(other.TryGetComponent(out Enemy enemy)) {
            if (!alreadyHit.Contains(other)) {
                alreadyPen++;
                alreadyHit.Add(other);
                enemy.TakeDamage(damage*Random.Range(0.8f,1.2f), rb.velocity * 0.2f*force);
                if (alreadyPen >= penetration) {
                    Destroy(gameObject);
                }
            }
        }
        //
    }

    /*
    private void OnParticleCollision(GameObject other) {
        ParticleSystem.Particle[] par = new ParticleSystem.Particle[particleSystem.main.maxParticles];
        int events = particleSystem.GetCollisionEvents(other, colEvents);
        //Debug.Log("hit");
        for (int i = 0; i < events; i++){
            //piercecount++;
            Instantiate(hit, colEvents[i].intersection, Quaternion.LookRotation(colEvents[i].normal));
        }

        if(other.TryGetComponent(out Enemy enemy)) {
            enemy.TakeDamage(damage*Random.Range(0.8f,1.2f));
        }
    }*/
}
