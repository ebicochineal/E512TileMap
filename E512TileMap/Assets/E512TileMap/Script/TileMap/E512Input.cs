using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class E512Input {
    public static void CameraWASDMove (float movespeed) {
        if (Input.GetKey(KeyCode.W)) { Camera.main.transform.position += new Vector3(0f, movespeed, 0f); }
        if (Input.GetKey(KeyCode.S)) { Camera.main.transform.position += new Vector3(0f, -movespeed, 0f); }
        if (Input.GetKey(KeyCode.A)) { Camera.main.transform.position += new Vector3(-movespeed, 0f, 0f); }
        if (Input.GetKey(KeyCode.D)) { Camera.main.transform.position += new Vector3(movespeed, 0f, 0f); }
    }

    public static void CameraArrowMove (float movespeed) {
        if (Input.GetKey(KeyCode.UpArrow)) { Camera.main.transform.position += new Vector3(0f, movespeed, 0f); }
        if (Input.GetKey(KeyCode.DownArrow)) { Camera.main.transform.position += new Vector3(0f, -movespeed, 0f); }
        if (Input.GetKey(KeyCode.LeftArrow)) { Camera.main.transform.position += new Vector3(-movespeed, 0f, 0f); }
        if (Input.GetKey(KeyCode.RightArrow)) { Camera.main.transform.position += new Vector3(movespeed, 0f, 0f); }
    }

    // public static void CameraGyroMove(float movespeed){
    // 	Vector3 g = Input.gyro.rotationRateUnbiased;
    // 	if(g.x > 0.1){
    // 		Camera.main.transform.position = new Vector3(g.x * movespeed, 0f, 0f);
    // 	}
    // }

    private static Vector3 middle = new Vector3();
    public static void CameraMouseMiddleMove () {
        if (Input.GetMouseButtonDown(2)) {
            middle = Input.mousePosition;
        }
        if (Input.GetMouseButton(2)) {
            Vector3 prev = Camera.main.ScreenToWorldPoint(middle);
            Vector3 now = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.transform.position += prev - now;
        }
        middle = Input.mousePosition;
    }

    private static Vector3 left = new Vector3();
    public static void CameraMouseLeftMove () {
        if (Input.GetMouseButtonDown(0)) {
            left = Input.mousePosition;
        }
        if (Input.GetMouseButton(0)) {
            Vector3 prev = Camera.main.ScreenToWorldPoint(left);
            Vector3 now = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.transform.position += prev - now;
        }
        left = Input.mousePosition;
    }
}
