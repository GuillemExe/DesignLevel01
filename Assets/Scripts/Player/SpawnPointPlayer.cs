using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointPlayer : MonoBehaviour
{
    private Transform lastSpawnPointReached;

    public Transform GetLastSpawnPointReached() => lastSpawnPointReached;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint")
        {
            Checkpoint checkpoint = other.GetComponent<Checkpoint>();
            if (checkpoint.IsReached() == false)
            {
                checkpoint.SetReached(true);
                lastSpawnPointReached = checkpoint.GetSpawnPoint();
            }
        }
    }
}
