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
    
    private bool pignore = false;
    
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
                if (i.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()) { continue; }
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
                if (i.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()) { continue; }
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
                if (i.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()) { continue; }
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
                if (i.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()) { continue; }
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
                if (i.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()) { continue; }
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
                if (i.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()) { continue; }
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
    
    public void PushMoveX (float v, List<DotCollision> cols, int cnt = 8) {
        for (int i = 0; i < cols.Count - 1; ++i) {
            for (int j = i + 1; j < cols.Count; ++j) {
                if (DotCollision.AABBTest(cols[i], cols[j])) {
                    cols[i].pignore = true;
                    cols[j].pignore = true;
                }
            }
        }
        
        int tdx = this.dpx;
        int ndx = this.ToDot(this.px + v);
        int mv = Mathf.Abs(tdx-ndx);
        float f = (this.px + v) % this.dotsize;
        
        if (this.pignore) {
            this.px += v;
            this.dpx = this.ToDot(this.px);
            this.FixPos();
        } else {
            if (v < 0) {
                int tmv = this.PushMoveLeft(mv, cols, cnt);
                this.px = this.dpx * this.dotsize;
                this.px += f;
                this.FixPos();
            }
            if (v > 0) {
                int tmv = this.PushMoveRight(mv, cols, cnt);
                this.px = this.dpx * this.dotsize;
                this.px += f;
                this.FixPos();
            }
        }
        
        for (int i = 0; i < cols.Count; ++i) { cols[i].pignore = false; }
    }
    public void PushMoveY (float v, List<DotCollision> cols, int cnt = 8) {
        for (int i = 0; i < cols.Count - 1; ++i) {
            for (int j = i + 1; j < cols.Count; ++j) {
                if (DotCollision.AABBTest(cols[i], cols[j])) {
                    cols[i].pignore = true;
                    cols[j].pignore = true;
                }
            }
        }
        
        
        
        int tdy = this.dpy;
        int ndy = this.ToDot(this.py + v);
        int mv = Mathf.Abs(tdy-ndy);
        float f = (this.py + v) % this.dotsize;
        
        if (this.pignore) {
            this.py += v;
            this.dpy = this.ToDot(this.py);
            this.FixPos();
        } else {
            if (v < 0) {
                int tmv = this.PushMoveDown(mv, cols, cnt);
                this.py = this.dpy * this.dotsize;
                this.py += f;
                this.FixPos();
            }
            if (v > 0) {
                int tmv = this.PushMoveUp(mv, cols, cnt);
                this.py = this.dpy * this.dotsize;
                this.py += f;
                this.FixPos();
            }
        }
        for (int i = 0; i < cols.Count; ++i) { cols[i].pignore = false; }
        
    }
    public int PushMoveRight (int mv, List<DotCollision> cols, int cnt = 0) {
        if (cnt < 0) { return 0; }
        int l = this.dpx - this.halfwidth;
        int r = this.dpx + mv + this.halfwidth - 1;
        int d = this.dpy - this.halfheight;
        int u = this.dpy + this.halfheight - 1;
        int cl = this.DotCPos(l);
        int cr = this.DotCPos(r);
        int cd = this.DotCPos(d);
        int cu = this.DotCPos(u);
        
        mv = Mathf.Max(mv - this.CollisionRightDiff(r, cl, cr, cd, cu), 0);
        if (mv == 0) { return 0; }
        
        r = this.dpx + mv + this.halfwidth - 1;
        
        int minmv = mv;
        foreach (var i in cols) {
            if (i.pignore) { continue; }
            if (i.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()) { continue; }
            if (DotCollision.AABBTest(l, r, d, u, i)) {
                int il = i.dpx - i.halfwidth;
                int imv = (r + 1) - il;
                imv = i.VPushMoveRight(imv, cols, cnt - 1);
                imv = (il+imv) - (this.dpx + this.halfwidth - 1 + 1);
                minmv = Mathf.Min(imv, minmv);
            }
        }
        
        r = this.dpx + minmv + this.halfwidth - 1;
        foreach (var i in cols) {
            if (i.pignore) { continue; }
            if (i.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()) { continue; }
            if (DotCollision.AABBTest(l, r, d, u, i)) {
                int il = i.dpx - i.halfwidth;
                int imv = (r + 1) - il;
                i.PushMoveRight(imv, cols, cnt - 1);
            }
        }
        
        this.dpx += minmv;
        this.px = this.dpx * this.dotsize;
        this.FixPos();
        return minmv;
    }
    public int VPushMoveRight (int mv, List<DotCollision> cols, int cnt = 0) {
        if (cnt < 0) { return 0; }
        int l = this.dpx - this.halfwidth;
        int r = this.dpx + mv + this.halfwidth - 1;
        int d = this.dpy - this.halfheight;
        int u = this.dpy + this.halfheight - 1;
        int cl = this.DotCPos(l);
        int cr = this.DotCPos(r);
        int cd = this.DotCPos(d);
        int cu = this.DotCPos(u);
        
        mv = Mathf.Max(mv - this.CollisionRightDiff(r, cl, cr, cd, cu), 0);
        if (mv == 0) { return 0; }
        
        r = this.dpx + mv + this.halfwidth - 1;
        
        int minmv = mv;
        foreach (var i in cols) {
            if (i.pignore) { continue; }
            if (i.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()) { continue; }
            if (DotCollision.AABBTest(l, r, d, u, i)) {
                int il = i.dpx - i.halfwidth;
                int imv = (r + 1) - il;
                imv = i.VPushMoveRight(imv, cols, cnt - 1);
                imv = (il+imv) - (this.dpx + this.halfwidth - 1 + 1);
                minmv = Mathf.Min(imv, minmv);
            }
        }
        return minmv;
    }
    int CollisionRightDiff (int t, int cl, int cr, int cd, int cu) {
        int m = t + 1;
        foreach (var i in E512Pos.BoxList(new E512Pos(cl, cd), new E512Pos(cr, cu))) {
            if (this.GetCollision(i) == TileCollisionType.NoPassable) { m = Mathf.Min(this.LDot(i.x), m); }
        }
        return (t + 1) - m;
    }
    
    public int PushMoveLeft (int mv, List<DotCollision> cols, int cnt = 0) {
        if (cnt < 0) { return 0; }
        int l = this.dpx - mv - this.halfwidth;
        int r = this.dpx + this.halfwidth - 1;
        int d = this.dpy - this.halfheight;
        int u = this.dpy + this.halfheight - 1;
        int cl = this.DotCPos(l);
        int cr = this.DotCPos(r);
        int cd = this.DotCPos(d);
        int cu = this.DotCPos(u);
        
        mv = Mathf.Max(mv - this.CollisionLeftDiff(l, cl, cr, cd, cu), 0);
        if (mv == 0) { return 0; }
        l = this.dpx - mv - this.halfwidth;
        
        int minmv = mv;
        foreach (var i in cols) {
            if (i.pignore) { continue; }
            if (i.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()) { continue; }
            if (DotCollision.AABBTest(l, r, d, u, i)) {
                int ir = i.dpx + i.halfwidth - 1;
                int imv =  ir - (l - 1);
                imv = i.VPushMoveLeft(imv, cols, cnt - 1);
                imv = (this.dpx - this.halfwidth - 1) - (ir-imv);
                minmv = Mathf.Min(imv, minmv);
            }
        }
        
        l = this.dpx - minmv - this.halfwidth;
        foreach (var i in cols) {
            if (i.pignore) { continue; }
            if (i.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()) { continue; }
            if (DotCollision.AABBTest(l, r, d, u, i)) {
                int ir = i.dpx + i.halfwidth - 1;
                int imv = ir - (l - 1);
                i.PushMoveLeft(imv, cols, cnt - 1);
            }
        }
        
        this.dpx -= minmv;
        this.px = this.dpx * this.dotsize;
        this.FixPos();
        return minmv;
    }
    public int VPushMoveLeft (int mv, List<DotCollision> cols, int cnt = 0) {
        if (cnt < 0) { return 0; }
        int l = this.dpx - mv - this.halfwidth;
        int r = this.dpx + this.halfwidth - 1;
        int d = this.dpy - this.halfheight;
        int u = this.dpy + this.halfheight - 1;
        int cl = this.DotCPos(l);
        int cr = this.DotCPos(r);
        int cd = this.DotCPos(d);
        int cu = this.DotCPos(u);
        
        mv = Mathf.Max(mv - this.CollisionLeftDiff(l, cl, cr, cd, cu), 0);
        if (mv == 0) { return 0; }
        l = this.dpx - mv - this.halfwidth;
        
        int minmv = mv;
        foreach (var i in cols) {
            if (i.pignore) { continue; }
            if (i.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()) { continue; }
            if (DotCollision.AABBTest(l, r, d, u, i)) {
                int ir = i.dpx + i.halfwidth - 1;
                int imv =  ir - (l - 1);
                imv = i.VPushMoveLeft(imv, cols, cnt - 1);
                imv = (this.dpx - this.halfwidth - 1) - (ir-imv);
                minmv = Mathf.Min(imv, minmv);
            }
        }
        
        
        return minmv;
    }
    int CollisionLeftDiff (int t, int cl, int cr, int cd, int cu) {
        int m = t - 1;
        foreach (var i in E512Pos.BoxList(new E512Pos(cl, cd), new E512Pos(cr, cu))) {
            if (this.GetCollision(i) == TileCollisionType.NoPassable) { m = Mathf.Max(this.RDot(i.x), m); }
        }
        return m - (t - 1);
    }
    
    public int PushMoveUp (int mv, List<DotCollision> cols, int cnt = 0) {
        if (cnt < 0) { return 0; }
        int l = this.dpx - this.halfwidth;
        int r = this.dpx + this.halfwidth - 1;
        int d = this.dpy - this.halfheight;
        int u = this.dpy + mv + this.halfheight - 1;
        int cl = this.DotCPos(l);
        int cr = this.DotCPos(r);
        int cd = this.DotCPos(d);
        int cu = this.DotCPos(u);
        
        mv = Mathf.Max(mv - this.CollisionUpDiff(u, cl, cr, cd, cu), 0);
        if (mv == 0) { return 0; }
        
        u = this.dpy + mv + this.halfheight - 1;
        
        int minmv = mv;
        foreach (var i in cols) {
            if (i.pignore) { continue; }
            if (i.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()) { continue; }
            if (DotCollision.AABBTest(l, r, d, u, i)) {
                int id = i.dpy - i.halfheight;
                int imv = (u + 1) - id;
                imv = i.VPushMoveUp(imv, cols, cnt - 1);
                imv = (id+imv) - (this.dpy + this.halfheight - 1 + 1);
                minmv = Mathf.Min(imv, minmv);
            }
        }
        
        u = this.dpy + minmv + this.halfheight - 1;
        foreach (var i in cols) {
            if (i.pignore) { continue; }
            if (i.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()) { continue; }
            if (DotCollision.AABBTest(l, r, d, u, i)) {
                int id = i.dpy - i.halfheight;
                int imv = (u + 1) - id;
                i.PushMoveUp(imv, cols, cnt - 1);
            }
        }
        
        this.dpy += minmv;
        this.py = this.dpy * this.dotsize;
        this.FixPos();
        return minmv;
    }
    public int VPushMoveUp (int mv, List<DotCollision> cols, int cnt = 0) {
        if (cnt < 0) { return 0; }
        int l = this.dpx - this.halfwidth;
        int r = this.dpx + this.halfwidth - 1;
        int d = this.dpy - this.halfheight;
        int u = this.dpy + mv + this.halfheight - 1;
        int cl = this.DotCPos(l);
        int cr = this.DotCPos(r);
        int cd = this.DotCPos(d);
        int cu = this.DotCPos(u);
        
        mv = Mathf.Max(mv - this.CollisionUpDiff(u, cl, cr, cd, cu), 0);
        if (mv == 0) { return 0; }
        
        u = this.dpy + mv + this.halfheight - 1;
        
        int minmv = mv;
        foreach (var i in cols) {
            if (i.pignore) { continue; }
            if (i.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()) { continue; }
            if (DotCollision.AABBTest(l, r, d, u, i)) {
                int id = i.dpy - i.halfheight;
                int imv = (u + 1) - id;
                imv = i.VPushMoveUp(imv, cols, cnt - 1);
                imv = (id+imv) - (this.dpy + this.halfheight - 1 + 1);
                minmv = Mathf.Min(imv, minmv);
            }
        }
        return minmv;
    }
    int CollisionUpDiff (int t, int cl, int cr, int cd, int cu) {
        int m = t + 1;
        foreach (var i in E512Pos.BoxList(new E512Pos(cl, cd), new E512Pos(cr, cu))) {
            if (this.GetCollision(i) == TileCollisionType.NoPassable) { m = Mathf.Min(this.DDot(i.y), m); }
        }
        return (t + 1) - m;
    }
    
    public int PushMoveDown (int mv, List<DotCollision> cols, int cnt = 0) {
        if (cnt < 0) { return 0; }
        int l = this.dpx - this.halfwidth;
        int r = this.dpx + this.halfwidth - 1;
        int d = this.dpy - mv - this.halfheight;
        int u = this.dpy + this.halfheight - 1;
        int cl = this.DotCPos(l);
        int cr = this.DotCPos(r);
        int cd = this.DotCPos(d);
        int cu = this.DotCPos(u);
        
        mv = Mathf.Max(mv - this.CollisionDownDiff(d, cl, cr, cd, cu), 0);
        if (mv == 0) { return 0; }
        d = this.dpy - mv - this.halfheight;
        
        int minmv = mv;
        foreach (var i in cols) {
            if (i.pignore) { continue; }
            if (i.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()) { continue; }
            if (DotCollision.AABBTest(l, r, d, u, i)) {
                int iu = i.dpy + i.halfheight - 1;
                int imv =  iu - (d - 1);
                imv = i.VPushMoveDown(imv, cols, cnt - 1);
                imv = (this.dpy - this.halfheight - 1) - (iu-imv);
                minmv = Mathf.Min(imv, minmv);
            }
        }
        
        d = this.dpy - minmv - this.halfheight;
        foreach (var i in cols) {
            if (i.pignore) { continue; }
            if (i.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()) { continue; }
            if (DotCollision.AABBTest(l, r, d, u, i)) {
                int iu = i.dpy + i.halfheight - 1;
                int imv = iu - (d - 1);
                i.PushMoveDown(imv, cols, cnt - 1);
            }
        }
        
        this.dpy -= minmv;
        this.py = this.dpy * this.dotsize;
        this.FixPos();
        return minmv;
    }
    public int VPushMoveDown (int mv, List<DotCollision> cols, int cnt = 0) {
        if (cnt < 0) { return 0; }
        int l = this.dpx - this.halfwidth;
        int r = this.dpx + this.halfwidth - 1;
        int d = this.dpy - mv - this.halfheight;
        int u = this.dpy + this.halfheight - 1;
        int cl = this.DotCPos(l);
        int cr = this.DotCPos(r);
        int cd = this.DotCPos(d);
        int cu = this.DotCPos(u);
        
        mv = Mathf.Max(mv - this.CollisionDownDiff(d, cl, cr, cd, cu), 0);
        if (mv == 0) { return 0; }
        d = this.dpy - mv - this.halfheight;
        
        int minmv = mv;
        foreach (var i in cols) {
            if (i.pignore) { continue; }
            if (i.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()) { continue; }
            if (DotCollision.AABBTest(l, r, d, u, i)) {
                int iu = i.dpy + i.halfheight - 1;
                int imv =  iu - (d - 1);
                imv = i.VPushMoveDown(imv, cols, cnt - 1);
                imv = (this.dpy - this.halfheight - 1) - (iu-imv);
                minmv = Mathf.Min(imv, minmv);
            }
        }
        
        return minmv;
    }
    int CollisionDownDiff (int t, int cl, int cr, int cd, int cu) {
        int m = t - 1;
        foreach (var i in E512Pos.BoxList(new E512Pos(cl, cd), new E512Pos(cr, cu))) {
            if (this.GetCollision(i) == TileCollisionType.NoPassable) { m = Mathf.Max(this.UDot(i.y), m); }
        }
        return m - (t - 1);
    }
    
    private int LDot (int cpx) { return cpx * this.tilesize; }
    private int RDot (int cpx) { return cpx * this.tilesize + this.tilesize - 1; }
    private int DDot (int cpy) { return cpy * this.tilesize; }
    private int UDot (int cpy) { return cpy * this.tilesize + this.tilesize - 1; }
    
    
    
    
    
    /// <summary>
    /// 自身の範囲にindexのタイルが含まれるか
    /// </summary>
    public bool Contains (int index) {
        int l = this.DotCPos(this.dpx - this.halfwidth);
        int r = this.DotCPos(this.dpx + this.halfwidth - 1);
        int u = this.DotCPos(this.dpy + this.halfheight - 1);
        int d = this.DotCPos(this.dpy - this.halfheight);
        foreach (var i in E512Pos.BoxList(new E512Pos(l, d), new E512Pos(r, u))) {
            if (this.GetTileIndex(i) == index) { return true; }
        }
        return false;
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

