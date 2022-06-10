using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class E512Block {
    public const int SIZE = 16;
    public const int BLOCKSIZE = 16 * 16;
    public const int SECTORSIZE = 16;
    public int layer;// レイヤー数
    private int[] tileindex;// layer, X, Y
    private int[] autotileindex;// layer, X, Y
    private int[] light;// X, Y

    public bool updated_block;
    public bool inside_block;

    public E512Block (int layer) {
        this.layer = (byte)layer;
        this.tileindex = new int[layer * E512Block.BLOCKSIZE];
        this.autotileindex = new int[layer * E512Block.BLOCKSIZE];
        this.light = new int[E512Block.BLOCKSIZE];
    }

    private static int Convert (int layer, int x, int y) {
        return x + (E512Block.SIZE * y) + (E512Block.BLOCKSIZE * layer);
    }

    public void SetTileIndex (int index, int layer, int x, int y, bool update, bool inside) {
        this.updated_block = update;
        this.inside_block = inside;
        this.tileindex[E512Block.Convert(layer, x, y)] = index;
    }

    public void SetTileIndex (int index, int layer, int x, int y) {
        this.updated_block = true;
        this.tileindex[E512Block.Convert(layer, x, y)] = index;
    }

    public int GetTileIndex (int layer, int x, int y) {
        return this.tileindex[E512Block.Convert(layer, x, y)];
    }

    public void SetTileLight (int n, int x, int y, bool update, bool inside) {
        this.updated_block = update;
        this.inside_block = inside;
        this.light[E512Block.Convert(0, x, y)] = n;
    }

    public void SetTileLight (int n, int x, int y) {
        this.updated_block = true;
        this.light[E512Block.Convert(0, x, y)] = n;
    }


    public int GetTileLight (int x, int y) {
        return this.light[E512Block.Convert(0, x, y)];
    }


    public void SetAutoTileIndex (int index, int layer, int x, int y, bool update, bool inside) {
        this.updated_block = update;
        this.inside_block = inside;
        this.autotileindex[E512Block.Convert(layer, x, y)] = index;
    }

    public void SetAutoTileIndex (int index, int layer, int x, int y) {
        this.updated_block = true;
        this.autotileindex[E512Block.Convert(layer, x, y)] = index;
    }

    public int GetAutoTileIndex (int layer, int x, int y) {
        return this.autotileindex[E512Block.Convert(layer, x, y)];
    }

    /// <summary>
    /// AutoTileIndexを設定
    /// IndexArrayをIndexIntに変換,設定
    /// </summary>
    //public void SetAutoTileIndexArray (int[] autotileindexarray, int layer, int x, int y) {
    //    int[] a = autotileindexarray;
    //    this.SetAutoTileIndex(E512AutoTile.IndexArrayToInt(a[0], a[1], a[2], a[3]), layer, x, y);
    //}


    // ２点セル座標からボックス範囲内のブロックリストを作成
    public static List<E512Pos> BoxBlockList (E512Pos start, E512Pos end) {
        var r = new List<E512Pos>();
        var sx = E512Block.BValue(start.x);
        var ex = E512Block.BValue(end.x) + 1;
        var sy = E512Block.BValue(start.y);
        var ey = E512Block.BValue(end.y) + 1;
        for (int x = sx; x < ex; ++x) {
            for (int y = sy; y < ey; ++y) {
                r.Add(new E512Pos(x, y));
            }
        }
        return r;
    }

    public static int BValue (int v) {
        return v < 0 ? (((v + 1) / (E512Block.SIZE)) - 1) : v / E512Block.SIZE;
    }

    /// <summary>
    /// セル座標をブロック座標に変換
    /// </summary>
    public static E512Pos BPos (E512Pos cpos) {
        int x = E512Block.BValue(cpos.x);
        int y = E512Block.BValue(cpos.y);
        return new E512Pos(x, y);
    }

    public static int BLocalValue (int v) {
        return v < 0 ? ((v + 1) % E512Block.SIZE) + (E512Block.SIZE - 1) : v % E512Block.SIZE;
    }

    /// <summary>
    /// セル座標をブロック内座標に変換
    /// </summary>
    public static E512Pos BLocalPos (E512Pos cpos) {
        int x = E512Block.BLocalValue(cpos.x);
        int y = E512Block.BLocalValue(cpos.y);
        return new E512Pos(x, y);
    }

    public static E512Pos BPosToSector (E512Pos bpos) {
        E512Pos r = new E512Pos(bpos.x / E512Block.SECTORSIZE, bpos.y / E512Block.SECTORSIZE);
        r.x -= bpos.x < 0 ? 1 : 0;
        r.y -= bpos.y < 0 ? 1 : 0;
        return r;
    }

    public static E512Pos BPosNameToBPos (string s) {
        var t = s.Remove(0, 1).Replace('m', '-').Split('y');
        var x = int.Parse(t[0]);
        var y = int.Parse(t[1]);
        return new E512Pos(x, y);
    }

    public static string BPosName (E512Pos bpos) {
        string r = "";
        r += "x" + (bpos.x < 0 ? "m" + Mathf.Abs(bpos.x).ToString() : bpos.x.ToString());
        r += "y" + (bpos.y < 0 ? "m" + Mathf.Abs(bpos.y).ToString() : bpos.y.ToString());
        return r;
    }

    public static string SectorName (E512Pos spos) {
        string r = "";
        r += "x" + (spos.x < 0 ? "m" + Mathf.Abs(spos.x).ToString() : spos.x.ToString());
        r += "y" + (spos.y < 0 ? "m" + Mathf.Abs(spos.y).ToString() : spos.y.ToString());
        return r;
    }

    public static E512Pos SectorNameToSPos (string s) {
        var t = s.Remove(0, 1).Replace('m', '-').Split('y');
        var x = int.Parse(t[0]);
        var y = int.Parse(t[1]);
        return new E512Pos(x, y);
    }

    /// <summary>
    /// ブロック座標からセクター内インデックスに変換
    /// </summary>
    public static int SectorIndex (E512Pos bpos) {
        int x = bpos.x < 0 ? E512Block.SECTORSIZE + bpos.x % E512Block.SECTORSIZE : bpos.x % E512Block.SECTORSIZE;
        int y = bpos.y < 0 ? E512Block.SECTORSIZE + bpos.y % E512Block.SECTORSIZE : bpos.y % E512Block.SECTORSIZE;
        return x + y * E512Block.SECTORSIZE;
    }
    
    public int BlockByteSize () {
        return 1 + 2 * E512Block.BLOCKSIZE * 2 * this.layer + E512Block.BLOCKSIZE;
    }

    // {1byte layer size + 2byte tile block * layer, 2byte auto block * layer, 1byte light block}
    public byte[] ToBytes () {
        byte[] r = new byte[this.BlockByteSize()];
        r[0] = (byte)this.layer;
        // tileindex
        int cnt = 1;
        byte[] tmp;
        for (int i = 0; i < E512Block.BLOCKSIZE * this.layer; ++i) {
            tmp = BitConverter.GetBytes((short)this.tileindex[i]);
            r[cnt++] = tmp[0];
            r[cnt++] = tmp[1];
        }

        // autotileindex
        for (int i = 0; i < E512Block.BLOCKSIZE * this.layer; ++i) {
            tmp = BitConverter.GetBytes((short)this.autotileindex[i]);
            r[cnt++] = tmp[0];
            r[cnt++] = tmp[1];
        }

        // light
        for (int i = 0; i < E512Block.BLOCKSIZE; ++i) {
            tmp = BitConverter.GetBytes((byte)this.light[i]);
            r[cnt++] = tmp[0];
        }
        
        return r;
    }
}
