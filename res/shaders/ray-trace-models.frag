#version 460

#define EPSILON 1e-6
#define FLT_MAX 3.402823466e+38
#define PI 3.14159265359

struct Sun{
    //mat3 rotation;
    vec3 direction;
};

struct Camera {
    mat4 transform;
    float focal;
};

struct Vertex {
    int positionIndex;
    int normalIndex;
    int uvIndex;
};

struct Triangle {
    int vertexIndexA;
    int vertexIndexB;
    int vertexIndexC;
    bool cullBackface;
};

struct Mesh {
    int offset;
    int count;
};

struct Model {
    Mesh mesh;   
};

struct Ray {
    vec3 origin;
    vec3 direction;
};

struct HitInfo {
    bool didHit;
    float hitDistance;
    vec3 hitPosition;
    vec3 normal;
    vec3 barycentric;
};

uniform vec2 resolution;
uniform int modelCount;

layout(std140, binding = 0) uniform cameraUBO {
    Camera camera;
};

layout(std140, binding = 1) uniform sunUBO {
    Sun sun;
};

layout (std430, binding = 0) buffer PositionSSBO {
    vec4 positions[];
};

layout (std430, binding = 1) buffer NormalSSBO {
    vec4 normals[];
};

layout (std430, binding = 2) buffer UVSSBO {
    vec2 uvs[];
};

layout (std430, binding = 3) buffer VertexSSBO {
    Vertex vertices[];
};

layout (std430, binding = 4) buffer TriangleSSBO {
    Triangle triangles[];
};

layout (std430, binding = 5) buffer ModelSSBO {
    Mesh models[];
};

layout (std430, binding = 6) buffer MatrixSSBO {
    mat4 matrices[];
};

in vec2 v_UV;

out vec4 o_Color;

HitInfo rayTriangle(Ray ray, Triangle tri, mat4 matrix) {
    
    
    
    // vec3 a = vec3(0.0, 1.0, 0.0);
    // vec3 b = vec3(1.0, -0.5, 0.0);
    // vec3 c = vec3(-1.0, -0.5, 0.0);
    
    HitInfo hitInfo;
    hitInfo.didHit = false;
    
    vec3 a = (positions[vertices[tri.vertexIndexA].positionIndex] * matrix).xyz;
    vec3 b = (positions[vertices[tri.vertexIndexB].positionIndex] * matrix).xyz;
    vec3 c = (positions[vertices[tri.vertexIndexC].positionIndex] * matrix).xyz;
    
    vec3 e1 = b - a;
    vec3 e2 = c - a;
    
    hitInfo.normal = cross(e1, e2);
    
    float det = -dot(ray.direction, hitInfo.normal);
    
    if (tri.cullBackface && det <= 0.0) return hitInfo;
    
    float invDet = 1.0 / det; 
    vec3 ao = ray.origin - a; 
    
    hitInfo.hitDistance = dot(ao, hitInfo.normal) * invDet;
    
    if (hitInfo.hitDistance <= 0.0) return hitInfo;
    
    vec3 dao = cross(ao, ray.direction);
    
    float v = -dot(e1, dao) * invDet;
    float u = dot(e2, dao) * invDet;
    float w = 1.0 - u - v;
     
    hitInfo.didHit = (u >= 0.0 && v >= 0.0 && w >= 0.0);
    hitInfo.hitPosition = ray.origin + ray.direction * hitInfo.hitDistance;
    hitInfo.barycentric = vec3(w, u, v);
    
    return hitInfo;
     
}

HitInfo castRay(Ray ray, out int modelIndex, out int triangleIndex) {
      
    HitInfo closestHitInfo;
    closestHitInfo.hitDistance = FLT_MAX;
    //float closestHitDistance = FLT_MAX;
    
    for(int i = 0; i < modelCount; i++) {
        Mesh model = models[i];
        
        int offset = model.offset;
        int count = model.count;
        
        for(int j = offset; j < offset + count; j++) {
            
            Triangle triangle = triangles[j];
                  
            HitInfo hitInfo = rayTriangle(ray, triangle, matrices[i]);
            
            if (hitInfo.didHit && hitInfo.hitDistance < closestHitInfo.hitDistance) {
                closestHitInfo = hitInfo;  
                modelIndex = i;
                triangleIndex = j;     
            }                   
        }     
    }
    
    return closestHitInfo;
}

vec2 interpolateBarycentric(vec2 a, vec2 b, vec2 c, vec3 barycentric) {
    return a * barycentric.x + b * barycentric.y + c * barycentric.z;
}

vec3 interpolateBarycentric(vec3 a, vec3 b, vec3 c, vec3 barycentric) {
    return a * barycentric.x + b * barycentric.y + c * barycentric.z;
}

vec4 interpolateBarycentric(vec4 a, vec4 b, vec4 c, vec3 barycentric) {
    return a * barycentric.x + b * barycentric.y + c * barycentric.z;
}


void main() {
    
    vec2 aspectRatio = vec2(
        max(resolution.x / resolution.y, 1), 
        max(resolution.y / resolution.x, 1));
        
    vec4 bgColor = vec4(0.1, 0.1, 0.1, 1.0);
    vec2 pixel = (v_UV - 0.5) * 2.0;
    
    mat3 rotation = mat3(camera.transform[0].xyz,camera.transform[1].xyz,camera.transform[2].xyz);
    Ray cameraRay = Ray(camera.transform[3].xyz, normalize(vec3(pixel * aspectRatio, camera.focal)) * rotation);
     
    int modelIndex;
    int triangleIndex;
    
    HitInfo hitInfo = castRay(cameraRay, modelIndex, triangleIndex);
        
    if (!hitInfo.didHit) {
        o_Color = bgColor;
        
        return;
    }
    
    Triangle triangle = triangles[triangleIndex];
    
    vec4 normalA = normals[vertices[triangle.vertexIndexA].normalIndex];
    vec4 normalB = normals[vertices[triangle.vertexIndexB].normalIndex];
    vec4 normalC = normals[vertices[triangle.vertexIndexC].normalIndex];
    
    vec4 normal = interpolateBarycentric(normalA, normalB, normalC, hitInfo.barycentric) * matrices[modelIndex];
    
    //vec3 sunDir = vec3(0,0,1) * sun.rotation;
    
    o_Color = vec4(vec3(dot(-cameraRay.direction, normalize(normal.xyz)) * 0.5 + 0.5), 1.0);
    

}