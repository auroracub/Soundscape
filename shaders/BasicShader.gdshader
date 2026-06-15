shader_type spatial;
//render_mode unshaded;

uniform vec3 u_color: source_color = vec3(0.939, 0.0, 0.17);
uniform float u_emissive_scale = 1.0;
uniform float u_alpha = 1.0;

void fragment() {
	ALBEDO = u_color;
	EMISSION = u_color * u_emissive_scale;
	ALPHA = u_alpha;
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
