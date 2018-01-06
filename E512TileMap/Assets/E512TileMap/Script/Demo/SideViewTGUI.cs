﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideViewTGUI : MonoBehaviour {
    TGUIManager tgm;
    TGUIWindow menu;

    void Start () {
        this.tgm = TGUIManager.TGUI;
        var tgp = new TGUIWindowTexture("window16", "font16h");
        this.menu = this.tgm.AddGUI(new TGUIWindow(12, 4, new E512Pos(0, this.tgm.h - 6), this.ListText(), true, tgp));

        var t = new TGUIWindowTexture("window16", "font16y");
        this.tgm.AddGUI(new TGUIWindow(48, 2, new E512Pos(0, this.tgm.h - 2), "ｱｸｼｮﾝ:Right,GUIﾃｽﾄ:7,8,9,h,v,\\,ｸﾘｱ:space", true, t));
        
    }


    int n = 0;
    List<string> lis = new List<string>() { "egg", "fire", "ebiblue" };
    public void ListDown (TGUIWindow self) {
        this.n = (this.n + 1) % this.lis.Count;
        GameObject.Find("EbiCochineal").GetComponent<PlayerSide>().actiontype = n;
        if (this.tgm.gui_list.IndexOf(this.menu) > -1) {// SpaceキーでGUI削除できるようにしているため
            self.Text = this.ListText();
        }
    }

    public void ListUp (TGUIWindow self) {
        this.n = (this.n - 1 + this.lis.Count) % this.lis.Count;
        GameObject.Find("EbiCochineal").GetComponent<PlayerSide>().actiontype = n;
        if (this.tgm.gui_list.IndexOf(this.menu) > -1) {// SpaceキーでGUI削除できるようにしているため
            self.Text = this.ListText();
        }
    }

    public string ListText () {
        var s = "";
        for (int i = 0; i < this.lis.Count; i++) {
            var t = this.lis[i];
            if (i == this.n) {
                t = "[" + t + "]";
            } else {
                t = " " + t;
            }
            s += t + this.Space(11 - t.Length);
        }
        return s;
    }

    public string Space (int n) {
        var s = "";
        for (int i = 0; i < n; i++) {
            s += " ";
        }
        return s;
    }

    // Update is called once per frame
    void Update () {
        //var ac = this.tgm.ActiveGUI<TGUIWindow>();
        var mo = this.tgm.MouseOverGUI<TGUIWindow>();
        //if (Input.GetKeyDown(KeyCode.LeftArrow) && ac != null) {
        //    ac.PrevTextPage();
        //}
        //if (Input.GetKeyDown(KeyCode.RightArrow) && ac != null) {
        //    ac.NextTextPage();
        //}
        if (Input.GetKey(KeyCode.UpArrow) && mo != null && mo != this.menu) {
            mo.PrevTextLine();
        }
        if (Input.GetKey(KeyCode.DownArrow) && mo != null && mo != this.menu) {
            mo.NextTextLine();
        }
        if (Input.GetKeyDown(KeyCode.V)) {
            this.tgm.Vertical(new E512Pos(), true);
        }
        if (Input.GetKeyDown(KeyCode.H)) {
            this.tgm.Horizontal(new E512Pos(), true);
        }
        if (Input.GetKeyDown(KeyCode.Backslash)) {
            this.tgm.Slanting();
        }
        
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            this.ListDown(this.menu);
        }
        
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            this.ListUp(this.menu);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha9)) {
            var tgp = new TGUIWindowTexture("window16", "font16h");
            var t = this.tgm.AddGUI(new TGUIWindow(new E512Pos(), "Off", true, tgp));
            t.onclick += this.Button;
            this.tgm.Horizontal(new E512Pos(), true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha8)) {
            var tgp = new TGUIWindowTexture("window8", "font8");
            var t = this.tgm.AddGUI(new TGUIWindow(8, 8, new E512Pos(), "", true, tgp));
            this.SetSlowText(t);
            t.onclick += this.SetSlowText;
            this.tgm.Horizontal(new E512Pos(), true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha7)) {
            var tgp = new TGUIWindowTexture("window16", "font16h");
            var t = this.tgm.AddGUI(new TGUIWindow(8, 8, new E512Pos(), "", true, tgp));
            this.SetSlowText(t);
            t.onclick += this.SetSlowText;
            this.tgm.Horizontal(new E512Pos(), true);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            this.tgm.Clean();
        }
    }

    public void Button (TGUIWindow self) {
        if (self.Text == "Off") {
            self.Text = "On";
        } else {
            self.Text = "Off";
        }
    }

    public void SetSlowText (TGUIWindow self) {
        var s = "";
        for (int i = 0; i < 16; i++) { s += "(ｽｸﾛｰﾙ:ﾏｳｽｵｰﾊﾞ&UpDownkey), (ﾘｾｯﾄ:ｸﾘｯｸ), (ﾄﾞﾗｯｸﾞ:ｲﾄﾞｳ)"; }
        self.SlowText(s, 0.01f);
    }

}
