using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI")]
    public HealthBar healthBar; // assign the HealthBar component on the prefab child

    [Header("Death")]
    public bool destroyOnDeath = true; // enemy: true, player: maybe false (you can change)

    public GameObject deathEffectPrefab;

    void Awake()
    {
        currentHealth = maxHealth;
        if (healthBar != null) healthBar.SetMax(maxHealth);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null) healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0) Die();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if (healthBar != null) healthBar.SetHealth(currentHealth);
    }

    void Die()
    {
        //Spawn death effect
        if (deathEffectPrefab != null)
        {
            GameObject e = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

            // If explosion prefab has particle system, destroy after it finishes
            ParticleSystem ps = e.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();
            Destroy(e, ps != null ? ps.main.duration + ps.main.startLifetime.constantMax : 2f);
        }
        // basic behavior - customize later
        if (destroyOnDeath)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }
}
