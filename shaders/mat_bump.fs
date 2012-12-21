precision mediump float;

varying vec3 normout;
varying vec2 bumpcoordout;
varying mat4 normrot;
varying vec3 lightVector;
varying float dist;

uniform sampler2D tex0; // NB bump texture is the first texture loaded, not second as with UV shading
uniform vec3 unib[2];
//uniform float ntiles ===> unib[0][0]
//uniform float shiny ====> unib[0][1]
//uniform vec4 material ==> unib[1]
uniform vec3 unif[11];
//uniform vec3 fogshade ==> unif[4]
//uniform float fogdist ==> unif[5][0]
//uniform float fogalpha => unif[5][1]

void main(void) {
  vec4 texc = vec4(unib[1], 1.0); // ------ basic colour from material vector

  // ------ look up normal map value as a vector where each colour goes from -100% to +100% over its range so
  // ------ 0xFF7F7F is pointing right and 0X007F7F is pointing left. This vector is then rotated relative to the rotation
  // ------ of the normal at that vertex.
  vec3 bump = vec3(normrot * vec4(normalize(texture2D(tex0, bumpcoordout)).rgb * 2.0 - vec3(1.0, 1.0, 1.0), 0.0));
  float bfact = 1.0 - smoothstep(50.0, 150.0, dist); // ------ attenuate smoothly between 20 and 75 units

  float ffact = smoothstep(unif[5][0]/3.0, unif[5][0], dist); // ------ smoothly increase fog between 1/3 and full fogdist

  float intensity = max(dot(lightVector, normout + bump * bfact), 0.1); // ------ adjustment of colour according to combined normal
  if (texc.a < unib[0][2]) discard; // ------ to allow rendering behind the transparent parts of this object
  texc.rgb = texc.rgb * intensity;

  gl_FragColor =  (1.0 - ffact) * texc + ffact * vec4(unif[4], unif[5][1]); // ------ combine using factors
}

