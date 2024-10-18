#version 460

#define UINT_MAX 4294967295.0

layout(binding = 0) uniform sampler2D color_tex;
layout(binding = 1) uniform sampler2D depth_tex;
layout(binding = 2) uniform isampler2D object_tex;

uniform int textureID;

in vec2 v_UV;

out vec4 o_color;

uint hash(int seed) {
    uint x = uint(seed);
    x ^= x >> 16;
    x *= 0x7feb352dU;
    x ^= x >> 15;
    x *= 0x846ca68bU;
    x ^= x >> 16;
    return x;
}

vec4 randomVec4(int seed) {
    seed *= 3;
    uint r = hash(seed);
    uint g = hash(seed + 1);
    uint b = hash(seed + 2);
    
    return vec4(float(r) / UINT_MAX, float(g) / UINT_MAX, float(b) / UINT_MAX, 1.0);
}

void main() {
    
    switch(textureID) {
        case 0:
            o_color = texture(color_tex, v_UV);
            break;
        case 1:
            o_color = texture(depth_tex, v_UV) * 0.05;
            break;
        case 2:
            int id = texture(object_tex, v_UV).r;            
            o_color = randomVec4(id);
            break;
    } 
}

