using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerWeaponController : MonoBehaviour
{
    [Header("References")]
    public Transform firePoint;         // Bullet spawn point
    public GameObject arrowPrefab;      // Assign your arrow prefab (pivot at tail)

    [Header("Weapon settings")]
    public GameObject bulletPrefab;     // Bullet prefab
    public float maxShootDistance = 8f; // Short range
    public float bulletSpeed = 30f;
    public float fireCooldown = 0.5f;

    [Header("Ground plane Y")]
    public float groundY = 0f;

    [Header("Tank rotation")]
    public float rotationSpeed = 200f; // degrees per second

    private bool hasWeapon = false;
    private bool isAiming = false;
    private float lastFireTime = -999f;
    private Vector3 aimDirection = Vector3.forward;
    private GameObject gunVisualInstance;
    private GameObject arrowInstance;

    void Start()
    {
        // Instantiate arrow prefab and hide
        if (arrowPrefab != null)
        {
            arrowInstance = Instantiate(arrowPrefab);
            arrowInstance.SetActive(false);
        }
    }

    void Update()
    {
        if (!hasWeapon) return;

        // Start aiming
        if (Input.GetMouseButtonDown(0))
        {
            isAiming = true;
            if (arrowInstance != null) arrowInstance.SetActive(true);
        }

        // Update arrow and tank rotation while dragging
        if (isAiming && arrowInstance != null)
        {
            Vector3 target = GetMouseWorldPointOnPlane(groundY);
            Vector3 dir = target - firePoint.position;
            float distance = Mathf.Min(dir.magnitude, maxShootDistance);

            aimDirection = dir.sqrMagnitude > 0.001f ? dir.normalized : transform.forward;

            // Arrow position & rotation
            arrowInstance.transform.position = firePoint.position;
            arrowInstance.transform.rotation = Quaternion.LookRotation(aimDirection) * Quaternion.Euler(90f, 0f, 0f);

            // Arrow scale for distance
            Vector3 scale = arrowInstance.transform.localScale;
            scale.z = distance;
            arrowInstance.transform.localScale = scale;

            // Rotate tank (yaw only)
            Vector3 flatDir = new Vector3(aimDirection.x, 0f, aimDirection.z).normalized;
            if (flatDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(flatDir);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        // Fire on release
        if (Input.GetMouseButtonUp(0) && isAiming)
        {
            isAiming = false;
            if (arrowInstance != null) arrowInstance.SetActive(false);

            if (Time.time - lastFireTime >= fireCooldown)
                Fire();
        }
    }

    public void EquipWeapon(GameObject newBulletPrefab, GameObject gunVisualPrefab = null)
    {
        if (newBulletPrefab != null)
        {
            bulletPrefab = newBulletPrefab;
            hasWeapon = true;
        }

        if (gunVisualPrefab != null)
        {
            if (gunVisualInstance != null) Destroy(gunVisualInstance);
            gunVisualInstance = Instantiate(gunVisualPrefab, transform);
        }
    }

    private void Fire()
    {
        if (bulletPrefab == null) return;

        lastFireTime = Time.time;

        GameObject b = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(aimDirection));
        Rigidbody rb = b.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = aimDirection * bulletSpeed;

        float travelTime = Mathf.Max(0.05f, maxShootDistance / Mathf.Max(0.1f, bulletSpeed));
        Destroy(b, travelTime + 0.2f);
    }

    // Raycast from mouse to ground plane
    private Vector3 GetMouseWorldPointOnPlane(float y)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, y, 0));
        if (plane.Raycast(ray, out float enter))
            return ray.GetPoint(enter);
        return firePoint.position;
    }
}
