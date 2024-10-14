using UnityEngine;

public abstract class WeaponClass : MonoBehaviour
{
    public string weaponName;
    public int damage;
    public float range;
    public LayerMask enemyLayers;

    public abstract void Attack(Transform attackPoint);
    public abstract void UseSkill();
}