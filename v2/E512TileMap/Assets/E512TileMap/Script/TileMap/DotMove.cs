using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DotMove : MonoBehaviour {
    [HideInInspector]
    public float px, py;// 座標 オブジェクトの座標は修正されるので保存しておく 中心
    [HideInInspector]
    public int dpx, dpy;// ドット座標 中心
    [HideInInspector]
    public int width;
    [HideInInspector]
    public int height;
    [HideInInspector]
    public bool ismove = false;

    public int halfwidth = 8;
    public int halfheight = 8;

    [HideInInspector]
    public  float dotsize;
    protected int tilesize = 1;
    protected int halftilesize;

    public int cpx { get { return this.DotCPos(this.dpx); } }
    public int cpy { get { return this.DotCPos(this.dpy); } }

    protected bool init = false;

    protected E512Pos destination;// 目的地 dot pos
    protected Vector2 destinationf;// 目的地 dot pos vector2

    public int x {
        set {
            this.dpx = value;
            this.FixPos();
        }
        get { return this.dpx; }
    }

    public int y {
        set {
            this.dpy = value;
            this.FixPos();
        }
        get { return this.dpy; }
    }
    
    void Start () {
        this.Init();
    }
    
    public virtual void Init () {
        if (!this.init) {
            this.BaseInit();
        }
    }

    protected void BaseInit () {
        this.px = this.gameObject.transform.position.x;
        this.py = this.gameObject.transform.position.y;

        this.tilesize = E512TileMapData.SceneMap != null ? E512TileMapData.SceneMap.tilemanager.tilesize : 16;
        this.halftilesize = this.tilesize / 2;
        this.dotsize = 1f / this.tilesize;
        this.width = this.halfwidth * 2;
        this.height = this.halfheight * 2;
        this.dpx = this.ToDot(this.px);
        this.dpy = this.ToDot(this.py);
        this.FixPos();
        this.init = true;
    }

    /// <summary>
    /// ドット空間座標に修正
    /// </summary>
    protected void FixPos () {
        float z = this.gameObject.transform.position.z;
        this.gameObject.transform.position = new Vector3(this.dotsize * this.dpx, this.dotsize * this.dpy, z);
    }

    public void Warp (E512Pos cpos) {
        // MPosでの移動はその座標の中心へオブジェクトの中心を合わせる +halfwidth, +halfheight
        this.dpx = this.ToDot(cpos.x) + this.halfwidth;
        this.dpy = this.ToDot(cpos.y) + this.halfheight;
        this.px = this.dpx * this.dotsize;
        this.py = this.dpy * this.dotsize;
        this.FixPos();
    }

    public void Sync (GameObject obj) {
        this.dpx = this.ToDot(obj.transform.position.x);
        this.dpy = this.ToDot(obj.transform.position.y);
        this.px = this.dpx * this.dotsize;
        this.py = this.dpy * this.dotsize;
        this.FixPos();
    }

    public void SimpleMoveDeltaTime (float x, float y) {
        float v = Time.deltaTime * 60f;
        this.SimpleMove(x * v, y * v);
    }

    public void SimpleMove (float x, float y) {
        this.px += x;
        this.py += y;
        this.dpx = this.ToDot(this.px);
        this.dpy = this.ToDot(this.py);
        this.FixPos();
    }

    public void SimpleMove (int x, int y) {
        this.dpx += x;
        this.dpy += y;
        this.FixPos();
    }

    public void MoveToDestinationDeltaTime (float velocity) {
        this.MoveToDestination(velocity * Time.deltaTime * 60f);
    }

    // 目的地に移動毎フレーム呼ぶ必要がある
    public void MoveToDestination (float velocity) {
        if (this.ismove == false) { return; }
        if (Vector2.Distance(this.destinationf, new Vector2(this.px, this.py)) > velocity) {
            var v = this.destinationf - new Vector2(this.px, this.py);
            v = v.normalized * velocity;
            this.px += v.x;
            this.py += v.y;
            this.dpx = this.ToDot(this.px);
            this.dpy = this.ToDot(this.py);
            this.FixPos();
        } else {
            this.px = this.destinationf.x;
            this.py = this.destinationf.y;
            this.dpx = this.ToDot(this.px);
            this.dpy = this.ToDot(this.py);
            this.FixPos();
            this.ismove = false;
        }
    }

    // 目的地をセル座標でセット、目的地のセル座標の中心に自身の中心を合わせる
    public void SetDestinationCenter (E512Pos cpos) {
        this.SetDestinationCenter(this.ToDot(cpos.x) + this.halftilesize, this.ToDot(cpos.y) + this.halftilesize);
    }

    // 目的地をVector3でセット、目的地のセル座標の中心に自身の中心を合わせる
    public void SetDestinationCenter (Vector3 position) {
        this.SetDestinationCenter(this.ToDot(position.x), this.ToDot(position.y));
    }

    // 目的地をドット座標でセット、目的地のドットに自身の中心座標を合わせる
    public void SetDestinationCenter (int dpx, int dpy) {
        this.destination.x = dpx;
        this.destination.y = dpy;
        this.destinationf.x = this.destination.x * this.dotsize;
        this.destinationf.y = this.destination.y * this.dotsize;
        this.ismove = true;
    }

    // 目的地をドット座標でセット、目的地のドットに自身の左下座標を合わせる
    public void SetDestinationLeftDown (int dpx, int dpy) {
        this.destination.x = dpx + this.halfwidth;
        this.destination.y = dpy + this.halfheight;
        this.destinationf.x = this.destination.x * this.dotsize;
        this.destinationf.y = this.destination.y * this.dotsize;
        this.ismove = true;
    }

    public void LocalZero () {
        this.transform.localPosition = Vector3.zero;
        this.Sync(this.gameObject);

    }

    public void Lerp (Vector3 position, float t) {
        this.SetDestinationCenter(position);
        var dist = Vector2.Distance(new Vector2(position.x, position.y), new Vector2(this.px, this.py));
        this.MoveToDestination(dist * t);
    }

    /// <summary>
    /// ドット座標をセル座標に変換
    /// </summary>
    public E512Pos CPos () {
        return new E512Pos(this.DotCPos(this.dpx), this.DotCPos(this.dpy));
    }

    /// <summary>
    /// ドット座標をセル座標に変換
    /// </summary>
    public int DotCPos (int dp) {
        return dp < 0 ? (dp + 1) / this.tilesize - 1 : dp / this.tilesize;
    }

    /// <summary>
    /// floatをドット座標に変換
    /// </summary>
    protected int ToDot (float n) {
        return (int)(n / this.dotsize);
    }

    public float DotFloat (float n) {
        return this.dotsize * this.ToDot(n);
    }

    public Vector3 DotVector3 (Vector3 v) {
        return new Vector3(this.DotFloat(v.x), this.DotFloat(v.y), this.DotFloat(v.z));
    }
}

