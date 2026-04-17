using System.Collections;
using UnityEngine;

public class SlowObstacle : MonoBehaviour
{
    [Header("Slow Effect")]
    [SerializeField] private float slowMultiplier = 0.3f;
    [SerializeField] private float slowDuration = 2f;

    [Header("Visual feedback")]
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private Color normalColor = Color.white;

    private Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null) rend.material.color = normalColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        player.ApplySlow(slowMultiplier, slowDuration);

        if (rend != null)
            StartCoroutine(FlashColor());
    }

    private IEnumerator FlashColor()
    {
        rend.material.color = hitColor;
        yield return new WaitForSeconds(0.2f);
        rend.material.color = normalColor;
    }
}