using Unity.Mathematics;

namespace Misaki.ArtTool
{
    internal static partial class ShapeHelper
    {
        internal static float Linear01DistanceToSphereCenter(float3 pointPosition, float3 spherePosition, float3 sphereSize)
        {
            var x = (pointPosition.x - spherePosition.x) / sphereSize.x;
            var y = (pointPosition.y - spherePosition.y) / sphereSize.y;
            var z = (pointPosition.z - spherePosition.z) / sphereSize.z;

            var normalizedDistance = math.sqrt(x * x + y * y + z * z);

            return math.saturate(normalizedDistance);
        }

        internal static bool IsPointInsideSphere(float3 pointPosition, float3 spherePosition, float3 sphereSize)
        {
            sphereSize /= 2.0f;

            var x = (pointPosition.x - spherePosition.x) / sphereSize.x;
            var y = (pointPosition.y - spherePosition.y) / sphereSize.y;
            var z = (pointPosition.z - spherePosition.z) / sphereSize.z;

            return (x * x + y * y + z * z) <= 1.0f;
        }

        internal static bool IsPointInsideCylinder(float3 pointPosition, float3 cylinderPosition, float3 cylinderSize)
        {
            cylinderSize /= 2.0f;

            var dx = (pointPosition.x - cylinderPosition.x) / cylinderSize.x;
            var dz = (pointPosition.z - cylinderPosition.z) / cylinderSize.z;
            var distanceSquared = dx * dx + dz * dz;

            var withinRadius = distanceSquared <= 1.0f;

            var withinHeight = pointPosition.y >= (cylinderPosition.y - cylinderSize.y) && pointPosition.y <= (cylinderPosition.y + cylinderSize.y);

            return withinRadius && withinHeight;
        }

        internal static bool IsPointInsideMesh(float3 pointPosition, ref MeshData meshData)
        {
            var windingNumber = 0;
            var meshScale = meshData.worldMatrix.GetScale();
            var meshPosition = meshData.worldMatrix.GetPosition();

            for (var i = 0; i < meshData.triangles.Length; i += 3)
            {
                var v1 = meshData.vertices[meshData.triangles[i]] * meshScale + meshPosition;
                var v2 = meshData.vertices[meshData.triangles[i + 1]] * meshScale + meshPosition;
                var v3 = meshData.vertices[meshData.triangles[i + 2]] * meshScale + meshPosition;

                if (IsPointInsideTriangle(pointPosition, v1, v2, v3))
                {
                    windingNumber++;
                }
            }

            return windingNumber % 2 == 0;
        }

        //TODO: Fix it
        private static bool IsPointInsideTriangle(float3 point, float3 a, float3 b, float3 c)
        {
            var v0 = c - a;
            var v1 = b - a;
            var v2 = point - a;

            var dot00 = math.dot(v0, v0);
            var dot01 = math.dot(v0, v1);
            var dot02 = math.dot(v0, v2);
            var dot11 = math.dot(v1, v1);
            var dot12 = math.dot(v1, v2);

            var invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
            var u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            var v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            return (u >= 0) && (v >= 0) && (u + v < 1);
        }
    }
}