using UnityEngine;

public class ThirdPersonCameraFollow : MonoBehaviour
{
    [Header("Target to follow")]
    public Transform target;        // Player tank

    [Header("Camera Settings")]
    public float followSpeed = 10f;      // Smooth position
    public float rotationSpeed = 5f;     // Smooth rotation
    public float fixedPitch = 45f;       // Fixed X axis
    public float distance = 7f;          // Distance behind the tank
    public float height = 5f;            // Height above the tank

    void LateUpdate()
    {
        if (target == null) return;

        // --- Desired position: behind & above player tank ---
        Vector3 desiredPosition = target.position - target.forward * distance + Vector3.up * height;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // --- Look at tank with fixed pitch ---
        Vector3 lookPos = target.position + Vector3.up * (height / 2f); // look slightly above center
        Vector3 direction = (lookPos - transform.position).normalized;

        // Compute rotation while keeping fixed X
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Vector3 euler = targetRotation.eulerAngles;
        euler.x = fixedPitch; // fix pitch
        targetRotation = Quaternion.Euler(euler);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
