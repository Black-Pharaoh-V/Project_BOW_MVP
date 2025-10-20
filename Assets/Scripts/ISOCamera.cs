using UnityEngine;

public class FixedCameraFollow : MonoBehaviour
{
    public Transform target;        // Player tank
    public float followSpeed = 10f;

    private Vector3 offset;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target not assigned to camera!");
            enabled = false;
            return;
        }

        // Record the initial offset in editor
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        // Smoothly follow player using fixed offset
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Optional: Look at player
        transform.LookAt(target.position);
    }
}
