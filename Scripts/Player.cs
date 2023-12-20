using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed;
    public float speedSprintMult;
    public float speedFinal;
    public Vector3 worldPos;
    public Vector3 aimDir;
    Plane plane;

    #region Singleton
    public static Player instance;
    void Awake() {
        instance = this;
    }
    #endregion

    public float blood;
    public float maxBlood;

    [SerializeField] GameObject capsule;
    [SerializeField] GameObject turn;
    public Rigidbody rb;
    Vector3 Force = Vector3.zero;

    [SerializeField] GameObject weapon1;
    [SerializeField] GameObject weapon2;
    WeaponScript weapon1script;
    WeaponScript weapon2script;

    GameObject viewCone;
    GameObject viewSphere;

    //bool fire;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        speed = 2f;
        speedSprintMult = 1.5f;
        plane = new Plane(Vector3.up, 0.5f);
        weapon1script = weapon1.GetComponent<WeaponScript>();
        weapon2script = weapon2.GetComponent<WeaponScript>();

        viewCone = GameObject.FindGameObjectWithTag("viewcone");
        viewSphere = GameObject.FindGameObjectWithTag("viewsphere");

        blood = maxBlood/2;

    }

    // Update is called once per frame
    void Update() {

        Aim();

        Turn();

        CheckFire(weapon1script, weapon1script.weaponType,KeyCode.Mouse0);
        CheckFire(weapon2script, weapon2script.weaponType,KeyCode.Mouse1);

    }

    void CheckFire(WeaponScript weapon, int type, KeyCode keyCode) {
        bool fire = false;
        if (weapon.weaponType == 0 && Input.GetKey(keyCode)) {
            fire = true;
        } else if (weapon.weaponType == 1 && Input.GetKeyDown(keyCode)) {
            fire = true;
        } weapon.Check(fire);
        if (Input.GetKeyDown(KeyCode.R)) weapon.Reload();
    }

    void FixedUpdate() {

        speedFinal = speed;
        if (Input.GetKey(KeyCode.LeftShift)) {
            speedFinal *= speedSprintMult;
        }

        if (Input.GetKey(KeyCode.A))
            Force += Vector3.left;
        if (Input.GetKey(KeyCode.D))
            Force += Vector3.right;
        if (Input.GetKey(KeyCode.W))
            Force += Vector3.forward;
        if (Input.GetKey(KeyCode.S))
            Force += Vector3.back;
        rb.AddForce(Force.normalized*speedFinal, ForceMode.VelocityChange);
        //Debug.Log(rb.velocity.magnitude);
        Force = Force*0.1f;
    }

    private void Turn() {
        float angle = FieldofView.GetAngleFromVectorFloat(aimDir);
        turn.transform.rotation = Quaternion.AngleAxis(90-angle,Vector3.up);
    }

    private void Aim() {
        float distance;
        //Debug.Log(Camera.main);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance)) {
            worldPos = ray.GetPoint(distance);
            aimDir = (worldPos - transform.localPosition).normalized;
        }
        viewCone.GetComponent<FieldofView>().SetAim(aimDir);
        //Debug.DrawLine(transform.localPosition, aimDir);
    }

    public void Recoil(float force) {
        rb.AddForce(force*-aimDir, ForceMode.Impulse);
    }

}
