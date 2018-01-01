using UnityEngine;
using System.Collections;

public class TGround : E512TileTerrain {
    public int index = 1;
    public override int GetTileIndex (E512Pos cpos, int layer) {
        if (layer == 0) {
            if (cpos.y < 0) { return this.index; }
            if (cpos.y < 48 + (Mathf.Sin(cpos.x * 0.1f) * 4 + 4) ) { return 9 + Random.Range(0, 4); }
            return E512Tile.Blank;
        } else {
            return E512Tile.OutSide;
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
}
