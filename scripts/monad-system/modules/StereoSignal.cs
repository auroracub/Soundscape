using Godot;
using System;

[GlobalClass]
public partial class StereoSignal : BaseSignal<Vector2>
{
	public StereoSignal() { evaluate = () => Vector2.Zero; }
	public StereoSignal(Func<Vector2> p_evaluate) { evaluate = p_evaluate; }
}
