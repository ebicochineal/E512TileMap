using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TGUIPalette {
    public Texture2D texture;
    public int tilesize = 16;
    public List<E512Tile> tiles = E512Tile.CommonTiles();
    
    public TGUIPalette (Texture2D texture, int tilesize) {
        this.tilesize = tilesize;
        this.texture = texture;
        for (int y = 0; y < this.texture.height / this.tilesize; ++y) {
            for (int x = 0; x < this.texture.width / this.tilesize; ++x) {
                this.tiles.Add(new E512Tile(x, y, TileCollisionType.NoPassable, TileType.NormalTile, 0));
            }
        }
    }

    public TGUIPalette (Texture2D texture, int tilesize, int animsize) {
        this.tilesize = tilesize;
        this.texture = texture;
        for (int y = 0; y < this.texture.height / this.tilesize; ++y) {
            for (int x = 0; x < this.texture.width / this.tilesize / (animsize + 1); ++x) {
                this.tiles.Add(new E512Tile(x, y, TileCollisionType.NoPassable, TileType.NormalTile, animsize));
            }
        }
    }
}