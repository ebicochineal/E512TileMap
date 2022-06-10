using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CellMove : DotCollision {
    public bool avoid = false;
    public int wait = 0;
    public int speed = 2;
    public int dist = 16;
    
    private int twait = 0;
    private int counter = 0;
    private int xlast = 0;
    private int ylast = 0;
    private int cmx = 0;
    private int cmy = 0;
    private char last_axis = 'y';
    private int order = 0;
    private int cols = 0;
    
    public const int Up = 1;
    public const int Right = 4;
    public const int Down = 16;
    public const int Left = 64;
    
    [HideInInspector]
    public bool iscellmovestart = false;
    [HideInInspector]
    public bool iscellmove = false;
    
    // Update is called once per frame
    public void Move () {
        this.iscellmovestart = false;
        
        this.SurroundingCollision();
        this.SetOrder();
        this.order = 0;
        if (this.iscellmove && this.twait == 0 && this.counter > 0) {
            this.SetLastAxis();
            this.SimpleMove(this.cmx, this.cmy);
            this.counter -= 1;
            this.twait = this.wait;

        } else if (this.iscellmove && this.twait == 0) {
            this.SetLastAxis();
            this.SimpleMove(this.xlast, this.ylast);
            this.iscellmove = false;
            this.xlast = 0;
            this.ylast = 0;
            this.cmx = 0;
            this.cmy = 0;
        } else {
            this.twait -= 1;
        }
        
    }

    public int GetDirection () {
        if (this.cmx != 0) { return this.cmx > 0 ? CellMove.Right : CellMove.Left; }
        if (this.cmy != 0) { return this.cmy > 0 ? CellMove.Up : CellMove.Down; }
        return 0;
    }

    public void SetRight () {
        this.twait = 0;
        this.counter = this.dist / this.speed;
        this.xlast = this.dist % this.speed;
        this.cmx = this.speed;
        this.iscellmove = true;
        this.iscellmovestart = true;
    }

    public void SetLeft () {
        this.twait = 0;
        this.counter = this.dist / this.speed;
        this.xlast = -(this.dist % this.speed);
        this.cmx = -this.speed;
        this.iscellmove = true;
        this.iscellmovestart = true;
    }

    public void SetUp () {
        this.twait = 0;
        this.counter = this.dist / this.speed;
        this.ylast = this.dist % this.speed;
        this.cmy = this.speed;
        this.iscellmove = true;
        this.iscellmovestart = true;
    }

    public void SetDown () {
        this.twait = 0;
        this.counter = this.dist / this.speed;
        this.ylast = -(this.dist % this.speed);
        this.cmy = -this.speed;
        this.iscellmove = true;
        this.iscellmovestart = true;
    }

    private void SetLastAxis () {
        if (this.cmx != 0) { this.last_axis = 'x'; }
        if (this.cmy != 0) { this.last_axis = 'y'; }
    }
    //    128  1   2
    //     64      4
    //     32  16  8
    private void SetOrder () {
        if (this.Test(0, 0).Count > 0) { return; }

        this.OneOrder();
        
        if (this.order == 0 || this.iscellmove) { return; }

        if (this.order == CellMove.Up) {
            if (this.ColsHasFlag(this.order) && this.avoid) {
                if (this.ColsNotHasFlag(2 + 4)) {
                    this.SetRight();
                    return;
                }
                if (this.ColsNotHasFlag(64 + 128)) {
                    this.SetLeft();
                    return;
                }
            } else {
                if (this.ColsNotHasFlag(this.order)) { this.SetUp(); }
            }
        }
        if (this.order == CellMove.Right) {
            if (this.ColsHasFlag(this.order) && this.avoid) {
                if (this.ColsNotHasFlag(1 + 2)) {
                    this.SetUp();
                    return;
                }
                if (this.ColsNotHasFlag(8 + 16)) {
                    this.SetDown();
                    return;
                }
            } else {
                if (this.ColsNotHasFlag(this.order)) { this.SetRight(); }
            }
        }
        if (this.order == CellMove.Down) {
            if (this.ColsHasFlag(this.order) && this.avoid) {
                if (this.ColsNotHasFlag(4 + 8)) {
                    this.SetRight();
                    return;
                }
                if (this.ColsNotHasFlag(32 + 64)) {
                    this.SetLeft();
                    return;
                }
            } else {
                if (this.ColsNotHasFlag(this.order)) { this.SetDown(); }
            }
        }
        if (this.order == CellMove.Left) {
            if (this.ColsHasFlag(this.order) && this.avoid) {
                if (this.ColsNotHasFlag(1 + 128)) {
                    this.SetUp();
                    return;
                }
                if (this.ColsNotHasFlag(16 + 32)) {
                    this.SetDown();
                    return;
                }
            } else {
                if (this.ColsNotHasFlag(this.order)) { this.SetLeft(); }
            }
        }
    }

    public void Order (int d) {
        if (this.iscellmove == false) { this.order |= d; }
    }

    private void OneOrder () {
        // ud ld反対が入力されてたら打ち消し
        if (this.OrderHasFlag(1) && this.OrderHasFlag(16)) {
            this.order ^= 1;
            this.order ^= 16;
        }
        if (this.OrderHasFlag(4) && this.OrderHasFlag(64)) {
            this.order ^= 4;
            this.order ^= 64;
        }

        // １方向接触しているなら接触してない入力を優先
        this.TwoAxisContactOneAxis();

        // 斜め入力時どちらに動くか決める
        if (this.order == 5 << 0) {
            this.order = this.last_axis == 'x' ? 1 << 0 : 4 << 0;
            return;
        }
        if (this.order == 5 << 2) {
            this.order = this.last_axis == 'y' ? 1 << 2 : 4 << 2;
            return;
        }
        if (this.order == 5 << 4) {
            this.order = this.last_axis == 'x' ? 1 << 4 : 4 << 4;
            return;
        }
        if (this.order == (1 << 6 | 1)) {
            this.order = this.last_axis == 'y' ? 1 << 6 : 1 << 0;
            return;
        }
    }

    private void SurroundingCollision () {
        this.cols = 0;
        var d = this.dist;
        this.cols |= this.Test(0, d).Count > 0 ? 1 << 0 : 0;
        this.cols |= this.Test(d, d).Count > 0 ? 1 << 1 : 0;

        this.cols |= this.Test(d, 0).Count > 0 ? 1 << 2 : 0;
        this.cols |= this.Test(d, -d).Count > 0 ? 1 << 3 : 0;

        this.cols |= this.Test(0, -d).Count > 0 ? 1 << 4 : 0;
        this.cols |= this.Test(-d, -d).Count > 0 ? 1 << 5 : 0;

        this.cols |= this.Test(-d, 0).Count > 0 ? 1 << 6 : 0;
        this.cols |= this.Test(-d, d).Count > 0 ? 1 << 7 : 0;
    }

    private bool ColsHasFlag (int a) { return (this.cols & a) == a; }
    private bool ColsNotHasFlag (int a) { return (this.cols & a) == 0; }
    private bool OrderHasFlag (int a) { return (this.order & a) == a; }

    private void TwoAxisContactOneAxis () {
        if (this.order == 1 || this.order == 4 || this.order == 16 || this.order == 64) { return; }
        if (this.OrderHasFlag(1) && this.ColsHasFlag(1)) { this.order ^= 1; }
        if (this.OrderHasFlag(4) && this.ColsHasFlag(4)) { this.order ^= 4; }
        if (this.OrderHasFlag(16) && this.ColsHasFlag(16)) { this.order ^= 16; }
        if (this.OrderHasFlag(64) && this.ColsHasFlag(64)) { this.order ^= 64; }
    }
}
