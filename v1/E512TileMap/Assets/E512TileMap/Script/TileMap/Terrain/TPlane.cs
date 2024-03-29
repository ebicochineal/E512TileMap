﻿using UnityEngine;
using System.Collections;

public class TPlane : E512TileTerrain{
    public int index = 18;
    public override int GetTileIndex(E512Pos cpos, int layer){
        return this.index;
    }
    public override int GetAutoTileIndex(E512Pos cpos, int layer) {
        return E512AutoTile.IndexArrayToInt(4, 4, 4, 4);
    }
}