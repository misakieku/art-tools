using Unity.Mathematics;
using UnityEngine;

namespace Misaki.ArtTool
{
    public struct Int2ToVector2IntConverter
    {
        public static Vector2Int ConvertTo(int2 value)
        {
            return new Vector2Int(value.x, value.y);
        }

        public static int2 ConvertBack(Vector2Int value)
        {
            return new int2(value.x, value.y);
        }
    }

    public struct Int3ToVector3IntConverter
    {
        public static Vector3Int ConvertTo(int3 value)
        {
            return new Vector3Int(value.x, value.y, value.z);
        }

        public static int3 ConvertBack(Vector3Int value)
        {
            return new int3(value.x, value.y, value.z);
        }
    }
}