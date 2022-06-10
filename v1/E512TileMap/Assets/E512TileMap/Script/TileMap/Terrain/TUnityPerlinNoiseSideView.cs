using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TUnityPerlinNoiseSideView : E512TileTerrain {
    // public int index = 1;
    public override int GetTileIndex (E512Pos cpos, int layer) {
        if (layer > 0) { return E512Tile.Blank; }
        if (this.Perlin(cpos)) {
            
            if (this.Perlin2(cpos)) { return 23; }
            
            return 27;
        }
        return E512Tile.Blank;
    }

    private bool Perlin (E512Pos cpos) {
        float x, n, a, b, c, cx;
        cx = cpos.x < 0 ? cpos.x - 100 : cpos.x;
        x = cx * 0.025f;
        c = Mathf.PerlinNoise(x, 0);
        x = cx * 0.05f;
        a = Mathf.PerlinNoise(x, 0);
        x = cx * 0.2f;
        b = Mathf.PerlinNoise(x, 0);
        n = a * 32 + b * 8 + c * 64;

        return cpos.y < n;

    }


    private bool Perlin2 (E512Pos cpos) {
        float x, y, a, b, c, cx, cy;
        cx = cpos.x;
        cy = cpos.y + -128;
        x = cx * 0.1f;
        y = cy * 0.1f;
        a = Mathf.PerlinNoise(x, y);
        cx = cpos.x + 32;
        cy = cpos.y + -128;
        x = cx * 0.05f;
        y = cy * 0.05f;
        b = Mathf.PerlinNoise(x, y);
        cx = cpos.x + 64;
        cy = cpos.y + -128;
        x = cx * 0.2f;
        y = cy * 0.2f;
        c = Mathf.PerlinNoise(x, y);
        return (a+b+c)/3 < 0.60 && (a + b + c) / 3 > 0.45;

    }

    public override int GetAutoTileIndex (E512Pos cpos, int layer) {
        if (layer > 0) { return 0; }

        int r = 0;
        int[] indexarray9 = this.AdjacentTileIndex(cpos, layer);
        bool[] boolarray = E512AutoTile.BoolArray(indexarray9);
        int[] indexarray4 = E512AutoTile.BoolArrayToIndexArray(boolarray);
        r = E512AutoTile.IndexArrayToInt(indexarray4);

        return r;
    }


    public override int GetTileLight (E512Pos cpos) {
        return E512Tile.BrightLevel;
    }

}
