using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -8f);
    [SerializeField] private float smoothSpeed = 8f;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = target.position + target.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up);
    }
}