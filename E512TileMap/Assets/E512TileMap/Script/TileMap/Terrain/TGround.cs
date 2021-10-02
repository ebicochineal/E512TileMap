using UnityEngine;
using System.Collections;

public class TGround : E512TileTerrain {
    public int ground = 8;
    public override int GetTileIndex (E512Pos cpos, int layer) {
        if (layer == 0) {
            return cpos.y < 1 ? this.ground : E512Tile.Blank;
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
