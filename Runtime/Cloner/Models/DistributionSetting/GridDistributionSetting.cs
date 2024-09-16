using System;
using Unity.Mathematics;
using UnityEngine;

namespace Misaki.ArtTool
{
    [Serializable]
    public struct GridDistributionSetting
    {
        public int3 count;
        public float3 spacing;
        public GridShape shape;
        [Range(0.0f, 1.0f)]
        public float fill;

        public readonly int DistributionCount => count.x * count.y * count.z;
    }
}
