using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTileObject {
    private static Shader s_tile = Shader.Find("Custom/Tile");

    public static GameObject CreateSingleTileMap (E512TilePalette palette, int index) {
        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        var mat = new Material(SingleTileObject.s_tile);
        
        mat.SetTexture("_MainTex", palette.texture);
        mat.SetInt("_X", palette.tiles[index].x);
        mat.SetInt("_Y", palette.tiles[index].y);
        mat.SetInt("_TileSizeX", palette.tilesize);
        mat.SetInt("_TileSizeY", palette.tilesize);

        quad.GetComponent<Renderer>().material = mat;
        
        return quad;
    }
}
