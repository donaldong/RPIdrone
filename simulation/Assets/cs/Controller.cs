using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Reward {
	public float value;

	public Reward(float value) {
		this.value = value;
	}
}

class State {
	public Vector3 gyro;
	public Vector3 accel;

	public State(Vector3 gyro, Vector3 accel) {
		this.gyro = gyro;
		this.accel = accel;
	}
}

class Observation {
	public Reward reward;
	public State nextState;
	public bool done;

	public Observation(Reward reward, State nextState, bool done) {
		this.reward = reward;
		this.nextState = nextState;
		this.done = done;
	}
}

class Motor {
	public const float MAX_THRUST = 0.6f;
	private GameObject obj;
	private float percent;

	public Motor(GameObject obj) {
		this.obj = obj;
		percent = 0.0f;
	}

	public void increase() {
		percent += 0.01f;
		if (percent > 1.0f)
			percent = 1.0f;
	}

	public void decrease() {
		percent -= 0.01f;
		if (percent < 0.0f)
			percent = 0.0f;
	}

	public float getThrust() {
		return MAX_THRUST * percent;
	}

	public void reset() {
		percent = 0.0f;
	}
}

class Environment {
	private GameObject drone;
	private Motor r1, r2, b1, b2;
	private Observation observation;
	private Vector3 start_pos, start_rot;

	public Environment(GameObject drone, GameObject r1, GameObject r2, GameObject b1, GameObject b2) {
		this.drone = drone;
		this.r1 = new Motor(r1);
		this.r2 = new Motor(r2);
		this.b1 = new Motor(b1);
		this.b2 = new Motor(b2);
		observation = null;
		start_pos = drone.transform.position;
		start_rot = drone.transform.eulerAngles;
	}

	public void step(Action action) {
		switch (action) {
		case Action.CW1_INC:
			r1.increase ();
			break;
		case Action.CW1_DEC:
			r1.decrease ();
			break;
		case Action.CCW1_INC:
			b1.increase ();
			break;
		case Action.CCW1_DEC:
			b1.decrease ();
			break;
		case Action.CW2_INC:
			r2.increase ();
			break;
		case Action.CW2_DEC:
			r2.decrease ();
			break;
		case Action.CCW2_INC:
			b2.increase ();
			break;
		case Action.CCW2_DEC:
			b2.decrease ();
			break;
		}
	}

	public Observation observe() {
		return observation;
	}

	public void setObservation(Observation observation) {
		this.observation = observation;
	}

	public void setText(Text gyroText, Text accelText) {
		Vector3 gyroVector = new Vector3 (0, 0, 0);
		Vector3 accelVector = new Vector3 (0, 0, 0);
		if (observation != null) {
			gyroVector = observation.nextState.gyro;
			accelVector = observation.nextState.accel;
		}
		gyroText.text = "X = " + gyroVector[2].ToString("F3") + "\nY = " + gyroVector[0].ToString("F3") + "\nZ = " + gyroVector[1].ToString("F3");
		accelText.text = "X = " + accelVector[0].ToString("F3") + "\nY = " + accelVector[1].ToString("F3") + "\nZ = " + accelVector[2].ToString("F3");
	}

	public void reset() {
		drone.transform.position = start_pos;
		drone.transform.eulerAngles = start_rot;
		r1.reset ();
		r2.reset ();
		b1.reset ();
		b2.reset ();
	}
}

public class Controller : MonoBehaviour {
	public GameObject drone;
	public GameObject r1, r2, b1, b2;
	public float max_thrust = 0.6f;
	public Text gyroText, accelText;

	private float step = 0.1f;
	private float[] thrust; // in grams
	private Environment environment;
	static Vector3 lastVelocity;

	void Start () {
		environment = new Environment (drone, r1, r2, b1, b2);
		environment.setText (gyroText, accelText);
		thrust = new float[4];
		lastVelocity = new Vector3 (0, 0, 0);
	}

	void Update () {
		Update (Input.GetKey (KeyCode.A), r1, 0);
		Update (Input.GetKey (KeyCode.S), b1, 1);
		Update (Input.GetKey (KeyCode.D), r2, 2);
		Update (Input.GetKey (KeyCode.F), b2, 3);
	}

	void LateUpdate() {
		Rigidbody rigidbody = drone.GetComponent<Rigidbody> ();
		Vector3 acceleration = (rigidbody.velocity - lastVelocity) / Time.fixedDeltaTime;
		lastVelocity = rigidbody.velocity;
		Observation observation = new Observation (
			new Reward(0),
			new State(transform.eulerAngles, acceleration),
			false
		);
		// make an observation from the enviroment
		environment.setObservation (observation);
		environment.setText (gyroText, accelText);
	}

	void Update(bool f, GameObject obj, int i) {
		// Keyboard Control
		if (f) {
			thrust [i] += step;
			if (thrust [i] > max_thrust)
				thrust [i] = max_thrust;
			Vector3 force = thrust [i] * Physics.gravity;
			force.y *= -1;
			obj.GetComponent<Rigidbody> ().AddForce (
				drone.transform.rotation * force
			);
		} else {
			thrust [i] = thrust [i] / 2 - step;
			if (thrust [i] < 0)
				thrust [i] = 0;
		}
	}

	void reset() {
		for (int i = 0; i < 4; ++i) {
			thrust [i] = 0.0f;
		}
		environment.reset ();
	}
}
