using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TileType {
    NormalTile,
    AutoTile
}

public enum TileCollisionType {
    Passable,
    NoPassable
}

[System.Serializable]
public class E512Tile {
    public const int OutSide = 0;
    public const int Blank = 1;
    public const int Black = 2;
    public const int BrightLevel = 32;
    public int x;
    public int y;
    public TileCollisionType collisiontype;
    public TileType tiletype;
    public int anim;
    public Vector2[,,] pos;
    
    

    public E512Tile (int x, int y, TileCollisionType collisiontype, TileType tiletype, int anim) {
        this.x = x;
        this.y = y;
        this.collisiontype = collisiontype;
        this.tiletype = tiletype;
        this.anim = anim;
    }

    public E512Tile (E512Tile o) {
        this.x = o.x;
        this.y = o.y;
        this.collisiontype = o.collisiontype;
        this.tiletype = o.tiletype;
        this.anim = o.anim;
    }

    public void InitPos (int texture_width, int texture_height, int tilesize) {
        // AutoTileIndex  <>, ||, =, +, [ ] 
        this.pos = new Vector2[5, 4, 4];// autotileindex, vartexindex, (ru, ld, lu, rd)
        float hpx = 1f / (float)(texture_width / (tilesize / 2));
        float hpy = 1f / (float)(texture_height / (tilesize / 2));
        E512Pos[] p = this.NormalTile();
        for (int i = 0; i < 5; ++i) {
            for (int j = 0; j < 4; ++j) {
                if (i > 0 && this.tiletype == TileType.AutoTile) {  p[j].y += 2; }// texture pos １つ飛ばしで下に
                float xpx = p[j].x * hpx;
                float ypy = p[j].y * hpy;
                float right = hpx + xpx + this.anim - 0.00001f;
                float left = xpx + this.anim + 0.00001f;
                float up = 1f - ypy - 0.00001f;
                float down = 1f - hpy - ypy + 0.00001f;
                this.pos[i, j, 0] = new Vector2(right, up);
                this.pos[i, j, 1] = new Vector2(left, down);
                this.pos[i, j, 2] = new Vector2(left, up);
                this.pos[i, j, 3] = new Vector2(right, down);
            }
        }
    }


    /// <summary>
    /// ノーマルタイルのテクスチャ座標を返します
    /// </summary>
    private E512Pos[] NormalTile () {
        E512Pos[] r = new E512Pos[4];
        E512Pos p = new E512Pos(this.x, this.y) * 2;
        r[0] = p + new E512Pos(0, 0);
        r[1] = p + new E512Pos(1, 0);
        r[2] = p + new E512Pos(0, 1);
        r[3] = p + new E512Pos(1, 1);
        return r;
    }


    public static List<E512Tile> CommonTiles () {
        List<E512Tile> r = new List<E512Tile>();
        r.Add(new E512Tile(-3, 0, TileCollisionType.NoPassable, TileType.NormalTile, 0));// アウトサイド
        r.Add(new E512Tile(-2, 0, TileCollisionType.Passable, TileType.NormalTile, 0));// ブランク
        r.Add(new E512Tile(-1, 0, TileCollisionType.NoPassable, TileType.NormalTile, 0));// ブラック
        return r;
    }
}


public class E512AutoTile {
    /// <summary>
    /// AutoTileIndexArrayからAutoTileIndexInt
    /// </summary>
    public static int IndexArrayToInt (int v1, int v2, int v3, int v4) {
        return (v4 << 12) | (v3 << 8) | (v2 << 4) | (v1);
    }
    public static int IndexArrayToInt (int[] v) {
        return (v[3] << 12) | (v[2] << 8) | (v[1] << 4) | (v[0]);
    }

    /// <summary>
    /// AutoTileIndexIntからAutoTileIndexArray
    /// 1つのintから4つのintに分ける
    /// </summary>
    public static int[] IntToIndexArray (int v) {
        int[] r = new int[4];
        r[0] = v & 15;
        r[1] = (v >> 4) & 15;
        r[2] = (v >> 8) & 15;
        r[3] = (v >> 12) & 15;
        return r;
    }

    /// <summary>
    /// BoolArrayからAutoTileIndexArray
    /// </summary>
    public static int[] BoolArrayToIndexArray (bool[] b) {
        // 0 1 2   0 1
        // 3 4 5   2 3
        // 6 7 8

        int[] r = { 0, 0, 0, 0 };

        // index 1,2 3
        if (b[1]) {
            r[0] = (r[0] == 0) ? 1 : 3;
            r[1] = (r[1] == 0) ? 1 : 3;
        }
        if (b[3]) {
            r[0] = (r[0] == 0) ? 2 : 3;
            r[2] = (r[2] == 0) ? 2 : 3;
        }
        if (b[5]) {
            r[1] = (r[1] == 0) ? 2 : 3;
            r[3] = (r[3] == 0) ? 2 : 3;
        }
        if (b[7]) {
            r[2] = (r[2] == 0) ? 1 : 3;
            r[3] = (r[3] == 0) ? 1 : 3;
        }

        // index4 斜め方向がtrueかつindex3なら4に
        if (b[0] && r[0] == 3) { r[0] = 4; }
        if (b[2] && r[1] == 3) { r[1] = 4; }
        if (b[6] && r[2] == 3) { r[2] = 4; }
        if (b[8] && r[3] == 3) { r[3] = 4; }

        return r;
    }

    /// <summary>
    /// 周囲８中心１の配列:int[9]
    /// 中心[4]と同じかどうかboolで:BoolArray
    /// int[9]からBoolArray
    /// </summary>
    public static bool[] BoolArray (int[] array, bool outside_connect = true) {
        bool[] r = new bool[9];
        if (outside_connect) {
            for (int i = 0; i < 9; ++i) {
                // 中心のindexまたは外側ならtrue
                r[i] = (array[4] == array[i] || E512Tile.OutSide == array[i]);
            }
        } else {
            for (int i = 0; i < 9; ++i) {
                r[i] = (array[4] == array[i]);
            }
        }
        
        
        return r;
    }

    /// <summary>
    /// int[9]からAutoTileIndexIntに変換して返します
    /// </summary>
    public static int IndexInt (int[] array, bool outside_connect) {
        bool[] boolarray = E512AutoTile.BoolArray(array, outside_connect);
        int[] indexarray = E512AutoTile.BoolArrayToIndexArray(boolarray);
        return E512AutoTile.IndexArrayToInt(indexarray);
    }
}

public class E512TileTexturePos {

    /*
    r[0] = p + new MPos (0, 0);
    r[1] = p + new MPos (1, 0);
    r[2] = p + new MPos (0, 1);
    r[3] = p + new MPos (1, 1);
    */
}

public class E512TilePen {
    public static int index = 0;

}








