using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Range(2f, 20f)]
    public float smoothSpeed = 12f;

    private void Start()
    {
        target = FindObjectOfType<PlayerController>().transform;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.unscaledDeltaTime);
    }
}