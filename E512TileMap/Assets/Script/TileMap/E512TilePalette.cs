using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class E512TilePalette : ScriptableObject {
    public Texture2D texture;
    public int tilesize = 16;
    public List<E512Tile> tiles = E512Tile.CommonTiles();
}