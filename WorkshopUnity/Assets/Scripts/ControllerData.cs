using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "ControllerData", menuName = "Data/ControllerData", order = 1) ]
public class ControllerData : ScriptableObject
{
    [Header("Control Mapping")]
    public string horizontalAxis;
    public string verticalAxis;
    public string secondHorizontalAxis;
    public string secondVerticalAxis;
    public string jumpButton;

    [Header("Stats")]
    public float horizontalSpeed;
    public float angularSpeed;
    [Range(-1f, 1f)]
    public float turnAccelerationThreshold;
    public AnimationCurve groundAcceleration;
    public float jumpForce;
    public float CamToPlayerDistance;
}
