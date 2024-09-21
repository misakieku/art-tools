using Unity.Mathematics;

namespace Misaki.ArtTool
{
    internal static partial class ShapeHelper
    {
        internal static float3 GetCubePosition(int index, float3 size)
        {
            float3 localPosition;
            var yIndex = index / (size.x * size.z);
            var remain = index % (size.x * size.z);
            var zIndex = remain / size.x;
            var xIndex = remain % size.x;

            localPosition = new float3(xIndex, yIndex, zIndex);
            localPosition -= (size - 1) * 0.5f;
            return localPosition;
        }

        internal static bool GetMeshVertexPosition(int index, ref MeshData meshData, out float3 position)
        {
            position = float3.zero;

            if (!meshData.vertices.IsCreated || meshData.vertices.Length <= index)
            {
                return false;
            }

            var meshScale = meshData.worldMatrix.GetScale();
            var meshPosition = meshData.worldMatrix.c3.xyz;

            if (index >= meshData.vertices.Length)
            {
                return false;
            }

            position = meshData.vertices[index] * meshScale + meshPosition;
            return true;
        }

        internal static bool GetMeshEdgePosition(int index, ref MeshData meshData, out float3 position, out float3 forwardDirection, out float3 upDirection)
        {
            position = float3.zero;
            forwardDirection = new float3(0.0f, 0.0f, 1.0f);
            upDirection = new float3(0.0f, 1.0f, 0.0f);

            if (!meshData.edges.IsCreated || index >= meshData.edges.Length)
            {
                return false;
            }

            var edge = meshData.edges[index];
            if (edge.x >= meshData.vertices.Length || edge.y >= meshData.vertices.Length)
            {
                return false;
            }

            var meshScale = meshData.worldMatrix.GetScale();
            var meshPosition = meshData.worldMatrix.c3.xyz;

            var a = meshData.vertices[edge.x] * meshScale + meshPosition;
            var b = meshData.vertices[edge.y] * meshScale + meshPosition;

            position = (a + b) / 2.0f;
            forwardDirection = a - b;

            var interpNormal = math.normalize(meshData.normals[edge.x] + meshData.normals[edge.y]);
            upDirection = math.normalize(math.cross(interpNormal, new float3(1.0f, 0.0f, 0.0f)));

            if (math.all(upDirection <= float3.zero))
            {
                upDirection = math.normalize(math.cross(interpNormal, new float3(0.0f, 0.0f, 1.0f)));
            }

            return true;
        }

        internal static bool GetMeshPolygonPosition(int index, ref MeshData meshData, out float3 position)
        {
            position = float3.zero;

            if (!meshData.edges.IsCreated || meshData.edges.Length <= index)
            {
                return false;
            }

            var meshScale = meshData.worldMatrix.GetScale();
            var meshPosition = meshData.worldMatrix.c3.xyz;

            var triangleIndex = index * 3;

            if (triangleIndex >= meshData.triangles.Length - 2)
            {
                return false;
            }

            var pointIndexA = meshData.triangles[triangleIndex];
            var pointIndexB = meshData.triangles[triangleIndex + 1];
            var pointIndexC = meshData.triangles[triangleIndex + 2];

            var a = meshData.vertices[pointIndexA] * meshScale + meshPosition;
            var b = meshData.vertices[pointIndexB] * meshScale + meshPosition;
            var c = meshData.vertices[pointIndexC] * meshScale + meshPosition;

            position = (a + b + c) / 3.0f;
            return true;
        }
    }
}