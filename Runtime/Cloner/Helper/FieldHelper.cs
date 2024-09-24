using Unity.Mathematics;

namespace Misaki.ArtTool
{
    internal class FieldHelper
    {
        internal static float BlendField(float a, float b, float t, BlendingMode blendingMode)
        {
            var result = 0.0f;
            switch (blendingMode)
            {
                case BlendingMode.Normal:
                    result = math.lerp(a, b, t);
                    break;

                case BlendingMode.Min:
                    result = math.lerp(a, math.min(a, b), t);
                    break;

                case BlendingMode.Subtract:
                    result = math.lerp(a, a - b, t);
                    break;

                case BlendingMode.Multiply:
                    result = math.lerp(a, a * b, t);
                    break;

                case BlendingMode.Overlay:
                    var o1 = 1.0f - 2.0f * (1.0f - a) * (1.0f - b);
                    var o2 = 2.0f * a * b;
                    var zeroOrOne = math.step(a, 0.5f);
                    result = o2 * zeroOrOne + (1.0f - zeroOrOne) * o1;
                    result = math.lerp(a, result, t);
                    break;

                case BlendingMode.Max:
                    result = math.lerp(a, math.max(a, b), t);
                    break;

                case BlendingMode.Add:
                    result = math.lerp(a, a + b, t);
                    break;

                case BlendingMode.Screen:
                    result = 1.0f - (1.0f - b) * (1.0f - a);
                    result = math.lerp(a, result, t);
                    break;

                default:
                    break;
            }

            return result;
        }

        internal static float ApplyRemapping(float weight, ref RemappingSetting remappingSetting)
        {
            if (!remappingSetting.enable)
            {
                return weight;
            }

            weight = math.saturate(weight / (1.0f - remappingSetting.innerOffset));
            weight = math.lerp(remappingSetting.min, remappingSetting.max, weight);
            weight = remappingSetting.invert ? 1.0f - weight : weight;
            weight *= remappingSetting.strength;
            return weight;
        }
    }
}
