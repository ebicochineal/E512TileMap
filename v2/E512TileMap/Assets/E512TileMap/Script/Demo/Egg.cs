using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(DotCollision))]
public class Egg : MonoBehaviour {
    public float speed = 0f;
    
    public DotCollision col;
    private Material material;
    public float xv = 0;
    public float gv = 0;
    public int animindex = 0;

    private int break_egg = -1;
    public bool jump = false;
    private int rn;
    private int life_counter = 600;
    
    // Use this for initialization
    void Start() {
        this.col = this.gameObject.GetComponent<DotCollision>();
        this.material = this.transform.GetChild(0).transform.GetComponent<MeshRenderer>().material;
        this.rn = Random.Range(0, 7);
    }
    
    void FixedUpdate() {
        if (this.gv > 0) {
            this.col.gravity = this.gv;
            this.gv = 0;
        }

        this.Animation();

        if (!this.col.isground) {
            this.speed = 0;
            this.jump = true;
        } else {
            if (this.jump) {
                this.jump = false;
                this.Damage();
            }
        }
        
        this.xv *= 0.99f;
        this.xv += Input.acceleration.x * 0.003f;
        
        if (this.col.isground) {
            var r = ((Mathf.Abs(this.col.dpx % 16 + 16) + 8) % 16);
            var l = ((Mathf.Abs(this.col.dpx % 16 + 16) + 7) % 16);
            if (this.col.Test(16 - r, -1).Count == 0) {
                this.xv += 0.0005f;
            } else if (this.col.Test(-(16 - (16 - l - 1)), -1).Count == 0) {
                this.xv += -0.0005f;
            }
        }

        this.life_counter -= 1;
        if (this.life_counter < 0) {
            GameObject.Destroy(this.gameObject.GetComponentInChildren<MeshFilter>().mesh);
            GameObject.Destroy(this.gameObject);
        }

        if (this.break_egg > 6) { this.xv = 0; }

        if (this.col.Test(1, 0).Count > 0 && this.col.Test(-1, 0).Count > 0) { this.xv = 0; }
        if (this.Reflect()) {
            if (Mathf.Abs(this.xv) > 0.01) { this.Damage(); }  
        }

        this.col.MoveX(this.xv);
        this.col.Gravity(0.02f);
    }

    private void Animation () {
        int n = ((this.col.dpx - 4) % 64 + 64) / 8;
        this.material.SetInt("_X", (n + this.rn) % 8);
        this.material.SetInt("_Y", Mathf.Min(Mathf.Max(this.break_egg, 0), 7));
    }

    private bool Reflect () {
        if (this.xv > 0) {
            var c = this.col.Test(1, 0);
            if (c.Count > 0) {
                if (Mathf.Abs(this.xv) > 0.1) {
                    this.col.map.SetTile(c[0], 23, 0);
                    this.col.map.FixAutoTile(c[0], 0, true);
                }

                this.xv *= -0.8f;
                this.xv = Mathf.Clamp(this.xv, -0.5f, 0.5f);
                return true;
            }
        }
        if (this.xv < 0) {
            var c = this.col.Test(-1, 0);
            if (c.Count > 0) {
                if (Mathf.Abs(this.xv) > 0.1) {
                    this.col.map.SetTile(c[0], 23, 0);
                    this.col.map.FixAutoTile(c[0], 0, true);
                }

                this.xv *= -0.8f;
                this.xv = Mathf.Clamp(this.xv, -0.5f, 0.5f);
                return true;
            }
        }
        return false;
    }


    private void Damage () {
        this.break_egg += 1;
    }

    List<E512Pos> lights = new List<E512Pos>();
    private void Light () {
        var cpos = this.col.CPos();

        int d = 10;
        int mu = cpos.y + d;
        int mr = cpos.x + d;
        int ml = cpos.x - d;
        int md = cpos.y - d;
        for (int i = md; i <= mu; ++i) {
            for (int j = ml; j <= mr; ++j) {
                var p = new E512Pos(j, i);
                var v = -(int)Vector2.Distance(new Vector2(cpos.x, cpos.y), new Vector2(p.x, p.y));

                this.col.map.SetTileLight(p, Mathf.Max(Mathf.Max(v, -10), this.col.map.GetTileLight(p)));
                this.lights.Add(p);

            }
        }
    }
}
