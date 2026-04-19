using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int playerId;
    
    // Identificador para el jugador local vs remoto
    public bool isLocalPlayer = true;
    public bool canMove = false; // Solo podremos movernos cuando ambos conecten

    [Header("Movement")]
    [SerializeField] private float forwardSpeed = 8f;
    [SerializeField] private float sidewaysSpeed = 6f;

    private Rigidbody rb;
    private bool raceFinished;

    private float slowMultiplier = 1f;
    private float SpeedMultiplier => slowMultiplier;

    private Coroutine slowCoroutine;
    
    // Variables para interpolación del jugador remoto
    private Vector3 targetPosition;
    private float updateRate = 10f; // Velocidad de interpolación

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = true;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        
        targetPosition = transform.position;
    }

    private void Start()
    {
        // Gestionar la cámara interna del prefab
        Camera internalCam = GetComponentInChildren<Camera>();
        if (internalCam != null)
        {
            internalCam.gameObject.SetActive(isLocalPlayer);
            
            AudioListener listener = internalCam.GetComponent<AudioListener>();
            if (listener != null) listener.enabled = isLocalPlayer;
        }

        // Si el jugador es remoto, convertimos su RigidBody a Kinematic para evitar
        // que el motor de físicas pelee contra nuestra interpolación de posición.
        if (!isLocalPlayer)
        {
            rb.isKinematic = true;
        }
    }

    private void Update()
    {
        // Para mejorar la suavidad visual, procesamos la interpolación del jugador remoto en el Update (vinculado a los FPS)
        if (!isLocalPlayer && !raceFinished)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * updateRate);
        }
    }

    private void FixedUpdate()
    {
        if (raceFinished) return;

        if (isLocalPlayer && canMove)
        {
            ApplyMovement();
        }
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

    public void ApplySlow(float multiplier, float duration)
    {
        if (!isLocalPlayer) return; 
        if (slowCoroutine != null) StopCoroutine(slowCoroutine);
        slowCoroutine = StartCoroutine(SlowCoroutine(multiplier, duration));
    }

    private IEnumerator SlowCoroutine(float multiplier, float duration)
    {
        slowMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        slowMultiplier = 1f;
    }

    public void MovePlayer(Vector3 position)
    {
        targetPosition = position;
        
        if (Vector3.Distance(transform.position, targetPosition) > 5f)
        {
            transform.position = targetPosition;
        }
    }
    
    public Vector3 GetPosition() => transform.position;
}