using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class E512TileMapObject : MonoBehaviour {//
    [SerializeField]
    public TGUIPalette tguipalette;
    [SerializeField]
    public E512TilePalette mtilepalette;
    [SerializeField]
    public int layer = 0;
    [SerializeField]
    public int x = 32;
    [SerializeField]
    public int y = 32;

    [NonSerialized]
    public E512TileManager tilemanager;

    public MTileTerrain tileterrain;
    private Mesh gridmesh;
    private E512Block[,] blocks;
    private GameObject[,][] objs;
    void Start () {
        MTileTerrain terrain = this.gameObject.GetComponent<MTileTerrain>();
        if (terrain) {
            this.tileterrain = terrain;
        } else {
            this.tileterrain = this.gameObject.AddComponent<MTileTerrain>();
        }

        this.gridmesh = E512Mesh.Grid(E512Block.SIZE, E512Block.SIZE, 0, 0, true);

        if (this.mtilepalette != null) { this.tilemanager = new E512TileManager(this.mtilepalette); }
        if (this.tguipalette != null) { this.tilemanager = new E512TileManager(this.tguipalette); }

        var w = this.x / E512Block.SIZE + (this.x % E512Block.SIZE > 0 ? 1 : 0);
        var h = this.y / E512Block.SIZE + (this.y % E512Block.SIZE > 0 ? 1 : 0);

        this.blocks = new E512Block[w, h];
        this.objs = new GameObject[w, h][];
        for (int i = 0; i < w; ++i) {
            for (int j = 0; j < h; ++j) {
                this.blocks[i, j] = this.CreateBlock(new E512Pos(i, j));
                this.objs[i, j] = this.CreateGameObject(new E512Pos(i, j));
                this.UVSet(new E512Pos(i, j));
            }
        }
    }

    public override string ToString () {
        return string.Format("GridSize:{0}");
    }

    /// <summary>
    /// マップ範囲内または無限マップなら真
    /// inside or infinity map is true
    /// </summary>
    private bool InSide (E512Pos cpos) {
        return ((cpos.x < this.x && cpos.x >= 0 && cpos.y < this.y && cpos.y >= 0)) ? true : false;
    }

    /// <summary>
    /// ブロックデータ作成
    /// </summary>
    private E512Block CreateBlock (E512Pos bpos) {
        E512Block b = new E512Block(this.layer);
        E512Pos bcpos = bpos * E512Block.SIZE;// ブロックのセル座標
        for (int x = 0; x < E512Block.SIZE; ++x) {
            for (int y = 0; y < E512Block.SIZE; ++y) {
                E512Pos cpos = bcpos + new E512Pos(x, y);// ブロックのセル座標＋ブロック内座標
                if (this.InSide(cpos)) {// 内側
                    for (int z = 0; z < this.layer; ++z) {
                        b.SetTileIndex(this.LoadTileIndex(cpos, z), z, x, y);
                        b.SetAutoTileIndex(this.LoadAutoTileIndex(cpos, z), z, x, y);
                    }
                } else {// 外側 アウトサイドで初期化
                    for (int z = 0; z < this.layer; ++z) {
                        b.SetTileIndex(E512Tile.OutSide, z, x, y);
                        b.SetAutoTileIndex(0, z, x, y);
                    }
                }
            }
        }
        return b;
    }
    

    /// <summary>
    /// GameObject作成,座標,親子関係設定
    /// </summary>
    private GameObject[] CreateGameObject (E512Pos bpos) {
        GameObject[] objs = new GameObject[this.layer];
        for (int i = 0; i < this.layer; ++i) {
            objs[i] = new GameObject(string.Format("TileMap [{0}, {1}] Layer{2}", bpos.x, bpos.y, i + 1));
            objs[i].AddComponent<MeshFilter>().mesh = this.gridmesh;
            objs[i].AddComponent<MeshRenderer>().material = this.tilemanager.material;
            objs[i].GetComponent<MeshRenderer>().material.SetInt("_Layer", i);
            objs[i].transform.parent = this.transform;
            objs[i].transform.localPosition = new Vector3((float)(bpos.x * E512Block.SIZE), (float)(bpos.y * E512Block.SIZE), -0.1f * i);
            objs[i].transform.localRotation = Quaternion.identity;
        }
        return objs;
    }


    /// <summary>
    /// UVセット ブロック座標
    /// </summary>
    private void UVSet (E512Pos bpos) {
        E512Block b = this.blocks[bpos.x, bpos.y];
        for (Byte z = 0; z < this.layer; ++z) {
            MeshFilter m = this.objs[bpos.x, bpos.y][z].GetComponent<MeshFilter>();
            Vector2[] uv = m.mesh.uv;
            for (int x = 0; x < E512Block.SIZE; ++x) {
                for (int y = 0; y < E512Block.SIZE; ++y) {
                    int index = b.GetTileIndex(z, x, y);
                    E512Tile t = this.tilemanager[index];
                    int[] ai = E512AutoTile.IntToIndexArray(b.GetAutoTileIndex(z, x, y));
                    int[] vi = this.VertexIndex(x, y);
                    this.tilemanager.SetUVHalf(uv, vi, ai, t, b.GetTileLight(x, y));
                }
            }
            m.mesh.uv = uv;
        }
    }


    /// <summary>
    /// ブロック内のxy座標から１次元のVertexIndexをオートタイル（タイル）用に４つ返す
    /// </summary>
    private int[] VertexIndex (int x, int y) {
        // ０左上 １右上 ２左下 ３右下
        int x2 = x * 2;
        int y2 = y * 2;
        int x2p = x2 + 1;
        int y2p = y2 + 1;
        int[] r = new int[4];
        r[0] = 4 * (x2 + (E512Block.SIZE * 2 * y2p));
        r[1] = 4 * (x2p + (E512Block.SIZE * 2 * y2p));
        r[2] = 4 * (x2 + (E512Block.SIZE * 2 * y2));
        r[3] = 4 * (x2p + (E512Block.SIZE * 2 * y2));
        return r;
    }

    private int LoadTileIndex (E512Pos cpos, int layer) {
        return this.tileterrain.GetTileIndex(cpos, layer);
    }

    private int LoadAutoTileIndex (E512Pos cpos, int layer) {
        return this.tileterrain.GetAutoTileIndex(cpos, layer);
    }



    /// <summary>
    /// マップデータにタイルを設定
    /// </summary>
    public void SetTile (E512Pos cpos, int index, int layer) {
        
    }

    /// <summary>
    /// セットオートタイル indexと同じオートタイルのみ
    /// </summary>
    public void NormalizeAutoTile (E512Pos cpos, int tileindex, int layer) {
        
    }

    /// <summary>
    /// セットオートタイル
    /// </summary>
    public void NormalizeAutoTileWithOther (E512Pos cpos, int layer) {
        
    }

    public int GetTile (E512Pos cpos, int layer) {
        return 0;
    }
    

}
