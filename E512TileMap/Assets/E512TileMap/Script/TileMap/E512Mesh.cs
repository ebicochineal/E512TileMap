using UnityEngine;
using System.Collections;

// グリッドメッシュ生成クラス
public class E512Mesh {
    // グリッドメッシュ生成 中心
    static public Mesh Grid (int gx, int gy) {
        return Grid(gx, gy, -((float)gx / 2), -((float)gy / 2), false);
    }

    // グリッドメッシュ生成 グリッドの中心座標指定
    static public Mesh Grid (int gx, int gy, float cx, float cy) {
        return Grid(gx, gy, cx, cy, false);
    }

    // グリッドメッシュ生成 グリッドの中心座標指定 セルサイズをハーフにするか指定
    static public Mesh Grid (int gx, int gy, float cx, float cy, bool half) {
        Mesh m = new Mesh();
        m.name = "GridMesh";
        m.vertices = GridVertice(gx, gy, cx, cy, half);
        m.triangles = GridTriangle(gx, gy, 1, half);
        m.uv = GridUV(gx, gy, 1, half);
        m.RecalculateNormals();
        m.RecalculateBounds();
        return m;
    }

    static Vector3[] GridVertice (int gx, int gy, float cx, float cy, bool half) {
        int max_x = gx * (half ? 2 : 1);
        int max_y = gy * (half ? 2 : 1);
        float fs = (half ? 0.5f : 1f);
        Vector3[] vertices = new Vector3[max_x * max_y * 4];
        for (int y = 0; y < max_y; ++y) {
            for (int x = 0; x < max_x; ++x) {
                float fx = (fs * (float)x) + cx;
                float fy = (fs * (float)y) + cy;
                int index = (x * 4) + (max_x * y * 4);
                vertices[index] = new Vector3(fs + fx, fs + fy, 0f);
                vertices[index + 1] = new Vector3(0f + fx, 0f + fy, 0f);
                vertices[index + 2] = new Vector3(0f + fx, fs + fy, 0f);
                vertices[index + 3] = new Vector3(fs + fx, 0f + fy, 0f);
            }
        }
        return vertices;
    }
    

    static int[] GridTriangle (int gx, int gy, int layer, bool half) {
        int max_x = gx * (half ? 2 : 1);
        int max_y = gy * (half ? 2 : 1);
        int[] triangles = new int[max_x * max_y * layer * 6];
        for (int i = 0; i < max_x * max_y * layer; ++i) {
            int t2 = i * 6;// 2tri
            int q = i * 4;// 2tri vert
            triangles[t2] = q + 0;
            triangles[t2 + 1] = q + 1;
            triangles[t2 + 2] = q + 2;
            triangles[t2 + 3] = q + 3;
            triangles[t2 + 4] = q + 1;
            triangles[t2 + 5] = q + 0;
        }
        return triangles;
    }

    static Vector2[] GridUV (int gx, int gy, int layer, bool half) {
        int max_x = gx * (half ? 2 : 1);
        int max_y = gy * (half ? 2 : 1);
        return new Vector2[max_x * max_y * layer * 4];
    }


    static readonly public Mesh Quad = E512Mesh._Quad();
    static readonly public Mesh RQuad = E512Mesh._RQuad();
    static private Mesh _Quad () {
        Mesh m = new Mesh();
        m.name = "MMeshQuad";
        m.vertices = new Vector3[] { new Vector3(-0.5f, -0.5f, 0), new Vector3(-0.5f, 0.5f, 0) , new Vector3(0.5f, 0.5f, 0) , new Vector3(0.5f, -0.5f, 0) };
        m.triangles = new int[] { 0, 1, 2, 0, 2, 3};
        m.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };
        m.RecalculateNormals();
        m.RecalculateBounds();
        return m;
    }
    static private Mesh _RQuad () {
        Mesh m = new Mesh();
        m.name = "MMeshQuad";
        m.vertices = new Vector3[] { new Vector3(-0.5f, -0.5f, 0), new Vector3(-0.5f, 0.5f, 0), new Vector3(0.5f, 0.5f, 0), new Vector3(0.5f, -0.5f, 0) };
        m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        m.uv = new Vector2[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), new Vector2(0, 0) };
        m.RecalculateNormals();
        m.RecalculateBounds();
        return m;
    }
}
