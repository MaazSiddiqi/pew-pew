using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlowMo : MonoBehaviour
{

    public static KeyCode slowMoKey = KeyCode.LeftShift;
    public float slowMoDuration = 3f;

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(slowMoKey)){
            InvokeSlowMotion();
        }

        if(Input.GetKeyUp(slowMoKey)){
            StopSlowMotion();
        }
    }

    public void InvokeSlowMotion(){
        GameManager.instance.StartSlowMotion();
    }

    public void StopSlowMotion(){
        GameManager.instance.StopSlowMotion();
    }
}
