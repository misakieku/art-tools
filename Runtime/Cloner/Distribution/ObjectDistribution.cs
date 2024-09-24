using Unity.Mathematics;

namespace Misaki.ArtTool
{
    public static partial class Distribution
    {
        public static void ObjectDistribution(int index, ref ObjectDistributionSetting setting, out float4x4 localMatrix, out bool isValid)
        {
            isValid = false;
            localMatrix = float4x4.identity;

            switch (setting.mode)
            {
                case ObjectDistributionMode.Surface:
                    isValid = ShapeHelper.TryGetMatrixOnMeshSurface(index, setting.seed, setting.isAlignNormal, ref setting.meshData, out localMatrix);
                    break;

                case ObjectDistributionMode.Volume:
                    isValid = ShapeHelper.TryGetPositionInMeshVolume(index, setting.seed, ref setting.meshData, out localMatrix);
                    break;

                case ObjectDistributionMode.Vertex:
                    isValid = ShapeHelper.TryGetMeshVertexMatrix(index, setting.isAlignNormal, ref setting.meshData, out localMatrix);
                    break;

                case ObjectDistributionMode.Edge:
                    isValid = ShapeHelper.TryGetMeshEdgeMatrix(index, setting.isAlignNormal, ref setting.meshData, out localMatrix);
                    break;

                case ObjectDistributionMode.PolygonCenter:
                    isValid = ShapeHelper.TryGetMeshPolygonMatrix(index, setting.isAlignNormal, ref setting.meshData, out localMatrix);
                    break;

                default:
                    break;
            }
        }
    }
}