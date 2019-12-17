#version 330
 
in vec2 v_TexCoord;
in vec3 v_Normal;
in vec3 v_FragPos;

uniform sampler2D s_texture;
uniform vec3 eye_position;

layout(location = 0) out vec4 Color;
 
const int levels = 6;
const float scaleFactor = 1.0/levels;

void main()
{
	vec3 lightColor = vec3(1,1,1);
	vec4 lightAmbient = vec4(0.1, 0.1, 0.1, 1.0);
	vec3 lightPos = vec3(0,20,0);

	vec3 norm = normalize(v_Normal);
	vec3 lightDir = normalize(lightPos - v_FragPos); 
	float diff = max(dot(norm, lightDir), 0.0);
	vec3 diffuse = (floor(diff * levels) * scaleFactor) * lightColor;

	vec4 preColour = lightAmbient + (texture2D(s_texture, v_TexCoord) * vec4(diffuse,0));
	//preColour = (floor(preColour * levels) * scaleFactor); //posterise
    Color = preColour;
}