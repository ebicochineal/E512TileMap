using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

public class XPBGLimit : MonoBehaviour {
    // WindowsXP�pFPSLimit WebGL�ŃI���ɂ����Edge�Ȃǂ�FPS�ቺ���N���邩��

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {
        if (Time.deltaTime < 0.005f) {
            Thread.Sleep(1);
        }
    }
}
