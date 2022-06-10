using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestDrawUpDown : MonoBehaviour {
    public int green = 4;
    public int sea = 18;
    
    public E512TileMapData map;
    
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButton(0)) {
            this.Up(Camera.main.ScreenToWorldPoint(Input.mousePosition).ToE512Pos());
        }
        if (Input.GetMouseButton(1)) {
            this.Down(Camera.main.ScreenToWorldPoint(Input.mousePosition).ToE512Pos());
        }

    }

    private int Height (E512Pos p) {
        if (this.map.GetTile(p, 0) == this.sea) {
            return this.To10(-this.map.GetTileLight(p)) + 6;
        } else {
            return 7 - this.To10(-this.map.GetTileLight(p));
        }
    }

    private int To32 (int n) {
        return n * 3;
    }

    private int To10 (int n) {
        return n / 3;
    }

    private void SetHeight (E512Pos p, int h) {
        if (h < 7) {
            this.map.SetTile(p, this.sea, 0);
            this.map.FixAutoTile(p, 0);
            this.map.SetTileLight(p, this.To32(-(h - 6)));
        } else {
            this.map.SetTile(p, this.green, 0);
            this.map.FixAutoTile(p, 0);
            this.map.SetTileLight(p, this.To32((h - 7)));
        }
    }

    private void Up (E512Pos p) {
        Queue<E512Pos> q = new Queue<E512Pos>();
        q.Enqueue(p);
        q.Enqueue(p + new E512Pos(1, 0));
        q.Enqueue(p + new E512Pos(-1, 0));
        q.Enqueue(p + new E512Pos(0, 1));
        q.Enqueue(p + new E512Pos(0, -1));
        q.Enqueue(p + new E512Pos(1, 1));
        q.Enqueue(p + new E512Pos(1, -1));
        q.Enqueue(p + new E512Pos(-1, 1));
        q.Enqueue(p + new E512Pos(-1, -1));

        while (q.Count > 0) {
            var qp = q.Dequeue();
            var h = this.Height(qp);
            var tp = qp;
            tp = qp + new E512Pos(1, 0);
            if (this.Height(tp) < h && !q.Contains(tp)) { q.Enqueue(tp); }
            tp = qp + new E512Pos(-1, 0);
            if (this.Height(tp) < h && !q.Contains(tp)) { q.Enqueue(tp); }
            tp = qp + new E512Pos(0, 1);
            if (this.Height(tp) < h && !q.Contains(tp)) { q.Enqueue(tp); }
            tp = qp + new E512Pos(0, -1);
            if (this.Height(tp) < h && !q.Contains(tp)) { q.Enqueue(tp); }
            this.SetHeight(qp, Mathf.Min(h + 1, 13));
        }
        
    }

    private void Down (E512Pos p) {
        Queue<E512Pos> q = new Queue<E512Pos>();
        q.Enqueue(p);

        q.Enqueue(p + new E512Pos(1, 0));
        q.Enqueue(p + new E512Pos(-1, 0));
        q.Enqueue(p + new E512Pos(0, 1));
        q.Enqueue(p + new E512Pos(0, -1));
        q.Enqueue(p + new E512Pos(1, 1));
        q.Enqueue(p + new E512Pos(1, -1));
        q.Enqueue(p + new E512Pos(-1, 1));
        q.Enqueue(p + new E512Pos(-1, -1));

        while (q.Count > 0) {
            var qp = q.Dequeue();
            var h = this.Height(qp);
            var tp = qp;
            tp = qp + new E512Pos(1, 0);
            if (this.Height(tp) > h && !q.Contains(tp)) { q.Enqueue(tp); }
            tp = qp + new E512Pos(-1, 0);
            if (this.Height(tp) > h && !q.Contains(tp)) { q.Enqueue(tp); }
            tp = qp + new E512Pos(0, 1);
            if (this.Height(tp) > h && !q.Contains(tp)) { q.Enqueue(tp); }
            tp = qp + new E512Pos(0, -1);
            if (this.Height(tp) > h && !q.Contains(tp)) { q.Enqueue(tp); }
            this.SetHeight(qp, Mathf.Max(h - 1, 0));
        }
    }


}
