using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TTextLoad : MTileTerrain {
    public TextAsset textmap;
    private bool isload = false;
    private List<int> tiles = new List<int>();
    private List<int> autos = new List<int>();
    private List<int> lights = new List<int>();
    private int w;
    private int h;
    private void Load () {
        this.isload = true;
        string[] v = this.textmap.ToString().Split(',');
        this.w = int.Parse(v[0]);
        this.h = int.Parse(v[1]);

        for (int i = 0; i < this.w * this.h; ++i) {
            this.tiles.Add(int.Parse(v[2 + i * 3]));
            this.autos.Add(int.Parse(v[3 + i * 3]));
            this.lights.Add(int.Parse(v[4 + i * 3]));
        }

    }


    public override int GetTileIndex (E512Pos cpos, int layer) {
        if (!isload) { Load(); }
        if (layer > 0) { return 1; }
        return this.tiles[cpos.y * this.w + cpos.x];
    }
    public override int GetAutoTileIndex (E512Pos cpos, int layer) {
        if (!isload) { Load(); }
        if (layer > 0) { return 0; }
        return this.autos[cpos.y * this.w + cpos.x];
    }

    public override int GetTileLight (E512Pos cpos) {
        if (!isload) { Load(); }
        return this.lights[cpos.y * this.w + cpos.x];
    }

}