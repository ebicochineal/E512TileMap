using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DotCollision))]
public class NewSpriteTestPlayer : MonoBehaviour {
    [HideInInspector]
    public DotCollision col;
    [HideInInspector]
    public E512TileMapData map;
    public int lr = 1;
    public bool jump = false;
    public float vx = 0;
    
    public Texture texture;
    
    private Material mat;
    List<DotCollision> cols = new List<DotCollision>();
    
    private Transform t_image;
    
    void Start () {
        if (this.map == null) { this.map = E512TileMapData.SceneMap; }
        this.col = this.GetComponent<DotCollision>();
        this.t_image = this.transform.GetChild(0).transform;
        this.mat = E512Sprite.CreateMaterial(texture);
    }
    

    // Update is called once per frame
    void FixedUpdate () {
        if (Input.GetMouseButton(0)) {
            Vector2 pos = new Vector2(this.transform.position.x, this.transform.position.y + 1f);
            E512Sprite sprite = E512Sprite.Create(this.mat, 7, 10, pos, 0, 16, 0);
            NewSpriteTestCoin coin = sprite.gameObject.AddComponent<NewSpriteTestCoin>();
            coin.col = sprite.gameObject.AddComponent<DotCollision>();
            coin.cols.Add(this.col);
            coin.vx = this.lr;
            this.cols.Add(coin.col);
            sprite.gameObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            coin.col.halfheight = 2;
            coin.col.halfwidth = 2;
            coin.col.Init();
        }
        if (Input.GetMouseButton(1)) {
            Vector2 pos = new Vector2(this.transform.position.x, this.transform.position.y + 1f);
            E512Sprite sprite = E512Sprite.Create(this.mat, 7, 12, pos, 0, 16, 0);
            NewSpriteTestCoin coin = sprite.gameObject.AddComponent<NewSpriteTestCoin>();
            coin.col = sprite.gameObject.AddComponent<DotCollision>();
            coin.vx = this.lr;
            sprite.gameObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            coin.col.halfheight = 2;
            coin.col.halfwidth = 2;
            coin.col.Init();
        }
        // null delete
        for (int i = 0; i < this.cols.Count; ++i) {
            if (this.cols[i] == null) {
                int cnt = this.cols.Count - 1;
                this.cols[i] = this.cols[cnt];
                this.cols.RemoveAt(cnt);
                i -= 1;
            }
        }
        
        // right
        if (Input.GetKey(KeyCode.D)) {
            this.vx += 0.05f;
            if (Mathf.Abs(this.vx) > 0.4f) { this.vx = 0.4f; }
            this.lr = 1;
        }

        // left
        if (Input.GetKey(KeyCode.A)) {
            this.vx -= 0.05f;
            if (Mathf.Abs(this.vx) > 0.4f) { this.vx = -0.4f; }
            this.lr = -1;
        }
        this.col.MoveX(this.vx, this.cols);
        //this.col.MoveX(this.vx);
        this.vx *= 0.8f;
        
        if (this.col.isground && Input.GetKey(KeyCode.W)) { this.col.gravity = 0.35f; }
        this.col.Gravity(0.010f, cols);
        //this.col.Gravity(0.010f);
        this.ImageLR();
        
    }

    private void ImageLR () {
        var ls = this.t_image.localScale;
        if (Input.GetKey(KeyCode.D)) { this.t_image.localScale = new Vector3(Mathf.Abs(ls.x), ls.y, ls.z); }
        if (Input.GetKey(KeyCode.A)) { this.t_image.localScale = new Vector3(-Mathf.Abs(ls.x), ls.y, ls.z); }
    }
}

