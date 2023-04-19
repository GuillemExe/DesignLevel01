using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public List<Transform> SpawnPoints;
    private int CurrentIndex;

    [SerializeField] private GameObject Character;
    [SerializeField] private Rigidbody Rb;

    // Start is called before the first frame update
    void Start()
    {
        CurrentIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            SpawnOnPoint(1);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnOnPoint(-1);
        }
    }

    private void SpawnOnPoint(int StepIndex)
    {
        int DesiredIndex = CurrentIndex + StepIndex;

        if (DesiredIndex < 0)
            DesiredIndex = SpawnPoints.Count;
        else if (DesiredIndex + 1 > SpawnPoints.Count)
            DesiredIndex = 0;

        CurrentIndex = DesiredIndex;

        Rb.isKinematic = true;

        Character.transform.position = SpawnPoints[DesiredIndex].position;

        Rb.isKinematic = false;
    }
}
