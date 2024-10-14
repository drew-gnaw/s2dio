using UnityEngine;

public class Mace : WeaponClass
{
    public Transform attackPoint;
    public float radius;
    
    public override void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, radius, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyClass>().TakeDamage(damage);
        }
    }

    public override void UseSkill()
    {
        
    }
}