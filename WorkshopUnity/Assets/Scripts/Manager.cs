using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public KeyCode restartKey;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(restartKey))
        {
            SceneManager.LoadScene(0);
        }
    }
}
