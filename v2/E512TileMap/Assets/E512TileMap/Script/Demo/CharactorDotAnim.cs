using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DotMove))]
public class CharactorDotAnim : MonoBehaviour {
    DotMove move;
    float t;
    // Use this for initialization
    void Start () {
        this.move = this.GetComponent<DotMove>();
        this.move.Init();
    }
    
    void FixedUpdate () {
        this.t += Time.deltaTime;
        this.move.LocalZero();
        this.move.y = Mathf.Sin(this.t * 10) > 0 ? this.move.y : this.move.y + 1;
    }
}
