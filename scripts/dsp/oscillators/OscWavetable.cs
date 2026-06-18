using Godot;
using System;

[GlobalClass]
public partial class OscWavetable : OscBase
{
	public Mod position_param = new Mod(0.0f);
	private const int NUM_WAVES = 4;
	private const int TABLE_SIZE = 512;
	private float[,] _tables = new float[NUM_WAVES, TABLE_SIZE];

	public OscWavetable()
	{
		for (int i = 0; i < TABLE_SIZE; i++)
		{
			float t = (float)i / TABLE_SIZE;
			_tables[0, i] = MathF.Sin(t * MathF.Tau);
			_tables[1, i] = t < 0.5f ? t * 2.0f - 1.0f : 1.0f - (t - 0.5f) * 2.0f;
			_tables[2, i] = t * 2.0f - 1.0f;
			_tables[3, i] = t < 0.5f ? -1.0f : 1.0f;
		}
	}
	
	public override Mod get_mod_from_name(string p_mod_name)
	{
		switch(p_mod_name.ToLower())
		{
			case "freq":
			case "frequency":
				return frequency_param;
			case "amp":
			case "amplitude":
				return amplitude_param;
			case "morph":
				return morph_param;
			case "position":
			case "pos":
				return position_param;
			default:
				GD.PrintErr($"[AudioModule Error] No mod named '{p_mod_name}' found in '{this.GetType().Name}'");
				return null;
		}
	}
	
	// 'Horizontal' morph not implemented, only 'vertical' position
	protected override void update_state(float p_sample_rate)
	{
		float frequency = frequency_param.evaluate();
		float amplitude = amplitude_param.evaluate();
		
		advance_phase(p_sample_rate, frequency);

		float lookup_pos = Mathf.PosMod(phase / MathF.Tau, 1.0f);
		float table_index_raw = lookup_pos * TABLE_SIZE;
		int index_a = (int)table_index_raw % TABLE_SIZE;
		int index_b = (index_a + 1) % TABLE_SIZE;
		float frac_x = table_index_raw - (int)table_index_raw;
		
		float morph = Mathf.Clamp(position_param.evaluate(), 0.0f, 1.0f);

		float wave_index_raw = morph * (NUM_WAVES - 1);
		int wave_a = (int)wave_index_raw;
		int wave_b = Mathf.Min(wave_a + 1, NUM_WAVES - 1);
		float frac_y = wave_index_raw - wave_a;

		float sample_wave_a = _tables[wave_a, index_a] + (_tables[wave_a, index_b] - _tables[wave_a, index_a]) * frac_x;
		float sample_wave_b = _tables[wave_b, index_a] + (_tables[wave_b, index_b] - _tables[wave_b, index_a]) * frac_x;

		cached_mono_sample = (sample_wave_a + (sample_wave_b - sample_wave_a) * frac_y) * amplitude;
		cached_stereo_sample.X = cached_mono_sample;
		cached_stereo_sample.Y = cached_mono_sample;
	}
	
	// Blehh
	public override void set_value(string p_mod_name, float p_value)
		=> base.set_value(p_mod_name, p_value);
	public override void set_patch(AudioModule p_target_module, string p_target_mod_name)
		=> base.set_patch(p_target_module, p_target_mod_name);
	public override void remove_patch(AudioModule p_target_module, string p_target_mod_name)
		=> base.remove_patch(p_target_module, p_target_mod_name);
	public override void map_add(AudioModule p_target_module, string p_target_mod_name, 
		float p_target_min = -1.0f, float p_target_max = 1.0f)
		=> base.map_add(p_target_module, p_target_mod_name, p_target_min, p_target_max);
	public override void map_add_default(AudioModule p_target_module, string p_target_mod_name)
		=> base.map_add_default(p_target_module, p_target_mod_name);
	public override void map_multiply(AudioModule p_target_module, string p_target_mod_name, 
		float p_target_min = -1.0f, float p_target_max = 1.0f)
		=> base.map_multiply(p_target_module, p_target_mod_name, p_target_min, p_target_max);
	public override void map_multiply_default(AudioModule p_target_module, string p_target_mod_name)
		=> base.map_multiply_default(p_target_module, p_target_mod_name);
}
