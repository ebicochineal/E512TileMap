using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TGUIManager : MonoBehaviour {
    public static Dictionary<string, TGUIManager> tgui = new Dictionary<string, TGUIManager>();
    public int wdiv = 0;// 横何マスか指定してスケールを調整
    public float scale = 1f;
    private int snap_size;
    
    public int w;
    public int h;

    [NonSerialized]
    public int mouseover = -1;

    public List<TGUI> gui_list = new List<TGUI>();

    // OnButtonEvent
    private TGUI prev_on_window;
    private Vector3 prev_mouse_pos = new Vector3();

    // Update
    private Vector3 press_mouse_pos = Vector3.zero;
    private bool is_window_drag = false;
    private E512Pos drag_pos = new E512Pos();

    public static TGUIManager TGUI {
        set {
            if (!TGUIManager.tgui.ContainsKey(SceneManager.GetActiveScene().name)) {
                TGUIManager.tgui.Add(SceneManager.GetActiveScene().name, value);
            }
        }
        get { return TGUIManager.tgui[SceneManager.GetActiveScene().name]; }
    }

    void Awake () {
        if (this.wdiv > 0) {
            float t = Screen.width / 16;
            this.scale = t / wdiv;
        }

        TGUIManager.TGUI = this;
        this.snap_size = (int)(16 * this.scale);
        this.w = Screen.width / this.snap_size;
        this.h = Screen.height / this.snap_size ;
    }
    
    void Update () {
        this.MouseOverUpdate();

        this.OnButtonEvent();

        
        if (Input.GetMouseButtonDown(0)) {
            this.press_mouse_pos = Input.mousePosition;
            var index = this.SelectIndex();
            var l = new List<TGUI>();
            for (int i = 0; i < this.gui_list.Count; ++i) {
                if (i != index) { l.Add(this.gui_list[i]); }
            }
            if (index > -1) {
                l.Add(this.gui_list[index]);
                if (this.gui_list[index].move) {
                    this.drag_pos = this.gui_list[index].pos;
                    this.is_window_drag = true;
                }
                
            }
            this.gui_list = l;
        }

        if (Input.GetMouseButton(0) && this.is_window_drag) {
            var v = Input.mousePosition - this.press_mouse_pos;
            var x = (int)v.x / this.snap_size;
            var y = (int)v.y / this.snap_size;
            this.ActiveGUI<TGUI>().pos.x = this.drag_pos.x + x;
            this.ActiveGUI<TGUI>().pos.y = this.drag_pos.y - y;
        }

        if (Input.GetMouseButtonUp(0) && this.is_window_drag) {
            this.is_window_drag = false;
            
            //this.OrderOnClickEvent(this.ActiveWindow());
        }
        
    }

    void LateUpdate () {
        this.scale = this.scale < 0.1f ? 0.1f : this.scale;
        this.snap_size = (int)(16 * this.scale);
        this.w = Screen.width / this.snap_size;
        this.h = Screen.height / this.snap_size;
        var a = Mathf.PI / 180f;
        var b = Camera.main.fieldOfView * 0.5f * a;
        var s = Mathf.Sin(b) / Mathf.Cos(b) * 0.625f * this.snap_size;

        for (int i = 0; i < this.gui_list.Count; i++) {
            this.gui_list[i].z = i;
        }
        
        foreach (var i in this.gui_list) {
            // GUI拡大縮小
            var cs = s * (1f - 0.1f - 0.01f * i.z) / this.scale;
            if (Camera.main.orthographic) { cs = Camera.main.orthographicSize; }
            var v = Vector3.one * cs / ((float)Screen.height / (float)this.snap_size / 2.0f);
            i.root.transform.localScale = v;

            // GUI移動
            i.root.transform.rotation = Camera.main.transform.rotation;
            var p = Camera.main.ScreenToWorldPoint(new Vector3(this.snap_size * i.pos.x, this.snap_size * (this.h - i.pos.y - i.h), 10 - 1f - (0.1f * i.z)));
            i.root.transform.position = p;
        }
        
    }

    public void SortGUI () {
        this.gui_list = this.gui_list.OrderBy(x => x.id).ToList();
    }

    public void Slanting () {
        this.SortGUI();
        for (int i = 0; i < this.gui_list.Count; ++i) {
            this.gui_list[i].pos.x = i + 1;
            this.gui_list[i].pos.y = i + 1;
        }
    }
    
    public void Vertical (E512Pos pos = new E512Pos(), bool flap = false) {
        this.SortGUI();
        var temp = pos.y;
        var max_x = 0;
        for (int i = 0; i < this.gui_list.Count; ++i) {
            this.gui_list[i].pos.x = pos.x;
            this.gui_list[i].pos.y = temp;
            temp += this.gui_list[i].h;
            var x = pos.x + this.gui_list[i].w;
            max_x = x > max_x ? x : max_x;
            if (flap && temp >= this.h) {
                temp = pos.y;
                pos.x = max_x;
            }
        }
    }

    public void Horizontal (E512Pos pos = new E512Pos(), bool flap = false) {
        this.SortGUI();
        var temp = pos.x;
        var max_y = 0;
        for (int i = 0; i < this.gui_list.Count; ++i) {
            this.gui_list[i].pos.x = temp;
            this.gui_list[i].pos.y = pos.y;
            temp += this.gui_list[i].w;
            var y = pos.y + this.gui_list[i].h;
            max_y = y > max_y ? y : max_y;
            if (flap && temp >= this.w) {
                temp = pos.x;
                pos.y = max_y;
            }
        }
    }
    
    public Type ActiveGUI<Type> () where Type : TGUI {
        if (this.gui_list.Count > 0 && this.gui_list[this.gui_list.Count - 1] is Type) {
            return (Type)this.gui_list[this.gui_list.Count - 1];
        } else {
            return null;
        }
    }

    public Type MouseOverGUI<Type> () where Type : TGUI {
        if (this.mouseover > -1 && this.gui_list[this.mouseover] is Type) {
            return (Type)this.gui_list[this.mouseover];
        } else {
            return null;
        }
    }
    
    public int SelectIndex () {
        if (Input.GetMouseButton(0)) {
            return this.mouseover;
        } else {
            return -1;
        }
    }

    public void MouseOverUpdate () {
        this.mouseover = -1;
        var v = Input.mousePosition;
        var x = (int)v.x / this.snap_size;
        var y = (int)v.y / this.snap_size;
        var cpos = new E512Pos(x, y);
        for (int i = this.gui_list.Count - 1; i >= 0; --i) {
            if (this.gui_list[i].InWindow(cpos, this.h)) {
                this.mouseover = i;
                break;
            }
        }
    }
    
    public Type AddGUI<Type> (Type t) where Type : TGUI {
        this.gui_list.Add(t);
        t.id = this.gui_list.Count;
        return t;
    }
    
    private void OnButtonEvent () {
        if (Vector3.Distance(this.prev_mouse_pos, Input.mousePosition) > 8 ) { this.prev_on_window = null; }// マウスが移動時ボタン無効
        this.prev_mouse_pos = Input.mousePosition;
        
        if (Input.GetMouseButtonDown(0)) {
            this.prev_on_window = this.MouseOverGUI<TGUI>();
        }
        if (Input.GetMouseButtonUp(0)) {
            if (this.prev_on_window == this.MouseOverGUI<TGUI>()) { this.OrderOnClickEvent(this.prev_on_window); }
            this.prev_on_window = null;
        }
    }

    public void Clean () {
        foreach (var i in this.gui_list) {
            i.Destroy();
        }
        this.gui_list = new List<TGUI>();
    }

    private void OrderOnClickEvent (TGUI t) {
        if (t != null) { t.OnClick(); } 
    }
}
