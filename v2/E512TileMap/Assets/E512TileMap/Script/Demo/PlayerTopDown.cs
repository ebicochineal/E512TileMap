using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CellMove))]
public class PlayerTopDown : MonoBehaviour {
    [HideInInspector]
    public DotCollision collision;
    [HideInInspector]
    public E512TileMapData map;

    private Transform t_image;
    [HideInInspector]
    public CellMove move;
    
    // Use this for initialization
    void Start () {
        if (this.map == null) { this.map = E512TileMapData.SceneMap; }
        this.move = this.GetComponent<CellMove>();
        this.t_image = this.transform.GetChild(0).transform;
    }

    
    void FixedUpdate () {
        for (int i = 0; i < 1; ++i) {
            if (Input.GetKey(KeyCode.D)) {
                this.move.Order(CellMove.Right);
            }
            if (Input.GetKey(KeyCode.A)) {
                this.move.Order(CellMove.Left);
            }
            if (Input.GetKey(KeyCode.W)) {
                this.move.Order(CellMove.Up);
            }
            if (Input.GetKey(KeyCode.S)) {
                this.move.Order(CellMove.Down);
            }
            this.move.Move();
            this.ImageLR(this.move.GetDirection());
        }
    }
    
    private void ImageLR (int d) {
        var ls = this.t_image.localScale;
        if (d == CellMove.Right) { this.t_image.localScale = new Vector3(Mathf.Abs(ls.x), ls.y, ls.z); }
        if (d == CellMove.Left) { this.t_image.localScale = new Vector3(-Mathf.Abs(ls.x), ls.y, ls.z); }
    }
    
}
