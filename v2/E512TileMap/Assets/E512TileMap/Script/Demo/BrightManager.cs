﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrightManager : MonoBehaviour {
    public HashSet<E512Pos> brights = new HashSet<E512Pos>();
    private Dictionary<E512Pos, int> dbrights = new Dictionary<E512Pos, int>();
    public E512TileMapData map;
    public float v = 4f;
    public int limitlevel = 32;
    float timer = 0;

    void Start () {
    }

    void Update () {
        this.timer += Time.deltaTime;
        if (this.timer > this.v / E512Tile.BrightLevel) {
            this.timer = 0;
            HashSet<E512Pos> brights = new HashSet<E512Pos>();
            foreach (var i in this.brights) {
                int v = Mathf.Min(this.map.GetTileLight(i) + 1, this.limitlevel);
                this.map.SetTileLight(i, Mathf.Min(v, E512Tile.BrightLevel));
                if (v < E512Tile.BrightLevel && v < this.limitlevel) { brights.Add(i); }
            }
            this.brights = brights;
        } else {
            foreach (var i in this.dbrights) {
                this._DynamicBrightness(i.Key, i.Value);
            }
            this.dbrights.Clear();
        }
    }
    
    public void DynamicBrightness (E512Pos cpos, int dist) {
        if (!this.dbrights.ContainsKey(cpos)) {
            this.dbrights.Add(cpos, dist);
        }
    }

    public void _DynamicBrightness (E512Pos cpos, int dist) {
        this.map.SetTileLight(cpos, 0);
        this.brights.Add(cpos);
        this.brights.Add(cpos);
        int mu = cpos.y + dist;
        int mr = cpos.x + dist;
        int ml = cpos.x - dist;
        int md = cpos.y - dist;
        for (int i = md; i <= mu; ++i) {
            for (int j = ml; j <= mr; ++j) {
                var p = new E512Pos(j, i);
                var v = Vector2.Distance(new Vector2(cpos.x, cpos.y), new Vector2(p.x, p.y));
                var e = E512Tile.BrightLevel - (int)(((dist - v) / dist) * E512Tile.BrightLevel);
                if (e < this.map.GetTileLight(p)) {
                    this.map.SetTileLight(p, e);
                    this.brights.Add(p);
                }
            }
        }
    }

}
