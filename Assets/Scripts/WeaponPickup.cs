using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject gunVisualPrefab;
    public AudioClip pickupClip;
    [Range(0f, 1f)]
    public float volume = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Equip weapon first
        PlayerWeaponController pwc = other.GetComponent<PlayerWeaponController>();
        if (pwc != null)
            pwc.EquipWeapon(bulletPrefab, gunVisualPrefab);

        // Play sound at pickup position
        if (pickupClip != null)
            AudioSource.PlayClipAtPoint(pickupClip, transform.position, volume);

        // Destroy pickup immediately
        Destroy(gameObject);
    }
}
