using Godot;
using System;

[GlobalClass]
public partial class StereoSignal : BaseSignal<Vector2>
{
	public StereoSignal() { evaluate = () => new Vector2(0.0f, 0.0f); }
	public StereoSignal(Func<Vector2> p_evaluate) { evaluate = p_evaluate; }
}
