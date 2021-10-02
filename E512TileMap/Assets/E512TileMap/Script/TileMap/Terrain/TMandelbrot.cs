using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMandelbrot : E512TileTerrain {
    public float scale = 0.00001f;
    int tmp = 0;
    public override int GetTileIndex (E512Pos cpos, int layer) {
        if (layer > 0) { return E512Tile.Blank; }
        var x = cpos.x * scale;
        var y = cpos.y * scale;
        var px = x;
        var py = y;
        Vector2 z = Vector2.zero;
        Vector2 t = Vector2.zero;
        int c = 0;
        bool f = false;
        for (int n = 0; n < 255; ++n) {
            t.x = z.x * z.x - z.y * z.y + px;
            t.y = 2f * z.x * z.y + py;
            z = t;
            c = n;
            if (Vector2.Distance(z, Vector2.zero) > 2) {
                f = true;
                break;
            }
        }
        if (f) {
            this.tmp = c / 8;
            return 10;
        } else {
            return 1;
        }

    }

    //public override int GetAutoTileIndex (MPos cpos, int layer) {
    //    int r = 0;
    //    int[] indexarray9 = this.AdjacentTileIndex(cpos, layer);
    //    bool[] boolarray = MAutoTile.BoolArray(indexarray9);
    //    int[] indexarray4 = MAutoTile.BoolArrayToIndexArray(boolarray);
    //    r = MAutoTile.IndexArrayToInt(indexarray4);

    //    return r;
    //}

    public override int GetTileLight (E512Pos cpos) {
        return this.tmp;
    }
}
