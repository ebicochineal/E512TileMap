using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(DotCollision))]
public class Fire : MonoBehaviour {
    public float speed = 0f;

    public DotCollision col;
    private Material material;
    public float ax = 0;
    public float ay = 0;
    public int animindex = 0;

    private Transform t_image;
    
    private int life_counter = 30;
    private float v = 0;

    private EnemySide[] ecols;
    private BrightManager brights;

    // Use this for initialization
    void Start () {
        this.col = this.gameObject.GetComponent<DotCollision>();
        this.material = this.transform.GetChild(0).transform.GetComponent<MeshRenderer>().material;
        this.t_image = this.transform.GetChild(0).transform;
        this.ecols = GameObject.Find("Enemy").GetComponentsInChildren<EnemySide>();
        this.brights = GameObject.Find("BrightManager").GetComponent<BrightManager>();
    }

    // Update is called once per frame
    void Update () {
        if (this.ay < 0) {
            this.col.gravity = this.ay;
        }

        this.Animation();
        this.Reflect();
        
        this.ay += 0.02f;
        this.ay = this.ay > 0.3f ? 0.3f : this.ay;
        this.v *= 0.9f;
        this.ax *= 0.985f;

        
        this.life_counter -= 1;
        if (this.life_counter < 0) {
            GameObject.Destroy(this.gameObject.GetComponentInChildren<MeshFilter>().mesh);
            GameObject.Destroy(this.gameObject);
        }
        
        if (this.col.MoveXDeltaTime(this.ax + this.v)) {
            if (this.ax + this.v > 0) {
                this.brights.DinamicBrightness(this.col.CPos() + new E512Pos(1, 0), 6);
            } else {
                this.brights.DinamicBrightness(this.col.CPos() + new E512Pos(-1, 0), 6);
            }

        }

        if (this.col.MoveYDeltaTime(this.ay)) {
            if (this.ay > 0) {
                this.brights.DinamicBrightness(this.col.CPos() + new E512Pos(0, 1), 6);
            } else {
                this.brights.DinamicBrightness(this.col.CPos() + new E512Pos(0, -1), 6);
            }

        }


        foreach (var i in this.ecols) {
            if (this.col.AABBTest(i.col)) {
                i.Damage(5);
            }
        }
        
        this.ImageLR();
    }


    private void Animation () {
        this.material.SetInt("_X", Random.Range(0, 4));
    }
    
    private void ImageLR () {

        var ls = this.t_image.localScale;
        if (this.ax < 0) {
            this.t_image.localScale = new Vector3(Mathf.Abs(ls.x), ls.y, ls.z);
        } else {
            this.t_image.localScale = new Vector3(-Mathf.Abs(ls.x), ls.y, ls.z);
        }
    }

    private bool Reflect () {
        if (this.ax > 0) {
            var c = this.col.Test(1, 0);
            if (c.Count > 0) {
                this.v = -Random.Range(0.5f, 0.8f);
                if (this.col.Test(0, -1).Count > 0) {
                    this.ay = 0.05f;
                } else {
                    this.ay -= 0.05f;
                }
                return true;
            }
        }
        if (this.ax < 0) {
            var c = this.col.Test(-1, 0);
            if (c.Count > 0) {
                this.v = Random.Range(0.5f, 0.8f);
                if (this.col.Test(0, -1).Count > 0) {
                    this.ay = 0.05f;
                } else {
                    this.ay -= 0.05f;
                }
                return true;
            }
        }
        return false;
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
