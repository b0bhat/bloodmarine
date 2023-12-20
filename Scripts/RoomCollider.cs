using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCollider : MonoBehaviour
{
    public List<GameObject> rooms = new List<GameObject>();
    [SerializeField] GameObject quad;
    public float posx;
    public float posz;
    public bool RoomEntered = false;
    public int RoomSpawnCount = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void AddRooms(ooparts.dungen.Room room) {
        rooms.Add(room.RoomCollider);

    }

    public void SetRoom(float x, float z, Vector3 pos) {
        this.posx = x;
        this.posz = z;
        this.transform.localScale = new Vector3(x, 1, z)*ooparts.dungen.Map.instance.TileSize;
        this.transform.position = pos;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Trigger() {
        RoomEntered = true;
        foreach (GameObject room in rooms) {
            //ooparts.dungen.Room roomScript = room.GetComponent<ooparts.dungen.Room>();
            //Debug.Log(roomScript);
            if (Random.Range(0f,1f) > 0.3f) {
				for (int i = 0; i < Random.Range(15,40); i++) {
                    EnemySpawner.instance.Spawn(new Vector2(room.GetComponent<RoomCollider>().posx, room.GetComponent<RoomCollider>().posz),room.transform.position);
                }
            }
        }

    }
}
