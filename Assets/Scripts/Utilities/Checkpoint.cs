using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    private bool reached;

    public Transform GetSpawnPoint() => spawnPoint;

    public bool IsReached() => reached;
    public void SetReached(bool newStatusReach) => reached = newStatusReach;
}
