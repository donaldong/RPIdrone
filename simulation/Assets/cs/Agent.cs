using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public enum Action {
	CW1_INC = 0,
	CW1_DEC = 1,
	CW2_INC = 2,
	CW2_DEC = 3,
	CCW1_INC = 4,
	CCW1_DEC = 5,
	CCW2_INC = 6,
	CCW2_DEC = 7,
	NONE = 8
};
	
class ActionValues {
	public static int nA = Enum.GetNames(typeof(Action)).Length;

	private ArrayList values;

	public ActionValues() {
		values = ArrayList.Repeat(0.0, nA);
	}

	public Action bestAction() {
		int action = 0;
		double value = (double) values [0];
		for (int i = 1; i < nA; ++i) {
			if ((double) values [i] > value) {
				value = (double) values [i];
				action = i;
			}
		}
		return (Action) action;
	}

	public double getValue(Action action) {
		return (double) values [(int)action];
	}

	public void setValue(Action action, double value) {
		values [(int)action] = value;
	}
}

class Policy {
	public static int nA = Enum.GetNames(typeof(Action)).Length;
	private static Dictionary<State, ActionValues> Q = new Dictionary<State, ActionValues>();

	public static Action epsilon_greedy_policy(State state, float epsilon) {
		ArrayList probs = ArrayList.Repeat(epsilon / nA, nA);
		ActionValues actionValues;
		if (Q.ContainsKey (state)) { 
			actionValues = Q [state];
		} else {
			actionValues = new ActionValues ();
			Q.Add(state, actionValues); 
		}
		probs [(int)actionValues.bestAction ()] = 1.0 - epsilon + epsilon / nA;
		Action action = randomChoice (probs);
		Debug.Log (action);
		return action;
	}

	public static Action randomChoice(ArrayList p) {
		System.Random r = new System.Random();
		double diceRoll = r.NextDouble();
		double cumulative = 0.0;
		for (int i = 0; i < nA; i++)
		{
			cumulative += Convert.ToDouble(p[i]);
			if (diceRoll < cumulative)
				return (Action) i;
		}
		return Action.NONE;
	}

	public static double getQ(State state, Action action) {
		if (Q.ContainsKey(state)) { 
			return Q [state].getValue (action);
		}
		else {
			Q.Add(state, new ActionValues ());
			return 0;
		}
	}

	public static void setQ(State state, Action action, double value) {
		if (Q.ContainsKey(state)) { 
			Q [state].setValue (action, value);
		} else {
			Q.Add(state, new ActionValues ());
			Q[state].setValue (action, value);
		}
	}
}
