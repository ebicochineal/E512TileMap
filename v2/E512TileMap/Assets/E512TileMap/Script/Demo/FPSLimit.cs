using UnityEngine;
using System.Collections;

public class FPSLimit : MonoBehaviour {

    // Use this for initialization
    void Start () {
        //QualitySettings.SetQualityLevel(0, true);
        Time.fixedDeltaTime = 0.01666f;

        if (Application.platform == RuntimePlatform.WebGLPlayer) {
            // FixedUpdate
        } else {
            Application.targetFrameRate = 60;
        }
    }

    // Update is called once per frame
    void Update () {

    }
}
