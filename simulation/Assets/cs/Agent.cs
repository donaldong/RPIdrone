using System;

class Action {
	public enum Type {
		CW1_INC,
		CW1_DEC,
		CW2_INC,
		CW2_DEC,
		CCW1_INC,
		CCW1_DEC,
		CCW2_INC,
		CCW2_DEC,
		NONE
	};
	public float probability;
	public Type type;
}

public class Agent
{
	public Agent ()
	{
	}
}