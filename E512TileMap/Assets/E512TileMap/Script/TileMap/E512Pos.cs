using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct E512Pos : IEquatable<E512Pos> {
    public int x;
    public int y;

    public E512Pos (int x, int y) {
        this.x = x;
        this.y = y;
    }

    public static E512Pos operator + (E512Pos a, E512Pos b) {
        return new E512Pos(a.x + b.x, a.y + b.y);
    }
    public static E512Pos operator + (E512Pos a, int b) {
        return new E512Pos(a.x + b, a.y + b);
    }

    public static E512Pos operator - (E512Pos a, E512Pos b) {
        return new E512Pos(a.x - b.x, a.y - b.y);
    }
    public static E512Pos operator - (E512Pos a, int b) {
        return new E512Pos(a.x - b, a.y - b);
    }

    public static E512Pos operator * (E512Pos a, int b) {
        return new E512Pos(a.x * b, a.y * b);
    }

    public static bool operator == (E512Pos a, E512Pos b) {
        if (System.Object.ReferenceEquals(a, b)) { return true; }// インスタンスが同じ
        if ((System.Object)a == null || (System.Object)b == null) { return false; }// どちらかがnullならfalse
        return (a.x == b.x && a.y == b.y);
    }

    public static bool operator != (E512Pos a, E512Pos b) {
        return !(a == b);
    }

    public override bool Equals (System.Object obj) {
        if (System.Object.ReferenceEquals(this, obj)) { return true; }// インスタンスが同じ
        if (obj == null) { return false; }
        E512Pos p = (E512Pos)obj;// オブジェクト型なのでキャスト
        return (this.x == p.x && this.y == p.y);
    }
    

    bool IEquatable<E512Pos>.Equals (E512Pos other) {
        //if (other == null) { return false; }
        return (this.x == other.x && this.y == other.y);
    }
    
    public override int GetHashCode () {
        return (this.x << 16) | this.y;
    }

    public override string ToString () {
        return string.Format("{0}, {1}", this.x, this.y);
    }

    // ２点セル座標からボックス範囲内のセルリストを作成
    public static List<E512Pos> BoxList (E512Pos v1, E512Pos v2) {
        List<E512Pos> list = new List<E512Pos>();
        E512Pos s = E512Pos.Min(v1, v2);
        E512Pos e = E512Pos.Max(v1, v2);
        e += 1;
        for (int x = s.x; x < e.x; ++x) {
            for (int y = s.y; y < e.y; ++y) {
                list.Add(new E512Pos(x, y));
            }
        }
        return list;
    }
    // ２点セル座標からボックス範囲内のセルリストを作成 v範囲拡大縮小
    public static List<E512Pos> BoxList (E512Pos v1, E512Pos v2, int v) {
        List<E512Pos> list = new List<E512Pos>();
        E512Pos s = E512Pos.Min(v1, v2) - v;
        E512Pos e = E512Pos.Max(v1, v2) + v;
        e += 1;
        for (int x = s.x; x < e.x; ++x) {
            for (int y = s.y; y < e.y; ++y) {
                list.Add(new E512Pos(x, y));
            }
        }
        return list;
    }

    public static E512Pos Min (E512Pos v1, E512Pos v2) {
        return new E512Pos(Mathf.Min(v1.x, v2.x), Mathf.Min(v1.y, v2.y));
    }

    public static E512Pos Max (E512Pos v1, E512Pos v2) {
        return new E512Pos(Mathf.Max(v1.x, v2.x), Mathf.Max(v1.y, v2.y));
    }

    public E512Pos AddPos (int x, int y) {
        this.x += x;
        this.y += y;
        return this;
    }
}

public static class MTileMapPosExtensions {
    public static E512Pos ToMPos (this Vector2 v) {
        int ix = v.x < 0 ? ((int)v.x) - 1 : (int)v.x;
        int iy = v.y < 0 ? ((int)v.y) - 1 : (int)v.y;
        return new E512Pos(ix, iy);
    }

    public static E512Pos ToMPos (this Vector3 v) {
        int ix = v.x < 0 ? ((int)v.x) - 1 : (int)v.x;
        int iy = v.y < 0 ? ((int)v.y) - 1 : (int)v.y;
        return new E512Pos(ix, iy);
    }
}