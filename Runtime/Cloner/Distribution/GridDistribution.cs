using Unity.Mathematics;

namespace Misaki.ArtTool
{
    public static partial class Distribution
    {
        public static void GridDistribution(int index, GridDistributionSetting setting, out float4x4 localMatrix, out bool isValid)
        {
            var random = Random.CreateFromIndex((uint)index);

            var localPosition = GetCubePosition(index, setting.count) * setting.spacing;

            switch (setting.shape)
            {
                case GridShape.Cube:
                    isValid = true;
                    break;

                case GridShape.Sphere:
                    var isInsideSphere = ShapeHelper.IsInsideSphere(localPosition, 0.0f, setting.count * setting.spacing);

                    isValid = isInsideSphere;
                    break;

                case GridShape.Cylinder:
                    var isInsideCylinder = ShapeHelper.IsInsideCylinder(localPosition, 0.0f, setting.count * setting.spacing);
                    isValid = isInsideCylinder;
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

        private static float3 GetCubePosition(int index, int3 size)
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
    }
}
