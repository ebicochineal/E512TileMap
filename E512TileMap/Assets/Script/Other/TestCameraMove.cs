using UnityEngine;
using System.Collections;

public class TestCameraMove : MonoBehaviour {

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {
        E512Input.CameraArrowMove(1f);
        if (Application.platform == RuntimePlatform.Android) {
            E512Input.CameraMouseLeftMove();
        }
    }
}
