using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class E512TileMapData : MonoBehaviour {
    [SerializeField]
    public E512TilePalette palette;
    [SerializeField]
    public int layer = 1;
    [SerializeField]
    private int x = 32;
    [SerializeField]
    private int y = 32;
    [SerializeField]
    public bool infinitymap = true;// 無限マップか
    [SerializeField]
    public bool initmap = true;// 範囲内を最初にすべて読み込むか
    [SerializeField]
    public bool ismap = true;// マップとして使用するか
    

    [SerializeField]
    public List<Camera> cameras = new List<Camera>();

    [NonSerialized]
    public E512TileManager tilemanager;
    
    public bool onceEveryTwoDraw = true;
    
    public E512TileTerrain terrain = null;

    private Dictionary<E512Pos, E512Block> dict_mapdata = new Dictionary<E512Pos, E512Block>();// マップブロックデータ辞書
    private Dictionary<E512Pos, GameObject[]> dict_draw = new Dictionary<E512Pos, GameObject[]>();// ドロー辞書
    private List<E512Pos> list_uvupdate = new List<E512Pos>();// uvupdateリスト
    private Mesh gridmesh;

    public List<GameObject[]> objectpool = new List<GameObject[]>();

    private int gameobjectlayer;

    public static Dictionary<string, E512TileMapData> scenemap = new Dictionary<string, E512TileMapData>();

    public static E512TileMapData SceneMap {
        set {
            if (!E512TileMapData.scenemap.ContainsKey(SceneManager.GetActiveScene().name)) {
                E512TileMapData.scenemap.Add(SceneManager.GetActiveScene().name, value);
            } else {
                E512TileMapData.scenemap[SceneManager.GetActiveScene().name] = value;
            }
        }
        get { return E512TileMapData.scenemap[SceneManager.GetActiveScene().name]; }
    }
    
    // save
    [SerializeField]
    public E512TileSave save = E512TileSave.None;
    
    [SerializeField]
    public string save_dir;

    private HashSet<E512Pos> data_block = new HashSet<E512Pos>();

    void Awake () {
        QualitySettings.antiAliasing = 0;// すべてのグラフィックでアンチエイリアシング オフ

        if (this.ismap) { E512TileMapData.SceneMap = this; }
        
        if (this.terrain == null) { this.terrain = this.gameObject.GetComponent<E512TileTerrain>(); }
        if (this.terrain == null) { this.terrain = this.gameObject.AddComponent<E512TileTerrain>(); }
        
        this.terrain.Init();
        this.data_block = this.terrain.data_block;
        if (this.save_dir == "") { this.save_dir = "temp"; }

        this.gridmesh = E512Mesh.Grid(E512Block.SIZE, E512Block.SIZE, 0, 0, true);

        this.tilemanager = new E512TileManager(this.palette);

        if (this.initmap) {
            for (int i = 0; i < this.x / E512Block.SIZE + (this.x % E512Block.SIZE > 0 ? 1 : 0); ++i) {
                for (int j = 0; j < this.y / E512Block.SIZE + (this.y % E512Block.SIZE > 0 ? 1 : 0); ++j) {
                    this.AddBlock(new E512Pos(i, j));
                }
            }
        }

        this.gameobjectlayer = this.gameObject.layer;
    }

    //void Update () {
    //    this.Draw();
    //    //if (this.list_uvupdate.Count < 1) { GC.Collect(); };
    //    this.UVUpdate();
    //}

    //int frame;
    //int gccnt;
    //void Update () {
    //    if (this.frame == 0) { this.Draw(); }
    //    if (this.frame == 1) { this.UVUpdate(); }
    //    if (this.frame == 2) {
    //        if (this.gccnt > 4) {
    //            GC.Collect();
    //            this.gccnt = 0;
    //        } else {
    //            this.gccnt += 1;
    //        }
    //    }
    //    this.frame = (this.frame + 1) % 3;
    //}
    int frame;
    void Update () {
        if (this.onceEveryTwoDraw) {
            if (this.frame % 2 == 0) { this.Draw(); }
            if (this.frame % 2 == 1) { this.UVUpdate(); }
        } else {
            this.Draw();
            this.UVUpdate();
        }
        
        if (this.frame == 0 && this.save == E512TileSave.AutoSave) {
            if (this.Save()) { print("AutoSave"); }
        }
        this.frame = (this.frame + 1) % 600;
    }
    
    public override string ToString () {
        return string.Format("GridSize:{0} Infinity:{2} Block:{3}", E512Block.SIZE, this.infinitymap, this.dict_mapdata.Count);
    }
    
    /// <summary>
    /// マップ範囲内または無限マップなら真
    /// inside or infinity map is true
    /// </summary>
    private bool InSide (E512Pos cpos) {
        return ((cpos.x < this.x && cpos.x >= 0 && cpos.y < this.y && cpos.y >= 0) || this.infinitymap);
    }

    /// <summary>
    /// ブロックデータ作成
    /// </summary>
    private E512Block CreateBlock (E512Pos bpos) {
        E512Block b = new E512Block(this.layer);
        E512Pos bcpos = bpos * E512Block.SIZE;// ブロックのセル座標
        for (int x = 0; x < E512Block.SIZE; ++x) {
            for (int y = 0; y < E512Block.SIZE; ++y) {
                E512Pos cpos = bcpos + new E512Pos(x, y);// ブロックのセル座標＋ブロック内座標
                if (this.InSide(cpos)) {// 内側
                    for (int z = 0; z < this.layer; ++z) {
                        b.SetTileIndex(this.terrain.LoadTileIndex(cpos, z), z, x, y, false, true);
                        b.SetAutoTileIndex(this.terrain.LoadAutoTileIndex(cpos, z), z, x, y, false, true);
                    }
                    b.SetTileLight(this.terrain.LoadTileLight(cpos), x, y, false, true);
                } else {// 外側 アウトサイドで初期化
                    for (int z = 0; z < this.layer; ++z) {
                        b.SetTileIndex(E512Tile.OutSide, z, x, y, false, false);
                        b.SetAutoTileIndex(0, z, x, y, false, false);
                    }
                }
            }
        }
        
        return b;
    }

    /// <summary>
    /// ブロックデータ追加
    /// </summary>
    private void AddBlock (E512Pos bpos) {
        this.dict_mapdata.Add(bpos, this.CreateBlock(bpos));
    }

    /// <summary>
    /// GameObject作成,座標,親子関係設定
    /// </summary>
    private GameObject[] CreateGameObject (E512Pos bpos) {
        GameObject[] objs = new GameObject[this.layer];
        if (this.objectpool.Count > 0) {// オブジェクトプールから
            int index = this.objectpool.Count - 1;// 末尾
            objs = this.objectpool[index];
            this.objectpool.RemoveAt(index);
            for (int i = 0; i < this.layer; ++i) {
                objs[i].name = string.Format("Map [{0}, {1}] Layer{2}", bpos.x, bpos.y, i + 1);
                objs[i].transform.localPosition = new Vector3((float)(bpos.x * E512Block.SIZE), (float)(bpos.y * E512Block.SIZE), -0.1f * i);
                objs[i].transform.localRotation = Quaternion.identity;
                //objs[i].SetActive(true);// オブジェクト生成とUVを分けたのでUVでtrueにするそうしないと前のUVのマップが表示される。しばらく使って問題が無ければ削除
            }
        } else {// 新規オブジェクト作成
            for (int i = 0; i < this.layer; ++i) {
                objs[i] = new GameObject(string.Format("Map [{0}, {1}] Layer{2}", bpos.x, bpos.y, i + 1));
                objs[i].AddComponent<MeshFilter>().mesh = this.gridmesh;
                objs[i].AddComponent<MeshRenderer>().material = this.tilemanager.material;
                objs[i].GetComponent<MeshRenderer>().material.SetInt("_Layer", i);
                objs[i].transform.parent = this.transform;
                objs[i].transform.localPosition = new Vector3((float)(bpos.x * E512Block.SIZE), (float)(bpos.y * E512Block.SIZE), -0.1f * i);
                objs[i].transform.localRotation = Quaternion.identity;
                objs[i].SetActive(false);
                objs[i].layer = this.gameobjectlayer;
            }
        }
        return objs;
    }
    

    /// <summary>
    /// ブロック内のxy座標から１次元のVertexIndexをオートタイル（タイル）用に４つ返す
    /// </summary>
    private int[] VertexIndex (int x, int y) {
        // ０左上 １右上 ２左下 ３右下
        int x2 = x * 2;
        int y2 = y * 2;
        int x2p = x2 + 1;
        int y2p = y2 + 1;
        int[] r = new int[4];
        r[0] = 4 * (x2 + (E512Block.SIZE * 2 * y2p));
        r[1] = 4 * (x2p + (E512Block.SIZE * 2 * y2p));
        r[2] = 4 * (x2 + (E512Block.SIZE * 2 * y2));
        r[3] = 4 * (x2p + (E512Block.SIZE * 2 * y2));
        return r;
    }

    // 各カメラのドローリスト追加
    private List<E512Pos> DrawList () {
        var r = new List<E512Pos>();
        
        foreach (var i in this.cameras) {
            var t = (float)i.pixelWidth / (float)i.pixelHeight;
            var nx = (int)(i.orthographicSize * t) + E512Block.SIZE / 2;
            var ny = (int)i.orthographicSize + E512Block.SIZE / 2;
            
            var cpos = i.transform.position.ToE512Pos();
            var start = new E512Pos(cpos.x - nx, cpos.y - ny);
            var end = new E512Pos(cpos.x + nx, cpos.y + ny);

            foreach (var j in E512Block.BoxBlockList(start, end)) { r.Add(j); }
        }
        return r;
    }


    /// <summary>
    /// Draw
    /// </summary>
    private void Draw () {
        var draws = this.DrawList();
        if (this.dict_draw.Count > 0) {
            this.DrawMeshCreate(draws, 1);
        } else {
            this.DrawMeshCreate(draws, 0);// ALL
        }
        this.DrawMeshDelete(draws);
    }

    /// <summary>
    /// 描画範囲内のオブジェクトを全てまたは少しづつ作成
    /// ドローリストのブロックデータ作成、オブジェクト作成
    /// </summary>
    private void DrawMeshCreate (List<E512Pos> list_bpos, int limit) {
        int count = 0;
        foreach (E512Pos i in list_bpos) {
            if (count > limit && limit > 0) { break; }
            if (!(this.dict_draw.ContainsKey(i))) {// ドローリストに無いなら
                if (this.dict_mapdata.ContainsKey(i)) {// マップデータにある 座標iにメッシュ作成, UV設定
                    this.dict_draw.Add(i, this.CreateGameObject(i));
                    this.list_uvupdate.Add(i);
                    count += 1;
                } else {// マップデータに無い
                    if (this.InSide(new E512Pos(i.x * E512Block.SIZE, i.y * E512Block.SIZE))) {
                        // マップデータ作成, 座標iにメッシュ作成, UV設定
                        this.AddBlock(i);
                        this.dict_draw.Add(i, this.CreateGameObject(i));
                        this.list_uvupdate.Add(i);
                        count += 1;
                    }
                }
            }
        }
    }
    
    private void DrawMeshDelete (List<E512Pos> list_bpos) {
        List<E512Pos> keys = new List<E512Pos>(this.dict_draw.Keys);
        foreach (E512Pos i in keys) {
            if (!(list_bpos.Contains(i))) {
                GameObject[] objs = new GameObject[this.layer];
                    
                for (int j = 0; j < this.layer; ++j) {
                    this.dict_draw[i][j].SetActive(false);
                    objs[j] = this.dict_draw[i][j];
                }
                this.objectpool.Add(objs);
                this.dict_draw.Remove(i);
            }
        }
    }

    /// <summary>
    /// UVUpdateリストのUV更新
    /// </summary>
    private void UVUpdate () {
        foreach (E512Pos i in this.list_uvupdate) {
            // ドローに含まれているなら
            if (this.dict_draw.ContainsKey(i)) { this.UVSet(i); }
        }
        this.list_uvupdate.Clear();
    }

    /// <summary>
    /// UV更新リスト追加 セル座標
    /// </summary>
    public void UVUpdateCellAdd (E512Pos cpos) {
        E512Pos bpos = E512Block.BPos(cpos);
        if (!this.list_uvupdate.Contains(bpos) && this.dict_draw.ContainsKey(bpos)) {
            this.list_uvupdate.Add(bpos);
        }
    }

    /// <summary>
    /// UV更新リスト追加 ブロック座標
    /// </summary>
    public void UVUpdateBlockAdd (E512Pos bpos) {
        if (!this.list_uvupdate.Contains(bpos) && this.dict_draw.ContainsKey(bpos)) {
            this.list_uvupdate.Add(bpos);
        }
    }

    /// <summary>
    /// UVセット ブロック座標
    /// </summary>
    private void UVSet (E512Pos bpos) {
        //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //sw.Start();

        E512Block b = this.dict_mapdata[bpos];
        for (int z = 0; z < this.layer; ++z) {
            MeshFilter m = this.dict_draw[bpos][z].GetComponent<MeshFilter>();
            Vector2[] uv = m.mesh.uv;
            for (int x = 0; x < E512Block.SIZE; ++x) {
                for (int y = 0; y < E512Block.SIZE; ++y) {
                    int index = b.GetTileIndex(z, x, y);
                    E512Tile t = this.tilemanager[index];
                    int[] ai = E512AutoTile.IntToIndexArray(b.GetAutoTileIndex(z, x, y));
                    int[] vi = this.VertexIndex(x, y);
                    this.tilemanager.SetUVHalf(uv, vi, ai, t, b.GetTileLight(x, y));
                }
            }
            m.mesh.uv = uv;
            this.dict_draw[bpos][z].SetActive(true);
        }

        //sw.Stop();
        //print(sw.Elapsed);
    }


    /// <summary>
    /// マップデータにタイルを設定
    /// </summary>
    public void SetTile (E512Pos cpos, int index, int layer) {
        if (!this.InSide(cpos)) { return; }
        E512Pos bpos = E512Block.BPos(cpos);
        E512Pos blpos = E512Block.BLocalPos(cpos);
        if (!this.dict_mapdata.ContainsKey(bpos)) { this.AddBlock(bpos); }

        this.UVUpdateBlockAdd(bpos);

        E512Block b = this.dict_mapdata[bpos];
        b.SetTileIndex(index, layer, blpos.x, blpos.y);
        b.SetAutoTileIndex(0, layer, blpos.x, blpos.y);// オートタイルクリア
        
    }

    /// <summary>
    /// オートタイル修正　withother true 違うタイルも含める false同じオートタイルのみ変化させたい場合
    /// </summary>
    public void FixAutoTile (E512Pos cpos, int layer, bool withother = true, bool outside_connect = true) {
        if (!this.InSide(cpos)) { return; }
        int target = this.GetTile(cpos, layer);
        foreach (E512Pos i in E512Pos.BoxList(cpos - 1, cpos + 1)) {// cpos周辺
            E512Pos bpos = E512Block.BPos(i);
            if (!this.dict_mapdata.ContainsKey(bpos)) { this.AddBlock(bpos); }
            int[] s = this.AdjacentTileIndex(i, layer);
            if (withother || s[4] == target) {
                int ati = E512AutoTile.IndexInt(s, outside_connect);
                E512Pos blpos = E512Block.BLocalPos(i);
                E512Block b = this.dict_mapdata[bpos];
                b.SetAutoTileIndex(ati, layer, blpos.x, blpos.y);
                this.UVUpdateBlockAdd(bpos);
            }
        }
    }

    private int[] AdjacentTileIndex (E512Pos cpos, int layer) {
        E512Pos leftup = cpos - 1;
        int[] r = new int[9];
        for (int x = 0; x < 3; ++x) {
            for (int y = 0; y < 3; ++y) {
                r[x + (2-y) * 3] = this.GetTile(leftup + new E512Pos(x, y), layer);
            }
        }
        return r;
    }

    public int GetTile (E512Pos cpos, int layer) {
        E512Pos bpos = E512Block.BPos(cpos);
        E512Pos blpos = E512Block.BLocalPos(cpos);
        int index = E512Tile.OutSide;
        if (this.dict_mapdata.ContainsKey(bpos)) {
            E512Block b = this.dict_mapdata[bpos];
            index = b.GetTileIndex(layer, blpos.x, blpos.y);
        } else {
            index = this.InSide(cpos) ? this.terrain.LoadTileIndex(cpos, layer) : E512Tile.OutSide;
        }
        return index;
    }

    public int GetAutoTile (E512Pos cpos, int layer) {
        E512Pos bpos = E512Block.BPos(cpos);
        E512Pos blpos = E512Block.BLocalPos(cpos);
        int index = 0;
        if (this.dict_mapdata.ContainsKey(bpos)) {
            E512Block b = this.dict_mapdata[bpos];
            index = b.GetAutoTileIndex(layer, blpos.x, blpos.y);
        } else {
            index = this.InSide(cpos) ? this.terrain.LoadAutoTileIndex(cpos, layer) : 0;
        }
        return index;
    }


    public int GetTileLight (E512Pos cpos) {
        E512Pos bpos = E512Block.BPos(cpos);
        E512Pos blpos = E512Block.BLocalPos(cpos);
        int light = 0;
        if (this.dict_mapdata.ContainsKey(bpos)) {
            E512Block b = this.dict_mapdata[bpos];
            light = b.GetTileLight(blpos.x, blpos.y);
        } else {
            light = this.InSide(cpos) ? this.terrain.LoadTileLight(cpos) : 0;
        }
        return light;
    }

    public void SetTileLight (E512Pos cpos, int light) {
        if (!this.InSide(cpos)) { return; }
        E512Pos bpos = E512Block.BPos(cpos);
        E512Pos blpos = E512Block.BLocalPos(cpos);
        if (!this.dict_mapdata.ContainsKey(bpos)) { this.AddBlock(bpos); }
        this.UVUpdateBlockAdd(bpos);
        E512Block b = this.dict_mapdata[bpos];
        b.SetTileLight(light, blpos.x, blpos.y);
    }
    
    public string GetSavePath () {
        string path = this.save == E512TileSave.ResourcesSave ? Application.dataPath + "/E512TileMap/Resources": Application.persistentDataPath;
        path += "/TileMapDataSave/" + (this.save_dir == "" ? "temp" : this.save_dir) + "/";
        return path;
    }
    
    private void BlockSave (E512Pos bpos) {
        var b = this.dict_mapdata[bpos];
        var bsize = b.BlockByteSize();
        var pos = bsize * E512Block.SectorIndex(bpos);//書き込み位置
        var filename = E512Block.SectorName(E512Block.BPosToSector(bpos));
        var filepath = this.GetSavePath() + filename + ".csv";

        if (!System.IO.Directory.Exists(this.GetSavePath())) {// ディレクトリチェック
            System.IO.Directory.CreateDirectory(this.GetSavePath());
        }
        byte[] sbd = b.ToBytes();// ブロックデータ
        if (!System.IO.File.Exists(filepath)) {// ファイルチェック
            byte[] bs = new byte[bsize * E512Block.SECTORSIZE * E512Block.SECTORSIZE];
            var fs = new System.IO.FileStream(filepath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            fs.Write(bs, 0, bs.Length);
            fs.Seek(pos, System.IO.SeekOrigin.Begin);
            fs.Write(sbd, 0, sbd.Length);
            fs.Close();
        } else {
            var fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Write);
            fs.Seek(pos, System.IO.SeekOrigin.Begin);
            fs.Write(sbd, 0, sbd.Length);
            fs.Close();
        }
    }

    void OnApplicationQuit () {
#if UNITY_EDITOR
        if (this.save == E512TileSave.None) { return; }

        if (this.Save()) {
            this.ResourcesHeaderSave();
            print("Save");
        }
        AssetDatabase.Refresh();
#endif
    }

    public bool Save () {
        bool r = false;
        foreach (var i in this.dict_mapdata) {
            var t = this.dict_mapdata[i.Key];
            if (t.updated_block && t.inside_block) {
                this.BlockSave(i.Key);
                if (this.save == E512TileSave.ResourcesSave) { this.data_block.Add(i.Key); }
                t.updated_block = false;
                r = true;
            }
        }
        return r;
    }

    private void ResourcesHeaderSave () {
        if (!System.IO.Directory.Exists(this.GetSavePath())) {
            System.IO.Directory.CreateDirectory(this.GetSavePath());
        }
        var filepath = this.GetSavePath() + "header.csv";
        var t = new List<string>();
        t.Add(this.layer.ToString());
        foreach (var i in this.data_block) {
            t.Add(E512Block.BPosName(i));
        }
        var header = string.Join(",", t.ToArray());
        System.IO.File.WriteAllText(filepath, header);
    }

//    public void TextAllSave () {
//#if UNITY_EDITOR
//        if (this.x > 256 || this.y > 256 || this.infinitymap) { return; }
//        System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources/TileMapDataSave/temp/");
//        StringBuilder sb = new StringBuilder();
//        sb.Append("" + this.x.ToString() + "," + this.y.ToString());
//        for (int y = 0; y < this.y; ++y) {
//            for (int x = 0; x < this.x; ++x) {
//                sb.Append(",");
//                sb.Append(this.GetTile(new MPos(x, y), 0).ToString());
//                sb.Append(",");
//                sb.Append(this.GetAutoTile(new MPos(x, y), 0).ToString());
//                sb.Append(",");
//                sb.Append(this.GetTileLight(new MPos(x, y)).ToString());
//            }
//        }

//        System.IO.File.WriteAllText(this.GetSavePath() + "temp.csv", sb.ToString());
//        print("save");
//#endif
//    }

}
