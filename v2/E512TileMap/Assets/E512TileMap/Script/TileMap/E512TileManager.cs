using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class E512TileManager {
    public Shader shader;
    public Texture2D texture;
    public Material material;
    public int tilesize;

    public float px;
    public float py;
    public float hpx;
    public float hpy;

    private Dictionary<int, Texture2D> images = new Dictionary<int, Texture2D>();

    public List<E512Tile> tiles = new List<E512Tile>();
    
    public E512TileManager (E512TilePalette palette) {
        this.TileFileLoad(palette);
        this.InitMaterial();
        this.InitUVSetValue();
        this.InitTilePos();
    }

    public E512TileManager (TGUIPalette palette) {
        this.TileFileLoad(palette);
        this.InitMaterial();
        this.InitUVSetValue();
        this.InitTilePos();
    }
    
    public E512Tile this[int index] {
        get { return this.tiles[index]; }
    }

    public IEnumerable Images () {
        foreach (var i in Enumerable.Range(0, this.tiles.Count)) {
            yield return this.GetTileTexture(i);
        }
    }

    public Texture2D[] ImagesArray () {
        Texture2D[] r = new Texture2D[this.tiles.Count];
        for (int i = 0; i < this.tiles.Count; ++i) {
            r[i] = this.GetTileTexture(i);
        }
        return r;
    }

    private void TileFileLoad (E512TilePalette palette) {
        if (palette.texture == null) {
            this.texture = new Texture2D(16, 16);
        } else {
            this.texture = palette.texture;
        }
        this.tilesize = palette.tilesize;
        this.shader = Shader.Find("Custom/AutoTileMap");
        foreach (var i in palette.tiles) {
            this.tiles.Add(i);
        }
    }

    private void TileFileLoad (TGUIPalette palette) {
        if (palette.texture == null) {
            this.texture = new Texture2D(16, 16);
        } else {
            this.texture = palette.texture;
        }
        this.tilesize = palette.tilesize;
        this.shader = Shader.Find("Custom/TGUI");
        foreach (var i in palette.tiles) {
            this.tiles.Add(i);
        }
    }

    private void InitMaterial () {
        this.material = new Material(this.shader);
        this.material.SetTexture("_MainTex", this.texture);
        this.material.SetInt("_TileSize", this.tilesize);
        this.material.SetFloat("_TexTileSize", 1f / this.texture.width * this.tilesize);
    }

    private void InitUVSetValue () {
        this.px = 1f / (this.texture.width / this.tilesize);
        this.py = 1f / (this.texture.height / this.tilesize);
        this.hpx = 1f / (this.texture.width / (this.tilesize / 2));
        this.hpy = 1f / (this.texture.height / (this.tilesize / 2));
    }

    private void InitTilePos () {
        foreach (var i in this.tiles) {
            i.InitPos(this.texture.width, this.texture.height, this.tilesize);
        }
    }

    public static TileType ToTileType (string s) {
        TileType t = TileType.NormalTile;
        if (s == "NormalTile") { t = TileType.NormalTile; }
        if (s == "AutoTile") { t = TileType.AutoTile; }
        return t;
    }

    public static TileCollisionType ToTileCollisionType (string s) {
        TileCollisionType c = TileCollisionType.Passable;
        if (s == "Passable") { c = TileCollisionType.Passable; }
        if (s == "NoPassable") { c = TileCollisionType.NoPassable; }
        return c;
    }

    // UV値計算済み
    public void SetUVHalf (Vector2[] uv, int[] indexs, int[] auto_tile_indexs, E512Tile t, int light) {
        for (int i = 0; i < 4; ++i) {
            uv[indexs[i]] = t.pos[auto_tile_indexs[i], i, 0];
            uv[indexs[i] + 1] = t.pos[auto_tile_indexs[i], i, 1];
            uv[indexs[i] + 2] = t.pos[auto_tile_indexs[i], i, 2];
            uv[indexs[i] + 3] = t.pos[auto_tile_indexs[i], i, 3];
            uv[indexs[i]].y += light;
            uv[indexs[i] + 1].y += light;
            uv[indexs[i] + 2].y += light;
            uv[indexs[i] + 3].y += light;
        }
    }

    public void SetUV (Vector2[] uv, int index, E512Tile t) {
        float xpx = t.x * this.px;
        float ypy = t.y * this.py;
        float right = this.px + xpx + t.anim - 0.00001f;
        float left = xpx + t.anim + 0.00001f;
        float up = 1f - ypy;
        float down = 1f - this.py - ypy;
        
        uv[index] = new Vector2(right, up);
        uv[index + 1] = new Vector2(left, down);
        uv[index + 2] = new Vector2(left, up);
        uv[index + 3] = new Vector2(right, down);
    }
    
    public void SetUV (Vector2[] uv, int index, E512Tile t, int light) {
        float xpx = t.x * this.px;
        float ypy = t.y * this.py;
        float right = this.px + xpx + t.anim - 0.00001f;
        float left = xpx + t.anim + 0.00001f;
        float up = 1f - ypy;
        float down = 1f - this.py - ypy;
        
        uv[index] = new Vector2(right, up);
        uv[index + 1] = new Vector2(left, down);
        uv[index + 2] = new Vector2(left, up);
        uv[index + 3] = new Vector2(right, down);
        
        uv[index].y += light;
        uv[index + 1].y += light;
        uv[index + 2].y += light;
        uv[index + 3].y += light;
    }
    
    
    
    // public void SetUV (Vector2[] uv, int index, E512Tile t, int p) {
    //     uv[index] = t.pos[0, 0, 0];
    //     uv[index + 1] = t.pos[0, 0, 0];
    //     uv[index + 2] = t.pos[0, 0, 0];
    //     uv[index + 3] = t.pos[0, 0, 0];
        
    // }

    public Texture2D GetTileTexture (int index) {
        if (this.images.ContainsKey(index)) { return this.images[index]; }
        var tex = this.SetTileTexture(index);
        this.images.Add(index, tex);
        return tex;
    }

    public Texture2D SetTileTexture (int index) {
        int ts = this.tilesize;
        Texture2D tex = new Texture2D(ts, ts);
        
        if (index == E512Tile.OutSide) {
            Color[] colors = new Color[this.tilesize * this.tilesize];
            for (int i = 0; i < colors.Length; ++i) {
                int x = i % this.tilesize;
                int y = i / this.tilesize;
                int rd = this.tilesize - 1;
                if (x == 0 || y == 0 || x == rd || y == rd || x == y) { colors[i] = Color.red; }
            }
            tex.SetPixels(colors);
            tex.Apply();
        }

        if (index == E512Tile.Black) {
            Color[] colors = new Color[this.tilesize * this.tilesize];
            for (int i = 0; i < colors.Length; ++i) {
                colors[i] = Color.black;
            }
            tex.SetPixels(colors);
            tex.Apply();
        }
        if (index > 2) {
            try {
                int x = this[index].x;
                int y = this[index].y;
                Color[] colors = this.texture.GetPixels(ts * x, this.texture.height - ts * y - ts, ts, ts);
                tex.SetPixels(colors);
                tex.Apply();
            } catch {
                Debug.Log("No Readable Texture");
            }
        }
        tex.filterMode = FilterMode.Point;
        return tex;
    }

    public void ResetTileTexture (int index) {
        if (!this.images.ContainsKey(index)) { return; }
        this.images[index] = this.SetTileTexture(index);
    }

    public void AllResetTileTexture () {
        for (int i = 0; i < this.tiles.Count; ++i) {
            this.ResetTileTexture(i);
        }
    }
}
