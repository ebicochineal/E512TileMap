using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TUnityPerlinNoiseTopDown : E512TileTerrain {
    public int index = 1;
    public override int GetTileIndex (E512Pos cpos, int layer) {
        if (layer > 0) { return E512Tile.Blank; }
        float x, y, n, a, b, c;
        x = cpos.x * 0.05f;
        y = (cpos.y + 256)* 0.05f;
        a = Mathf.PerlinNoise(x, y);
        x = (cpos.x + 64) * 0.03f;
        y = (cpos.y + 32) * 0.03f;
        b = Mathf.PerlinNoise(x, y);
        x = cpos.x * 0.1f;
        y = cpos.y * 0.1f;
        c = Mathf.PerlinNoise(x, y);
        n = (a + b + c) / 3;

        if (n < 0.5) {
            return 18;
        } else {
            return 4;
        }

        

    }

    public override int GetAutoTileIndex (E512Pos cpos, int layer) {
        int r = 0;
        int[] indexarray9 = this.AdjacentTileIndex(cpos, layer);
        bool[] boolarray = E512AutoTile.BoolArray(indexarray9);
        int[] indexarray4 = E512AutoTile.BoolArrayToIndexArray(boolarray);
        r = E512AutoTile.IndexArrayToInt(indexarray4);

        return r;
    }

    public override int GetTileLight (E512Pos cpos) {
        
        float x, y, n, a, b, c;
        x = cpos.x * 0.05f;
        y = (cpos.y + 256) * 0.05f;
        a = Mathf.PerlinNoise(x, y);
        x = (cpos.x + 64) * 0.03f;
        y = (cpos.y + 32) * 0.03f;
        b = Mathf.PerlinNoise(x, y);
        x = cpos.x * 0.1f;
        y = cpos.y * 0.1f;
        c = Mathf.PerlinNoise(x, y);
        n = (a + b + c) / 3;

        if (n < 0.5) {
            int l = -10 + Mathf.Min((int)(n * 10 * 2), 10) + 1;

            return Mathf.Abs(l > -1 ? 0 : l * 3);
        } else {
            int l = -(int)((n - 0.5) * 10 * 2);

            return Mathf.Abs(l > -1 ? 0 : l * 3);
        }
        
    }
}
