using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TBlock {
    public const int SIZE = 8;
    public const int BLOCKSIZE = 8 * 8;

    private int[] tileindex;// X, Y
    
    public TBlock () {
        this.tileindex = new int[TBlock.BLOCKSIZE];
    }

    private static int Convert (int x, int y) {
        return x + TBlock.SIZE * y;
    }

    public void SetTileIndex (int index, int x, int y) {
        this.tileindex[TBlock.Convert(x, y)] = index;
    }
    public int GetTileIndex (int x, int y) {
        return this.tileindex[TBlock.Convert(x, y)];
    }

    public static int BValue (int v) {
        return v < 0 ? (((v + 1) / (TBlock.SIZE)) - 1) : v / TBlock.SIZE;
    }

    public static int BLocalValue (int v) {
        return v < 0 ? ((v + 1) % TBlock.SIZE) + (TBlock.SIZE - 1) : v % TBlock.SIZE;
    }

    public static E512Pos BPos (E512Pos cpos) {
        int x = TBlock.BValue(cpos.x);
        int y = TBlock.BValue(cpos.y);
        return new E512Pos(x, y);
    }

    public static E512Pos BLocalPos (E512Pos cpos) {
        int x = TBlock.BLocalValue(cpos.x);
        int y = TBlock.BLocalValue(cpos.y);
        return new E512Pos(x, y);
    }
}
