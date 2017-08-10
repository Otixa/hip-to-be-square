using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class ThudBlock : MonoBehaviour {
	private Rigidbody2D myRigidbody;


	void OnEnable () {
		
		myRigidbody = GetComponent<Rigidbody2D> ();
		myRigidbody.gravityScale = 0;
		myRigidbody.bodyType = RigidbodyType2D.Kinematic;

	}
		

	void OnCollisionEnter2D(Collision2D other){
		
		if (other.gameObject.tag =="Player" &&  other.gameObject.GetComponent<PlayerController>().prevVelocity <= -13 && other.gameObject.name == "Chunk"){
			Debug.Log (other.gameObject.GetComponent<PlayerController> ().prevVelocity);
			//myRigidbody.gameObject.layer = LayerMask.NameToLayer ("Default");			//change the dropping block to be not ground, so we can't jump off of it
			//myRigidbody.gravityScale = 1;
			myRigidbody.bodyType = RigidbodyType2D.Dynamic;
			myRigidbody.AddForce (Vector2.down * 20f, ForceMode2D.Impulse);
			GetComponent<Collider2D> ().enabled = false;
		}
	}

    void OnDisable()
    {
        //when deactivated, reset my co-ordinates

        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;    
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = true;
        

    }

}
