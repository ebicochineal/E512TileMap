using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(DotCollision))]
public class PlayerSide : MonoBehaviour {
    [HideInInspector]
    public DotCollision col;
    [HideInInspector]
    public E512TileMapData map;
    private Transform t_image;
    public int lr = -1;
    public int mode = 0;


    private GameObject egg;
    private GameObject fire;
    private GameObject ebiblue;

    private float lighttmp;

    // Use this for initialization
    void Start () {
        if (this.map == null) { this.map = E512TileMapData.SceneMap; }
        this.col = this.GetComponent<DotCollision>();
        this.t_image = this.transform.GetChild(0).transform;

        this.egg = (GameObject)Resources.Load("Prefab/Egg");
        this.fire = (GameObject)Resources.Load("Prefab/Fire");
        this.ebiblue = (GameObject)Resources.Load("Prefab/EbiBlue");

        this.lighttmp = this.GetComponentInChildren<Light>().range;
    }

    int test = 0;

    // Update is called once per frame
    void Update () {
        for (int i = 0; i < 1; ++i) {
            if (mode == 0 && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))) {
                var obj = GameObject.Instantiate(egg, this.transform.position, Quaternion.identity);
                obj.GetComponent<Egg>().gv = Random.Range(0.2f, 0.5f);
                obj.GetComponent<Egg>().xv = this.lr * 0.5f;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)) { this.test = (this.test + 1) % 65; }
            if (mode == 1 && (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)) && this.col.Test(16 * this.lr, 0).Count < 1) {
                var obj = GameObject.Instantiate(this.fire, this.transform.position + new Vector3(this.lr, 0, 0), Quaternion.identity);
                obj.GetComponent<Fire>().ay = -Random.Range(0.0f, 0.3f);
                obj.GetComponent<Fire>().ax = this.lr * Random.Range(0.5f, 0.8f);
                this.GetComponentInChildren<Light>().range = Random.Range(2f, 24f);
            } else {
                this.GetComponentInChildren<Light>().range = this.lighttmp;
            }


            if (mode == 2 && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)) && this.col.Test(16 * this.lr, 0).Count < 1) {
                var obj = GameObject.Instantiate(this.ebiblue, this.transform.position + new Vector3(this.lr, 0, 0), Quaternion.identity);
                obj.transform.parent = GameObject.Find("Enemy").transform;

            }
            
            if (Input.GetKey(KeyCode.D)) {
                this.col.MoveX(0.2f);
                this.lr = 1;
            }
            if (Input.GetKey(KeyCode.A)) {
                this.col.MoveX(-0.2f);
                this.lr = -1;
            }
            
            if (Input.GetKey(KeyCode.S) && this.col.isground) {
                var r = ((Mathf.Abs(this.col.dpx % 16 + 16) + 8) % 16);
                var l = ((Mathf.Abs(this.col.dpx % 16 + 16) + 7) % 16);
                if (this.col.Test(16 - r, -1).Count == 0) {
                    this.col.MoveX(1);
                    
                }else if (this.col.Test(-(16 - (16 - l - 1)), -1).Count == 0) {
                    this.col.MoveX(-1);
                }
            }

            if (this.col.isground && Input.GetKey(KeyCode.W)) {
                this.col.gravity += 0.45f;
            }


            this.col.Gravity(0.015f);

            this.ImageLR();
        }
    }
    
    private void ImageLR () {
        var ls = this.t_image.localScale;
        if (Input.GetKey(KeyCode.D)) { this.t_image.localScale = new Vector3(Mathf.Abs(ls.x), ls.y, ls.z); }
        if (Input.GetKey(KeyCode.A)) { this.t_image.localScale = new Vector3(-Mathf.Abs(ls.x), ls.y, ls.z); }
    }


    List<E512Pos> lights = new List<E512Pos>();
    private void Light () {
        var cpos = this.col.CPos();

        int d = 10;
        int mu = cpos.y + d;
        int mr = cpos.x + d;
        int ml = cpos.x - d;
        int md = cpos.y - d;
        foreach (var i in this.lights) { this.map.SetTileLight(i, -10); }
        this.lights.Clear();
        for (int i = md; i <= mu; ++i) {
            for (int j = ml; j <= mr; ++j) {
                var p = new E512Pos(j, i);
                var v = -(int)Vector2.Distance(new Vector2(cpos.x, cpos.y), new Vector2(p.x, p.y));
                
                this.col.map.SetTileLight(p, Mathf.Max(v, -9));
                this.lights.Add(p);
            }
        }
    }
}
