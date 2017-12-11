using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Reward {
	public float value;

	public Reward(float value) {
		this.value = value;
	}

	public Reward(Vector3 eulerAngle) {
		var up = new Vector3 (0, 1, 0);
		var diff = up - eulerAngle;
		this.value = -diff.magnitude;
	}

	public float getValue() {
		return value;
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
	public State state;
	public bool done;

	public Observation(Reward reward, State state, bool done) {
		this.reward = reward;
		this.state = state;
		this.done = done;
	}
}

class Motor {
	public static float MAX_THRUST;
	private GameObject obj;
	private float percent;

	public Motor(GameObject obj) {
		this.obj = obj;
		percent = 1.0f;
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

	public void addForce() {
		Vector3 force = percent * MAX_THRUST * Physics.gravity;
		force.y *= -1;
		obj.GetComponent<Rigidbody> ().AddForce (
			obj.transform.rotation * force);
	}

	public float getThrust() {
		return MAX_THRUST * percent;
	}

	public void reset() {
		percent = 1.0f;
		obj.GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, 0);
	}
}

class Environment {
	private GameObject drone;
	private Motor r1, r2, b1, b2;
	private State state;
	private Observation observation;
	private Vector3 start_pos, start_rot;
	private Action current, last;

	public Environment(GameObject drone, GameObject r1, GameObject r2, GameObject b1, GameObject b2) {
		this.drone = drone;
		this.r1 = new Motor(r1);
		this.r2 = new Motor(r2);
		this.b1 = new Motor(b1);
		this.b2 = new Motor(b2);
		observation = new Observation (
			new Reward(0),
			new State (start_rot, new Vector3 (0, 0, 0)),
			false
		);
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
		last = current;
		current = action;
		r1.addForce ();
		r2.addForce ();
		b1.addForce ();
		b2.addForce ();
	}

	public Observation observe() {
		return observation;
	}

	public void setObservation(Observation observation) {
		this.observation = observation;
	}

	public void reset() {
		drone.transform.position = start_pos;
		drone.transform.eulerAngles = start_rot;
		drone.GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, 0);
		r1.reset ();
		r2.reset ();
		b1.reset ();
		b2.reset ();
	}

	public Action getAction() {
		return current;
	}

	public Action getLastAction() {
		return last;
	}
}

public class Controller : MonoBehaviour {
	public GameObject drone;
	public GameObject r1, r2, b1, b2;
	public float max_thrust = 0.6f;
    public static float learningRate = 0.5f;
    public static float epsilon = 0.2f;
    public static float discountFactor = 0.9f;
	public Text gyroText, accelText;

	private float step = 0.1f;
	private float[] thrust; // in grams
	private Environment environment;
	static Vector3 lastVelocity;

	void Start () {
		Motor.MAX_THRUST = max_thrust;
		environment = new Environment (drone, r1, r2, b1, b2);
		setText ();
		thrust = new float[4];
		lastVelocity = new Vector3 (0, 0, 0);
	}

	void Update () {
		Update (Input.GetKey (KeyCode.A), r1, 0);
		Update (Input.GetKey (KeyCode.S), b1, 1);
		Update (Input.GetKey (KeyCode.D), r2, 2);
		Update (Input.GetKey (KeyCode.F), b2, 3);
		if (Input.GetKeyDown (KeyCode.R))
			reset ();
		environment.step (Policy.epsilon_greedy_policy (
			environment.observe().state,
			epsilon
		));
	}

	void LateUpdate() {
		Rigidbody rigidbody = drone.GetComponent<Rigidbody> ();
		Vector3 acceleration = (rigidbody.velocity - lastVelocity) / Time.fixedDeltaTime;
		lastVelocity = rigidbody.velocity;
		Observation observation = new Observation (
			new Reward(transform.eulerAngles),
			new State(transform.eulerAngles, acceleration),
			false
		);
		// make an observation from the enviroment
		Observation prev = environment.observe();
		double td_delta = Policy.getQ(prev.state, environment.getLastAction ());
		environment.setObservation (observation);
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

    public void setText() {
        Vector3 gyroVector = new Vector3 (0, 0, 0);
        Vector3 accelVector = new Vector3 (0, 0, 0);
		gyroVector = environment.observe().state.gyro;
		accelVector = environment.observe().state.accel;
        gyroText.text = "Gyro: X = " + gyroVector[2].ToString("F3") + "; Y = " + gyroVector[0].ToString("F3") + "; Z = " + gyroVector[1].ToString("F3");
        accelText.text = "Accel: X = " + accelVector[0].ToString("F3") + "; Y = " + accelVector[1].ToString("F3") + "; Z = " + accelVector[2].ToString("F3");
    }

	void reset() {
		for (int i = 0; i < 4; ++i) {
			thrust [i] = 0.0f;
		}
		environment.reset ();
	}
}
