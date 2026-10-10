using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float attackCooldown = 1f;

    Animator animator;
    Health enemyHealth;
    Health playerHealth;

    readonly HashSet<Collider2D> playerColliders =
        new HashSet<Collider2D>();

    float nextAttackTime;

    public bool PlayerInAttackRange
    {
        get
        {
            if (playerColliders.Count == 0) return false;
            if (playerHealth == null) return false;
            if (playerHealth.IsDead) return false;

            return true;
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        enemyHealth = GetComponent<Health>();
    }

    void Update()
    {
        if (enemyHealth != null && enemyHealth.IsDead)
        {
            animator.SetBool("isAttacking", false);
            return;
        }

        animator.SetBool(
            "isAttacking",
            PlayerInAttackRange
        );
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        AddPlayerCollider(collision);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        AddPlayerCollider(collision);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerColliders.Remove(collision);

        if (playerColliders.Count == 0)
        {
            playerHealth = null;
            animator.SetBool("isAttacking", false);
        }
    }

    void AddPlayerCollider(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerColliders.Add(collision);

        playerHealth =
            collision.GetComponentInParent<Health>();
    }

    public void DealDamage()
    {
        if (!PlayerInAttackRange) return;

        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackCooldown;

        playerHealth.TakeDamage(damage);
    }

    void OnDisable()
    {
        playerColliders.Clear();
        playerHealth = null;
    }
}