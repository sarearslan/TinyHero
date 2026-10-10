using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int maxHealth = 5;
    [SerializeField] float hurtDuration = 0.5f;

    int currentHealth;

    bool isDead;
    bool isHurting;

    Animator animator;
    PlayerAttack playerAttack;

    Coroutine hurtCoroutine;

    public int CurrentHealth => currentHealth;
    public bool IsDead => isDead;
    public bool IsHurting => isHurting;

    void Awake()
    {
        currentHealth = maxHealth;

        animator = GetComponent<Animator>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        if (isHurting && CompareTag("Player")) return;
        if (damage <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log(gameObject.name + " kalan can: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // Enemy'de hurt animasyonu olmadığı için sadece Player hurt olur.
        if (CompareTag("Player"))
        {
            if (hurtCoroutine != null)
            {
                StopCoroutine(hurtCoroutine);
            }

            hurtCoroutine = StartCoroutine(HurtRoutine());
        }
    }

    IEnumerator HurtRoutine()
    {
        isHurting = true;

        if (animator != null)
        {
            animator.SetBool("isHurting", true);
            animator.SetBool("isAttacking", false);
        }

        if (playerAttack != null)
        {
            playerAttack.SetHurting(true);
        }

        yield return new WaitForSeconds(hurtDuration);

        isHurting = false;

        if (animator != null)
        {
            animator.SetBool("isHurting", false);
        }

        if (playerAttack != null)
        {
            playerAttack.SetHurting(false);
        }

        hurtCoroutine = null;
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        isHurting = false;

        if (hurtCoroutine != null)
        {
            StopCoroutine(hurtCoroutine);
            hurtCoroutine = null;
        }

        SendMessage("OnDeath", SendMessageOptions.DontRequireReceiver);
    }
}