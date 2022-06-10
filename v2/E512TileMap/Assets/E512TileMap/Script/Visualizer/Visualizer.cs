using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualizer : MonoBehaviour {
    private int vcount = 0;
    private string vtext = "";
    GUIStyle style;

    // Use this for initialization
    void Start () {
        this.style = new GUIStyle();
        this.style.fontSize = 24;
        this.style.normal.textColor = Color.red;
    }

    // Update is called once per frame
    void Update () {
        var c = 0;
        foreach (var i in this.GetComponents<Visualizer>()) {
            if (i == this) { break; }
            if (i.enabled) { c += 1; }
        }
        this.vcount = c;
        this.vtext = this.UpdateText();
    }

    public virtual string UpdateText () { return ""; }

    void OnGUI () {
        GUI.Label(new Rect(0, 24 * this.vcount , 256, 32), this.vtext, this.style);
    }
}

