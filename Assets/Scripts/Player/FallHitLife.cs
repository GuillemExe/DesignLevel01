using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallHitLife : MonoBehaviour
{
    private Rigidbody Rb;

    // Start is called before the first frame update
    void Start()
    {
        Rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Rb.velocity.y > -0.075f)
        {
            Debug.Log("HIT");
        }
    }
}
