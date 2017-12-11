using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Reward {
	public float value;

	public Reward(float value) {
		this.value = value;
	}

	public Reward(Vector3 diff, Vector3 accel) {
		this.value = -diff.magnitude * 2 -  accel.magnitude;
	}

	public float getValue() {
		return value;
	}
}

class State : IEquatable<State> {
	public Vector3 gyro;
	public Vector3 accel;

	public State(Vector3 gyro, Vector3 accel) {
		this.gyro = gyro;
		this.accel = accel;
	}

	public int getHashGyro() {
		int res = Convert.ToInt32(gyro.x / 6.0f);
		res <<= 6;
		res += Convert.ToInt32(gyro.y / 6.0f);
		res <<= 6;
		res += Convert.ToInt32(gyro.z / 6.0f);
		return res;
	}

	public int getHashAccel() {
		int res = Convert.ToInt32(accel.x / 7.0f);
		res <<= 4;
		res += Convert.ToInt32(accel.y / 7.0f);
		res <<= 4;
		res += Convert.ToInt32(accel.z / 7.0f);
		return res;
	}

	public override int GetHashCode() {
		int gyroHash = getHashGyro ();
		return gyroHash << 18 + getHashAccel();
	}

	public override bool Equals(object obj) {
		return Equals(obj as State);
	}

	public bool Equals(State state) {
		return state != null && state.GetHashCode() == this.GetHashCode();
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
		obj.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		obj.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
	}
}

class Environment {
	private GameObject drone;
	private Motor r1, r2, b1, b2;
	private State state;
	private Observation observation;
	private Vector3 start_pos, start_rot;
	private Action current, last;
	public static float MAXHEIGHT;
	public static float CUTOFF;
	public bool end_cut = false;

	public Environment(GameObject drone, GameObject r1, GameObject r2, GameObject b1, GameObject b2) {
		this.drone = drone;
		this.r1 = new Motor(r1);
		this.r2 = new Motor(r2);
		this.b1 = new Motor(b1);
		this.b2 = new Motor(b2);
		observation = new Observation (
			new Reward(0),
			new State (start_rot, Vector3.zero),
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
		drone.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		drone.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
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

	public static Vector3 getDiff(Transform t) {
		return t.rotation * Vector3.up - Vector3.up;
	}

	public bool end() {
		var diff = getDiff (drone.transform);
		if (diff.magnitude > CUTOFF) {
			end_cut = true;
			return true;
		}
		end_cut = false;
		if (drone.transform.position.y >= MAXHEIGHT)
			return true;
		return false;
	}
}

public class Controller : MonoBehaviour {
	public GameObject drone;
	public GameObject r1, r2, b1, b2;
	public float max_thrust = 0.6f;
    public float learningRate = 0.5f;
    public float epsilon = 0.2f;
    public float discountFactor = 0.9f;
	public int maxEpisodeLength = 1000000;
	public float maxHeight = 50.0f;
	public float cutOff = 0.5f;
	public float cutOffPenalty = 100.0f;
	public Text gyroText, accelText, episodeText;
	private int step_count = 0;
	private int episode = 1;
	private float step = 0.1f;
	private float[] thrust; // in grams
	private Environment environment;
	static Vector3 lastVelocity;

	void Start () {
		Motor.MAX_THRUST = max_thrust;
		Environment.MAXHEIGHT = maxHeight;
		Environment.CUTOFF = cutOff;
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
		if (Input.GetKeyDown (KeyCode.R) || environment.end () || step_count > maxEpisodeLength) {
			reset ();
			episode += 1;
			step_count = 0;
		}
		environment.step (Policy.epsilon_greedy_policy (
			environment.observe().state,
			epsilon
		));
	}

	void LateUpdate() {
		Rigidbody rigidbody = drone.GetComponent<Rigidbody> ();
		Vector3 acceleration = (rigidbody.velocity - lastVelocity) / Time.fixedDeltaTime;
		lastVelocity = rigidbody.velocity;
		Observation observation;
		if (!environment.end_cut) {
			observation = new Observation (
				new Reward (Environment.getDiff (transform), acceleration),
				new State (transform.eulerAngles, acceleration),
				false);
		} else {
			observation = new Observation (
				new Reward (-cutOffPenalty),
				new State (transform.eulerAngles, acceleration),
				false);
		}
		// make an observation from the enviroment
		Observation prev = environment.observe();
		double td_delta = Policy.getQ(prev.state, environment.getLastAction ());
		environment.setObservation (observation);
		double value = Policy.getQ(observation.state, environment.getAction ());
		double td_target = observation.reward.getValue () + discountFactor * value;
		td_delta = td_target - td_delta;
		Policy.setQ(observation.state, environment.getAction (), learningRate * td_delta);
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
		setText ();
	}

	public void setText() {
		Vector3 gyroVector = new Vector3 (0, 0, 0);
		Vector3 accelVector = new Vector3 (0, 0, 0);
		gyroVector = environment.observe().state.gyro;
		accelVector = environment.observe().state.accel;
		gyroText.text = "X = " + gyroVector[2].ToString("F3") + "\nY = " + gyroVector[0].ToString("F3") + "\nZ = " + gyroVector[1].ToString("F3");
		accelText.text = "X = " + accelVector[0].ToString("F3") + "\nY = " + accelVector[1].ToString("F3") + "\nZ = " + accelVector[2].ToString("F3");
		episodeText.text = episode.ToString();
	}

	void reset() {
		for (int i = 0; i < 4; ++i) {
			thrust [i] = 0.0f;
		}
		environment.reset ();
	}
}
