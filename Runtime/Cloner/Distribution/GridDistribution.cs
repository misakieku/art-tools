using Unity.Mathematics;

namespace Misaki.ArtTool
{
    public static partial class Distribution
    {
        public static void GridDistribution(int index, GridDistributionSetting setting, out float4x4 localMatrix, out bool isValid)
        {
            var random = Random.CreateFromIndex((uint)index);

            var localPosition = ShapeHelper.GetCubePosition(index, setting.count) * setting.spacing;

            switch (setting.shape)
            {
                case GridShape.Cube:
                    isValid = true;
                    break;

                case GridShape.Sphere:
                    isValid = ShapeHelper.IsPointInsideSphere(localPosition, 0.0f, setting.count * setting.spacing);
                    break;

                case GridShape.Cylinder:
                    isValid = ShapeHelper.IsPointInsideCylinder(localPosition, 0.0f, setting.count * setting.spacing);
                    break;
                default:
                    isValid = false;
                    break;
            }

            if (random.NextFloat() > setting.fill && isValid == true)
            {
                isValid = false;
            }

            localMatrix = float4x4.TRS(localPosition, quaternion.identity, new float3(1.0f));
        }
    }
}