using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float sprintSpeed = 18f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(10f, 20f);

    bool isSprinting;

    Vector2 moveInput;
    Health health;

    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;

    bool isAlive = true;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        health = GetComponent<Health>();
    }

    void Update()
    {
        if (!isAlive) return;

        CheckInstantDeath();

        if (!isAlive) return;

        if (health != null && health.IsHurting)
        {
            myRigidbody.linearVelocity =
                new Vector2(0f, myRigidbody.linearVelocity.y);

            myAnimator.SetBool("isRunning", false);
            return;
        }

        Run();
        FlipSprite();
    }

    void OnMove(InputValue value)
    {
        if (!isAlive) return;

        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!isAlive) return;

        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            return;

        if (value.isPressed)
        {
            myRigidbody.linearVelocity += new Vector2(0f, jumpSpeed);
        }
    }

    void OnSprint(InputValue value)
    {
        if (!isAlive) return;

        isSprinting = value.isPressed;
    }

    void Run()
    {
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        myRigidbody.linearVelocity =
            new Vector2(moveInput.x * currentSpeed,
            myRigidbody.linearVelocity.y);

        myAnimator.SetBool(
            "isRunning",
            Mathf.Abs(myRigidbody.linearVelocity.x) > 0.01f);
    }

    void FlipSprite()
    {
        if (Mathf.Abs(myRigidbody.linearVelocity.x) > 0.01f)
        {
            transform.localScale =
                new Vector2(Mathf.Sign(myRigidbody.linearVelocity.x), 1);
        }
    }

    void OnDeath()
    {
        if (!isAlive) return;

        isAlive = false;
        moveInput = Vector2.zero;

        myAnimator.SetBool("isRunning", false);
        myAnimator.SetBool("isAttacking", false);
        myAnimator.SetTrigger("Dying");

        myBodyCollider.enabled = false;
        myFeetCollider.enabled = false;

        myRigidbody.linearVelocity = deathKick;
    }

    void CheckInstantDeath()
    {
        if (myBodyCollider.IsTouchingLayers(
            LayerMask.GetMask("Hazards", "Water")))
        {
            health.TakeDamage(health.CurrentHealth);
        }
    }
}