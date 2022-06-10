using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class TGUIData : MonoBehaviour {
    private int width = 32;
    private int height = 32;

    [NonSerialized]
    public E512TileManager tilemanager;
    
    private Mesh gridmesh;
    private TBlock[,] blocks;
    private GameObject[,] objs;

    private int w;
    private int h;

    void Start () { }

    public void Init (TGUIPalette tguipalette, int w, int h) {
        this.width = w;
        this.height = h;

        this.gridmesh = E512Mesh.Grid(TBlock.SIZE, TBlock.SIZE, 0, 0);
        this.tilemanager = new E512TileManager(tguipalette);

        this.w = this.width / TBlock.SIZE + (this.width % TBlock.SIZE > 0 ? 1 : 0);
        this.h = this.height / TBlock.SIZE + (this.height % TBlock.SIZE > 0 ? 1 : 0);

        this.blocks = new TBlock[this.w, this.h];
        this.objs = new GameObject[this.w, this.h];
        for (int i = 0; i < this.w; ++i) {
            for (int j = 0; j < this.h; ++j) {
                this.blocks[i, j] = this.CreateBlock(new E512Pos(i, j));
                this.objs[i, j] = this.CreateGameObject(new E512Pos(i, j));
                this.UVSet(new E512Pos(i, j));
            }
        }
    }

    public void ReUV () {
        for (int i = 0; i < this.w; ++i) {
            for (int j = 0; j < this.h; ++j) {
                this.UVSet(new E512Pos(i, j));
            }
        }
    }


    public override string ToString () {
        return string.Format("GridSize:{0}");
    }

    /// <summary>
    /// マップ範囲内なら真
    /// inside or infinity map is true
    /// </summary>
    private bool InSide (E512Pos cpos) {
        return (cpos.x < this.width && cpos.x >= 0 && cpos.y < this.height && cpos.y >= 0);
    }

    /// <summary>
    /// ブロックデータ作成
    /// </summary>
    private TBlock CreateBlock (E512Pos bpos) {
        TBlock b = new TBlock();
        E512Pos bcpos = bpos * TBlock.SIZE;// ブロックのセル座標
        for (int x = 0; x < TBlock.SIZE; ++x) {
            for (int y = 0; y < TBlock.SIZE; ++y) {
                E512Pos cpos = bcpos + new E512Pos(x, y);// ブロックのセル座標＋ブロック内座標
                if (this.InSide(cpos)) {// 内側
                    b.SetTileIndex(this.LoadTileIndex(cpos), x, y);
                } else {// 外側 アウトサイドで初期化
                    b.SetTileIndex(E512Tile.OutSide, x, y);
                }
            }
        }
        return b;
    }
    

    /// <summary>
    /// GameObject作成,座標,親子関係設定
    /// </summary>
    private GameObject CreateGameObject (E512Pos bpos) {
        GameObject obj;
        obj = new GameObject(string.Format("TileMap [{0}, {1}]", bpos.x, bpos.y));
        obj.AddComponent<MeshFilter>().mesh = this.gridmesh;
        obj.AddComponent<MeshRenderer>().material = this.tilemanager.material;
        obj.GetComponent<MeshRenderer>().material.SetInt("_Layer", 0);
        obj.transform.parent = this.transform;
        obj.transform.localPosition = new Vector3((float)(bpos.x * TBlock.SIZE), (float)(bpos.y * TBlock.SIZE), 0);
        obj.transform.localRotation = Quaternion.identity;
        return obj;
    }


    /// <summary>
    /// UVセット ブロック座標
    /// </summary>
    public void UVSet (E512Pos bpos) {
        TBlock b = this.blocks[bpos.x, bpos.y];
        MeshFilter m = this.objs[bpos.x, bpos.y].GetComponent<MeshFilter>();
        Vector2[] uv = m.mesh.uv;
        for (int x = 0; x < TBlock.SIZE; ++x) {
            for (int y = 0; y < TBlock.SIZE; ++y) {
                int index = b.GetTileIndex(x, y);
                E512Tile t = this.tilemanager[index];
                this.tilemanager.SetUV(uv, 4 * TBlock.SIZE * y + x * 4, t);
                

                //this.tilemanager.SetUV(uv, 4 * TBlock.SIZE * y + x * 4, t, 0);
            }
        }
        m.mesh.uv = uv;
    }

    private int LoadTileIndex (E512Pos cpos) {
        return 1;
    }
    
    
    public void SetTile (E512Pos cpos, int index) {
        if (!this.InSide(cpos)) { return; }
        E512Pos bpos = TBlock.BPos(cpos);
        E512Pos blpos = TBlock.BLocalPos(cpos);
        TBlock b = this.blocks[bpos.x, bpos.y];
        b.SetTileIndex(index, blpos.x, blpos.y);
    }

    public void Destroy () {
        foreach (var i in this.objs) {
            GameObject.Destroy(i.GetComponent<MeshFilter>().mesh);
            GameObject.Destroy(i);
        }
    }
}
