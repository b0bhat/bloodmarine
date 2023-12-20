using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] GameObject vcam;
    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<CinemachineVirtualCamera>().LookAt = Player.instance.transform;
        this.GetComponent<CinemachineVirtualCamera>().Follow = Player.instance.transform;

    }
}
