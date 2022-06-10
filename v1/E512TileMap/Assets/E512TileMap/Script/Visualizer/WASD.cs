using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WASD : Visualizer {
    // WASDキーをが入力されると画面に表示
    public override string UpdateText () {
        var r = "";
        if (Input.GetKey(KeyCode.W)) { r += "W"; }
        if (Input.GetKey(KeyCode.A)) { r += "A"; }
        if (Input.GetKey(KeyCode.S)) { r += "S"; }
        if (Input.GetKey(KeyCode.D)) { r += "D"; }
        return r;
    }
}
