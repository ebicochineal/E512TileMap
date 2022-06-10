using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCount : Visualizer {
    float timer = 1f;
    int fps = 60;
    int fpscount = 0;

    public override string UpdateText () {
        this.timer -= Time.deltaTime;
        this.fpscount += 1;
        if (this.timer < 0) {
            this.timer = 1f;
            this.fps = this.fpscount;
            this.fpscount = 0;
        }
        return this.fps.ToString();
    }
}
