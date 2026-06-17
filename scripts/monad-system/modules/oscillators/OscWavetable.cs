using Godot;
using System;

[GlobalClass]
public partial class OscWavetable : OscBase
{
	public Mod morph_param = new Mod(0.0f);
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
			_tables[3, i] = t < 0.5f ? -0.5f : 0.5f;
		}
		rebuild_signal_chain();
	}
	
	public override Mod get_mod_from_name(string p_mod_name)
	{
		switch (p_mod_name.ToLower())
		{
			case "freq":
			case "frequency":
				return frequency_param;
			case "morph":
			case "position":
			case "pos":
				return morph_param;
			default:
				GD.PrintErr($"[AudioModule Error] No mod named '{p_mod_name}' found in '{this.GetType().Name}'");
				return null;
		}
	}
	
	public override void patch_in(string p_parameter_name, Func<float> p_mod_provider)
		=> base.patch_in(p_parameter_name, p_mod_provider);
	
	public override AudioModule patch_out(AudioModule p_target_module, string p_target_param, float p_source_min = 0.0f, float p_source_max = 1.0f, float p_target_min = -1.0f, float p_target_max = 1.0f)
		=> base.patch_out(p_target_module, p_target_param, p_source_min, p_source_max, p_target_min, p_target_max);
	
	protected override void update_state()
	{
		if (last_ticked_frame == AudioClock.current_sample_index) return;
		last_ticked_frame = AudioClock.current_sample_index;

		float frequency = frequency_param.evaluate();
		advance_phase(frequency);

		float lookup_pos = Mathf.PosMod(phase / MathF.Tau, 1.0f);
		float table_index_raw = lookup_pos * TABLE_SIZE;
		int index_a = (int)table_index_raw % TABLE_SIZE;
		int index_b = (index_a + 1) % TABLE_SIZE;
		float frac_x = table_index_raw - (int)table_index_raw;

		float pos = Mathf.Clamp(morph_param.evaluate(), 0.0f, 1.0f);
		float wave_index_raw = pos * (NUM_WAVES - 1);
		int wave_a = (int)wave_index_raw;
		int wave_b = Mathf.Min(wave_a + 1, NUM_WAVES - 1);
		float frac_y = wave_index_raw - wave_a;

		float sample_wave_a = _tables[wave_a, index_a] + (_tables[wave_a, index_b] - _tables[wave_a, index_a]) * frac_x;
		float sample_wave_b = _tables[wave_b, index_a] + (_tables[wave_b, index_b] - _tables[wave_b, index_a]) * frac_x;

		_cached_mod_sample = sample_wave_a + (sample_wave_b - sample_wave_a) * frac_y;
		_cached_audio_left_sample = _cached_mod_sample;
		_cached_audio_right_sample = _cached_mod_sample;
	}
}
