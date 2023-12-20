using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Enemy enemy;
    [SerializeField] GameObject test;
    public List<Enemy> enemies;

    private float range = 200.0f;

    #region Singleton
    public static EnemySpawner instance;
    void Awake() {
        instance = this;
    }
    #endregion

    public void Spawn(Vector2 Size,Vector3 pos) {
        //var loc = RandomNavmeshLocation(range);
        Vector3 location = new Vector3(pos.x+Random.Range(-Size.x+1,Size.x-1),0f,pos.z+Random.Range(-Size.y+1,Size.y-1));
        var loc = CheckNavmeshLocation(location);
        Enemy spawned = Instantiate(enemy, location, Quaternion.identity) as Enemy;
        //var testy = Instantiate(test, location, Quaternion.identity);
        spawned.transform.parent = transform;
        enemies.Add(spawned);
    }

    public Vector3 CheckNavmeshLocation(Vector3 location) {
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(location, out hit, 3, 1)) {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    public Vector3 RandomNavmeshLocation(float radius, Vector3 pos) {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += pos;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    public void Alert(Vector3 position, float range) {
        var numList = enemies.Count;
        for (int i = 0; i < numList; i++) {
            if (enemies[i]) {
                enemies[i].SoundAlert(position, range);
            }
        }

    }
}
