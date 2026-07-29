using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EntityController : MonoBehaviour
{
    [Header("NPC Info")]
    [SerializeField] public NPCIdentity identity;

    [Header("Entity Combat Settings")]
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float attackCooldown = 1.5f;

    protected bool isDead = false;
    protected NavMeshAgent agent;
    protected Animator animator;
    protected Canvas canvas;

    protected float lastAttackTime;

    public bool IsDead => isDead;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        canvas = GetComponentInChildren<Canvas>();
    }

    public virtual void Initialize(NPCIdentity npcIdentity)
    {
        identity = npcIdentity;
        isDead = !identity.isAlive;

        if (isDead) {
            Kill();
            return;
        }

        if (agent != null) {
            agent.enabled = false;

            if (NavMesh.SamplePosition(identity.position, out NavMeshHit hit, 30f, NavMesh.AllAreas)) {
                identity.position = hit.position;
                
                transform.position = hit.position;
                
                agent.enabled = true;
                agent.Warp(hit.position);
            } else {
                transform.position = identity.position;
                Debug.LogError($"[NPCControllers] Impossible de trouver le NavMesh près de {identity.position} pour le PNJ {identity.npcId}");
                return;
            }
        }
    }

    protected virtual void Update()
    {
        if (isDead)
            return;

        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh && animator != null && animator.enabled)
            animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    #region Combat (One-Hit Kill & Attaque)

    /// <summary>
    /// One shot
    /// </summary>
    public virtual void Kill()
    {
        if (isDead)
            return;

        isDead = true;
        identity.isAlive = false;

        if (agent != null && agent.enabled)
            agent.enabled = false;

        if (animator != null)
            animator.SetTrigger("Die");
        
        Destroy(gameObject);
    }

    /// <summary>
    /// Tente de porter un coup mortel à une cible si elle est à portée et que le cooldown est prêt.
    /// </summary>
    public virtual bool TryAttack(EntityController target)
    {
        if (isDead || target == null || target.IsDead)
            return false;

        if (Time.time >= lastAttackTime + attackCooldown) {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            if (distanceToTarget <= attackRange) {
                PerformAttack(target);
                lastAttackTime = Time.time;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Exécute l'animation d'attaque et tue la cible en 1 coup.
    /// </summary>
    protected virtual void PerformAttack(EntityController target)
    {
        if (animator != null)
            animator.SetTrigger("Attack");

        target.Kill();
    }

    #endregion

    #region Navigation Helpers

    public virtual void SetDestination(Vector3 targetPosition)
    {
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh)
            return;

        agent.SetDestination(targetPosition);
    }

    #endregion

    #region Optimisation Camera (Culling)

    protected virtual void OnBecameInvisible()
    {
        if (canvas != null)
            canvas.enabled = false;

        if (animator != null)
            animator.enabled = false;
    }

    protected virtual void OnBecameVisible()
    {
        if (canvas != null)
            canvas.enabled = true;

        if (animator != null)
            animator.enabled = true;
    }

    #endregion
}
