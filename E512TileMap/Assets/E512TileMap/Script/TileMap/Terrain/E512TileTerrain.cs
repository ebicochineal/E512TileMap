using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// タイルマップの初期化、生成
// GetTileLight 0明、32暗
public class E512TileTerrain : MonoBehaviour {
    
    private Dictionary<E512Pos, E512Block> dict_mapdata = new Dictionary<E512Pos, E512Block>();// マップブロックデータ辞書
    [NonSerialized]
    public HashSet<E512Pos> data_block = new HashSet<E512Pos>();
    private string path = "";
    private int block_byte_size = 0;
    private int data_layer = 0;

    public Dictionary<E512Pos, byte[]> resources_bytes_temp = new Dictionary<E512Pos, byte[]>();

    public E512TileSave save = E512TileSave.None;
    public string load_dir = "";

    public string GetSavePath () {
        string path = this.save == E512TileSave.ResourcesSave ? Application.dataPath + "/Resources" : Application.persistentDataPath;
        path += "/TileMapDataSave/" + this.load_dir + "/";
        return path;
    }
    
    // MapData Awake, Call セーブされているBPos登録
    public void Init () {
        this.TerrainAwake();
        if (this.load_dir == "") { this.load_dir = "temp"; }
        this.path = this.GetSavePath();
        if (this.save == E512TileSave.ResourcesSave) { this.ResourcesInit(); }
        if (this.save == E512TileSave.AutoSave) { this.AutoSaveInit(); }
    }

    private void AutoSaveInit () {
        if (System.IO.Directory.Exists(this.path)) {
            foreach (var i in System.IO.Directory.GetFiles(this.path, "x*.csv", System.IO.SearchOption.AllDirectories)) {
                var filepath = i.Replace('\\', '/');
                var t = filepath.Split('/');
                var name = t[t.Length - 1].Split('.')[0];
                var fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                this.block_byte_size = (int)(fs.Length / (E512Block.SECTORSIZE * E512Block.SECTORSIZE));
                
                var spos = E512Block.SectorNameToSPos(name);
                var ldbpos = new E512Pos(spos.x * E512Block.SECTORSIZE, spos.y * E512Block.SECTORSIZE);// セクターのindex1のbpos
                var index = 0;
                byte[] b = new byte[1];
                for (int y = 0; y < E512Block.SECTORSIZE; ++y) {
                    for (int x = 0; x < E512Block.SECTORSIZE; ++x) {
                        var bpos = new E512Pos(ldbpos.x + x, ldbpos.y + y);
                        fs.Seek(this.block_byte_size * index, System.IO.SeekOrigin.Begin);
                        fs.Read(b, 0, b.Length);
                        if (b[0] > 0) {
                            this.data_layer = b[0];
                            data_block.Add(bpos);
                        }
                        index += 1;
                    }
                }
                fs.Close();
            }
        }
    }

    private void ResourcesInit () {
        TextAsset header = Resources.Load("TileMapDataSave/" + this.load_dir + "/header") as TextAsset;
        if (header == null) { return; }
        var t = header.text.Split(',');
        this.data_layer = int.Parse(t[0]);
        this.block_byte_size = 1 + 2 * E512Block.BLOCKSIZE * 2 * this.data_layer + E512Block.BLOCKSIZE;
        for (int i = 1; i < t.Length; ++i) {
            this.data_block.Add(E512Block.BPosNameToBPos(t[i]));
        }
    }

    public int[] AdjacentTileIndex (E512Pos cpos, int layer) {
        int[] r = new int[9];
        int x = cpos.x;
        int y = cpos.y;
        r[0] = this.GetTileIndex(new E512Pos(x-1, y+1), layer);
        r[1] = this.GetTileIndex(new E512Pos(x  , y+1), layer);
        r[2] = this.GetTileIndex(new E512Pos(x+1, y+1), layer);
        
        r[3] = this.GetTileIndex(new E512Pos(x-1, y  ), layer);
        r[4] = this.GetTileIndex(new E512Pos(x  , y  ), layer);
        r[5] = this.GetTileIndex(new E512Pos(x+1, y  ), layer);
        
        r[6] = this.GetTileIndex(new E512Pos(x-1, y-1), layer);
        r[7] = this.GetTileIndex(new E512Pos(x  , y-1), layer);
        r[8] = this.GetTileIndex(new E512Pos(x+1, y-1), layer);
        
        return r;
    }

    private void LoadBlock (E512Pos bpos) {
        E512Pos spos = E512Block.BPosToSector(bpos);
        byte[] data = new byte[block_byte_size];
        if (this.save == E512TileSave.ResourcesSave) {
            if (!this.resources_bytes_temp.ContainsKey(spos)) {
                TextAsset ta = Resources.Load("TileMapDataSave/" + this.load_dir + "/" + E512Block.SectorName(spos)) as TextAsset;
                this.resources_bytes_temp.Add(spos, ta.bytes);
            }
            var t = this.resources_bytes_temp[spos];
            var p = this.block_byte_size * E512Block.SectorIndex(bpos);
            for (int i = 0; i < this.block_byte_size; ++i) {
                data[i] = t[p + i];
            }
        } else {
            var filepath = this.path + "/" + E512Block.SectorName(spos) + ".csv";
            var fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            fs.Seek(this.block_byte_size * E512Block.SectorIndex(bpos), System.IO.SeekOrigin.Begin);
            fs.Read(data, 0, data.Length);
            fs.Close();
        }
        
        
        E512Block b = new E512Block(this.data_layer);
        
        int cnt = 1;
        // tileindex
        for (int z = 0; z < b.layer; ++z) {
            for (int y = 0; y < E512Block.SIZE; ++y) {
                for (int x = 0; x < E512Block.SIZE; ++x) {
                    b.SetTileIndex(BitConverter.ToInt16(data, cnt), z, x, y);
                    cnt += 2;
                }
            }
        }

        // autotileindex
        for (int z = 0; z < b.layer; ++z) {
            for (int y = 0; y < E512Block.SIZE; ++y) {
                for (int x = 0; x < E512Block.SIZE; ++x) {
                    b.SetAutoTileIndex(BitConverter.ToInt16(data, cnt), z, x, y);
                    cnt += 2;
                }
            }
        }

        // light
        for (int y = 0; y < E512Block.SIZE; ++y) {
            for (int x = 0; x < E512Block.SIZE; ++x) {
                b.SetTileLight(data[cnt], x, y);
                cnt += 1;
            }
        }
        
        this.dict_mapdata.Add(bpos, b);
    }
    public int LoadTileIndex (E512Pos cpos, int layer) {
        E512Pos bpos = E512Block.BPos(cpos);
        if (!this.data_block.Contains(bpos)) { return this.GetTileIndex(cpos, layer); }
        if (!this.dict_mapdata.ContainsKey(bpos)) { this.LoadBlock(bpos); }
        if (layer >= this.data_layer) { return this.GetTileIndex(cpos, layer); }
        E512Block b = this.dict_mapdata[bpos];
        E512Pos blpos = E512Block.BLocalPos(cpos);
        return b.GetTileIndex(layer, blpos.x, blpos.y);
    }
    public int LoadAutoTileIndex (E512Pos cpos, int layer) {
        E512Pos bpos = E512Block.BPos(cpos);
        if (!this.data_block.Contains(bpos)) { return this.GetAutoTileIndex(cpos, layer); }
        if (!this.dict_mapdata.ContainsKey(bpos)) { this.LoadBlock(bpos); }
        if (layer >= this.data_layer) { return this.GetAutoTileIndex(cpos, layer); }
        E512Block b = this.dict_mapdata[bpos];
        E512Pos blpos = E512Block.BLocalPos(cpos);
        return b.GetAutoTileIndex(layer, blpos.x, blpos.y);
    }
    public int LoadTileLight (E512Pos cpos) {
        E512Pos bpos = E512Block.BPos(cpos);
        if (!this.data_block.Contains(bpos)) { return this.GetTileLight(cpos); }
        if (!this.dict_mapdata.ContainsKey(bpos)) { this.LoadBlock(bpos); }
        E512Block b = this.dict_mapdata[bpos];
        E512Pos blpos = E512Block.BLocalPos(cpos);
        return b.GetTileLight(blpos.x, blpos.y);
    }
    
    public int CalcAutoTileIndex (E512Pos cpos, int layer) {
        int r = 0;
        int[] indexarray9 = this.AdjacentTileIndex(cpos, layer);
        bool[] boolarray = E512AutoTile.BoolArray(indexarray9);
        int[] indexarray4 = E512AutoTile.BoolArrayToIndexArray(boolarray);
        r = E512AutoTile.IndexArrayToInt(indexarray4);
        return r;
    }
    
    public virtual int GetTileIndex (E512Pos cpos, int layer) { return 1; }
    public virtual int GetAutoTileIndex (E512Pos cpos, int layer) { return this.CalcAutoTileIndex(cpos, layer); }
    public virtual int GetTileLight (E512Pos cpos) { return 0; }
    
    public virtual void TerrainAwake () {}
}