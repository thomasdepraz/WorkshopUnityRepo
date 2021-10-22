using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController3D : MonoBehaviour
{
    [Header("KeyMapping")]
    [SerializeField] private KeyCode up;
    [SerializeField] private KeyCode down;
    [SerializeField] private KeyCode left;
    [SerializeField] private KeyCode right;
    [SerializeField] private KeyCode jumpKey;

    [Header("Stats")]
    [SerializeField]float horizontalSpeed;
    [SerializeField] float angularSpeed;
    [Range(-1f, 1f)]
    [SerializeField] float accelerationThreshold;
    [SerializeField] AnimationCurve groundAcceleration;
    [SerializeField] float jumpForce;

    [Header("References")]
    [SerializeField] Transform m_Transfrom;
    [SerializeField] CharacterRaycaster2D raycaster;
    [SerializeField] Animator anim;
    [SerializeField] Transform camTransfrom;
    [SerializeField] Rigidbody selfBody;

    [Header("Unity Events")]
    public UnityEvent onJumped;


    bool isGrounded;
    private float movement;
    float horizontalTimestamp;
    Vector3 lastDirection;
    private float currentRotation;
    

    void Start()
    {
    }

    void Update()
    {
        currentRotation = 0;
        movement = 0;

        HorizontalMovement();
        VerticalMovement();
    }

    private void HorizontalMovement()
    {
        //Get player forward
        Vector3 fw = m_Transfrom.position - camTransfrom.position;
        fw.y = 0;
        fw = fw.normalized;
        Vector3 rg = new Vector3(fw.z, 0, -fw.x);

        //Process Inputs
        if (Input.GetKey(left))
        {
            currentRotation--;
        }
        if (Input.GetKey(right))
        {
           currentRotation++;
        }
        if (Input.GetKey(up))
        {
            movement++;
        }
        if (Input.GetKey(down))
        {
            movement--;
        }

        if (movement != 0)
        {
            //Process Acceleration
            if (Vector3.Dot(fw * movement, lastDirection) < accelerationThreshold)
                horizontalTimestamp = Time.time;

            float accelerationStep = Time.time - horizontalTimestamp;
            float curAcceleration = groundAcceleration.Evaluate(accelerationStep);


            //Movement
            m_Transfrom?.Translate(fw * movement *  horizontalSpeed * curAcceleration * Time.deltaTime, Space.World);
            Debug.Log(fw);

        }
        else
        {
            horizontalTimestamp = Time.time;
        }


        //Update last frame value
        lastDirection = fw * movement;
        camTransfrom.LookAt(m_Transfrom.position);

        m_Transfrom?.Rotate(Vector3.up * angularSpeed * currentRotation * Time.deltaTime);
        anim.SetFloat("Turn", currentRotation);
        anim.SetFloat("Speed", movement);
    }

    /*private void VerticalMovement()
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



    }*/

    void VerticalMovement()
    {
        if (Input.GetKeyDown(jumpKey))
        {
            selfBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            anim.SetBool("isGrounded", false);
            anim.SetTrigger("Jump");

            onJumped.Invoke();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.position.y < m_Transfrom.position.y)
            SetGrounded();
    }

    public void SetGrounded()
    {
        isGrounded = true;
        anim.SetBool("isGrounded", true);
    }
}
