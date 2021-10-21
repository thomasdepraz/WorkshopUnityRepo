using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Header("Controller Mapping")]
    [SerializeField] private string horizontalAxis;
    [SerializeField] private string verticalAxis;
    [SerializeField] private string secondHorizontalAxis;
    [SerializeField] private string secondVerticalAxis;
    [SerializeField] private string jumpButton;

    [Header("Stats")]
    [SerializeField] float horizontalSpeed;
    [SerializeField] float angularSpeed;
    [Range(-1f, 1f)]
    [SerializeField] float turnAccelerationThreshold;
    [SerializeField] AnimationCurve groundAcceleration;
    [SerializeField] float jumpForce;
    [SerializeField] float CamToPlayerDistance;

    [Header("References")]
    [SerializeField] Transform self;
    [SerializeField] Animator selfAnim;
    [SerializeField] Rigidbody selfBody;
    [SerializeField] Transform camTransform;
    [SerializeField] Transform vcamTransform;

    //private variables
    bool isGrounded;
    float currentMovement;
    float currentRotation;
    float horizontalTimeStamp;
    float currentAcceleration;
    float distanceToCam;
    float camHeight;
    Vector3 movementDelta;
    Vector3 lastDirection;
    Vector3 lastPosition;
    Vector3 characterForward;


    void Start()
    {
        distanceToCam = (self.position - vcamTransform.position).magnitude;
        camHeight = vcamTransform.position.y - self.position.y;
    }

    void Update()
    {
        HorizontalMovement();
        VerticalMovement();
        CameraMovement();
    }

    private void HorizontalMovement()
    {
        //Get Player forward based on camera
        characterForward = camTransform.forward;
        characterForward.y = 0;
        characterForward = characterForward.normalized;
        Vector3 rg = new Vector3(characterForward.z, 0, -characterForward.x); 


        //Get inputs
        currentMovement = Input.GetAxis(verticalAxis);
        currentRotation = Input.GetAxis(horizontalAxis);


        //Cast input vetor in camera space
        Vector3 desiredDir = characterForward * currentMovement + rg * currentRotation;


        //Process movement
        if (desiredDir!=Vector3.zero)
        {
            if (Vector3.Dot(desiredDir, lastDirection) < turnAccelerationThreshold)
                horizontalTimeStamp = 0;
            else horizontalTimeStamp += Time.deltaTime;
            currentAcceleration = groundAcceleration.Evaluate(horizontalTimeStamp);

            self?.Translate(desiredDir.normalized * horizontalSpeed  * currentAcceleration *  Time.deltaTime, Space.World);
        }
        else horizontalTimeStamp = 0;

        //Last frame values
        lastDirection = characterForward * currentMovement + rg * currentRotation;
        movementDelta = self.position - lastPosition;
        lastPosition = new Vector3(self.position.x, self.position.y, self.position.z);


        //Process rotation
        if (desiredDir.magnitude >0.1)
        {
            self.forward = Vector3.Slerp(self.forward, desiredDir, Time.deltaTime*angularSpeed);
        }

        //Process animation
        float rotation = currentMovement >= 0 ? currentRotation : -currentRotation;
        selfAnim.SetFloat("Turn", rotation);
        selfAnim.SetFloat("Speed", desiredDir.magnitude);
    }

    private void VerticalMovement()
    {
        if (Input.GetButtonDown(jumpButton) && isGrounded)
        {
            selfBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            selfAnim.SetBool("isGrounded", false);
            selfAnim.SetTrigger("Jump");
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.position.y -0.1f < self.position.y)
            SetGrounded();
    }

    public void SetGrounded()
    {
        isGrounded = true;
        selfAnim.SetBool("isGrounded", true);
    }

    private void CameraMovement()
    {
        //Translate camera x z plane
        distanceToCam = (self.position - vcamTransform.position).magnitude;
        print(distanceToCam);
        float deltaY = movementDelta.y;
        movementDelta.y = 0;
        if (distanceToCam <= 0.5f + CamToPlayerDistance && distanceToCam >= CamToPlayerDistance - 0.5f)
        {
            vcamTransform.position = vcamTransform.position + movementDelta;
        }
        else if (distanceToCam > 0.5f + CamToPlayerDistance)
        {
            vcamTransform.Translate(vcamTransform.forward.normalized * horizontalSpeed * currentAcceleration * Time.deltaTime, Space.World);
        }

        //Translate camera y
        if (deltaY != 0)
            vcamTransform.position += new Vector3(0, deltaY, 0);

        //Add small rotation if cam is too low
        if (vcamTransform.position.y - self.position.y <= camHeight && deltaY < 0.1f)
            vcamTransform.position += new Vector3(0, camHeight - vcamTransform.position.y, 0) * Time.deltaTime;


        //Par manque de temps et pour eviter de devoir clamp la cam et réparer le gimball lock, je retire le controle de la rotation verticale de la cam
        vcamTransform.RotateAround(self.position, new Vector3(0, -Input.GetAxis(secondHorizontalAxis), 0) ,Time.deltaTime * 180 * Math.Abs(Input.GetAxis(secondHorizontalAxis)));
    }
}
