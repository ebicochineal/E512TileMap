using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CellMove))]
public class SimpleCharacterControllerTopDownCellMove : MonoBehaviour {
    [HideInInspector]
    private CellMove cmove;
    void Start () { this.cmove = this.GetComponent<CellMove>(); }
    void Update () { /* input up,down */ }
    void FixedUpdate () {
        if (Input.GetKey(KeyCode.D)) { this.cmove.Order(CellMove.Right); }
        if (Input.GetKey(KeyCode.A)) { this.cmove.Order(CellMove.Left); }
        if (Input.GetKey(KeyCode.W)) { this.cmove.Order(CellMove.Up); }
        if (Input.GetKey(KeyCode.S)) { this.cmove.Order(CellMove.Down); }
        this.cmove.Move();
    }
}
