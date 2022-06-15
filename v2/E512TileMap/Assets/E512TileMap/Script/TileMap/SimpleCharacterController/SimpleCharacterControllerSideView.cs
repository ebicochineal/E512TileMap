using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DotCollision))]
public class SimpleCharacterControllerSideView : MonoBehaviour {
    [HideInInspector]
    private DotCollision col;
    void Start () { this.col = this.GetComponent<DotCollision>(); }
    void Update () { /* input getkey down */ }
    void FixedUpdate () {
        if (Input.GetKey(KeyCode.D)) { this.col.MoveX(0.125f); }
        if (Input.GetKey(KeyCode.A)) { this.col.MoveX(-0.125f); }
        if (this.col.isground && Input.GetKey(KeyCode.W)) { this.col.gravity = 0.35f; }
        this.col.Gravity(0.010f);
    }
}

