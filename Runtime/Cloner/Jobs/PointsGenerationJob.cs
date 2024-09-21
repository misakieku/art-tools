using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Misaki.ArtTool
{
    [BurstCompile]
    public struct PointsGenerationJob : IJobParallelForBatch
    {
        public float4x4 worldMatrix;
        public int pointSize;

        public DistributionMode distributionMode;

        public ObjectDistributionSetting objectDistributionSetting;
        public SplineDistributionSetting splineDistributionSetting;
        public LinearDistributionSetting linearDistributionSetting;
        public GridDistributionSetting gridDistributionSetting;

        [WriteOnly]
        public NativeArray<PointData> points;

        public void Execute(int startIndex, int count)
        {
            for (var i = startIndex; i < startIndex + count; i++)
            {
                var pointMatrix = float4x4.identity;
                var isValid = true;
                switch (distributionMode)
                {
                    case DistributionMode.Object:
                        Distribution.ObjectDistribution(i, objectDistributionSetting, out pointMatrix, out isValid);
                        break;
                    case DistributionMode.Spline:
                        Distribution.SplineDistribution(i, pointSize, splineDistributionSetting, out pointMatrix, out isValid);
                        break;
                    case DistributionMode.Linear:
                        Distribution.LinearDistribution(i, linearDistributionSetting, out pointMatrix, out isValid);
                        break;
                    case DistributionMode.Grid:
                        Distribution.GridDistribution(i, gridDistributionSetting, out pointMatrix, out isValid);
                        break;
                    case DistributionMode.Radial:
                        break;
                    case DistributionMode.Honeycomb:
                        break;
                    default:
                        break;
                }

                pointMatrix = math.mul(worldMatrix, pointMatrix);

                points[i] = new PointData()
                {
                    matrix = pointMatrix,
                    isValid = isValid
                };
            }

        }

    }
}
