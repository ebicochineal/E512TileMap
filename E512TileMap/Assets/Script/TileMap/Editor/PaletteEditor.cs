using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PaletteEditor : EditorWindow {
    [MenuItem("E512TileEditor/PaletteEditor")]
    public static void ShowWindow () {
        PaletteEditor window = EditorWindow.GetWindow<PaletteEditor>();
        window.position = new Rect(64, 64, 512, 512);
    }

    private bool play_status = false;

    private E512TileManager tm;

    Object tex_selector;
    Texture2D texture;
    
    private int select_num = 0;
    
    private Vector2 svp_tilelist = Vector2.zero;
    private Vector2 svp_texarea = Vector2.zero;
    private Vector2 svp_info = Vector2.zero;
    private Vector2 svp_buttons = Vector2.zero;

    int cx, cy;
    int mw = 256;
    MEditorInput input = new MEditorInput();

    E512TilePalette palette;

    void Awake () {

    }
    
    
    void OnGUI () {
        if (this.play_status != EditorApplication.isPlaying) { this.LoadPalette(); }
        this.play_status = EditorApplication.isPlaying;
        
        if (this.palette != null) {
            if (this.palette != null) {
                var t = this.palette.GetHashCode();
                this.palette = (E512TilePalette)EditorGUILayout.ObjectField("Palette", this.palette, typeof(E512TilePalette), false, GUILayout.MaxWidth(mw));
                if (this.palette != null && this.palette.GetHashCode() != t) { this.LoadPalette(); }
            }
            this.SelectTextureGUI();
            
            if (this.tm == null) { this.LoadPalette(); }// スクリプト変更時のエラー対策 完成したら必要ないかも
            this.TextureGUI();
            this.InfoGUI();
            this.TileListGUI();
            this.ButtonsGUI();

        } else {
            if (GUILayout.Button("New", GUILayout.MaxWidth(256))) { this.NewTileAsset(); }
            this.palette = (E512TilePalette)EditorGUILayout.ObjectField("Load", this.palette, typeof(E512TilePalette), false, GUILayout.MaxWidth(mw));
            this.LoadPalette();
        }
    }

    private void LoadPalette () {
        if (this.palette != null) {
            this.tm = new E512TileManager(this.palette);
            this.texture = this.tm.texture;
            this.Init();
        }
    }


    private void NewTileAsset () {
        ProjectWindowUtil.CreateAsset(E512TilePalette.CreateInstance<E512TilePalette>(), "Tile.asset");
    }

    private void TileAssetSave () {
        Debug.Log("SaveTileAsset");
        this.palette.texture = this.tm.texture;
        this.palette.tiles = this.tm.tiles;
        EditorUtility.SetDirty(this.palette);
        AssetDatabase.SaveAssets();
    }
    
    private bool IsTileAsset (Object obj) {
        if (obj is TextAsset) {
            try {
                return ((TextAsset)obj).text.Split('\n')[0] == "TileAsset";
            } catch {

            }
        }
        return false;
    }

    private void Init () {
        this.svp_tilelist = Vector2.zero;
        this.svp_texarea = Vector2.zero;
        this.svp_info = Vector2.zero;
        this.svp_buttons = Vector2.zero;
        this.cx = 0;
        this.cy = 0;
        this.select_num = 0;
    }

    private void SelectTextureGUI () {
        int t = 0;
        if (this.texture != null) { t = this.texture.GetHashCode(); }
        this.texture = (Texture2D)EditorGUILayout.ObjectField("Texture", this.texture, typeof(Texture2D), false, GUILayout.MaxWidth(mw));
        if (this.texture != null && this.texture.GetHashCode() != t) {
            this.tm.texture = this.texture;
            this.cx = 0;
            this.cy = 0;
        }

        if (this.texture == null) {
            this.texture = this.tm.texture;
            this.cx = 0;
            this.cy = 0;
        }
    }

    private void TextureGUI () {
        int tx = this.texture.width;
        int ty = this.texture.height;
        int bs = this.tm.tilesize;
        this.svp_texarea = EditorGUILayout.BeginScrollView(this.svp_texarea, GUILayout.Height(160));

        // securing texture draw area
        EditorGUILayout.LabelField("", GUILayout.Width(tx + 6), GUILayout.Height(ty + 6));

        GUI.Box(new Rect(3, 3, tx + 6, ty + 6), "");// bg
        GUI.DrawTexture(new Rect(6, 6, tx, ty), (Texture2D)this.texture, ScaleMode.StretchToFill, true, 10.0f);


        if (Event.current.type == EventType.MouseDown) {
            
            Vector2 mpos = Event.current.mousePosition;
            int ix = ((int)mpos.x - 6) / bs;
            int iy = ((int)mpos.y - 6) / bs;
            
            if (this.input.DoubleClick() && !EditorApplication.isPlaying) { this.AddItem(); }

            if (ix >= 0 && ix < tx / bs && iy >= 0 && iy < ty / bs) {
                this.cx = ix;
                this.cy = iy;
                Repaint();
            }
        }
        if (this.cx >= 0) {
            int px = 6 + bs * this.cx;
            int py = 6 + bs * this.cy;
            int pxs = px + bs;
            int pys = py + bs;

            Handles.BeginGUI();
            Handles.color = Color.red;
            Handles.DrawLine(new Vector3(px, py), new Vector3(px, pys));
            Handles.DrawLine(new Vector3(pxs, py), new Vector3(pxs, pys));
            Handles.DrawLine(new Vector3(px, py), new Vector3(pxs, py));
            Handles.DrawLine(new Vector3(px, pys), new Vector3(pxs, pys));
            Handles.EndGUI();
        }
        EditorGUILayout.EndScrollView();
    }

    
    private void TileListGUI () {
        this.svp_tilelist = EditorGUILayout.BeginScrollView(this.svp_tilelist, GUILayout.Height(160));
        var n = this.tm.ImagesArray().Length;   
        this.select_num = GUILayout.SelectionGrid(this.select_num, this.tm.ImagesArray(), 8, GUILayout.Height(mw / 8 * (n / 8 + (n % 8 > 0 ? 1 : 0))), GUILayout.Width(mw));
        E512TilePen.index = this.select_num;
        EditorGUILayout.EndScrollView();
    }

    
    private void InfoGUI () {
        this.svp_info = EditorGUILayout.BeginScrollView(this.svp_info, GUILayout.Height(88));
        GUILayout.Label(this.select_num.ToString());
        if (this.select_num > 2 && !EditorApplication.isPlaying) {
            this.tm[this.select_num].tiletype = (TileType)GUILayout.SelectionGrid((int)this.tm[this.select_num].tiletype, new string[] { "Normal", "Auto" }, 2, GUILayout.Width(mw));
            this.tm[this.select_num].collisiontype = (TileCollisionType)GUILayout.SelectionGrid((int)this.tm[this.select_num].collisiontype, new string[] { "Passable", "NoPassable" }, 2, GUILayout.Width(mw));
            this.tm[this.select_num].anim = EditorGUILayout.IntField("Anim", this.tm[this.select_num].anim, GUILayout.Width(mw));
        } else {// gameplay
            GUILayout.SelectionGrid((int)this.tm[this.select_num].tiletype, new string[] { "Normal", "Auto" }, 2, GUILayout.Width(256));
            GUILayout.SelectionGrid((int)this.tm[this.select_num].collisiontype, new string[] { "Passable", "NoPassable" }, 2, GUILayout.Width(mw));
            EditorGUILayout.IntField("Anim", this.tm[this.select_num].anim, GUILayout.Width(256));
        }
        EditorGUILayout.EndScrollView();
    }

    
    private void ButtonsGUI () {
        this.svp_buttons = EditorGUILayout.BeginScrollView(this.svp_buttons, GUILayout.Height(64));
        if (!EditorApplication.isPlaying) {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add", GUILayout.Width(64))) {
                this.AddItem();
            }
            if (GUILayout.Button("Delete", GUILayout.Width(64)) && this.select_num > 2) {

                this.tm.tiles.RemoveAt(this.select_num);
                if (this.select_num > this.tm.tiles.Count - 1) { this.select_num = this.tm.tiles.Count - 1; }
                this.tm.AllResetTileTexture();
            }

            if (GUILayout.Button("Sort", GUILayout.Width(64))) {
                var size = this.tm.texture.width / this.tm.tilesize;
                this.tm.tiles = this.tm.tiles.OrderBy(x => x.x + size * x.y).ToList();
                this.tm.AllResetTileTexture();
            }

            EditorGUILayout.EndHorizontal();
        
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save", GUILayout.Width(64))) {
                this.TileAssetSave();
            }
            if (GUILayout.Button("Load", GUILayout.Width(64))) {
                this.LoadPalette();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

    }

    private void AddItem () {
        this.tm.tiles.Add(new E512Tile(Mathf.Max(0, this.cx), Mathf.Max(0, this.cy), TileCollisionType.Passable, TileType.NormalTile, 0));
        this.tm.AllResetTileTexture();

        this.select_num = this.tm.tiles.Count - 1;
    }
}

class MEditorInput {
    double double_click_timer = 0;
    public bool DoubleClick () {
        bool r = false;
        if (Event.current.type == EventType.MouseDown) {
            if (EditorApplication.timeSinceStartup - this.double_click_timer < 0.3) { r = true; }
            this.double_click_timer = EditorApplication.timeSinceStartup;
        }
        return r;
    }
}