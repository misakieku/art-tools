using Unity.Mathematics;
using UnityEngine.Splines;

namespace Misaki.ArtTool
{
    public static partial class Distribution
    {
        public static void SplineDistribution(int index, int pointSize, float splineLength, float4x4 splineWorldMatrix, SplineDistributionSetting setting, out float4x4 localMatrix, out bool isValid)
        {
            var pointIndex = index + setting.indexOffset;


            if (pointIndex > pointSize)
            {
                localMatrix = float4x4.zero;
                isValid = false;

                return;
            }

            var spline = setting.spline;
            float t;

            if (setting.isSpacingMode)
            {
                t = (pointIndex * setting.spacing) / splineLength;
            }
            else
            {
                t = pointIndex / (float)(pointSize - 1);
            }

            if (SplineUtility.Evaluate(spline.Spline, t, out var position, out var normal, out var upVector))
            {
                var localRotation = quaternion.LookRotationSafe(normal, upVector);

                localMatrix = math.mul(splineWorldMatrix, float4x4.TRS(position, localRotation, new float3(1.0f)));
                isValid = true;
            }
            else
            {
                localMatrix = float4x4.zero;
                isValid = false;
            }
        }
    }
}