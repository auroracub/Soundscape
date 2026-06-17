using Godot;
using System;

public partial class BaseSignal<T> : RefCounted
{
	public Func<T> evaluate { get; protected set; }
	public BaseSignal() { evaluate = () => default; }
	public BaseSignal(Func<T> p_evaluate) { evaluate = p_evaluate; }
}
