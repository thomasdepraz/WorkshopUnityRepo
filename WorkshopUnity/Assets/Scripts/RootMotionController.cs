using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotionController : MonoBehaviour
{

    public Animator selfAnimator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        selfAnimator.SetFloat("Speed", Input.GetAxis("Vertical"));
        selfAnimator.SetFloat("Turn", Input.GetAxis("Horizontal"));
    }
}
