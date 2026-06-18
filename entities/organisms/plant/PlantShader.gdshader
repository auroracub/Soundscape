shader_type spatial;
// render_mode unshaded;

uniform vec3 u_color: source_color = vec3(0.939, 0.0, 0.17);
uniform sampler2D u_noise: hint_default_white;
uniform float u_alpha_scale = 1.0f;
uniform float u_emissive_scale = 1.0f;
instance uniform float u_param_a = 1.0;
instance uniform float u_param_b = 0.0;

float fresnel(float p_amount, vec3 p_normal, vec3 p_view) {
	return pow((1.0 - dot(normalize(p_normal), normalize(p_view))), p_amount);
}

void fragment() {
	vec2 uv = UV + TIME * 0.025;
	float fresnel = fresnel(1.25, NORMAL, VIEW);
	float noise = texture(u_noise, uv).r;
	ALBEDO = u_color + fresnel * u_param_a; // + fresnel * (smoothstep(0.9, 1.0, u_amp));
	// ALPHA = smoothstep(0.0, 0.1, u_amp); // clamp(noise, 0.2, 0.8) + fresnel;
	ALPHA = u_alpha_scale;
	EMISSION = u_color * u_emissive_scale + u_param_b;
}
