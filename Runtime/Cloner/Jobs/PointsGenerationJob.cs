using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Misaki.ArtTool
{
    [ExecuteInEditMode]
    public struct PointsGenerationJob : IJobParallelForBatch
    {
        public float4x4 worldMatrix;

        public DistributionMode distributionMode;

        public GridDistributionSetting gridDistributionSetting;

        [WriteOnly]
        public NativeArray<float4x4> points;

        public void Execute(int startIndex, int count)
        {
            switch (distributionMode)
            {
                case DistributionMode.Object:
                    break;
                case DistributionMode.Linear:
                    break;
                case DistributionMode.Grid:
                    GridDistribution(startIndex, count);
                    break;
                case DistributionMode.Radial:
                    break;
                case DistributionMode.Honeycomb:
                    break;
                default:
                    break;
            }
        }

        private void GridDistribution(int startIndex, int count)
        {
            var xIndex = 0;
            var yIndex = 0;
            var zIndex = 0;

            switch (gridDistributionSetting.shape)
            {
                case GridShape.Cube:

                    for (var i = startIndex; i < startIndex + count; i++)
                    {
                        yIndex = i / (gridDistributionSetting.count.x * gridDistributionSetting.count.z);
                        var remain = i % (gridDistributionSetting.count.x * gridDistributionSetting.count.z);
                        zIndex = remain / gridDistributionSetting.count.x;
                        xIndex = remain % gridDistributionSetting.count.x;

                        var localPosition = new float3(xIndex * gridDistributionSetting.spacing.x, yIndex * gridDistributionSetting.spacing.y, zIndex * gridDistributionSetting.spacing.z);

                        localPosition.x -= (gridDistributionSetting.count.x - 1) * gridDistributionSetting.spacing.x / 2.0f;
                        localPosition.y -= (gridDistributionSetting.count.y - 1) * gridDistributionSetting.spacing.y / 2.0f;
                        localPosition.z -= (gridDistributionSetting.count.z - 1) * gridDistributionSetting.spacing.z / 2.0f;

                        var localMatrix = float4x4.TRS(localPosition, quaternion.identity, new float3(1.0f));

                        points[i] = math.mul(worldMatrix, localMatrix);
                    }

                    break;
                case GridShape.Sphere:
                    break;
                case GridShape.Cylinder:
                    break;
                default:
                    break;
            }
        }
    }
}
