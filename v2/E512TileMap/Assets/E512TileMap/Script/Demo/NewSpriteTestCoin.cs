using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSpriteTestCoin : MonoBehaviour {
    public DotCollision col;
    public E512Sprite sprite;
    public float vx = 0f;
    public float vy = 0.5f;
    
    private int sx;
    private int sy;
    private int z = 0;
    public List<DotCollision> cols = new List<DotCollision>();
    void Start() {
        this.col = this.GetComponent<DotCollision>();
        this.col.gravity = this.vy;
        this.sprite = this.GetComponent<E512Sprite>();
        this.sx = this.sprite.tx;
        this.sy = this.sprite.ty;
    }
    void FixedUpdate() {
        if (this.col.MoveX(this.vx, this.cols)) { this.vx *= -0.8f+ + Random.Range(0.0f, 0.2f); }
        this.vx *= 0.98f;
        this.col.Gravity(0.010f, this.cols);
        if (this.col.isground) {
            this.vy *= 0.5f + Random.Range(0.0f, 0.2f);
            this.col.gravity = this.vy;
        }
        this.sprite.SetUV(this.sx + this.z / 2 % 4, this.sy);
        this.z += 1;
        if (this.z > 256) { this.sprite.Destroy(); }
    }
}
