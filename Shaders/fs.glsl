#version 330
in vec2 v_TexCoord;
in vec4 v_Normal;
in vec4 v_FragPos;
uniform sampler2D s_texture;

layout(location = 0) out vec4 Color;
 
const int levels = 8;
const float scaleFactor = 1.0/levels;


struct Light{
	vec4 Position;
	vec3 Colour;
};
uniform Light[8] lights;
uniform int lightCount;
void main()
{
	vec4 lightAmbient = vec4(0.1, 0.1, 0.1, 1.0);
	Color = vec4(0,0,0,1);
	for(int i = 0; i < 5; i++) //Loop through lights
	{

		vec4 lightDir = normalize(lights[i].Position - v_FragPos);

		float diff = max(dot(v_Normal, lightDir), 0.0);
		diff = diff;
		vec3 diffuse = (floor(diff * levels) * scaleFactor) * lights[i].Colour;
		
		float distance = length(lights[i].Position - v_FragPos);
		float attenuation = clamp(10.0 / distance, 0.0, 1.0);

		vec4 preColour = lightAmbient + (texture2D(s_texture, v_TexCoord) * vec4(diffuse,0));
		//preColour = (floor(preColour * levels) * scaleFactor); //posterise
	    Color += attenuation * preColour;
	}
	

}