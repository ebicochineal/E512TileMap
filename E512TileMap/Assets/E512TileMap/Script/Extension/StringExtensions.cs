using UnityEngine;
using System.Collections;

static public class StringExtensions {
    static public string Color (this string s, string color) {
        return "<color=" + color + ">" + s + "</color>";
    }
    //static public string Red (this string s) {
    //    return "<color=red>" + s + "</color>";
    //}
}
