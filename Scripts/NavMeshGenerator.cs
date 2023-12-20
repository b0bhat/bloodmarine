using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshGenerator : MonoBehaviour {

    public NavMeshSurface surface;
    public Transform[] objectsToRotate;
    bool done = false;

    public void NavMeshGen ()
    {

        for (int j = 0; j < objectsToRotate.Length; j++) {
            objectsToRotate [j].localRotation = Quaternion.Euler (new Vector3 (0, Random.Range (0, 360), 0));
        }
    }

    public void NavMeshBuild(NavMeshSurface newsur) {
        newsur.BuildNavMesh();
    }

}
