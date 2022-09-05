using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public float MELEE_WEAPON_ACTIVE_TIME = 0.1f;
    public const int MAX_JUMP_COUNT = 2;
    public const float INPUT_DEAD_ZONE = 0.1f;

    public float maxXSpeed;
    public float maxYSpeed;

    private float inputX;
    private bool inputY;

    public float jumpForce;

    private Rigidbody2D rb;

    private int jumpCount;

    private bool isGrounded;
    private MeleeWeapon meleeWeapon;
    private TimerManager timerManager;
    private const int MELEE_TIMER = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        meleeWeapon = GetComponentInChildren<MeleeWeapon>();
        meleeWeapon.gameObject.SetActive(false);
        timerManager = new TimerManager();
    }

    void Update()
    {
        timerManager.Update(Time.deltaTime);

        inputX = Input.GetAxisRaw("Horizontal");

        if (jumpCount < MAX_JUMP_COUNT && Input.GetKeyDown(KeyCode.Space))
        {
            inputY = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            MeleeAttack();
        }
    }

    private void MeleeAttack()
    {
        if (timerManager.ContainsTimer(MELEE_TIMER))
        {
            return;
        }

        meleeWeapon.gameObject.SetActive(true);
        List<Action> actions = new List<Action>();
        actions.Add(() => meleeWeapon.gameObject.SetActive(false));
        ActionTimer actionTimer = new ActionTimer(MELEE_TIMER, actions, MELEE_WEAPON_ACTIVE_TIME);
        timerManager.RegisterTimer(actionTimer);
    }

    private void FixedUpdate()
    {

        if (inputX > INPUT_DEAD_ZONE || inputX < -INPUT_DEAD_ZONE)
        {
            rb.AddForce(new Vector2(inputX * maxYSpeed, 0f), ForceMode2D.Impulse);
        }

        if (inputY)
        {
            jumpCount++;
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            inputY = false;
        }

        float currentXSpeed = rb.velocity.x;
        float currentYSpeed = rb.velocity.y;

        Vector2 clampedVelocity = rb.velocity;

        clampedVelocity.x = Mathf.Clamp(currentXSpeed, -maxXSpeed, maxXSpeed);
        clampedVelocity.y = Mathf.Clamp(currentYSpeed, -maxYSpeed, maxYSpeed);

        rb.velocity = clampedVelocity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            isGrounded = false;
        }
    }
}