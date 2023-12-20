using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private enum EnemyState {
        Charge,
        Melee,
        Heal,
        Shoot,
        Retreat,
    }

    [SerializeField] float slowspeed = 2f;
    [SerializeField] float fastspeed = 10f;
    [SerializeField] float fullHealth = 30;
    [SerializeField] float detectRange = 20f;
    [SerializeField] EnemyState enemyState;

    private Transform target;
    EnemySpawner enemySpawner;
    bool alert = false;

    public float health;
    public bool charging = false;

    Vector3 walk;
    Rigidbody rb;
    private float bloodLow;

    NavMeshAgent navAgent;
    GameObject bloodHolder;

    [SerializeField] GameObject Capsule;
    [SerializeField] GameObject DeathPrefab;
    [SerializeField] GameObject BloodPrefab;
    [SerializeField] Material flashMat;
    [SerializeField] Material origMat;



    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        target = Player.instance.transform;
        enemySpawner = EnemySpawner.instance;
        walk = enemySpawner.RandomNavmeshLocation(8f,transform.position);
        navAgent.SetDestination(walk);
        health = fullHealth;
        rb = gameObject.GetComponent<Rigidbody>();

        navAgent.updatePosition = false;
        bloodHolder = GameObject.FindWithTag("bloodholder");

    }

    // Update is called once per frame
    void Update()
    {
        if (enemyState == EnemyState.Charge) {
            Charge();
        } else if (enemyState == EnemyState.Melee) {
            Melee();
        }

        if (health <= 0) {
            Die();
        }
    }

    void Charge() {
        //Debug.DrawLine(transform.position, target.position, Color.green, 3.5f);
        if (!Physics.Linecast(transform.position, target.position) && alert && !charging) {
            Vector3 chargeDir = (target.position - transform.position).normalized;
            StartCoroutine(ChargeAttack(chargeDir));
        }
        else if (Vector3.Distance(transform.position, target.position) < detectRange+5 && !charging) {
            if (!alert) {
                if (Vector3.Distance(transform.position, target.position) < detectRange) {
                    if (CheckLength(target.position) < detectRange) alert = true;
                } else {
                    alert = false;
                }
            }
            if (alert) {
                walk = Vector3.zero;
                navAgent.SetDestination(target.position);
                navAgent.speed = fastspeed;
            } else if (Vector3.Distance(walk, transform.position) < 2f) {
                walk = enemySpawner.RandomNavmeshLocation(8f, transform.position);
                navAgent.SetDestination(walk);
                navAgent.speed = slowspeed;
            }
        }
    }

    IEnumerator ChargeAttack(Vector3 chargeDir) {
        charging = true;
        rb.velocity = (fastspeed * 150 * chargeDir * Time.deltaTime);
        yield return new WaitForSeconds(1f);
        charging = false;
        yield return null;
    }

    void Melee() {
        if (Vector3.Distance(transform.position, target.position) < detectRange+5) {
            if (!alert) {
                if (Vector3.Distance(transform.position, target.position) < detectRange) {
                    if (CheckLength(target.position) < detectRange) alert = true;
                } else {
                    alert = false;
                }
            }
            if (alert) {
                walk = Vector3.zero;
                navAgent.SetDestination(target.position);
                navAgent.speed = fastspeed;
            } else if (Vector3.Distance(walk, transform.position) < 2f) {
                walk = enemySpawner.RandomNavmeshLocation(8f, transform.position);
                navAgent.SetDestination(walk);
                navAgent.speed = slowspeed;
            }
        }
    }

    void FixedUpdate() {
        if (!charging) {
            navAgent.nextPosition = transform.position;
            rb.AddForce(navAgent.desiredVelocity);
        }
    }

    public float CheckLength(Vector3 otherPos) {
        float distance = 0f;
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(transform.position, otherPos, navAgent.areaMask, path)){
            distance += Vector3.Distance(transform.position, path.corners[0]);
            //Debug.DrawLine(transform.position, path.corners[0]);
            for (int i = 1; i < path.corners.Length; i++) {
                distance += Vector3.Distance(path.corners[i-1], path.corners[i]);
                //Debug.DrawLine(path.corners[i-1], path.corners[i]);
            }
            return distance;
        } else {
            return 1000f;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 11) {
            if (Mathf.Abs(other.attachedRigidbody.velocity.magnitude) > 8f) {
                 Hurt(Mathf.Abs(other.attachedRigidbody.velocity.magnitude));
            }
        }
    }

    public void SoundAlert(Vector3 soundPos, float range) {
        if (CheckLength(soundPos) < 25) alert = true;
    }

    public void TakeDamage(float damage, Vector3 force) {
        //navAgent.enabled = false;
        rb.AddForce(force, ForceMode.Impulse);
        if (!alert) EnemySpawner.instance.Alert(transform.position, 8f);
        Hurt(damage);
    }

    IEnumerator DamageFlash() {
        Capsule.GetComponent<MeshRenderer>().material = flashMat;
        yield return new WaitForSeconds(0.1f);
        Capsule.GetComponent<MeshRenderer>().material = origMat;
    }

    public void Hurt(float damage) {
        StartCoroutine(DamageFlash());
        if (bloodLow + damage <= 10) {
            bloodLow += damage;
        } else {
            var Blood = Instantiate(BloodPrefab, new Vector3(transform.position.x,0.2f,transform.position.z), Quaternion.Euler(0,0,0), bloodHolder.transform);
            Blood.GetComponent<BloodScript>().SetBlood(damage+bloodLow);
            bloodLow = 0f;
        }
        health -= damage;
    }

    void Die() {
        var death = Instantiate(DeathPrefab);
        death.transform.position = transform.position;
        Destroy(gameObject);
    }
}
