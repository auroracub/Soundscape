using System;
using Godot;

namespace Soundscape.Libraries;

[GlobalClass]
public partial class ColorLib : Godot.GodotObject
{
	private static readonly Vector3 LWeights = new Vector3(0.299f, 0.587f, 0.114f);

	public static Color HclToRgb(float h, float c, float l)
	{
		if (c <= 0.00001f) return new Color(l, l, l);

		float hPrime = h / 60.0f;
		float x = c * (1.0f - Math.Abs((hPrime % 2.0f) - 1.0f));

		float r1 = 0.0f, g1 = 0.0f, b1 = 0.0f;
		int segment = (int)Math.Floor(hPrime);

		switch (segment)
		{
			case 0: r1 = c; g1 = x; break;
			case 1: r1 = x; g1 = c; break;
			case 2: g1 = c; b1 = x; break;
			case 3: g1 = x; b1 = c; break;
			case 4: r1 = x; b1 = c; break;
			default: r1 = c; b1 = x; break;
		}

		float m = l - ((r1 * LWeights.X) + (g1 * LWeights.Y) + (b1 * LWeights.Z));

		return new Color(
			Math.Clamp(r1 + m, 0.0f, 1.0f),
			Math.Clamp(g1 + m, 0.0f, 1.0f),
			Math.Clamp(b1 + m, 0.0f, 1.0f)
		);
	}
}
