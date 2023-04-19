using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterControllerV1 : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidad de movimiento
    public float jumpForce = 10f; // Fuerza del salto
    public float jumpChargeRate = 0.5f; // Velocidad de carga del salto
    public float jumpChargeLimit = 0.0f; // L�mite de carga del salto
    public float jumpChargeMax = 1f; // M�ximo de carga del salto
    public float lookSpeed = 5f; // Velocidad de rotaci�n de la c�mara
    [SerializeField] [Range(1f, 1.5f)] private float jumpOnUpMultiplier = 1.05f;

    public float smoothness = 5f;
    public float sensitivity = 2f;
    private float _currentRotation;

    private bool isJumping = false; // Indica si se est� ejecutando el salto
    private float jumpCharge = 0.25f; // Cantidad de carga del salto
    private Rigidbody rb; // Referencia al Rigidbody
    public Transform camTransform; // Referencia a la transformaci�n de la c�mara

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
        
        // Rotar la c�mara con el rat�n
        // float mouseX = Input.GetAxis("Mouse X");
        // camTransform.RotateAround(transform.position, Vector3.up, mouseX * lookSpeed);

        // Obtener el movimiento del rat�n en los ejes horizontal
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;

        // Actualizar la rotaci�n actual del personaje
        _currentRotation += mouseX;

        // Suavizar la rotaci�n del personaje mediante una interpolaci�n lineal
        float angle = Mathf.LerpAngle(transform.eulerAngles.y, _currentRotation, Time.deltaTime * smoothness);
        transform.eulerAngles = new Vector3(0f, angle, 0f);

        if (!isJumping)
        {
            // Calcular la direcci�n hacia adelante del personaje en base a la orientaci�n de la c�mara
            Vector3 moveDirection = camTransform.forward;
            moveDirection.y = 0f; // Asegurarse de que la direcci�n no tenga componente vertical
            moveDirection.Normalize(); // Normalizar la direcci�n para mantener la misma velocidad en todas las direcciones

            // Calcular la velocidad de movimiento y aplicarla al Rigidbody
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 moveVelocity = (horizontalInput * camTransform.right + verticalInput * moveDirection) * moveSpeed;
            rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
        }

        // Saltar si se cumple la condici�n
        if (Input.GetKey(KeyCode.Space))
        {
            // Si se est� manteniendo pulsada la tecla espacio
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
                // Si no se est� ejecutando el salto y se ha cargado lo suficiente
                Vector3 jumpDirection = transform.forward + Vector3.up * jumpOnUpMultiplier; // Direcci�n del salto
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
            isJumping = false; // Marcar que se ha terminado el salto
        }
    }
}
