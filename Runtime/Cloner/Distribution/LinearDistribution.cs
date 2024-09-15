using Unity.Mathematics;

namespace Misaki.ArtTool
{
    public static partial class Distribution
    {
        public static void LinearDistribution(int index, LinearDistributionSetting setting, out float4x4 localMatrix, out bool isValid)
        {
            var pointIndex = (uint)index + setting.indexOffset;
            var localPosition = pointIndex * setting.positionSpacing;
            var localEulerRotation = pointIndex * setting.rotationSpacing;
            var localScale = 1.0f - (pointIndex * (1.0f - setting.scaleSpacing));

            localPosition = math.mul(float3x3.EulerXYZ(math.radians(setting.stepRotation * pointIndex)), localPosition);
            var localRotation = math.mul(quaternion.EulerXYZ(math.radians(localEulerRotation)), quaternion.EulerXYZ(math.radians(setting.stepRotation * pointIndex)));

            localMatrix = float4x4.TRS(localPosition, localRotation, localScale);
            isValid = true;
        }
    }
}