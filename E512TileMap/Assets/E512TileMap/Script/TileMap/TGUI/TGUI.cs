using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TGUIWindowEvent (TGUIWindow self);

public class TGUI {
    public int w;
    public int h;
    public int tw;
    public int th;
    public bool move;
    public E512Pos pos;
    public float scale;

    public int id = -1;
    public int z = 0;
    public GameObject root = new GameObject("TGUI");

    public bool InWindow (E512Pos pos, int h) {
        var p = pos;
        p.y = h - p.y - 1;
        return p.x >= this.pos.x && p.x < this.pos.x + this.w && p.y >= this.pos.y && p.y < this.pos.y + this.h;
    }
    
    public int ConvertChar (int c) {
        //byte bytesUTF8 = System.Text.Encoding.Default.GetBytes(((char)c).ToString())[0];
        //return bytesUTF8;

        if (c <= 255) { return c; }
        if (c >= (int)'｡') { c = 176 + (c - (int)'｡') - 15; }
        
        return c % 256;
    }

    public virtual void OnClick () { }

    public virtual void Destroy () {
        GameObject.Destroy(this.root);
    }
}

public class TGUIWindowTexture {
    static public TGUIWindowTexture DEFAULT = new TGUIWindowTexture();
    static public TGUIWindowTexture DEFAULT8 = new TGUIWindowTexture("window8", "font8");
    static public TGUIWindowTexture DEFAULT16 = new TGUIWindowTexture("window16", "font16");
    private string w_texture_str = "window16";
    private string t_texture_str = "font16";
    public Texture2D w_texture;
    public Texture2D t_texture;
    public int wsize;
    public int tsize;
    public float scale;
    public TGUIPalette w_palette;
    public TGUIPalette t_palette;

    public TGUIWindowTexture () {
        this.Init();
    }

    public TGUIWindowTexture (string w_texture_str, string t_texture_str) {
        this.w_texture_str = w_texture_str;
        this.t_texture_str = t_texture_str;
        this.Init();
    }

    public TGUIWindowTexture (string w_texture_str, string t_texture_str, int animsize) {
        this.w_texture_str = w_texture_str;
        this.t_texture_str = t_texture_str;
        this.Init(animsize);
    }

    public void Init () {
        this.w_texture = (Texture2D)Resources.Load(this.w_texture_str);
        this.t_texture = (Texture2D)Resources.Load(this.t_texture_str);
        this.wsize = this.w_texture.height / 3;
        this.tsize = this.t_texture.width / 16;
        this.scale = this.wsize / 16f;
        this.w_palette = new TGUIPalette(this.w_texture, this.wsize);
        this.t_palette = new TGUIPalette(this.t_texture, this.tsize);
    }

    public void Init (int animsize) {
        this.w_texture = (Texture2D)Resources.Load(this.w_texture_str);
        this.t_texture = (Texture2D)Resources.Load(this.t_texture_str);
        this.wsize = this.w_texture.height / 3;
        this.tsize = this.t_texture.width / 16;
        this.scale = this.wsize / 16f;
        this.w_palette = new TGUIPalette(this.w_texture, this.wsize, animsize);
        this.t_palette = new TGUIPalette(this.t_texture, this.tsize);
    }
}

public class TGUIWindow : TGUI{
    public TGUIWindowEvent onclick;
    private string text;
    private int textpage = 0;
    private int textline = 0;
    
    private GameObject wobj;
    private GameObject tobj;
    private TGUIData wdata;
    private TGUIData tdata;
    
    private IEnumerator slow_message_coroutine;

    private TGUIWindowTexture tgp;

    public TGUIWindow (int w, int h, E512Pos pos, string text, bool move, TGUIWindowTexture tgp) {
        this.tgp = tgp;
        this.scale = this.tgp.scale;
        this.w = (int)(w * this.scale);
        this.h = (int)(h * this.scale);
        this.tw = w;
        this.th = h;
        this.pos = pos;
        this.text = text;
        this.move = move;
        this.Init();
    }

    public TGUIWindow (int w, int h, E512Pos pos, string text, bool move) {
        this.tgp = TGUIWindowTexture.DEFAULT;
        this.scale = this.tgp.scale;
        this.w = (int)(w * this.scale);
        this.h = (int)(h * this.scale);
        this.tw = w;
        this.th = h;
        this.pos = pos;
        this.text = text;
        this.move = move;
        this.Init();
    }

    public TGUIWindow (E512Pos pos, string text, bool move, TGUIWindowTexture tgp) {
        this.tgp = tgp;
        this.scale = this.tgp.scale;
        this.w = (int)((text.Length + 1) * this.scale);
        this.h = (int)(2 * this.scale);
        this.tw = text.Length + 1;
        this.th = 2;
        this.pos = pos;
        this.text = text;
        this.move = move;
        this.Init();
    }

    public TGUIWindow (E512Pos pos, string text, bool move) {
        this.tgp = TGUIWindowTexture.DEFAULT;
        this.scale = this.tgp.scale;
        this.w = (int)((text.Length + 1) * this.scale);
        this.h = (int)(2 * this.scale);
        this.tw = text.Length + 1;
        this.th = 2;
        this.pos = pos;
        this.text = text;
        this.move = move;
        this.Init();
    }

    public TGUIWindow (E512Pos pos, string text, bool move, bool half) {
        this.tgp = half ? TGUIWindowTexture.DEFAULT8 : TGUIWindowTexture.DEFAULT16;
        this.scale = this.tgp.scale;
        this.w = (int)((text.Length + 1) * this.scale);
        this.h = (int)(2 * this.scale);
        this.tw = text.Length + 1;
        this.th = 2;
        this.pos = pos;
        this.text = text;
        this.move = move;
        this.Init();
    }

    private void Init () {
        this.InitWindow();
        this.InitText();
        this.SetDrawWindow();
        this.SetDrawText();
    }

    public void InitWindow () {
        this.wobj = new GameObject("Window");
        this.wdata = wobj.AddComponent<TGUIData>();
        this.wdata.Init(this.tgp.w_palette, this.tw, this.th);
        this.wobj.transform.localScale = Vector3.one * this.tgp.scale;
        this.wobj.transform.parent = this.root.transform;
    }

    public void InitText () {
        this.tobj = new GameObject("Text");
        this.tdata = tobj.AddComponent<TGUIData>();
        this.tdata.Init(this.tgp.t_palette, this.tw, this.th);
        this.tobj.transform.localScale = Vector3.one * this.tgp.scale;
        this.tobj.transform.position = new Vector3(0.5f * this.tgp.scale, -0.5f * this.tgp.scale, -0.005f);
        this.tobj.transform.parent = this.root.transform;
    }

    public override void OnClick () {
        if (this.onclick != null) { this.onclick(this); }
    }

    public void NextTextPage () {
        var n = ((this.tw - 1) * (this.th - 1) * (this.textpage + 1));
        if (n < this.text.Length) { this.textpage += 1; }
        this.SetDrawText();
    }

    public void PrevTextPage () {
        if (this.textpage > 0) { this.textpage -= 1; }
        this.SetDrawText();
    }

    public void NextTextLine () {
        var n = ((this.tw - 1) * (this.textline + 1));
        if (n < this.text.Length) { this.textline += 1; }
        this.SetDrawText();
    }

    public void PrevTextLine () {
        if (this.textline > 0) { this.textline -= 1; }
        this.SetDrawText();
    }
    
    public int GetChar (int x, int y) {
        var w = this.tw - 1;
        var n = w * y + x + (w * (this.th - 1) * this.textpage) + (w * this.textline);
        if (n >= this.text.Length) { return 2; }
        return this.ConvertChar((int)this.text[n]);
    }
    
    public void SetDrawText () {
        for (int x = 0; x < this.tw - 1; ++x) {
            for (int y = 0; y < this.th - 1; ++y) {
                this.tdata.SetTile(new E512Pos(x, this.th - y - 1), (int)this.GetChar(x, y) + 3);
            }
        }
        this.tdata.ReUV();
    }
    
    private void SetDrawWindow () {
        for (int x = 0; x < this.tw; ++x) {
            for (int y = 0; y < this.th; ++y) {
                var n = 7;
                if (y == 0) { n = 10; }
                if (y == this.th - 1) { n = 4; }
                if (x == 0) {
                    n = 6;
                    if (y == 0) { n = 9; }
                    if (y == this.th - 1) { n = 3; }
                }
                if (x == this.tw - 1) {
                    n = 8;
                    if (y == 0) { n = 11; }
                    if (y == this.th - 1) { n = 5; }
                }
                this.wdata.SetTile(new E512Pos(x, y), n);
            }
        }
        this.wdata.ReUV();
    }
    
    public string Text {
        get { return this.text; }
        set {
            this.text = value;
            this.SetDrawText();
        }
    }
    
    public void SlowText (string text, float time) {
        if (this.slow_message_coroutine != null) {
            this.tdata.StopCoroutine(this.slow_message_coroutine);
        }
        this.slow_message_coroutine = this.SlowTextCoroutine(text, time);
        this.tdata.StartCoroutine(this.slow_message_coroutine);
    }

    private IEnumerator SlowTextCoroutine (string text, float time) {
        this.Text = "";
        foreach (var i in text) {
            this.Text += i;
            if (this.Text.Length % (this.tw - 1) == 0) {
                yield return new WaitForSeconds(time * 32);
            } else {
                yield return new WaitForSeconds(time);
            }
        }
    }

    public override void Destroy () {
        if (this.slow_message_coroutine != null) {
            this.tdata.StopCoroutine(this.slow_message_coroutine);
        }
        this.wdata.Destroy();
        this.tdata.Destroy();
        GameObject.Destroy(this.wobj);
        GameObject.Destroy(this.tobj);
        GameObject.Destroy(this.root);
    }
}

