using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DotCollision : DotMove {
    public E512TileMapData map;
    public int layer = 0;

    [HideInInspector]
    public float gravity;
    [HideInInspector]
    public bool isground;
    
    void Start () {
        this.Init();
    }

    // Start
    public override void Init () {
        if (!this.init) {
            if (this.map == null) { this.map = E512TileMapData.SceneMap; }
            this.BaseInit();
        }
    }

    /// <summary>
    /// ドット単位で移動と衝突したかを返す
    /// </summary>
    public bool MoveXDeltaTime (float v) { return this.MoveX(v * Time.deltaTime * 60f); }


    /// <summary>
    /// ドット単位で移動と衝突したかを返す
    /// </summary>
    public bool MoveX (float v) {
        if (this.InNoPassable()) { return true; }
        int dpx = this.ToDot(this.px + v);
        int dpy = this.ToDot(this.py);
        bool b = false;
        if (v < 0) {// left
            int l = this.DotCPos(dpx - this.halfwidth);
            int r = this.DotCPos(this.ToDot(this.px) + this.halfwidth - 1);
            int u = this.DotCPos(dpy + this.halfheight - 1);
            int d = this.DotCPos(dpy - this.halfheight);
            int c = l - 1;
            foreach (var i in E512Pos.BoxList(new E512Pos(l, d), new E512Pos(r, u))) {
                if (this.GetCollision(i) == TileCollisionType.NoPassable) {
                    c = i.x > c ? i.x : c;
                    b = true;
                }
            }
            if (b && this.DotCPos(this.dpx) != c) {
                this.dpx = (c * this.tilesize) + this.halfwidth + this.tilesize;
                this.px = this.dpx * this.dotsize;
            } else {
                this.dpx = dpx;
                this.px += v;
            }
        } else {// right
            int l = this.DotCPos(this.ToDot(this.px) - this.halfwidth);
            int r = this.DotCPos(dpx + this.halfwidth - 1);
            int u = this.DotCPos(dpy + this.halfheight - 1);
            int d = this.DotCPos(dpy - this.halfheight);
            int c = r + 1;
            foreach (var i in E512Pos.BoxList(new E512Pos(l, d), new E512Pos(r, u))) {
                if (this.GetCollision(i) == TileCollisionType.NoPassable) {
                    c = i.x < c ? i.x : c;
                    b = true;
                }
            }
            if (b) {
                this.dpx = (c * this.tilesize) - this.halfwidth;
                this.px = this.dpx * this.dotsize;
            } else {
                this.dpx = dpx;
                this.px += v;
            }
        }
        this.FixPos();
        return b;
    }

    /// <summary>
    /// ドット単位で移動と衝突したかを返す
    /// </summary>
    public bool MoveX (int v) {
        if (this.InNoPassable()) { return true; }
        int dpx = this.dpx + v;
        int dpy = this.dpy;
        bool b = false;
        if (v < 0) {// left
            int l = this.DotCPos(dpx - this.halfwidth);
            int r = this.DotCPos(this.ToDot(this.px) + this.halfwidth - 1);
            int u = this.DotCPos(dpy + this.halfheight - 1);
            int d = this.DotCPos(dpy - this.halfheight);
            int c = l - 1;
            foreach (var i in E512Pos.BoxList(new E512Pos(l, d), new E512Pos(r, u))) {
                if (this.GetCollision(i) == TileCollisionType.NoPassable) {
                    c = i.x > c ? i.x : c;
                    b = true;
                }
            }
            if (b && this.DotCPos(this.dpx) != c) {
                this.dpx = (c * this.tilesize) + this.halfwidth + this.tilesize;
            } else {
                this.dpx = dpx;
            }
        } else {// right
            int l = this.DotCPos(this.ToDot(this.px) - this.halfwidth);
            int r = this.DotCPos(dpx + this.halfwidth - 1);
            int u = this.DotCPos(dpy + this.halfheight - 1);
            int d = this.DotCPos(dpy - this.halfheight);
            int c = r + 1;
            foreach (var i in E512Pos.BoxList(new E512Pos(l, d), new E512Pos(r, u))) {
                if (this.GetCollision(i) == TileCollisionType.NoPassable) {
                    c = i.x < c ? i.x : c;
                    b = true;
                }
            }
            if (b) {
                this.dpx = (c * this.tilesize) - this.halfwidth;
            } else {
                this.dpx = dpx;
            }
        }
        this.px = this.dpx * this.dotsize;
        this.FixPos();
        return b;
    }


    /// <summary>
    /// ドット単位で移動と衝突したかを返す
    /// </summary>
    public bool MoveYDeltaTime (float v) { return this.MoveY(v * Time.deltaTime * 60f); }


    /// <summary>
    /// ドット単位で移動と衝突したかを返す
    /// </summary>
    public bool MoveY (float v) {
        if (this.InNoPassable()) { return true; }
        int dpx = this.ToDot(this.px);
        int dpy = this.ToDot(this.py + v);
        bool b = false;
        if (v < 0) {// down
            int l = this.DotCPos(dpx - this.halfwidth);
            int r = this.DotCPos(dpx + this.halfwidth - 1);
            int u = this.DotCPos(this.ToDot(this.py) + this.halfheight - 1);
            int d = this.DotCPos(dpy - this.halfheight);
            int c = d - 1;
            foreach (var i in E512Pos.BoxList(new E512Pos(l, d), new E512Pos(r, u))) {
                if (this.GetCollision(i) == TileCollisionType.NoPassable) {
                    c = i.y > c ? i.y : c;
                    b = true;
                }
            }
            if (b) {
                this.dpy = (c * this.tilesize) + this.halfheight + this.tilesize;
                this.py = this.dpy * this.dotsize;
            } else {
                this.dpy = dpy;
                this.py += v;
            }
        } else {// up
            int l = this.DotCPos(dpx - this.halfwidth);
            int r = this.DotCPos(dpx + this.halfwidth - 1);
            int u = this.DotCPos(dpy + this.halfheight - 1);
            int d = this.DotCPos(this.ToDot(this.py) - this.halfheight);
            int c = u + 1;
            foreach (var i in E512Pos.BoxList(new E512Pos(l, d), new E512Pos(r, u))) {
                if (this.GetCollision(i) == TileCollisionType.NoPassable) {
                    c = i.y < c ? i.y : c;
                    b = true;
                }
            }
            if (b && this.DotCPos(this.dpy) != c) {
                this.dpy = (c * this.tilesize) - this.halfheight;
                this.py = this.dpy * this.dotsize;
            } else {
                this.dpy = dpy;
                this.py += v;
            }
        }
        this.FixPos();
        return b;
    }

    /// <summary>
    /// ドット単位で移動と衝突したかを返す
    /// </summary>
    public bool MoveY (int v) {
        if (this.InNoPassable()) { return true; }
        int dpx = this.dpx;
        int dpy = this.dpy + v;
        bool b = false;
        if (v < 0) {// down
            int l = this.DotCPos(dpx - this.halfwidth);
            int r = this.DotCPos(dpx + this.halfwidth - 1);
            int u = this.DotCPos(this.ToDot(this.py) + this.halfheight - 1);
            int d = this.DotCPos(dpy - this.halfheight);
            int c = d - 1;
            foreach (var i in E512Pos.BoxList(new E512Pos(l, d), new E512Pos(r, u))) {
                if (this.GetCollision(i) == TileCollisionType.NoPassable) {
                    c = i.y > c ? i.y : c;
                    b = true;
                }
            }
            if (b) {
                this.dpy = (c * this.tilesize) + this.halfheight + this.tilesize;
            } else {
                this.dpy = dpy;
            }
        } else {// up
            int l = this.DotCPos(dpx - this.halfwidth);
            int r = this.DotCPos(dpx + this.halfwidth - 1);
            int u = this.DotCPos(dpy + this.halfheight - 1);
            int d = this.DotCPos(this.ToDot(this.py) - this.halfheight);
            int c = u + 1;
            foreach (var i in E512Pos.BoxList(new E512Pos(l, d), new E512Pos(r, u))) {
                if (this.GetCollision(i) == TileCollisionType.NoPassable) {
                    c = i.y < c ? i.y : c;
                    b = true;
                }
            }
            if (b && this.DotCPos(this.dpy) != c) {
                this.dpy = (c * this.tilesize) - this.halfheight;
            } else {
                this.dpy = dpy;
            }
        }
        this.py = this.dpy * this.dotsize;
        this.FixPos();
        return b;
    }

    /// <summary>
    /// 自身の範囲に移動不可があるか
    /// </summary>
    private bool InNoPassable () {
        int dpx = this.ToDot(this.px);
        int dpy = this.ToDot(this.py);
        int l = this.DotCPos(dpx - this.halfwidth);
        int r = this.DotCPos(dpx + this.halfwidth - 1);
        int u = this.DotCPos(dpy + this.halfheight - 1);
        int d = this.DotCPos(dpy - this.halfheight);
        bool b = false;
        foreach (var i in E512Pos.BoxList(new E512Pos(l, d), new E512Pos(r, u))) {
            if (this.GetCollision(i) == TileCollisionType.NoPassable) { b = true; }
        }
        return b;
    }

    public void GravityDeltaTime (float g) {
        this.SetIsGround();
        if (!this.isground) { this.gravity -= g * Time.deltaTime * 60f; }
        if (this.MoveYDeltaTime(this.gravity)) { this.gravity = 0; }
        this.SetIsGround();
        if (this.isground) { this.gravity = 0; }
    }

    public void GravityDeltaTime (float g, float x, float lim) {
        this.SetIsGround();
        if (!this.isground) {
            this.gravity -= g * Time.deltaTime * 60f;
            if (this.gravity < lim) { this.gravity = lim; }
        }
        if (this.MoveYDeltaTime(this.gravity * x)) { this.gravity = 0; }
        this.SetIsGround();
        if (this.isground) { this.gravity = 0; }
    }


    public void Gravity (float g) {
        this.SetIsGround();
        if (!this.isground) { this.gravity -= g; }
        if (this.MoveY(this.gravity)) { this.gravity = 0; }
        this.SetIsGround();
        if (this.isground) { this.gravity = 0; }
    }

    private void SetIsGround () {
        int dpx = this.dpx;
        int dpy = this.dpy - 1;
        bool b = false;
        int l = this.DotCPos(dpx - this.halfwidth);
        int r = this.DotCPos(dpx + this.halfwidth - 1);
        int u = this.DotCPos(dpy + this.halfheight - 1);
        int d = this.DotCPos(dpy - this.halfheight);
        foreach (var i in E512Pos.BoxList(new E512Pos(l, d), new E512Pos(r, u))) {
            if (this.GetCollision(i) == TileCollisionType.NoPassable) { b = true; }
        }
        this.isground = b;
    }

    /// <summary>
    /// xyドット先のコリジョンリストを返す
    /// </summary>
    public List<E512Pos> Test (int x, int y) {
        int dpx = this.dpx + x;
        int dpy = this.dpy + y;
        List<E512Pos> c = new List<E512Pos>();
        int l = this.DotCPos(dpx - this.halfwidth);
        int r = this.DotCPos(dpx + this.halfwidth - 1);
        int u = this.DotCPos(dpy + this.halfheight - 1);
        int d = this.DotCPos(dpy - this.halfheight);
        foreach (var i in E512Pos.BoxList(new E512Pos(l, d), new E512Pos(r, u))) {
            if (this.GetCollision(i) == TileCollisionType.NoPassable) { c.Add(i); }
        }
        return c;
    }

    public List<E512Pos> Test (int x, int y, int ignore) {
        int dpx = this.dpx + x;
        int dpy = this.dpy + y;
        List<E512Pos> c = new List<E512Pos>();
        int l = this.DotCPos(dpx - this.halfwidth);
        int r = this.DotCPos(dpx + this.halfwidth - 1);
        int u = this.DotCPos(dpy + this.halfheight - 1);
        int d = this.DotCPos(dpy - this.halfheight);
        foreach (var i in E512Pos.BoxList(new E512Pos(l, d), new E512Pos(r, u))) {
            if (this.GetCollision(i) == TileCollisionType.NoPassable) {
                if (this.GetTileIndex(i) != ignore) { c.Add(i); }
            }
        }
        return c;
    }

    public bool AABBTest (DotCollision b) {
        var al = this.dpx - this.halfwidth;
        var ar = this.dpx + this.halfwidth - 1;
        var ad = this.dpy - this.halfheight;
        var au = this.dpy + this.halfheight - 1;
        var bl = b.dpx - b.halfwidth;
        var br = b.dpx + b.halfwidth - 1;
        var bd = b.dpy - b.halfheight;
        var bu = b.dpy + b.halfheight - 1;
        if (al > br || bl > ar) { return false; }
        if (ad > bu || bd > au) { return false; }
        return true;
    }

    static public bool AABBTest (DotCollision a, DotCollision b) {
        var al = a.dpx - a.halfwidth;
        var ar = a.dpx + a.halfwidth - 1;
        var ad = a.dpy - a.halfheight;
        var au = a.dpy + a.halfheight - 1;
        var bl = b.dpx - b.halfwidth;
        var br = b.dpx + b.halfwidth - 1;
        var bd = b.dpy - b.halfheight;
        var bu = b.dpy + b.halfheight - 1;
        if (al > br || bl > ar) { return false; }
        if (ad > bu || bd > au) { return false; }
        return false;
    }

    public TileCollisionType GetCollision (E512Pos cpos) {
        return this.map.tilemanager[this.map.GetTile(cpos, this.layer)].collisiontype;
    }

    public int GetTileIndex (E512Pos cpos) {
        return this.map.GetTile(cpos, this.layer);
    }

}

