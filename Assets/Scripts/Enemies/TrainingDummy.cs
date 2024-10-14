using UnityEngine;

public class TrainingDummy : EnemyClass
{
    public override void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log("Training dummy took " + damage + " damage.");
    }

}