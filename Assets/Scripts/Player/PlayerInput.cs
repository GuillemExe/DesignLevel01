using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerInput : MonoBehaviour, IInput
{
    public Action<Vector2> OnMovementInput { get; set; }
    public Action<Vector3> OnMovementDirectionInput { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        GetMovementInput();
        GetMovementDirection();
    }

    private void GetMovementDirection()
    {
        Vector3 l_CameraForwardDirection = Camera.main.transform.forward;
        var l_DirectionToMoveIn = Vector3.Scale(l_CameraForwardDirection, (Vector3.right + Vector3.forward));
        OnMovementDirectionInput?.Invoke(l_DirectionToMoveIn);

    }

    private void GetMovementInput()
    {
        Vector2 l_Input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        OnMovementInput?.Invoke(l_Input);
    }
}