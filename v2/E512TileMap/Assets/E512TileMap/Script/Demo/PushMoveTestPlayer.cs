using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DotCollision))]
public class PushMoveTestPlayer : MonoBehaviour {
    [HideInInspector]
    public DotCollision col;
    
    public int lr = 1;
    public Texture texture;
    private Transform t_image;
    
    private Material mat;
    public List<DotCollision> cols = new List<DotCollision>();
    
    
    float vx = 0f;
    
    void Start () {
        this.col = this.GetComponent<DotCollision>();
        this.t_image = this.transform.GetChild(0).transform;
        this.mat = E512Sprite.CreateMaterial(texture);
    }
    
    void FixedUpdate () {
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
        this.col.PushMoveX(this.vx, this.cols);
        this.vx *= 0.8f;
        
        if (this.col.isground && Input.GetKey(KeyCode.W)) { this.col.gravity = 0.35f; }
        this.col.Gravity(0.010f, this.cols);
        
        if (this.col.isground) { this.col.PushMoveY(-1/32f, this.cols); }
        
        this.ImageLR();
    }

    private void ImageLR () {
        var ls = this.t_image.localScale;
        if (Input.GetKey(KeyCode.D)) { this.t_image.localScale = new Vector3(Mathf.Abs(ls.x), ls.y, ls.z); }
        if (Input.GetKey(KeyCode.A)) { this.t_image.localScale = new Vector3(-Mathf.Abs(ls.x), ls.y, ls.z); }
    }
}

