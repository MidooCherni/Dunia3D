using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	// Movement stats
	public float speed = 10.0f;
	public float rotateSpeed = 100.0f;
	public bool grounded = true;

	// Camera
	public GameObject cam;

    void Update(){
        if (Physics.Raycast(transform.position, Vector3.down, 0.5f)){
            grounded = true;
        } else {
            grounded = false;
        }
		if (Input.GetMouseButton(1)){
			transform.Rotate(0, (Input.GetAxis("Mouse X"))*1000*(Time.deltaTime), 0);
		}
   		if (Input.GetMouseButtonUp(1)){
   			Cursor.visible = true;
   		}
    }

    void FixedUpdate(){
        float transAmount = speed * Time.deltaTime;
    	float rotateAmount = rotateSpeed * Time.deltaTime;

        if ((Input.GetKey("up")) || (Input.GetKey("z"))) {
    	    transform.Translate(0, 0, transAmount);
    	}
    	if ((Input.GetKey("down")) || (Input.GetKey("s"))) {
    	    transform.Translate(0, 0, -transAmount);
  	 	}
  	 	if ((Input.GetKey("left")) || (Input.GetKey("q"))) {
			if (Input.GetMouseButton(1)){
				if ((Input.GetKey("up")) || (Input.GetKey("down")) || (Input.GetKey("w")) || (Input.GetKey("s"))) { transAmount /= 2; }
				transform.Translate(-transAmount, 0, 0);
			} else {
	  	 	    transform.Rotate(0, -rotateAmount, 0);
			}
  	 	}
  	 	if (Input.GetKey("a")) {
  	 		transform.Translate(-transAmount, 0, 0);
  	 	}
  	 	if (Input.GetKey("e")) {
			transform.Translate(transAmount, 0, 0);
  	 	}
   	 	if ((Input.GetKey("right")) || (Input.GetKey("d"))) {
  	      	if (Input.GetMouseButton(1)){
				if ((Input.GetKey("up")) || (Input.GetKey("down")) || (Input.GetKey("w")) || (Input.GetKey("s"))) { transAmount /= 2; }
				transform.Translate(transAmount, 0, 0);
			} else {
	  	 	    transform.Rotate(0, rotateAmount, 0);
			}	
  	 	}
  	 	if ((Input.GetKeyDown (KeyCode.Space)) && (grounded)){
        	GetComponent<Rigidbody>().AddForce(new Vector3(0, 500, 0), ForceMode.Impulse);
    	}
    }
}
