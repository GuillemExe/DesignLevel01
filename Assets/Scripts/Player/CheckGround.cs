using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : MonoBehaviour
{
    [SerializeField] private CharacterControllerV1 characterControllerV1;

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Ground")
            characterControllerV1.SetStatusIsJumping(false);
    }
}
