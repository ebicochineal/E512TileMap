using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(DotMove))]
public class DotCameraLerp : MonoBehaviour {
    private DotMove mov;
    public Transform target;
    public float lerp = 1;

    // Use this for initialization
    void Start() {
        this.mov = this.GetComponent<DotMove>();
    }
    
    void LateUpdate() {
        var p = this.transform.position;
        var tp = this.target.position;
        this.mov.Lerp(tp, lerp);
        //this.transform.position = new Vector3(Mathf.Lerp(p.x, tp.x, this.lerp), Mathf.Lerp(p.y, tp.y, this.lerp), p.z);
        
    }
}
