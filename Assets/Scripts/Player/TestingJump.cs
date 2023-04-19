using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingJump : MonoBehaviour
{
    public float jumpForce = 10f; // Fuerza del salto
    public float jumpChargeRate = 0.5f; // Velocidad de carga del salto
    public float jumpChargeLimit = 0.0f; // L�mite de carga del salto
    public float jumpChargeMax = 1f; // M�ximo de carga del salto
    private bool isJumping = false; // Indica si se est� ejecutando el salto
    private float jumpCharge = 0f; // Cantidad de carga del salto
    private Rigidbody rb; // Referencia al Rigidbody

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("LOAD");
            // Si se est� manteniendo pulsada la tecla espacio
            if (jumpCharge < jumpChargeMax)
            {
                jumpCharge += jumpChargeRate * Time.deltaTime; // Incrementar la carga del salto
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("JUMP");
            // Si se suelta la tecla espacio
            if (!isJumping && jumpCharge > jumpChargeLimit)
            {
                // Si no se est� ejecutando el salto y se ha cargado lo suficiente
                Vector3 jumpDirection = transform.forward + Vector3.up; // Direcci�n del salto
                rb.AddForce(jumpDirection * jumpForce * jumpCharge, ForceMode.Impulse); // Ejecutar el salto
                isJumping = true; // Marcar que se est� ejecutando el salto
            }
            jumpCharge = 0.25f; // Resetear la carga del salto
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Ground")
        {
            isJumping = false; // Marcar que se ha terminado el salto al colisionar con el suelo
        }
    }
}
