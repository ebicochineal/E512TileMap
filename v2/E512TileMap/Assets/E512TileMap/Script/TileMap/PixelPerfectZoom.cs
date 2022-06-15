using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PixelPerfect))]
public class PixelPerfectZoom : MonoBehaviour {

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {
        if (Input.mouseScrollDelta.y < 0) {
        //if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (this.GetComponent<PixelPerfect>().scale > 0.125) {
                this.GetComponent<PixelPerfect>().scale *= 0.5f;
            }

        }

        if (Input.mouseScrollDelta.y > 0) {
        //if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if (this.GetComponent<PixelPerfect>().scale < 8) {
                this.GetComponent<PixelPerfect>().scale *= 2f;
            }
        }
    }
}
