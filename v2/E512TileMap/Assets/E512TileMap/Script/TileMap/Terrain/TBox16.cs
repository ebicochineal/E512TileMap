using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TBox16 : E512TileTerrain {
    int GetInside (E512Pos cpos) {
        List<int> l = new List<int>(){
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
            
        };
        return l[(15-cpos.y)*16+cpos.x];
    }
    
    
    public override int GetTileIndex (E512Pos cpos, int layer) {
        if (0 <= cpos.x && cpos.x < 16 && 0 <= cpos.y && cpos.y < 16) {
            return this.GetInside(cpos) == 1 ? 3 : 23;
        } else {
            return E512Tile.OutSide;
        }
    }
    
    public override int GetTileDark(E512Pos cpos) {
        return 16;
    }
    
}
