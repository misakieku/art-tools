using Unity.Mathematics;

namespace Misaki.ArtTool
{
    public static partial class Distribution
    {
        public static void RadialDistribution(int index, RadialDistributionSetting setting, out float4x4 localMatrix, out bool isValid)
        {
            localMatrix = ShapeHelper.GetRadialMatrix(index, setting);
            isValid = true;
        }
    }
}