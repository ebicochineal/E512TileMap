using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelPerfect : MonoBehaviour {
    public int pixels_per_unit = 100;
    public float scale = 1f;
    private Camera cam;
    void Start () {
        this.cam = this.GetComponent<Camera>();
        
        
    }

    //void OnPreRender() {
    //    if (this.cam) {
    //        this.cam.orthographicSize = (float)Screen.height / (float)this.pixels_per_unit / 2.0f / this.scale;
    //    }
    //}

    void Update () {
        if (this.cam) {
            if (this.cam.targetTexture != null) {
                this.cam.orthographicSize = (float)this.cam.targetTexture.height / (float)this.pixels_per_unit / 2.0f / this.scale;
            } else {
                this.cam.orthographicSize = (float)Screen.height / (float)this.pixels_per_unit / 2.0f / this.scale;
            }
        }
    }
}
