using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestDraw : MonoBehaviour {
    public E512TileMapData map;
    public int layer = 0;

    private int gindex = 3;
    private Texture2D image;

    // boxdraw
    Vector3 left = new Vector3();
    bool m = false;

    // Use this for initialization
    void Start() {
        E512TilePen.index = 3;
        this.image = this.map.tilemanager.GetTileTexture(this.gindex);
    }

    // Update is called once per frame
    void Update() {

        // 右クリック　タイルをコピー
        if (Input.GetMouseButtonDown(1) && Input.mousePosition.y < Screen.height - 24) {
            E512Pos p = Camera.main.ScreenToWorldPoint(Input.mousePosition).ToE512Pos();
            E512TilePen.index = this.map.GetTile(p, this.layer);
        }

        // ボックス入力
        if (Input.GetMouseButtonDown(0) && Input.mousePosition.y < Screen.height - 24) {
            this.left = Input.mousePosition;
            this.m = true;
        }
        if (Input.GetMouseButtonUp(0) && Input.mousePosition.y < Screen.height - 24 && this.m) {
            this.m = false;
            E512Pos s = Camera.main.ScreenToWorldPoint(this.left).ToE512Pos();
            E512Pos e = Camera.main.ScreenToWorldPoint(Input.mousePosition).ToE512Pos();
            foreach (E512Pos i in E512Pos.BoxList(s, e)) {
                this.map.SetTile(i, E512TilePen.index, this.layer);// マップ書き換え
            }
            foreach (E512Pos i in E512Pos.BoxList(s, e, 0)) {
                this.map.FixAutoTile(i, this.layer);// オートタイル修正
            }
        }
    }

    private void OnGUI() {
        float w = Screen.width / 2;
        if (GUI.Button(new Rect(w - 24 * 2, 0, 24, 24), "<")) {
            var l = this.map.tilemanager.ImagesArray().Length - 3;
            this.gindex = 3 + (l + this.gindex - 3 - 1) % l;
            E512TilePen.index = this.gindex;
            this.image = this.map.tilemanager.GetTileTexture(this.gindex);
        }
        if (GUI.Button(new Rect(w - 24, 0, 24, 24), ">")) {
            var l = this.map.tilemanager.ImagesArray().Length - 3;
            this.gindex = 3 + (l + this.gindex - 3 + 1) % l;
            E512TilePen.index = this.gindex;
            this.image = this.map.tilemanager.GetTileTexture(this.gindex);
        }
        if (GUI.Button(new Rect(w, 0, 24, 24), this.image)) {
            E512TilePen.index = this.gindex;
        }
        if (GUI.Button(new Rect(w + 24, 0, 24, 24), "")) {
            E512TilePen.index = 1;
        }
    }
}
