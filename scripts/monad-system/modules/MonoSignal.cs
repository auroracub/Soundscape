using Godot;
using System;

public partial class MonoSignal : BaseSignal<float>
{
	// public Func<float> evaluate { get; private set; }
	public MonoSignal() { evaluate = () => 0.0f; }
	public MonoSignal(Func<float> p_evaluate) { evaluate = p_evaluate; }
}
