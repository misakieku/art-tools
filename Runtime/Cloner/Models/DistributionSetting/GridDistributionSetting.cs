using System;
using Unity.Mathematics;

namespace Misaki.ArtTool
{
    [Serializable]
    public struct GridDistributionSetting
    {
        public int3 count;
        public float3 spacing;
        public GridShape shape;
        public float fill;

        public readonly int DistributionCount => count.x * count.y * count.z;
    }
}
