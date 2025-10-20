using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    public GameObject explosionPrefab;
    public float lifeTime = 2f;
    public int damage = 20;

    private string ownerTag = "";
    private Collider ownerCollider;   // 🚀 we'll use this to ignore collision with shooter

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// Set the owner tag and ignore collision with the shooter's collider.
    /// </summary>
    public void SetOwnerTag(string tag, int dmg, Collider ownerCol = null)
    {
        ownerTag = tag;
        damage = dmg;

        if (ownerCol != null)
        {
            ownerCollider = ownerCol;
            Collider bulletCol = GetComponent<Collider>();
            if (bulletCol != null)
            {
                Physics.IgnoreCollision(bulletCol, ownerCollider);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Bullet OwnerTag: {ownerTag}, Hit Collider Tag: {collision.collider.tag}");

        // Ignore collision with the shooter itself just in case
        if (collision.collider.CompareTag(ownerTag))
        {
            return;
        }

        bool validHit = false;

        if (ownerTag == "Player" && collision.collider.CompareTag("Enemy"))
        {
            validHit = true;
        }
        if (ownerTag == "Enemy" && collision.collider.CompareTag("Player"))
        {
            validHit = true;
        }

        if (validHit)
        {
            Health h = collision.collider.GetComponentInParent<Health>();
            if (h != null)
            {
                h.TakeDamage(damage);
            }
        }

        // Explosion spawn at contact
        ContactPoint contact = collision.contacts[0];
        Vector3 spawnPos = contact.point + contact.normal * 0.1f;

        if (explosionPrefab != null)
        {
            GameObject e = Instantiate(explosionPrefab, spawnPos, Quaternion.identity);
            ParticleSystem ps = e.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();
            Destroy(e, ps != null ? ps.main.duration + ps.main.startLifetime.constantMax : 2f);
        }

        Destroy(gameObject);
    }
}
