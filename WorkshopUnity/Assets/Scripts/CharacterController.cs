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

    [Header("References")]
    [SerializeField] Transform self;
    [SerializeField] Animator selfAnim;
    [SerializeField] Transform camTransform;
    [SerializeField] Transform vcamTransform;

    //private variables
    private float currentMovement;
    private Vector3 direction;
    private float currentRotation;
    private float horizontalTimeStamp;
    private float currentAcceleration;
    private float stick;
    private float distanceToCam;
    Vector3 lastDirection;
    Vector3 characterForward;


    // Start is called before the first frame update
    void Start()
    {
        distanceToCam = (self.position - vcamTransform.position).magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        HorizontalMovement();
        //VerticalMovement();
        CameraMovement();
    }
    private void HorizontalMovement()
    {
        //Get Player forward based on camera ||Works only if camera is looking at the player ||
        characterForward = camTransform.forward;
        characterForward.y = 0;
        characterForward = characterForward.normalized;
        Vector3 rg = new Vector3(characterForward.z, 0, -characterForward.x); 


        //Get inputs
        direction = new Vector3(Input.GetAxis(horizontalAxis), 0, Input.GetAxis(verticalAxis));
        currentMovement = Input.GetAxis(verticalAxis);
        currentRotation = Input.GetAxis(horizontalAxis);
        Vector3 desiredDir = characterForward * currentMovement + rg * currentRotation;

        //Process movement
        if (desiredDir!=Vector3.zero)
        {
            if (Vector3.Dot(desiredDir, lastDirection) < turnAccelerationThreshold)
                horizontalTimeStamp = 0;
            else horizontalTimeStamp += Time.deltaTime;
            currentAcceleration = groundAcceleration.Evaluate(horizontalTimeStamp);

            self?.Translate(desiredDir.normalized * horizontalSpeed  * currentAcceleration *  Time.deltaTime, Space.World);

            vcamTransform.Translate(desiredDir.normalized * horizontalSpeed * currentAcceleration * Time.deltaTime, Space.World);
        }
        else horizontalTimeStamp = 0;

        //Last frame value
        lastDirection = characterForward * currentMovement + rg * currentRotation;

        //Process rotation
        if(desiredDir.magnitude >0.1)
            self.forward = Vector3.Lerp(self.forward, desiredDir, Time.deltaTime*angularSpeed);

        float rotation = currentMovement >= 0 ? currentRotation : -currentRotation;
        //Process animation
        selfAnim.SetFloat("Turn", rotation);
        selfAnim.SetFloat("Speed", desiredDir.magnitude);
    }

    private void VerticalMovement()
    {
        throw new NotImplementedException();
    }

    private void CameraMovement()
    {
        vcamTransform.RotateAround(self.position, new Vector3(Input.GetAxis(secondVerticalAxis), -Input.GetAxis(secondHorizontalAxis), 0) ,Time.deltaTime*180);
    }
}
