using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace Misaki.ArtTool
{
    public static partial class Distribution
    {
        public static void ObjectDistribution(int index, ObjectDistributionSetting setting, out float4x4 localMatrix, out bool isValid)
        {
            var random = new Random();
            if (index > uint.MaxValue - setting.seed)
            {
                random = Random.CreateFromIndex(setting.seed - (uint)index);
            }
            else
            {
                random = Random.CreateFromIndex(setting.seed + (uint)index);
            }

            var position = float3.zero;
            var forwardDirection = new float3(0.0f, 0.0f, 1.0f);
            var upDirection = new float3(0.0f, 1.0f, 0.0f);
            isValid = false;

            switch (setting.mode)
            {
                case ObjectDistributionMode.Surface:
                    break;
                case ObjectDistributionMode.Volume:

                    var meshScale = setting.meshData.worldMatrix.GetScale();
                    var meshPosition = setting.meshData.worldMatrix.c3.xyz;

                    position = random.NextFloat3(-setting.meshData.bounds.extents * meshScale + meshPosition, setting.meshData.bounds.extents * meshScale + meshPosition);

                    var isInsideMesh = ShapeHelper.IsPointInsideMesh(position, ref setting.meshData);
                    while (!isInsideMesh)
                    {
                        position = random.NextFloat3(-setting.meshData.bounds.extents * meshScale + meshPosition, setting.meshData.bounds.extents * meshScale + meshPosition);
                        isInsideMesh = ShapeHelper.IsPointInsideMesh(position, ref setting.meshData);
                    }
                    isValid = true;
                    break;

                case ObjectDistributionMode.Vertex:
                    if (ShapeHelper.GetMeshVertexPosition(index, ref setting.meshData, out var vertexPosition))
                    {
                        position = vertexPosition;
                        isValid = true;
                    }
                    break;

                case ObjectDistributionMode.Edge:
                    if (ShapeHelper.GetMeshEdgePosition(index, ref setting.meshData, out var edgePosition, out var edgeForward, out var edgeUp))
                    {
                        position = edgePosition;
                        forwardDirection = edgeForward;
                        upDirection = edgeUp;
                        isValid = true;
                    }
                    break;

                case ObjectDistributionMode.PolygonCenter:
                    if (ShapeHelper.GetMeshPolygonPosition(index, ref setting.meshData, out var polygonPosition))
                    {
                        position = polygonPosition;
                        isValid = true;
                    }
                    break;

                default:
                    break;
            }

            var rotation = quaternion.LookRotationSafe(forwardDirection, upDirection);

            localMatrix = float4x4.TRS(position, rotation, new float3(1.0f));
        }
    }
}