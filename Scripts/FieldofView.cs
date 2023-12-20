using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldofView : MonoBehaviour
{
    [SerializeField] private LayerMask layermask;
    public Mesh mesh;
    [SerializeField] float fov = 90;
    [SerializeField] float startAngle;
    [SerializeField] Vector3 origin;
    [SerializeField] int rayCount = 500;
    [SerializeField] float viewDistance = 20f;

    [SerializeField] bool primary = false;
    Player player;

    /*
    #region Singleton
    public static FieldofView instance;
    void Awake() {
        instance = this;
        mesh = new Mesh();
    }
    #endregion*/

    void Awake() {
        mesh = new Mesh();
    }

    // Start is called before the first frame update
    void Start() {
        startAngle = 0f;
        //Debug.Log(mesh);
        GetComponent<MeshFilter>().mesh = mesh;
        player = Player.instance;
        //transform.parent = player.transform;
        //transform.position += new Vector3(transform.parent.position.x, 0.8f, transform.parent.position.z);
        //transform.position = new Vector3(0,0.5f,0);
    }

    void Update() {
        if (ooparts.dungen.Map.instance.Flag) {
            origin = player.transform.position;
            origin.y = 0.9f;
            float angle = startAngle;
            float angleIncrease = fov/rayCount;

            Vector3[] vertices = new Vector3[rayCount+2];
            Vector2[] uv = new Vector2[rayCount+2];
            int[] triangles = new int[rayCount*3];

            vertices[0] = origin;
            int vertexIndex = 1;
            int triangleIndex = 0;
            for (int i = 0; i <= rayCount; i++) {
                Vector3 vertex;

                if (Physics.Raycast(origin, GetVectorFromAngle(angle), out RaycastHit rayhit, viewDistance, layermask)) {
                    vertex = rayhit.point;
                } else {
                    vertex = origin + GetVectorFromAngle(angle) * viewDistance;
                }

                vertices[vertexIndex] = vertex;

                //Debug.Log(triangleIndex);
                //Debug.Log(triangles);
                if (i > 0) {
                    triangles[triangleIndex + 0] = 0;
                    triangles[triangleIndex + 1] = vertexIndex-1;
                    triangles[triangleIndex + 2] = vertexIndex;
                    triangleIndex += 3;
                }
                vertexIndex++;

                angle += -angleIncrease;
            }

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
        }

    }

    public static Vector3 GetVectorFromAngle(float angle) {
        float angleRad = angle * (Mathf.PI/180f);
        return new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
    }

    public static float GetAngleFromVectorFloat(Vector3 dir) {
        float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        if (n<0) n+=360f;
        return n;
    }

    public void SetAim(Vector3 aimDir) {
        //startAngle = GetAngleFromVectorFloat(aimDir)-fov/2;
        startAngle = GetAngleFromVectorFloat(aimDir)+fov/2;
        //Debug.Log(startAngle);

        //LineRenderer lr = reticle.GetComponent<LineRenderer>();
        //lr.SetPosition(0, new Vector3(transform.position.x, reticle.transform.position.y, transform.position.z));
        //lr.SetPosition(1, new Vector3(worldPos.x, reticle.transform.position.y, worldPos.z));
    }
}
