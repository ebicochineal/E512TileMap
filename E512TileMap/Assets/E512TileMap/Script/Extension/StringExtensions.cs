using UnityEngine;
using System.Collections;

public static class StringExtensions{
	public static string Color(this string s, string color){
		return "<color=" + color + ">" + s + "</color>";
	}
//	public static string Red(this string s){
//		return "<color=red>" + s + "</color>";
//	}
}
