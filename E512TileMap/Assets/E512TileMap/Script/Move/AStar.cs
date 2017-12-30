using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStar {
    class Node {
        public E512Pos pos;
        public float fs;
        public Node parentnode;

        public int x { get { return this.pos.x; } }
        public int y { get { return this.pos.y; } }

        public Node (E512Pos pos, float fs) {
            this.pos = pos;
            this.fs = fs;
        }
    }


    List<Node> open = new List<Node>();
    List<Node> close = new List<Node>();
    Node goalnode;
    int[,] mapdata;// 可0 不可-1 不可ビット左上下右8421
    int[,] mapweight;
    int[,] mapheight;
    E512Pos start;
    E512Pos goal;
    int jump;

    public bool endSearch { get { return goalnode != null; } }

    public AStar (E512Pos start, E512Pos goal, int[,] mapdata) {
        this.open.Add(new Node(start, Distance(start, goal)));
        this.mapdata = mapdata;
        this.start = start;
        this.goal = goal;
        this.jump = 1;

        this.mapweight = new int[mapdata.GetLength(0), mapdata.GetLength(1)];
        for (int x = 0; x < mapdata.GetLength(0); ++x) {
            for (int y = 0; y < mapdata.GetLength(1); ++y) {
                this.mapweight[x, y] = 1;
            }
        }
        this.mapheight = new int[mapdata.GetLength(0), mapdata.GetLength(1)];
        for (int x = 0; x < mapdata.GetLength(0); ++x) {
            for (int y = 0; y < mapdata.GetLength(1); ++y) {
                this.mapheight[x, y] = 1;
            }
        }
    }

    public AStar (E512Pos start, E512Pos goal, int[,] mapdata, int[,] mapweight, int[,] mapheight, int jump) {
        this.open.Add(new Node(start, Distance(start, goal)));
        this.mapdata = mapdata;
        this.mapweight = mapweight;
        this.mapheight = mapheight;
        this.start = start;
        this.goal = goal;
        this.jump = jump;
    }

    public static List<E512Pos> Path (E512Pos start, E512Pos goal, int[,] mapdata) {
        AStar a = new AStar(start, goal, mapdata);
        a.Search();
        if (a.endSearch) {
            return a.GoalPathList();
        } else {
            return null;
        }
    }
    public static List<E512Pos> Path (E512Pos start, E512Pos goal, int[,] mapdata, int[,] mapweight, int[,] mapheight, int jump) {
        AStar a = new AStar(start, goal, mapdata, mapweight, mapheight, jump);
        a.Search();
        if (a.endSearch) {
            return a.GoalPathList();
        } else {
            return null;
        }
    }

    // ルートの計算をする
    public bool Search () {
        // オープンリストが空なら失敗
        while (this.open.Count > 0) {
            // 最小fsのインデックス
            int index = this.MinFS();
            // 最小ｆｓを持つノード
            Node minfsnode = this.open[index];
            // 最小ｆｓを持つノードをクローズリストに
            this.close.Add(this.open[index]);
            // 最小ｆｓを持つノードをオープンリストから消す
            this.open.RemoveAt(index);
            // ゴールと同じなら終わり
            if (minfsnode.pos == this.goal) {
                this.goalnode = minfsnode;
                return true;
            }
            // 周囲開く
            this.OpenNode(minfsnode);
        }
        return false;
    }

    // 計算を進める OpenCloseをが視覚的に見える
    public void VisualSearch () {
        // オープンリストが空なら失敗
        if (this.open.Count > 0 && !endSearch) {
            // 最小fsのインデックス
            int index = this.MinFS();
            // 最小ｆｓを持つノード
            Node minfsnode = this.open[index];
            // 最小ｆｓを持つノードをクローズリストに
            this.close.Add(this.open[index]);
            // 最小ｆｓを持つノードをオープンリストから消す
            this.open.RemoveAt(index);
            // ゴールと同じなら終わり
            if (minfsnode.pos == this.goal) {
                this.goalnode = minfsnode;
                return;
            }
            // 周囲開く
            this.OpenNode(minfsnode);
        }
    }

    // 周囲４つを開きオープンに追加する
    void OpenNode (Node n) {
        float gs = this.GStar(n);
        foreach (E512Pos i in this.Direction4(n.pos)) {
            //		foreach(MTileMap2DInt i in this.Direction8(n.pos)){
            if (Filter(i, n.pos)) {
                float fsm = gs + this.Distance(i, this.goal) + this.Cost(i, n.pos);
                if (!(this.FindPos(i, this.open) < 0)) {
                    // open
                    int j = this.FindPos(i, this.open);
                    if (this.open[j].fs > fsm) {
                        Node m = new Node(i, fsm);
                        m.parentnode = n;
                        this.open[j] = m;
                    }
                } else if (!(this.FindPos(i, this.close) < 0)) {
                    // close
                    int j = this.FindPos(i, this.close);
                    if (this.close[j].fs > fsm) {
                        this.close.RemoveAt(j);
                        Node m = new Node(i, fsm);
                        m.parentnode = n;
                        this.open.Add(m);
                    }
                } else {
                    Node m = new Node(i, fsm);
                    m.parentnode = n;
                    this.open.Add(m);
                }
            }
        }
    }

    // リストの中にあるか調べる
    int FindPos (E512Pos o, List<Node> n) {
        for (int i = 0; i < n.Count; ++i) {
            if (o == n[i].pos) {
                return i;
            }
        }
        return -1;
    }

    // ４方向イテレータ
    IEnumerable<E512Pos> Direction4 (E512Pos p) {
        yield return new E512Pos(p.x, p.y + 1);
        yield return new E512Pos(p.x + 1, p.y);
        yield return new E512Pos(p.x, p.y - 1);
        yield return new E512Pos(p.x - 1, p.y);
    }

    // ８方向イテレータ
    IEnumerable<E512Pos> Direction8 (E512Pos p) {
        yield return new E512Pos(p.x, p.y + 1);
        yield return new E512Pos(p.x + 1, p.y);
        yield return new E512Pos(p.x, p.y - 1);
        yield return new E512Pos(p.x - 1, p.y);
        yield return new E512Pos(p.x + 1, p.y + 1);
        yield return new E512Pos(p.x + 1, p.y - 1);
        yield return new E512Pos(p.x - 1, p.y - 1);
        yield return new E512Pos(p.x - 1, p.y + 1);
    }

    // open o  prev p
    bool Filter (E512Pos o, E512Pos p) {
        if (this.InSide(o) && this.NoPassable(o) && this.Jump(o, p) && this.DirectionBlock(o, p)) {
            return true;
        } else {
            return false;
        }
    }

    // 進む先のセルが進入可能ならtrue
    bool DirectionBlock (E512Pos o, E512Pos p) {
        E512Pos dire = o - p;
        int b = this.mapdata[p.x, p.y];
        if (b == 0) { return true; }
        if (b == -1) { return false; }
        if (dire.x == -1 && (b & 8) == 8) { return false; }// 左
        if (dire.y == 1 && (b & 4) == 4) { return false; }// 上
        if (dire.y == -1 && (b & 2) == 2) { return false; }// 下
        if (dire.x == 1 && (b & 1) == 1) { return false; }// 右
        return true;
    }

    // ジャンプ可能ならtrue
    bool Jump (E512Pos o, E512Pos p) {
        return (this.mapheight[o.x, o.y] - this.mapheight[p.x, p.y]) <= this.jump;
    }

    // 進む先のセルが進入可能ならtrue
    bool NoPassable (E512Pos o) {
        return this.mapdata[o.x, o.y] < 0 ? false : true;
    }

    // マップサイズ内ならtrue
    bool InSide (E512Pos o) {
        if (o.x < 0 || o.x >= this.mapdata.GetLength(0)
        || o.y < 0 || o.y >= this.mapdata.GetLength(1)) {
            return false;
        }
        return true;
    }

    float Cost (E512Pos a, E512Pos b) {
        //　コストの最低値は１として計算
        // コストは入るときに払うなので斜めから入り従事方向に出ると計算がおかしい
        return this.Distance(a, b) * (this.mapweight[a.x, a.y]);
    }

    //　マンハッタン距離
    float ManhattanDistance (E512Pos a, E512Pos b) {
        return (float)(Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y));
    }

    // ユーグリッド距離
    float EuclideanDistance (E512Pos a, E512Pos b) {
        return Vector2.Distance(new Vector2((float)a.x, (float)a.y), new Vector2((float)b.x, (float)b.y));
    }

    // ８方向移動可能なときの正確な距離
    float EuclideanManhattanDistance (E512Pos a, E512Pos b) {
        int x = Mathf.Abs(a.x - b.x);
        int y = Mathf.Abs(a.y - b.y);
        int n = Mathf.Min(x, y);
        return (1.414f * n) + (float)Mathf.Abs(x - y);
    }

    float Distance (E512Pos a, E512Pos b) {
        return this.ManhattanDistance(a, b);
    }

    // ゴールからｎまでの距離　
    float HStar (Node n) {
        return this.Distance(n.pos, this.goal);
    }
    // スタートからｎまでの距離
    float GStar (Node n) {
        return n.fs - this.HStar(n);
    }
    // スタートからｎまでの距離＋ゴールからｎまでの距離
    float FStar (Node n) {
        return n.fs;
    }

    // nからstartの直線距離 + nからgoalの直線距離
    float DistanceSG (E512Pos p) {
        return this.EuclideanDistance(p, this.start) + this.EuclideanDistance(p, this.goal);
    }


    // コストが最小のopenを返す
    int MinFS () {
        int a = 0;
        for (int i = 1; i < this.open.Count; ++i) {
            if (this.open[a].fs > this.open[i].fs) {
                a = i;
            } else if (this.open[a].fs == this.open[i].fs) {// fsが同じならスタートからゴールの直線状に近いオープンを優先的に選ぶ
                if (this.DistanceSG(this.open[a].pos) > this.DistanceSG(this.open[i].pos)) {
                    a = i;
                }
            }
        }
        return a;
    }

    // スタートからｎまでのルートを返す
    IEnumerable<Node> Path (Node n) {
        if (this.start != n.pos) {
            foreach (Node i in Path(n.parentnode)) {
                yield return i;
            }
        }
        yield return n;
    }

    public IEnumerable<E512Pos> OpenIter () {
        foreach (Node i in this.open) {
            yield return i.pos;
        }
    }

    public IEnumerable<E512Pos> CloseIter () {
        foreach (Node i in this.close) {
            yield return i.pos;
        }
    }

    public List<E512Pos> GoalPathList () {
        List<E512Pos> l = new List<E512Pos>();
        foreach (Node i in Path(this.goalnode)) {
            l.Add(i.pos);
        }
        l.Reverse();
        return l;
    }
}