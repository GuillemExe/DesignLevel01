using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
    [Header("Camera")]
    public Transform mainCameraTransform;

    [Header("Basic movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpChargeRate = 0.5f;
    [SerializeField] private float jumpChargeLimit = 0.0f;
    [SerializeField] private float jumpChargeMax = 1f;
    [SerializeField][Range(0f, 2f)] private float jumpOnUpMultiplier = 1.05f;

    [Header("Basic movement HUD")]
    [SerializeField] private Slider loadJumpSlider;

    [Header("Components")]
    private Rigidbody rb;
    private SpawnPointPlayer spawnPointPlayer;

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

    [Header("Collider")]
    [SerializeField] private SphereCollider sphereCollider;

    [Header("Unity Events")]
    [SerializeField] private UnityEvent playerDies;

    private bool isJumping;
    private bool isLoadingTheJump;
    private float jumpCharge = 0.25f;

    public void SetStatusIsJumping(bool status)
    {
        isJumping = status;
    }

    void Start()
    {
        // Basic
        rb = GetComponent<Rigidbody>();
        spawnPointPlayer = GetComponent<SpawnPointPlayer>();

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
        if (!isJumping)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                isLoadingTheJump = true;
                loadJumpSlider.gameObject.SetActive(true);

                rb.velocity = new Vector3(0, 0, 0);

                if (jumpCharge < jumpChargeMax)
                {
                    jumpCharge += jumpChargeRate * Time.deltaTime;
                    loadJumpSlider.value = jumpCharge;
                }
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                loadJumpSlider.gameObject.SetActive(false);

                rb.velocity = new Vector3(0, 0, 0);

                if (!isJumping && jumpCharge > jumpChargeLimit)
                {
                    Vector3 jumpDirection = jumpDirection = transform.forward + Vector3.up * jumpOnUpMultiplier;

                    rb.AddForce(jumpDirection * jumpForce * jumpCharge, ForceMode.Impulse);
                    isJumping = true;
                }
                jumpCharge = 0.25f;
            }
        }
    }

    void FixedUpdate()
    {
        if (rb.velocity.y < -1 || rb.velocity.y > 1)
            isJumping = true;

        FallDamageCheck();

        if (!isJumping) {
            float cameraRotationY = mainCameraTransform.transform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, cameraRotationY, 0f);
        }

        if (!isJumping && !isLoadingTheJump)
        {
            Vector3 cameraForward = mainCameraTransform.forward;
            cameraForward.y = 0f;
            cameraForward.Normalize();

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 moveVelocity = (horizontalInput * mainCameraTransform.right + verticalInput * cameraForward) * moveSpeed;

            rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
        }
    }

    void FallDamageCheck()
    {
        if (rb.velocity.y < velocityYToHitInstaKill)
        {
            typeFallDamage = TypeFallDamage.InstaKill;
            setDamage = instaKillDamage;
        }
        else if (rb.velocity.y < velocityYToHitCritical)
        {
            typeFallDamage = TypeFallDamage.Critical;
            setDamage = criticalDamage;
        }
        else if (rb.velocity.y < velocityYToHitNormal)
        {
            typeFallDamage = TypeFallDamage.Normal;
            setDamage = normalDamage;
        }
        else if (rb.velocity.y < velocityYToHitLow)
        {
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
        if (col.gameObject.tag == "Ground" && rb.velocity.y > -1 && rb.velocity.y < 1)
        {
            isLoadingTheJump = false;

            if (typeFallDamage != TypeFallDamage.None)
            {
                currentLife -= setDamage;
                currentLifeSlider.value = currentLife;
            }                

            isJumping = false;
            typeFallDamage = TypeFallDamage.None;
        }

        if (currentLife == 0 || currentLife < 0)
            Respawn();
    }

    void Respawn()
    {
        rb.isKinematic = true;
        transform.position = spawnPointPlayer.GetLastSpawnPointReached().position;
        rb.isKinematic = false;

        currentLife = maxLife;
        currentLifeSlider.value = currentLife;
    }
}
