using Unity.Mathematics;

namespace Misaki.ArtTool
{
    public class Clamp : FieldBase
    {
        public float2 minMax;

        public override float Operate(float3 position, float weight)
        {
            return math.clamp(weight, minMax.x, minMax.y);
        }
    }
}