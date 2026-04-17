using System.Collections;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    [Header("Boost settings")]
    [SerializeField] private float speedMultiplier = 2f;
    [SerializeField] private float boostDuration = 3f;

    [Header("Respawn")]
    [SerializeField] private bool respawns = true;
    [SerializeField] private float respawnDelay = 8f;

    [Header("Visual")]
    [SerializeField] private Color boostColor = Color.yellow;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.3f;

    private Vector3 startPos;
    private Renderer rend;
    private Collider col;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();
        startPos = transform.position;

        if (rend != null) rend.material.color = boostColor;
        if (col != null) col.isTrigger = true;
    }

    private void Update()
    {

        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        transform.Rotate(0f, 90f * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        player.ApplySpeedBoost(speedMultiplier, boostDuration);
        StartCoroutine(PickedUp());
    }

    private IEnumerator PickedUp()
    {

        if (rend != null) rend.enabled = false;
        if (col != null) col.enabled = false;

        if (respawns)
        {
            yield return new WaitForSeconds(respawnDelay);
            if (rend != null) rend.enabled = true;
            if (col != null) col.enabled = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}