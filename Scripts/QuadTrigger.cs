using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTrigger : MonoBehaviour
{
    RoomCollider RoomScript;
    // Start is called before the first frame update
    void Start()
    {
        RoomScript = transform.parent.gameObject.GetComponent<RoomCollider>();


    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider other) {
        if (RoomScript.RoomEntered == false && ooparts.dungen.Map.instance.Flag == true) {
            Debug.Log("WOW");
            RoomScript.Trigger();
        }
    }
}
