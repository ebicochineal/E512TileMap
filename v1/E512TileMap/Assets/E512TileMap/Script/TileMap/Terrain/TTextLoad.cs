using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TTextLoad : E512TileTerrain {
    public TextAsset textasset;
    private List<int> tiles = new List<int>();
    private int w;
    private int h;
    public override void TerrainAwake () {
        string[] v = this.textasset.ToString().Split('\n');
        this.w = int.Parse(v[0].Split(',')[0]);
        this.h = int.Parse(v[0].Split(',')[1]);
        for (int y = 0; y < this.h; ++y) {
            string[] t =  v[y+1].Split(',');
            for (int x = 0; x < this.w; ++x) {
                this.tiles.Add(int.Parse(t[x]));
            }
        }
    }
    
    public override int GetTileIndex (E512Pos cpos, int layer) {
        if (layer > 0 || cpos.x < 0 || cpos.x >= this.w || cpos.y < 0 || cpos.y >= this.h) { return 1; }
        return this.tiles[(this.h-1-cpos.y) * this.w + cpos.x];
    }
}