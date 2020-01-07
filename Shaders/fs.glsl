#version 330
 
in vec2 v_TexCoord;
in vec4 v_Normal;
in vec4 v_FragPos;
uniform sampler2D s_texture;
uniform vec4 lightPos;

layout(location = 0) out vec4 Color;
 
const int levels = 8;
const float scaleFactor = 1.0/levels;

void main()
{
	vec3 lightColor = vec3(0.85f,0.95f,1f);
	vec4 lightAmbient = vec4(0.1, 0.1, 0.1, 1.0);
	//vec3 lightPos = vec3(0,20,0);

	
	vec4 lightDir = normalize(lightPos - v_FragPos); 
	float diff = max(dot(v_Normal, lightDir), 0.0);
	vec3 diffuse = (floor(diff * levels) * scaleFactor) * lightColor;

	vec4 preColour = lightAmbient + (texture2D(s_texture, v_TexCoord) * vec4(diffuse,0));
	//preColour = (floor(preColour * levels) * scaleFactor); //posterise
    Color = preColour;
}