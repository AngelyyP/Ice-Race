using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int playerId;

    [Header("Camera")]
    [SerializeField] private CameraFollow assignedCamera;

    [Header("Movement")]
    [SerializeField] private float forwardSpeed = 8f;
    [SerializeField] private float sidewaysSpeed = 6f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float constantDownForce = 5f;
    [SerializeField] private float fallMultiplier = 3f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    private bool isGrounded;
    private bool jumpRequested;
    private bool raceFinished;

    private float slowMultiplier = 1f;
    private float boostMultiplier = 1f;
    private float SpeedMultiplier => slowMultiplier * boostMultiplier;

    private Coroutine slowCoroutine;
    private Coroutine boostCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Start()
    {
        if (assignedCamera != null)
            assignedCamera.SetTarget(transform);
    }

    private void Update()
    {
        if (!raceFinished && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)))
            jumpRequested = true;
    }

    private void FixedUpdate()
    {
        if (raceFinished) return;

        CheckGrounded();
        ApplyMovement();
        HandleJump();
        ApplyGravity();
    }

    private void CheckGrounded()
    {
        float halfHeight = GetComponent<Collider>().bounds.extents.y;
        isGrounded = Physics.Raycast(transform.position, Vector3.down,
                                     halfHeight + 0.2f, groundLayer);
    }

    private void ApplyMovement()
    {
        float h = Input.GetAxis("Horizontal");
        Vector3 vel = rb.linearVelocity;
        vel.z = forwardSpeed * SpeedMultiplier;
        vel.x = h * sidewaysSpeed * SpeedMultiplier;

        // Clamp Y when grounded to kill the bounce
        if (isGrounded && vel.y < 0f)
            vel.y = 0f;

        rb.linearVelocity = vel;
    }

    private void HandleJump()
    {
        if (jumpRequested && isGrounded)
        {
            Vector3 vel = rb.linearVelocity;
            vel.y = 0f;
            rb.linearVelocity = vel;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        jumpRequested = false;
    }

    private void ApplyGravity()
    {
        if (isGrounded) return;

        rb.AddForce(Vector3.down * constantDownForce, ForceMode.Acceleration);
        if (rb.linearVelocity.y < 0f)
            rb.AddForce(Vector3.down * constantDownForce * (fallMultiplier - 1f),
                        ForceMode.Acceleration);
    }

    public void FinishRace()
    {
        if (raceFinished) return;
        raceFinished = true;
        rb.linearVelocity = Vector3.zero;
    }

    public void SetCamera(CameraFollow cam)
    {
        assignedCamera = cam;
        assignedCamera.SetTarget(transform);
    }

    public void ApplySlow(float multiplier, float duration)
    {
        if (slowCoroutine != null) StopCoroutine(slowCoroutine);
        slowCoroutine = StartCoroutine(SlowCoroutine(multiplier, duration));
    }

    public void ApplySpeedBoost(float multiplier, float duration)
    {
        if (boostCoroutine != null) StopCoroutine(boostCoroutine);
        boostCoroutine = StartCoroutine(BoostCoroutine(multiplier, duration));
    }

    private IEnumerator SlowCoroutine(float multiplier, float duration)
    {
        slowMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        slowMultiplier = 1f;
    }

    private IEnumerator BoostCoroutine(float multiplier, float duration)
    {
        boostMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        boostMultiplier = 1f;
    }

    public void MovePlayer(Vector3 position) => transform.position = position;
    public Vector3 GetPosition() => transform.position;
}