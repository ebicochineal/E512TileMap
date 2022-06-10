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
    public bool MoveYDeltaTime (float v) { return this.MoveY(v * Time.deltaTime * 60f); }


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

    public void Gravity (float v) {
        this.gravity -= v;
        v = this.gravity;
        
        int dotx = this.dpx;
        int doty = this.dpy;
        int mvdoty = this.ToDot(this.py + v);
        
        bool iscol = false;
        bool ismove = true;
        
        // ground 1
        if (v < 0 && doty - 1 < mvdoty) {
            mvdoty = doty - 1;
            ismove = false;
        }
        
        if (v < 0) {// down
            // int l = dotx - this.halfwidth;
            // int r = dotx + this.halfwidth - 1;
            int d = mvdoty - this.halfheight;
            // int u = doty + this.halfheight - 1;
            int t = d;
            int cl = this.DotCPos(dotx - this.halfwidth);
            int cr = this.DotCPos(dotx + this.halfwidth - 1);
            int cd = this.DotCPos(mvdoty - this.halfheight);
            int cu = this.DotCPos(doty + this.halfheight - 1);
            foreach (var i in E512Pos.BoxList(new E512Pos(cl, cd), new E512Pos(cr, cu))) {
                if (this.GetCollision(i) == TileCollisionType.NoPassable) {
                    int idpy = (i.y * this.tilesize) + this.tilesize;
                    t = idpy > t ? idpy : t;
                    iscol = true;
                }
            }
            
            if (iscol && ismove) {
                if (t + this.halfheight > doty) {
                    // this.dpy = mvdoty;
                    // this.py += v;
                } else {
                    this.dpy = t + this.halfheight;
                    this.py = this.dpy * this.dotsize;
                }
            } else {
                if (ismove) { this.dpy = mvdoty; }
                this.py += v;
            }
        } else {// up
            // int l = dotx - this.halfwidth;
            // int r = dotx + this.halfwidth - 1;
            // int d = doty - this.halfheight;
            int u = mvdoty + this.halfheight - 1;
            int t = u;
            int cl = this.DotCPos(dotx - this.halfwidth);
            int cr = this.DotCPos(dotx + this.halfwidth - 1);
            int cd = this.DotCPos(doty - this.halfheight);
            int cu = this.DotCPos(mvdoty + this.halfheight - 1);
            foreach (var i in E512Pos.BoxList(new E512Pos(cl, cd), new E512Pos(cr, cu))) {
                if (this.GetCollision(i) == TileCollisionType.NoPassable) {
                    int idpy = (i.y * this.tilesize);
                    t = idpy < t ? idpy : t;
                    iscol = true;
                    
                }
            }
            
            if (iscol && ismove) {
                if (t - this.halfheight < doty) {
                    // this.dpy = mvdoty;
                    // this.py += v;
                } else {
                    this.dpy = t - this.halfheight;
                    this.py = this.dpy * this.dotsize;
                }
            } else {
                if (ismove) { this.dpy = mvdoty; }
                this.py += v;
            }
        }
        
        // ground 2
        this.isground = false;
        if (this.gravity < 0 && iscol) { this.isground = true; }
        
        
        if (iscol) { this.gravity = 0f; }
        this.FixPos();
    }
    
    public void Gravity (float v, List<DotCollision> cols) {
        this.gravity -= v;
        v = this.gravity;
        
        int dotx = this.dpx;
        int doty = this.dpy;
        int mvdoty = this.ToDot(this.py + v);
        
        bool iscol = false;
        bool ismove = true;
        
        // ground 1
        if (v < 0 && doty - 1 < mvdoty) {
            mvdoty = doty - 1;
            ismove = false;
        }
        
        if (v < 0) {// down
            int l = dotx - this.halfwidth;
            int r = dotx + this.halfwidth - 1;
            int d = mvdoty - this.halfheight;
            int u = doty + this.halfheight - 1;
            int t = d;
            foreach (var i in cols) {
                if (DotCollision.AABBTest(l, r, d, u, i)) {
                    int idpy = i.dpy + i.halfheight;
                    t = idpy > t ? idpy : t;
                    iscol = true;
                }
            }
            
            int cl = this.DotCPos(dotx - this.halfwidth);
            int cr = this.DotCPos(dotx + this.halfwidth - 1);
            int cd = this.DotCPos(mvdoty - this.halfheight);
            int cu = this.DotCPos(doty + this.halfheight - 1);
            foreach (var i in E512Pos.BoxList(new E512Pos(cl, cd), new E512Pos(cr, cu))) {
                if (this.GetCollision(i) == TileCollisionType.NoPassable) {
                    int idpy = (i.y * this.tilesize) + this.tilesize;
                    t = idpy > t ? idpy : t;
                    iscol = true;
                }
            }
            
            if (iscol && ismove) {
                if (t + this.halfheight > doty) {
                    // this.dpy = mvdoty;
                    // this.py += v;
                } else {
                    this.dpy = t + this.halfheight;
                    this.py = this.dpy * this.dotsize;
                }
            } else {
                if (ismove) { this.dpy = mvdoty; }
                this.py += v;
            }
        } else {// up
            int l = dotx - this.halfwidth;
            int r = dotx + this.halfwidth - 1;
            int d = doty - this.halfheight;
            int u = mvdoty + this.halfheight - 1;
            int t = u;
            foreach (var i in cols) {
                if (DotCollision.AABBTest(l, r, d, u, i)) {
                    int idpy = i.dpy - i.halfheight;
                    t = idpy < t ? idpy : t;
                    iscol = true;
                }
            }
            int cl = this.DotCPos(dotx - this.halfwidth);
            int cr = this.DotCPos(dotx + this.halfwidth - 1);
            int cd = this.DotCPos(doty - this.halfheight);
            int cu = this.DotCPos(mvdoty + this.halfheight - 1);
            foreach (var i in E512Pos.BoxList(new E512Pos(cl, cd), new E512Pos(cr, cu))) {
                if (this.GetCollision(i) == TileCollisionType.NoPassable) {
                    int idpy = (i.y * this.tilesize);
                    t = idpy < t ? idpy : t;
                    iscol = true;
                    
                }
            }
            
            if (iscol && ismove) {
                if (t - this.halfheight < doty) {
                    // this.dpy = mvdoty;
                    // this.py += v;
                } else {
                    this.dpy = t - this.halfheight;
                    this.py = this.dpy * this.dotsize;
                }
            } else {
                if (ismove) { this.dpy = mvdoty; }
                this.py += v;
            }
        }
        
        // ground 2
        this.isground = false;
        if (this.gravity < 0 && iscol) { this.isground = true; }
        
        
        if (iscol) { this.gravity = 0f; }
        this.FixPos();
    }
    
    public bool MoveY (float v, List<DotCollision> cols) {
        int dotx = this.dpx;
        int doty = this.dpy;
        int mvdoty = this.ToDot(this.py + v);
        
        bool iscol = false;
        
        if (v < 0) {// down
            int l = dotx - this.halfwidth;
            int r = dotx + this.halfwidth - 1;
            int d = mvdoty - this.halfheight;
            int u = doty + this.halfheight - 1;
            int t = d;
            foreach (var i in cols) {
                if (DotCollision.AABBTest(l, r, d, u, i)) {
                    int idpy = i.dpy + i.halfheight;
                    t = idpy > t ? idpy : t;
                    iscol = true;
                }
            }
            
            int cl = this.DotCPos(dotx - this.halfwidth);
            int cr = this.DotCPos(dotx + this.halfwidth - 1);
            int cd = this.DotCPos(mvdoty - this.halfheight);
            int cu = this.DotCPos(doty + this.halfheight - 1);
            foreach (var i in E512Pos.BoxList(new E512Pos(cl, cd), new E512Pos(cr, cu))) {
                if (this.GetCollision(i) == TileCollisionType.NoPassable) {
                    int idpy = (i.y * this.tilesize) + this.tilesize;
                    t = idpy > t ? idpy : t;
                    iscol = true;
                }
            }
            
            if (iscol) {
                if (t + this.halfheight > doty) {
                    // this.dpy = mvdoty;
                    // this.py += v;
                } else {
                    this.dpy = t + this.halfheight;
                    this.py = this.dpy * this.dotsize;
                }
            } else {
                this.dpy = mvdoty;
                this.py += v;
            }
            
            
        } else {// up
            int l = dotx - this.halfwidth;
            int r = dotx + this.halfwidth - 1;
            int d = doty - this.halfheight;
            int u = mvdoty + this.halfheight - 1;
            int t = u;
            foreach (var i in cols) {
                if (DotCollision.AABBTest(l, r, d, u, i)) {
                    int idpy = i.dpy - i.halfheight;
                    t = idpy < t ? idpy : t;
                    iscol = true;
                }
            }
            int cl = this.DotCPos(dotx - this.halfwidth);
            int cr = this.DotCPos(dotx + this.halfwidth - 1);
            int cd = this.DotCPos(doty - this.halfheight);
            int cu = this.DotCPos(mvdoty + this.halfheight - 1);
            foreach (var i in E512Pos.BoxList(new E512Pos(cl, cd), new E512Pos(cr, cu))) {
                if (this.GetCollision(i) == TileCollisionType.NoPassable) {
                    int idpy = (i.y * this.tilesize);
                    t = idpy < t ? idpy : t;
                    iscol = true;
                }
            }
            
            if (iscol) {
                if (t - this.halfheight < doty) {
                    // this.dpy = mvdoty;
                    // this.py += v;
                } else {
                    this.dpy = t - this.halfheight;
                    this.py = this.dpy * this.dotsize;
                }
            } else {
                this.dpy = mvdoty;
                this.py += v;
            }
            
        }
        this.FixPos();
        return iscol;
    }
    public bool MoveX (float v, List<DotCollision> cols) {
        int dotx = this.dpx;
        int doty = this.dpy;
        int mvdotx = this.ToDot(this.px + v);
        
        bool iscol = false;
        
        if (v < 0) {// left
            int l = mvdotx - this.halfwidth;
            int r = dotx + this.halfwidth - 1;
            int d = doty - this.halfheight;
            int u = doty + this.halfheight - 1;
            int t = l;
            foreach (var i in cols) {
                if (DotCollision.AABBTest(l, r, d, u, i)) {
                    int idpx = i.dpx + i.halfwidth;
                    t = idpx > t ? idpx : t;
                    iscol = true;
                }
            }
            
            int cl = this.DotCPos(mvdotx - this.halfwidth);
            int cr = this.DotCPos(dotx + this.halfwidth - 1);
            int cd = this.DotCPos(doty - this.halfheight);
            int cu = this.DotCPos(doty + this.halfheight - 1);
            foreach (var i in E512Pos.BoxList(new E512Pos(cl, cd), new E512Pos(cr, cu))) {
                if (this.GetCollision(i) == TileCollisionType.NoPassable) {
                    int idpx = (i.x * this.tilesize) + this.tilesize;
                    t = idpx > t ? idpx : t;
                    iscol = true;
                }
            }
            
            if (iscol) {
                if (t + this.halfwidth > dotx) {
                    // this.dpx = mvdotx;
                    // this.px += v;
                } else {
                    this.dpx = t + this.halfwidth;
                    this.px = this.dpx * this.dotsize;
                }
            } else {
                this.dpx = mvdotx;
                this.px += v;
            }
            
            
        } else {// right
            int l = dotx - this.halfwidth;
            int r = mvdotx + this.halfwidth - 1;
            int d = doty - this.halfheight;
            int u = doty + this.halfheight - 1;
            int t = r;
            foreach (var i in cols) {
                if (DotCollision.AABBTest(l, r, d, u, i)) {
                    int idpx = i.dpx - i.halfwidth;
                    t = idpx < t ? idpx : t;
                    iscol = true;
                }
            }
            
            
            int cl = this.DotCPos(dotx - this.halfwidth);
            int cr = this.DotCPos(mvdotx + this.halfwidth - 1);
            int cd = this.DotCPos(doty - this.halfheight);
            int cu = this.DotCPos(doty + this.halfheight - 1);
            foreach (var i in E512Pos.BoxList(new E512Pos(cl, cd), new E512Pos(cr, cu))) {
                if (this.GetCollision(i) == TileCollisionType.NoPassable) {
                    int idpx = (i.x * this.tilesize);
                    t = idpx < t ? idpx : t;
                    iscol = true;
                }
            }
            
            if (iscol) {
                if (t - this.halfwidth < dotx) {
                    // this.dpx = mvdotx;
                    // this.px += v;
                } else {
                    this.dpx = t - this.halfwidth;
                    this.px = this.dpx * this.dotsize;
                }
            } else {
                this.dpx = mvdotx;
                this.px += v;
            }
            
        }
        this.FixPos();
        return iscol;
    }
    
    
    public bool MoveY (float v) {
        int dotx = this.dpx;
        int doty = this.dpy;
        int mvdoty = this.ToDot(this.py + v);
        
        bool iscol = false;
        
        if (v < 0) {// down
            // int l = dotx - this.halfwidth;
            // int r = dotx + this.halfwidth - 1;
            int d = mvdoty - this.halfheight;
            // int u = doty + this.halfheight - 1;
            int t = d;
            
            int cl = this.DotCPos(dotx - this.halfwidth);
            int cr = this.DotCPos(dotx + this.halfwidth - 1);
            int cd = this.DotCPos(mvdoty - this.halfheight);
            int cu = this.DotCPos(doty + this.halfheight - 1);
            foreach (var i in E512Pos.BoxList(new E512Pos(cl, cd), new E512Pos(cr, cu))) {
                if (this.GetCollision(i) == TileCollisionType.NoPassable) {
                    int idpy = (i.y * this.tilesize) + this.tilesize;
                    t = idpy > t ? idpy : t;
                    iscol = true;
                }
            }
            
            if (iscol) {
                if (t + this.halfheight > doty) {
                    this.dpy = mvdoty;
                    this.py += v;
                } else {
                    this.dpy = t + this.halfheight;
                    this.py = this.dpy * this.dotsize;
                }
            } else {
                this.dpy = mvdoty;
                this.py += v;
            }
            
            
        } else {// up
            // int l = dotx - this.halfwidth;
            // int r = dotx + this.halfwidth - 1;
            // int d = doty - this.halfheight;
            int u = mvdoty + this.halfheight - 1;
            int t = u;
            
            int cl = this.DotCPos(dotx - this.halfwidth);
            int cr = this.DotCPos(dotx + this.halfwidth - 1);
            int cd = this.DotCPos(doty - this.halfheight);
            int cu = this.DotCPos(mvdoty + this.halfheight - 1);
            foreach (var i in E512Pos.BoxList(new E512Pos(cl, cd), new E512Pos(cr, cu))) {
                if (this.GetCollision(i) == TileCollisionType.NoPassable) {
                    int idpy = (i.y * this.tilesize);
                    t = idpy < t ? idpy : t;
                    iscol = true;
                }
            }
            
            if (iscol) {
                if (t - this.halfheight < doty) {
                    this.dpy = mvdoty;
                    this.py += v;
                } else {
                    this.dpy = t - this.halfheight;
                    this.py = this.dpy * this.dotsize;
                }
            } else {
                this.dpy = mvdoty;
                this.py += v;
            }
            
        }
        this.FixPos();
        return iscol;
    }
    public bool MoveX (float v) {
        int dotx = this.dpx;
        int doty = this.dpy;
        int mvdotx = this.ToDot(this.px + v);
        
        bool iscol = false;
        
        if (v < 0) {// left
            int l = mvdotx - this.halfwidth;
            // int r = dotx + this.halfwidth - 1;
            // int d = doty - this.halfheight;
            // int u = doty + this.halfheight - 1;
            int t = l;
            
            int cl = this.DotCPos(mvdotx - this.halfwidth);
            int cr = this.DotCPos(dotx + this.halfwidth - 1);
            int cd = this.DotCPos(doty - this.halfheight);
            int cu = this.DotCPos(doty + this.halfheight - 1);
            foreach (var i in E512Pos.BoxList(new E512Pos(cl, cd), new E512Pos(cr, cu))) {
                if (this.GetCollision(i) == TileCollisionType.NoPassable) {
                    int idpx = (i.x * this.tilesize) + this.tilesize;
                    t = idpx > t ? idpx : t;
                    iscol = true;
                }
            }
            
            if (iscol) {
                if (t + this.halfwidth > dotx) {
                    this.dpx = mvdotx;
                    this.px += v;
                } else {
                    this.dpx = t + this.halfwidth;
                    this.px = this.dpx * this.dotsize;
                }
            } else {
                this.dpx = mvdotx;
                this.px += v;
            }
            
            
        } else {// right
            // int l = dotx - this.halfwidth;
            int r = mvdotx + this.halfwidth - 1;
            // int d = doty - this.halfheight;
            // int u = doty + this.halfheight - 1;
            int t = r;
            
            int cl = this.DotCPos(dotx - this.halfwidth);
            int cr = this.DotCPos(mvdotx + this.halfwidth - 1);
            int cd = this.DotCPos(doty - this.halfheight);
            int cu = this.DotCPos(doty + this.halfheight - 1);
            foreach (var i in E512Pos.BoxList(new E512Pos(cl, cd), new E512Pos(cr, cu))) {
                if (this.GetCollision(i) == TileCollisionType.NoPassable) {
                    int idpx = (i.x * this.tilesize);
                    t = idpx < t ? idpx : t;
                    iscol = true;
                }
            }
            
            if (iscol) {
                if (t - this.halfwidth < dotx) {
                    this.dpx = mvdotx;
                    this.px += v;
                } else {
                    this.dpx = t - this.halfwidth;
                    this.px = this.dpx * this.dotsize;
                }
            } else {
                this.dpx = mvdotx;
                this.px += v;
            }
            
        }
        this.FixPos();
        return iscol;
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
        return true;
    }
    
    static public bool AABBTest (int l, int r, int d, int u, DotCollision b) {
        var bl = b.dpx - b.halfwidth;
        var br = b.dpx + b.halfwidth - 1;
        var bd = b.dpy - b.halfheight;
        var bu = b.dpy + b.halfheight - 1;
        if (l > br || bl > r) { return false; }
        if (d > bu || bd > u) { return false; }
        return true;
    }
    
    public TileCollisionType GetCollision (E512Pos cpos) {
        return this.map.tilemanager[this.map.GetTile(cpos, this.layer)].collisiontype;
    }

    public int GetTileIndex (E512Pos cpos) {
        return this.map.GetTile(cpos, this.layer);
    }

}

