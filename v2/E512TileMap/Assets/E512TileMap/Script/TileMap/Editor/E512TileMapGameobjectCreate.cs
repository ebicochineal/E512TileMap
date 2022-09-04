using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class E512TileMapGameobjectCreate {
    
    [MenuItem("GameObject/E512Sprite")]
    public static void E512SpriteCreate () {
        E512Sprite.Create(0, 0, new E512Pos(0, 0), 1, 0);
    }
    
    [MenuItem("GameObject/E512TileMap")]
    public static void E512TileMapCreate () {
        GameObject obj = new GameObject("E512Tilemap");
        E512TileMapData md = obj.AddComponent<E512TileMapData>();
        md.palette = Resources.Load<E512TilePalette>("DefaultTilePalette");
        md.cameras.Add(Camera.main);
        obj.AddComponent<TPlane>();
    }
}
