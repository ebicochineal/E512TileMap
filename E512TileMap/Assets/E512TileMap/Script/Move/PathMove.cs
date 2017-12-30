using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathMove {
    List<Vector2> path;
    Vector2 position;
    int index;

    public PathMove (List<Vector2> path, Vector2 position) {
        this.path = path;
        this.position = position;
        this.index = this.path.Count - 1;
    }

    public void SetPath (List<Vector2> path) {
        this.path = path;
    }

    public Vector2 MovePosition (float movespeed) {
        this.Move(this.position, movespeed);
        return this.position;
    }

    public Vector2 MoveLoopPosition (float movespeed) {
        this.MoveLoop(this.position, movespeed);
        return this.position;
    }

    void Move (Vector2 p, float s) {
        Vector2 pm = this.path[this.index];
        float dist = Vector2.Distance(pm, this.position);

        if (dist > s) {
            this.position += (pm - p).normalized * s;
        } else {
            if (this.index > 0) {
                this.index -= 1;
                this.Move(pm, s - dist);
            } else {
                this.position = this.path[0];
            }
        }
    }

    void MoveLoop (Vector2 p, float s) {
        Vector2 pm = this.path[this.index];
        float dist = Vector2.Distance(pm, this.position);

        if (dist > s) {
            this.position += (pm - p).normalized * s;
        } else {
            if (this.index > 0) {
                this.index -= 1;
                this.Move(pm, s - dist);
            } else {
                this.index = this.path.Count - 1;
                this.Move(pm, s - dist);
            }
        }
    }

    static public List<Vector2> ConvertCellPath (List<E512Pos> path, Vector2 startpoint, float celldistance) {
        List<Vector2> pathv2 = new List<Vector2>();
        foreach (E512Pos i in path) {
            Vector2 v = new Vector2((float)i.x, (float)i.y);
            pathv2.Add((v + startpoint) * celldistance);
        }
        return pathv2;
    }
}
