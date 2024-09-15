using Unity.Mathematics;
using UnityEngine;

namespace Misaki.ArtTool
{
    public struct Float2ToVector2Converter
    {
        public static Vector2 ConvertTo(float2 value)
        {
            return (Vector2)value;
        }

        public static float2 ConvertBack(Vector2 value)
        {
            return (float2)value;
        }
    }

    public struct Float3ToVector3Converter
    {
        public static Vector3 ConvertTo(float3 value)
        {
            return (Vector3)value;
        }

        public static float3 ConvertBack(Vector3 value)
        {
            return (float3)value;
        }
    }
}
