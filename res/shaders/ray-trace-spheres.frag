#version 460

#define FLT_MAX 3.402823466e+38
#define PI 3.14159265359


struct Sun{
    mat3 rotation;
    vec3 direction;
};

struct Camera {
    mat4 transform;
    float focal;
};

struct Vertex {
    int position;
    int normal;
    int uv;
    int bones;
    int weights;
};

struct Triangle {
    int vertexA;
    int vertexB;
    int vertexC;
};

struct Mesh {
    int index;
    int count;
    mat4 matrix;
};

struct Sphere {
    vec4 transform;
    // xyz = position
    // w = radius
    
    vec4 color;
};

struct SphereHitInfo {
    int index;
    float distance0;
    float distance1;
};

uniform vec2 resolution;

layout(std140, binding = 0) uniform cameraUBO {
    Camera camera;
};

layout(std140, binding = 1) uniform sunUBO {
    Sun sun;
};

layout(std430, binding = 0) buffer positionsSSBO {
    vec3 positions[];
};

layout(std430, binding = 1) buffer normalsSSBO {
    vec3 normals[];
};

layout(std430, binding = 2) buffer uvsSSBO {
    vec2 uvs[];
};

layout(std430, binding = 3) buffer bonesSSBO {
    int bones[];
};

layout(std430, binding = 4) buffer weightsSSBO {
    int weights[];
};

layout(std430, binding = 5) buffer triangleSSBO{
    Triangle triangles[];
};

layout(std430, binding = 6) buffer meshSSBO {
    Mesh meshes[];
};

layout(std430, binding = 7) buffer sphereSSBO {
    Sphere spheres[];
};

in vec2 v_UV;

out vec4 o_Color;



SphereHitInfo castRayAtSpheres(vec3 origin, vec3 direction, int ignoreIndex = -1) {
    
    int closestIndex = -1;
    float closestT0 = FLT_MAX;
    float closestT1 = FLT_MAX;
   
    
    for(int i = 0; i < spheres.length(); i++) {
        
        if (i == ignoreIndex) continue;
        
        Sphere sphere = spheres[i];
    
        float a = dot(direction, direction);
        float b = 2 * dot(origin - sphere.transform.xyz, direction);
        float c = dot(origin - sphere.transform.xyz, origin - sphere.transform.xyz) - sphere.transform.w * sphere.transform.w;
        
        float discriminant = b * b - 4.0 * a * c;
           
        if (discriminant < 0) continue;
        
        float d = sqrt(discriminant);
        float t0 = (-b - d) / (2.0 * a); 
        
        if (t0 >= 0 && t0 < closestT0) {      
            closestIndex = i;
            closestT0 = t0;
            closestT1 = discriminant == 1 ? t0 : (-b + d) / (2.0 * a);     
        }    
    }
    
    if (closestIndex == -1) return SphereHitInfo(closestIndex, closestT0, closestT1);
    
    return SphereHitInfo(closestIndex, closestT0, closestT1);
    
    
}

vec4 over(vec4 src, vec4 dst) {   
    float alpha = src.a + dst.a * (1.0 - src.a);  
    return vec4(
        (src.rgb * src.a + dst.rgb * dst.a * (1.0 - src.a)) / alpha,
        alpha
    );
    
    //return src.a * src + (1.0 - src.a) * dst;
}

vec4 sphereColor(SphereHitInfo hitInfo) {
    //float halfC = (hitInfo.distance1 - hitInfo.distance0) / 2.0;
    Sphere sphere = spheres[hitInfo.index];
    
    return sphere.color;
    
    // return vec4(
    //     sphere.color.rgb,
    //     1.0 - sqrt(sphere.transform.w * sphere.transform.w - halfC * halfC) / sphere.transform.w
    // );    
}

void main() {
    
    vec2 aspectRatio = vec2(
        max(resolution.x / resolution.y, 1), 
        max(resolution.y / resolution.x, 1));
        
    vec4 bgColor = vec4(0.0, 0.0, 0.0, 1.0);
        
        
    vec2 pixel = (v_UV - 0.5) * 2.0;   
        
    mat3 rotation = mat3(camera.transform[0].xyz,camera.transform[1].xyz,camera.transform[2].xyz);

    vec3 rayOrigin = camera.transform[3].xyz;
    vec3 rayDirection = vec3(pixel * aspectRatio, 1) * rotation;
    
    SphereHitInfo hitInfo = castRayAtSpheres(rayOrigin, rayDirection);
    
    if (hitInfo.index < 0 ) {
        o_Color = bgColor;
        return;
    }
    
    vec4 color = sphereColor(hitInfo);
    
    while (hitInfo.index >= 0 && color.a < 1.0) {
        
        rayOrigin = rayOrigin + rayDirection * (hitInfo.distance0 + 0.00001);
        
        hitInfo = castRayAtSpheres(rayOrigin, rayDirection, hitInfo.index);
        
        if (hitInfo.index <= -1) break;
        
        vec4 under = sphereColor(hitInfo);
        
        color = over(color, under);
    }
    
    o_Color = over(color, bgColor);

}
