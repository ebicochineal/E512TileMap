using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DotCollision))]
public class EnemySide : MonoBehaviour {
    public DotCollision col;
    private Transform t_image;
    private Material mat;

    private int hp = 600;
    
    private float ax = 0f;
    public int lr = -1;
    private float heat = 0;
    private bool islife = true;

    private float dcnt = 0;
    private Mesh dmesh;
    private Mesh drmesh;
    private Material dmat;
    private Vector3 dlast;

    // Use this for initialization
    void Start() {
        this.col = this.gameObject.GetComponent<DotCollision>();
        this.t_image = this.transform.GetChild(0).transform;
        this.mat = this.GetComponentInChildren<MeshRenderer>().material;
        
        this.dmesh = E512Mesh.Quad;
        this.drmesh = E512Mesh.RQuad;
        this.dmat = Resources.Load<Material>("Material/ebiblue");
    }
    
    void FixedUpdate() {
        if (this.islife) {
            if (Random.Range(-3.1f, 3.1f) > 3f) { this.ax += 0.05f; this.col.gravity = 0.03f; }
            if (Random.Range(-3.1f, 3.1f) < -3f) { this.ax -= 0.05f; this.col.gravity = 0.03f; }
            this.ax = Mathf.Clamp(this.ax, -0.2f, 0.2f);
            this.ax *= 0.98f;
            this.col.MoveX(this.ax);

            var d = 0;
            foreach (var i in this.col.Test(0, -1)) {
                var v = E512Tile.BrightLevel - Mathf.Abs(this.col.map.GetTileLight(i));
                if (v > E512Tile.BrightLevel / 2 + E512Tile.BrightLevel / 4) {
                    d = v > d ? v : d;
                }
            }
            if (d > 0) { this.Damage(d); }

            this.ImageLR();
            this.Died();

            this.dlast = this.transform.position;
            
        } else {
            if (this.dcnt < 5) {
                var t = this.col.DotVector3(this.dlast + new Vector3(0, this.dcnt, 0));
                if (this.t_image.localScale.x > 0) {
                    Graphics.DrawMesh(this.drmesh, t, Quaternion.identity, this.dmat, 0);
                } else {
                    Graphics.DrawMesh(this.dmesh, t, Quaternion.identity, this.dmat, 0);
                }
                
                this.dcnt += 0.02f;
            }
        }

        this.HPColor();
        this.Heat();

        this.col.Gravity(0.02f);
        
    }

    private void Heat () {
        this.mat.SetFloat("_Emission", this.heat / E512Tile.BrightLevel);
        this.heat = this.heat > 0 ? this.heat - 0.2f : 0;
    }

    public void Damage () {
        this.heat = E512Tile.BrightLevel;
        this.hp -= 1;
        if (this.col.isground && this.islife) { this.col.gravity = 0.25f; }
    }

    public void Damage (int n) {
        this.heat = E512Tile.BrightLevel;
        this.hp -= n;
        if (this.col.isground && this.islife) { this.col.gravity = 0.25f; }
    }

    private void ImageLR () {
        var ls = this.t_image.localScale;
        if (this.ax > 0) {
            this.t_image.localScale = new Vector3(Mathf.Abs(ls.x), ls.y, ls.z);
        } else {
            this.t_image.localScale = new Vector3(-Mathf.Abs(ls.x), ls.y, ls.z);
        }
    }

    private void HPColor () {
        this.hp = this.hp < 0 ? 0 : this.hp;
        var v = (float)this.hp / (float)600;
        var r = Mathf.Max(1f - v, 0.2f);
        var g = 0.2f;
        var b = Mathf.Max(v, 0.2f);
        this.mat.SetColor("_BaseColor", new Color(r, g, b));
    }

    private void Died () {
        if (this.hp < 1) {
            this.gameObject.GetComponentInChildren<CharactorDotAnim>().enabled = false;

            var ls = this.t_image.localScale;
            this.t_image.localScale = new Vector3(ls.x, -ls.y, ls.z);
            this.t_image.localPosition = new Vector3(0, -this.col.dotsize * 7, 0);
            this.islife = false;
        }
    }

}
