using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterControllerV1 : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidad de movimiento
    public float jumpForce = 10f; // Fuerza del salto
    public float jumpChargeRate = 0.5f; // Velocidad de carga del salto
    public float jumpChargeLimit = 0.0f; // Límite de carga del salto
    public float jumpChargeMax = 1f; // Máximo de carga del salto
    public float lookSpeed = 5f; // Velocidad de rotación de la cámara
    [SerializeField] [Range(1f, 1.5f)] private float jumpOnUpMultiplier = 1.05f;

    public float smoothness = 5f;
    public float sensitivity = 2f;
    private float _currentRotation;

    private bool isJumping = false; // Indica si se está ejecutando el salto
    private float jumpCharge = 0.25f; // Cantidad de carga del salto
    private Rigidbody rb; // Referencia al Rigidbody
    public Transform camTransform; // Referencia a la transformación de la cámara

    // DEBUG
    [SerializeField] private TextMeshProUGUI fallVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _currentRotation = transform.eulerAngles.y;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        fallVelocity.text = "Fall-Y: " + Mathf.Round(rb.velocity.y * 100f) / 10000f;
        
        // Rotar la cámara con el ratón
        // float mouseX = Input.GetAxis("Mouse X");
        // camTransform.RotateAround(transform.position, Vector3.up, mouseX * lookSpeed);

        // Obtener el movimiento del ratón en los ejes horizontal
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;

        // Actualizar la rotación actual del personaje
        _currentRotation += mouseX;

        // Suavizar la rotación del personaje mediante una interpolación lineal
        float angle = Mathf.LerpAngle(transform.eulerAngles.y, _currentRotation, Time.deltaTime * smoothness);
        transform.eulerAngles = new Vector3(0f, angle, 0f);

        if (!isJumping)
        {
            // Calcular la dirección hacia adelante del personaje en base a la orientación de la cámara
            Vector3 moveDirection = camTransform.forward;
            moveDirection.y = 0f; // Asegurarse de que la dirección no tenga componente vertical
            moveDirection.Normalize(); // Normalizar la dirección para mantener la misma velocidad en todas las direcciones

            // Calcular la velocidad de movimiento y aplicarla al Rigidbody
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 moveVelocity = (horizontalInput * camTransform.right + verticalInput * moveDirection) * moveSpeed;
            rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
        }

        // Saltar si se cumple la condición
        if (Input.GetKey(KeyCode.Space))
        {
            // Si se está manteniendo pulsada la tecla espacio
            if (jumpCharge < jumpChargeMax)
            {
                jumpCharge += jumpChargeRate * Time.deltaTime; // Incrementar la carga del salto
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            // Si se suelta la tecla espacio
            if (!isJumping && jumpCharge > jumpChargeLimit)
            {
                // Si no se está ejecutando el salto y se ha cargado lo suficiente
                Vector3 jumpDirection = transform.forward + Vector3.up * jumpOnUpMultiplier; // Dirección del salto
                rb.AddForce(jumpDirection * jumpForce * jumpCharge, ForceMode.Impulse); // Ejecutar el salto
                isJumping = true; // Marcar que se está ejecutando el salto
            }
            jumpCharge = 0.25f; // Resetear la carga del salto
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Ground")
        {
            isJumping = false; // Marcar que se ha terminado el salto
        }
    }
}
