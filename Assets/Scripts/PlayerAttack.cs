using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRange = 1f;

    [SerializeField] int damage = 1;

    [SerializeField] float attackDuration = 0.4f;
    [SerializeField] float attackCooldown = 0.6f;

    [SerializeField] LayerMask enemyLayer;

    Animator animator;
    Health health;

    bool isAttacking;
    bool isHurting;
    bool canAttack = true;

    Coroutine attackCoroutine;

    void Awake()
    {
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
    }

    void OnAttack(InputValue value)
    {
        if (!value.isPressed) return;

        if (!canAttack) return;
        if (isAttacking) return;
        if (isHurting) return;

        if (health != null && health.IsDead) return;

        attackCoroutine = StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        canAttack = false;
        isAttacking = true;

        animator.SetBool("isAttacking", true);

        yield return new WaitForSeconds(attackDuration);

        isAttacking = false;
        animator.SetBool("isAttacking", false);

        float remainingCooldown = attackCooldown - attackDuration;

        if (remainingCooldown > 0f)
        {
            yield return new WaitForSeconds(remainingCooldown);
        }

        canAttack = true;
        attackCoroutine = null;
    }

    // Attack animasyonunda, silahın düşmana değdiği kareye
    // Animation Event olarak DealDamage ekleyeceğiz.
    public void DealDamage()
    {
        if (!isAttacking) return;
        if (isHurting) return;
        if (attackPoint == null) return;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayer);

        foreach (Collider2D enemyCollider in enemies)
        {
            Health enemyHealth = enemyCollider.GetComponent<Health>();

            if (enemyHealth == null)
            {
                enemyHealth = enemyCollider.GetComponentInParent<Health>();
            }

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }
    }

    public void SetHurting(bool value)
    {
        isHurting = value;

        if (isHurting)
        {
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }

            isAttacking = false;
            animator.SetBool("isAttacking", false);

            canAttack = false;
        }
        else
        {
            canAttack = true;
        }
    }

    void OnDeath()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        isAttacking = false;
        canAttack = false;

        animator.SetBool("isAttacking", false);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}