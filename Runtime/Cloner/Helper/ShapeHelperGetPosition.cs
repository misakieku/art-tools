using Unity.Mathematics;

namespace Misaki.ArtTool
{
    internal static partial class ShapeHelper
    {
        internal static float3 GetCubePosition(int index, int3 size)
        {
            float3 localPosition;
            var yIndex = index / (size.x * size.z);
            var remain = index % (size.x * size.z);
            var zIndex = remain / size.x;
            var xIndex = remain % size.x;

            localPosition = new float3(xIndex, yIndex, zIndex);
            localPosition -= (float3)(size - 1) * 0.5f;
            return localPosition;
        }

        internal static bool GetMeshVertexPosition(int index, MeshData meshData, out float3 position)
        {
            if (!meshData.vertices.IsCreated || meshData.vertices.Length <= index)
            {
                position = float3.zero;
                return false;
            }

            position = meshData.vertices[index];
            return true;
        }

        internal static bool GetMeshEdgePosition(int index, MeshData meshData, out float3 position)
        {
            if (!meshData.edges.IsCreated || meshData.edges.Length <= index)
            {
                position = float3.zero;
                return false;
            }

            var edge = meshData.edges[index];
            if (meshData.vertices.Length <= edge.x || meshData.vertices.Length <= edge.y)
            {
                position = float3.zero;
                return false;
            }

            position = (meshData.vertices[edge.x] + meshData.vertices[edge.y]) / 2.0f;
            return true;
        }
    }
}