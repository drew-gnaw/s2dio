using UnityEngine;

public abstract class EnemyClass : MonoBehaviour
{
    public int maxHealth;
    protected int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }
    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Enemy took " + damage + " damage.");

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    protected virtual void Die()
    {
        Debug.Log("Enemy died.");
        Destroy(gameObject);
    }
}