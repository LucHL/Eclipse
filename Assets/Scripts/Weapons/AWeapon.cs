using UnityEngine;

public class AWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] protected string weaponName = "weapon";
    [SerializeField] protected float damage = 1f;
    [SerializeField] protected float attackCoolDown = 0.5f;
    [SerializeField] protected float solidity = 100f;
}
