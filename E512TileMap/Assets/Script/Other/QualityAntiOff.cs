using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualityAntiOff : MonoBehaviour {

    // Use this for initialization
    void Start () {
        QualitySettings.antiAliasing = 0;
    }

    // Update is called once per frame
    void Update () {

    }
}
