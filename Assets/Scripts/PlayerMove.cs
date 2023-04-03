using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
	// Movement stats
	public float speed = 10.0f;
	public float rotateSpeed = 100.0f;
	public bool grounded = true;
	public GameObject cam;

	public GameObject walksoundui;

	void Start(){
        walksoundui.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Sounds/walkloop");
	}

	void Update(){
        grounded = Physics.Raycast(transform.position, Vector3.down, 0.5f);
	}

    void FixedUpdate(){
		if(!(((Input.GetKey("up")) || (Input.GetKey("z"))) ||
			((Input.GetKey("down")) || (Input.GetKey("s"))) ||
			((Input.GetKey("left")) || (Input.GetKey("q"))) ||
			((Input.GetKey("right")) || (Input.GetKey("d"))))){
				if(walksoundui.GetComponent<AudioSource>().isPlaying) walksoundui.GetComponent<AudioSource>().Stop();
			}
			
		if(GetComponent<StatHandler>().isStunned || GetComponent<StatHandler>().isRooted){ return; }

        float transAmount = speed * Time.deltaTime;
    	float rotateAmount = rotateSpeed * Time.deltaTime;

        if ((Input.GetKey("up")) || (Input.GetKey("z"))) {		// TODO: change to wasd
			if(!walksoundui.GetComponent<AudioSource>().isPlaying) walksoundui.GetComponent<AudioSource>().Play();
    	    transform.Translate(0, 0, transAmount);
    	}
    	if ((Input.GetKey("down")) || (Input.GetKey("s"))) {
			if(!walksoundui.GetComponent<AudioSource>().isPlaying) walksoundui.GetComponent<AudioSource>().Play();
    	    transform.Translate(0, 0, -transAmount);
  	 	}
  	 	if ((Input.GetKey("left")) || (Input.GetKey("q"))) {
			if(!walksoundui.GetComponent<AudioSource>().isPlaying) walksoundui.GetComponent<AudioSource>().Play();
			if (cam.GetComponent<MouseLook>().mouselook){
				if ((Input.GetKey("up")) || (Input.GetKey("down")) || (Input.GetKey("w")) || (Input.GetKey("s"))) { transAmount /= 2; }
				transform.Translate(-transAmount, 0, 0);
			} else {
	  	 	    transform.Rotate(0, -rotateAmount, 0);
			}
  	 	}
   	 	if ((Input.GetKey("right")) || (Input.GetKey("d"))) {
			if(!walksoundui.GetComponent<AudioSource>().isPlaying) walksoundui.GetComponent<AudioSource>().Play();
  	      	if (cam.GetComponent<MouseLook>().mouselook){
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
