using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    [Header("KeyMapping")]
    [SerializeField] private KeyCode up;
    [SerializeField] private KeyCode down;
    [SerializeField] private KeyCode left;
    [SerializeField] private KeyCode right;

    [Header("Stats")]
    [SerializeField]float horizontalSpeed;
    [SerializeField] float defaultGravity;
    [SerializeField] int maxAllowedJumps;
    [Range(1,10)]
    [SerializeField] float jumpReleaseAcceleration;
    [SerializeField] AnimationCurve groundAcceleration;
    [SerializeField] AnimationCurve jumpCurve;

    [Header("References")]
    [SerializeField] Transform m_Transfrom;
    [SerializeField] CharacterRaycaster2D raycaster;
    [SerializeField] Transform graphics;
    [SerializeField] Animator anim;

    bool isGrounded;
    private Vector2 movement;
    float horizontalTimestamp;
    private float curJumpDuration;
    bool isJumping;
    float lastDirection;
    int remainingJumps;

    void Start()
    {
        remainingJumps = maxAllowedJumps;
    }

    void Update()
    {
        movement = Vector2.zero;
        HorizontalMovement();
        VerticalMovement();
    }

    private void HorizontalMovement()
    {
        //Process Inputs
        movement = Vector2.zero;
        if (Input.GetKey(left))
        {
            movement.x--;
            graphics.localScale = new Vector3(-Mathf.Abs(graphics.localScale.x), graphics.localScale.y, graphics.localScale.z);
        }
        if (Input.GetKey(right))
        {
            movement.x++;
            graphics.localScale = new Vector3(Mathf.Abs(graphics.localScale.x), graphics.localScale.y, graphics.localScale.z);
        }

        //Process Acceleration
        if (movement.x != lastDirection)
            horizontalTimestamp = Time.time;

        float accelerationStep = Time.time - horizontalTimestamp;
        float curAcceleration = groundAcceleration.Evaluate(accelerationStep);


        //Movement
        m_Transfrom?.Translate(movement * horizontalSpeed * curAcceleration * Time.deltaTime);

        //Update last frame value
        lastDirection = movement.x;

        anim.SetFloat("Speed", Mathf.Abs(movement.x) * horizontalSpeed * curAcceleration * Time.deltaTime);
    }

    private void VerticalMovement()
    {
        if(Input.GetKeyDown(up))
        {
            if(remainingJumps > 0)
            {
                remainingJumps--;
                isJumping = true;
                isGrounded = false;
                anim.SetBool("isGrounded", false);
                anim.SetTrigger("Jump");
                curJumpDuration = 0;
            }
        }
        bool movesUp = false;
        if (isJumping)
        {
            curJumpDuration += Time.deltaTime * (Input.GetKey(up) ? 1 : jumpReleaseAcceleration);
            movement.y = defaultGravity * jumpCurve.Evaluate(curJumpDuration);
            if (movement.y >= 0) movesUp = true;

            if(curJumpDuration > jumpCurve.keys[jumpCurve.keys.Length - 1].time)
                isJumping = false;
        }
        else movement.y = defaultGravity * -1f;


        if (raycaster.ThrowRays(movesUp ? RayDirection.Up : RayDirection.Down))
        {
            if(!movesUp)
            {
                isGrounded = true;
                isJumping = false;
                anim.SetBool("isGrounded", true);
                remainingJumps = maxAllowedJumps;
            }
        }
        else
        {
            if(!movesUp)
            {
                isGrounded = false;
                anim.SetBool("isGrounded", false);
            }
            m_Transfrom?.Translate(Vector3.up * defaultGravity * movement.y * Time.deltaTime);
        }



    }
}
