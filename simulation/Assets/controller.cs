using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controller : MonoBehaviour {

	public GameObject drone;
	public GameObject r1, r2, b1, b2;
	private float step = 1.0f;
	private float[] thrust;

	// Use this for initializatio
	void Start () {
		thrust = new float[4];
	}
	
	// Update is called once per frame
	void Update () {
		Update (Input.GetKey (KeyCode.A), r1, 0);
		Update (Input.GetKey (KeyCode.S), b1, 1);
		Update (Input.GetKey (KeyCode.D), r2, 2);
		Update (Input.GetKey (KeyCode.F), b2, 3);
	}

	void Update(bool f, GameObject obj, int i) {
		if (f) {
			thrust [i] += step;
			obj.GetComponent<Rigidbody> ().AddForce (
				drone.transform.rotation * new Vector3 (0, thrust [i], 0)
			);
		} else {
			thrust [i] -= step;
			if (thrust [i] < 0)
				thrust [i] = 0;
		}
	}
}
