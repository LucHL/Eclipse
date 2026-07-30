using UnityEngine;

public class AWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] protected string weaponName = "weapon";
    [SerializeField] protected float damage = 1f;
    [SerializeField] protected float attackCoolDown = 0.5f;
    [SerializeField] protected float solidity = 100f;
    [SerializeField] protected Animator weaponAnimator;

    protected MeshSlicer slicer;
    protected Collider weaponCollider;
    protected bool isAttacking = false;

    protected virtual void Awake()
    {
        slicer = FindFirstObjectByType<MeshSlicer>();
        weaponCollider = GetComponent<BoxCollider>();
        
        if (weaponCollider != null) {
            weaponCollider.isTrigger = true;
            weaponCollider.enabled = false;
        }
    }

    public virtual void Attack()
    {
        isAttacking = true;

        if (weaponAnimator != null)
            weaponAnimator.SetTrigger("Attack");
    }

    #region Animation Events (called in animation event)

    /// <summary>
    /// Event 1 : Enable Hitbox
    /// </summary>
    protected virtual void EnableWeaponHitbox()
    {
        if (weaponCollider != null)
            weaponCollider.enabled = true;
    }

    /// <summary>
    /// Event 2 : Disable Hitbox
    /// </summary>
    protected virtual void DisableWeaponHitbox()
    {
        if (weaponCollider != null)
            weaponCollider.enabled = false;

        isAttacking = false;
    }

    #endregion

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cuttable")) {
            if (slicer != null) {
                EntityController entityController = other.GetComponentInParent<EntityController>();
                if (entityController != null)
                    entityController.Kill();

                slicer.CutObject(other.gameObject, transform.position, transform.up);

                DisableWeaponHitbox();
            }
        }
    }
}
