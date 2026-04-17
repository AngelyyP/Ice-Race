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

    private Rigidbody rb;
    private bool raceFinished;

    private float slowMultiplier = 1f;
    private float SpeedMultiplier => slowMultiplier;

    private Coroutine slowCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = true;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Start()
    {
        if (assignedCamera != null)
            assignedCamera.SetTarget(transform);
    }

    private void FixedUpdate()
    {
        if (raceFinished) return;

        ApplyMovement();
    }

    private void ApplyMovement()
    {
        float h = Input.GetAxis("Horizontal");

        Vector3 vel = rb.linearVelocity;
        vel.z = forwardSpeed * SpeedMultiplier;
        vel.x = h * sidewaysSpeed * SpeedMultiplier;

        rb.linearVelocity = vel;
    }

    public void FinishRace()
    {
        if (raceFinished) return;

        raceFinished = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll;
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

    private IEnumerator SlowCoroutine(float multiplier, float duration)
    {
        slowMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        slowMultiplier = 1f;
    }

    public void MovePlayer(Vector3 position) => transform.position = position;
    public Vector3 GetPosition() => transform.position;
}