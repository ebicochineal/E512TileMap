using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelPerfect : MonoBehaviour {
    public int pixels_per_unit = 100;
    public float scale = 1f;
    Camera cam;
    
    void Start () {
        this.cam = GetComponent<Camera>();
    }

    //void OnPreRender() {
    //    if (this.cam) {
    //        this.cam.orthographicSize = (float)Screen.height / (float)this.pixels_per_unit / 2.0f / this.scale;
    //    }
    //}

    void Update () {
        if (this.cam) {
            this.cam.orthographicSize = (float)Screen.height / (float)this.pixels_per_unit / 2.0f / this.scale;
        }
    }
}
