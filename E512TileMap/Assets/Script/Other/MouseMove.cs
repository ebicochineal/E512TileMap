using UnityEngine;
using System.Collections;

public class MouseMove : MonoBehaviour{
	void Update(){
		if(Input.GetMouseButton(2)){
			float x = Input.GetAxis("Mouse X");
			float y = Input.GetAxis("Mouse Y");
			this.transform.Translate(new Vector3(x, y, 0f));
		}
	}
}
