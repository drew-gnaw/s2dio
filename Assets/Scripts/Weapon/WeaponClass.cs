using UnityEngine;

public abstract class WeaponClass : MonoBehaviour
{
    public int damage;
    public LayerMask enemyLayers;

    public abstract void Attack();
    public abstract void UseSkill();
}