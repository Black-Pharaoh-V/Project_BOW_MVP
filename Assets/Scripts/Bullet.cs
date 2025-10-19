using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public GameObject explosionPrefab;
    public float lifeTime = 1.0f;
    private string ownerTag = "";
    public int damage = 20;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetOwnerTag(string tag, int dmg)
    {
        ownerTag = tag;
        damage = dmg;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player")) return;

        // find Health on hit object
        Health h = collision.collider.GetComponentInParent<Health>();
        if (h != null)
        {
            h.TakeDamage(20); // damage value
        }
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
