using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

public enum TypeFallDamage
{
    None = 0,
    Low = 1,
    Normal = 2,
    Critical = 3,
    InstaKill = 4
}

public class CharacterControllerV1 : MonoBehaviour
{
    [Header("Basic movement")]
    [SerializeField] private float moveSpeed = 5f; // Velocidad de movimiento
    [SerializeField] private float jumpForce = 10f; // Fuerza del salto
    [SerializeField] private float jumpChargeRate = 0.5f; // Velocidad de carga del salto
    [SerializeField] private float jumpChargeLimit = 0.0f; // Límite de carga del salto
    [SerializeField] private float jumpChargeMax = 1f; // Máximo de carga del salto
    [SerializeField] private float lookSpeed = 5f; // Velocidad de rotación de la cámara
    [SerializeField][Range(1f, 1.5f)] private float jumpOnUpMultiplier = 1.05f;

    [Header("Basic movement HUD")]
    [SerializeField] private Slider loadJumpSlider;

    [Header("Camera movement")]
    [SerializeField] private Transform camTransform; // Referencia a la transformación de la cámara
    [SerializeField] private float smoothness = 5f;
    [SerializeField] private float sensitivity = 2f;
    private float currentRotation;

    // Sistemas y controles de físicas, posible re-factorización
    private bool isJumping = false; // Indica si se está ejecutando el salto
    private float jumpCharge = 0.25f; // Cantidad de carga del salto
    private Rigidbody rb; // Referencia al Rigidbody
    
    [Header("Player life")]
    [SerializeField] private float maxLife = 100.0f;
    private float currentLife;

    [Header("Player life HUD")]
    [SerializeField] private Slider currentLifeSlider;

    [Header("Fall damage")]
    [SerializeField] private float velocityYToHitLow = -15.0f;
    [SerializeField] private float velocityYToHitNormal = -20.0f;
    [SerializeField] private float velocityYToHitCritical = -25.0f;
    [SerializeField] private float velocityYToHitInstaKill = -30.0f;
    [SerializeField] private float lowDamage = 25.0f;
    [SerializeField] private float normalDamage = 50.0f;
    [SerializeField] private float criticalDamage = 75.0f;
    [SerializeField] private float instaKillDamage = 125.0f;
    private float setDamage;
    private TypeFallDamage typeFallDamage = TypeFallDamage.None;

    [Header("Fall damage HUD")]
    private float saveVelocityYToHit;

    [Header("Collider")]
    [SerializeField] private SphereCollider sphereCollider;

    public void SetStatusIsJumping(bool status)
    {
        isJumping = status;
    }

    void Start()
    {
        // Basic
        rb = GetComponent<Rigidbody>();
        currentRotation = transform.eulerAngles.y;

        // Game debug
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Life
        currentLife = maxLife;

        // Refresh HUD
        loadJumpSlider.maxValue = jumpChargeMax;
        loadJumpSlider.minValue = jumpCharge;

        // Refresh HUD
        currentLifeSlider.maxValue = maxLife;
        currentLifeSlider.minValue = 0.0f;
        currentLifeSlider.value = currentLife;

        // Jump
        loadJumpSlider.gameObject.SetActive(false);
    }

    void Update()
    {
        if (rb.velocity.y < -1 || rb.velocity.y > 1)
        {
            isJumping = true;
        }

        FallDamageCheck();

        // Rotar la cámara con el ratón
        // float mouseX = Input.GetAxis("Mouse X");
        // camTransform.RotateAround(transform.position, Vector3.up, mouseX * lookSpeed);

        // Obtener el movimiento del ratón en los ejes horizontal
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;

        // Actualizar la rotación actual del personaje
        currentRotation += mouseX;

        // Suavizar la rotación del personaje mediante una interpolación lineal
        float angle = Mathf.LerpAngle(transform.eulerAngles.y, currentRotation, Time.deltaTime * smoothness);
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

        if (!isJumping) { 
            // Saltar si se cumple la condición
            if (Input.GetKey(KeyCode.Space))
            {
                // Mostramos el slider para enseñar la carga que lleva el jugador
                loadJumpSlider.gameObject.SetActive(true);

                // Congelamos la velocidad actual para hacer más preciso el salto, ojo que tienen cosas interesantes el quitarlo
                rb.velocity = new Vector3(0, 0, 0);

                // Si se está manteniendo pulsada la tecla espacio
                if (jumpCharge < jumpChargeMax)
                {
                    jumpCharge += jumpChargeRate * Time.deltaTime; // Incrementar la carga del salto
                    loadJumpSlider.value = jumpCharge; // Actualizamos el HUD
                }
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                // Ocultamos el slider una vez ya comenzó el salto
                loadJumpSlider.gameObject.SetActive(false);

                // Congelamos la velocidad actual para hacer más preciso el salto, ojo que tienen cosas interesantes el quitarlo
                rb.velocity = new Vector3(0, 0, 0);

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
    }

    void FallDamageCheck()
    {
        if (rb.velocity.y < velocityYToHitInstaKill)
        {
            // Debug.Log("Velocity Y: " + rb.velocity.y);
            typeFallDamage = TypeFallDamage.InstaKill;
            setDamage = instaKillDamage;
        }
        else if (rb.velocity.y < velocityYToHitCritical)
        {
            // Debug.Log("Velocity Y: " + rb.velocity.y);
            typeFallDamage = TypeFallDamage.Critical;
            setDamage = criticalDamage;
        }
        else if (rb.velocity.y < velocityYToHitNormal)
        {
            // Debug.Log("Velocity Y: " + rb.velocity.y);
            typeFallDamage = TypeFallDamage.Normal;
            setDamage = normalDamage;
        }
        else if (rb.velocity.y < velocityYToHitLow)
        {
            // Debug.Log("Velocity Y: " + rb.velocity.y);
            typeFallDamage = TypeFallDamage.Low;
            setDamage = lowDamage;
        }
        else
        {
            typeFallDamage = TypeFallDamage.None;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        // Lo capamos en velocidad para evitar que salte entre paredes
        if (col.gameObject.tag == "Ground" && rb.velocity.y > -1 && rb.velocity.y < 1)
        {
            if (typeFallDamage != TypeFallDamage.None) // Comprobamos si tiene que recibir daño dada a su velocidad
            {
                currentLife -= setDamage;
                currentLifeSlider.value = currentLife;
            }                

            isJumping = false; // Marcar que se ha terminado el salto
            typeFallDamage = TypeFallDamage.None; // Marcamos que no recibira más daño
        }
    }
}
