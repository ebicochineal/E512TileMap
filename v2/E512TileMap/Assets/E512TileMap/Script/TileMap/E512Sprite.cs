using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E512Sprite : MonoBehaviour {
    [HideInInspector]
    public MeshFilter meshFilter;
    [HideInInspector]
    public MeshRenderer meshRenderer;
    [HideInInspector]
    public int tileindex = -1;
    
    [Range(0, 32)]
    public int tx = 0;
    
    [Range(0, 32)]
    public int ty = 0;
    
    [HideInInspector]
    public float px = 0;
    [HideInInspector]
    public float py = 0;
    
    [HideInInspector]
    public Mesh mesh;
    
    
    public Material material;
    
    [Range(0, 32)]
    public int autoanim = 0;
    
    
    [Range(0, 32)]
    public int darkness = 0;
    
    [HideInInspector]
    public bool editor_create = false;
    
    // void Start () { this.SetUVPosM(this.tx, this.ty); }
    // void OnValidate () {
    //     if (this.f) {
    //         this.InitMaterial();
    //         this.SetUV(this.tx, this.ty);
    //     }
    // }
    
    
    void OnDrawGizmos () {
        
        if (this.meshFilter != null) { return; }
        if (this.material == null) { return; }
        float tilesize = this.material.GetInt("_TileSize");
        float h = this.material.mainTexture.height;
        float w = this.material.mainTexture.width;
        if (h*w < 1) { return; }
        
        float th = 1f / (h / tilesize);
        float tw = 1f / (w / tilesize);
        float sx = tw * this.tx;
        float ey = 1f - th * this.ty;
        float ex = tw * (this.tx+1f);
        float sy = 1f - th * (this.ty+1f);
        
        GL.PushMatrix ();
        GL.MultMatrix (this.transform.localToWorldMatrix);
        
        
        this.material.SetPass(0);
        
        GL.Begin (GL.TRIANGLES);
        
        GL.TexCoord(new Vector3(sx, ey, 0));
        GL.Vertex (new Vector3 (-0.5f, 0.5f, 0));
        GL.TexCoord(new Vector3(ex, ey, 0));
        GL.Vertex (new Vector3 (0.5f, 0.5f, 0));
        GL.TexCoord(new Vector3(ex, sy, 0));
        GL.Vertex (new Vector3 (0.5f, -0.5f, 0));
    
        GL.TexCoord(new Vector3(sx, ey, 0));
        GL.Vertex (new Vector3 (-0.5f, 0.5f, 0));
        GL.TexCoord(new Vector3(ex, sy, 0));
        GL.Vertex (new Vector3 (0.5f, -0.5f, 0));
        GL.TexCoord(new Vector3(sx, sy, 0));
        GL.Vertex (new Vector3 (-0.5f, -0.5f, 0));
        
        
        GL.End ();
        GL.PopMatrix ();
    }
    
    void Awake () {
        if (!this.editor_create) { return; }
        this.meshFilter = this.gameObject.AddComponent<MeshFilter>();
        this.meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        
        this.InitMesh();
        this.meshRenderer.material = this.material;
        
        this.InitMaterial();
        this.SetUV(this.tx, this.ty);
    }
    
    
    private void UVUpdate () {
        Vector2[] uv = this.mesh.uv;
        float xpx = this.tx * this.px;
        float ypy = this.ty * this.py;
        float right = this.px + xpx + this.autoanim - 0.00001f;
        float left = xpx + this.autoanim + 0.00001f;
        float up = 1f - ypy - 0.00001f;
        float down = 1f - this.py - ypy + 0.00001f;
        uv[0] = new Vector2(right, up);
        uv[1] = new Vector2(left, down);
        uv[2] = new Vector2(left, up);
        uv[3] = new Vector2(right, down);
        
        uv[0].y += this.darkness;
        uv[1].y += this.darkness;
        uv[2].y += this.darkness;
        uv[3].y += this.darkness;
        this.mesh.uv = uv;
    }
    
    public void SetTile (E512TileMapData map, int tileindex) {
        this.tileindex = tileindex;
        this.autoanim = map.tilemanager[tileindex].anim;
        this.tx = map.tilemanager[tileindex].x;
        this.ty = map.tilemanager[tileindex].y;
        this.UVUpdate();
    }
    
    public void SetUV (int tx, int ty) {
        this.tileindex = -1;
        this.tx = tx;
        this.ty = ty;
        if (this.mesh == null) { this.InitMesh(); }
        this.UVUpdate();
    }
    
    public void InitMaterial () {
        Material material = this.meshRenderer.sharedMaterial;
        this.px = 1f / (material.GetTexture("_MainTex").width / material.GetInt("_TileSize"));
        this.py = 1f / (material.GetTexture("_MainTex").height / material.GetInt("_TileSize"));
        material.SetFloat("_TexTileSize", 1f / material.GetTexture("_MainTex").width * material.GetInt("_TileSize"));
    }
    
    public void SetLayerPosition (int layer) {
        this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, -0.1f * layer);
    }
    
    public void Destroy () {
        Object.Destroy(this.mesh);
        Object.Destroy(this.material);
        Object.Destroy(this.meshRenderer.material);
        Object.Destroy(this.meshFilter.mesh);
        Object.Destroy(this.gameObject);
    }
    
    void InitMesh () {
        this.mesh = new Mesh();
        this.mesh.name = "E512SpriteMesh";
        this.mesh.vertices = E512Mesh.GridVertice(1, 1, -0.5f, -0.5f, false);
        this.mesh.triangles = E512Mesh.GridTriangle(1, 1, 1, false);
        this.mesh.uv = E512Mesh.GridUV(1, 1, 1, false);
        this.meshFilter.mesh = this.mesh;
    }
    
    
    public static E512Sprite Create (Material material, int tx, int ty, Vector2 pos, int layer, int darkness = 0, int autoanim = 0) {
        GameObject obj = new GameObject();
        obj.name = "E512Sprite";
        E512Sprite sprite = obj.AddComponent<E512Sprite>();
        sprite.meshFilter = obj.AddComponent<MeshFilter>();
        sprite.InitMesh();
        sprite.meshRenderer = obj.AddComponent<MeshRenderer>();
        sprite.meshRenderer.material = material;
        sprite.darkness = darkness;
        sprite.autoanim = autoanim;
        sprite.InitMaterial();
        sprite.SetUV(tx, ty);
        obj.transform.position = new Vector3(pos.x, pos.y, -0.1f * layer);
        return sprite;
    }
    public static E512Sprite Create (Material material, int tx, int ty, E512Pos pos, int layer, int darkness = 0, int autoanim = 0) {
        return E512Sprite.Create(material, tx, ty, new Vector2(pos.x+0.5f, pos.y+0.5f), layer, darkness, autoanim);
    }
    public static E512Sprite Create (int tx, int ty, E512Pos pos, int layer, int darkness = 0, int autoanim = 0) {
        return E512Sprite.Create(Resources.Load<Material>("Material/DefaultE512SpriteMaterial"), tx, ty, new Vector2(pos.x+0.5f, pos.y+0.5f), layer, darkness, autoanim);
    }
    
    public static void EditorCreate (int tx, int ty, E512Pos pos, int layer, int darkness = 0, int autoanim = 0) {
        GameObject obj = new GameObject();
        obj.name = "E512Sprite";
        E512Sprite sprite = obj.AddComponent<E512Sprite>();
        sprite.tx = tx;
        sprite.ty = ty;
        
        sprite.darkness = darkness;
        sprite.autoanim = autoanim;
        sprite.material = Resources.Load<Material>("Material/DefaultE512SpriteMaterial");
        
        obj.transform.position = new Vector3(pos.x+0.5f, pos.y+0.5f, -0.1f * layer);
        
        sprite.editor_create = true;
    }
    
    
    
    public static E512Sprite CreateTile (E512TileMapData map, int tileindex, Vector2 pos, int layer, int darkness = 0) {
        GameObject obj = new GameObject();
        obj.name = "E512Sprite";
        E512Sprite sprite = obj.AddComponent<E512Sprite>();
        sprite.tileindex = tileindex;
        sprite.meshFilter = obj.AddComponent<MeshFilter>();
        sprite.InitMesh();
        sprite.meshRenderer = obj.AddComponent<MeshRenderer>();
        sprite.meshRenderer.material = map.tilemanager.material;
        sprite.InitMaterial();
        sprite.SetTile(map, tileindex);
        sprite.darkness = darkness;
        obj.transform.position = new Vector3(pos.x, pos.y, -0.1f * layer);
        return sprite;
    }
    
    public static E512Sprite CreateTile (E512TileMapData map, int tileindex, E512Pos pos, int layer, int darkness = 0) {
        return E512Sprite.CreateTile(map, tileindex, new Vector2(pos.x+0.5f, pos.y+0.5f), layer, darkness);
    }
    
    public static Material CreateMaterial (Texture texture, int tilesize = 16) {
        Shader tileshader = Shader.Find("Custom/AutoTileMap");
        Material mat = new Material(tileshader);
        mat.SetTexture("_MainTex", texture);
        mat.SetInt("_TileSize", tilesize);
        mat.SetFloat("_TexTileSize", 1f / texture.width * tilesize);
        return mat;
    }
}
