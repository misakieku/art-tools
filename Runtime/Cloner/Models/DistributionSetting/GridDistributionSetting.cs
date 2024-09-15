using System;
using Unity.Mathematics;
using UnityEngine;

namespace Misaki.ArtTool
{
    [Serializable]
    public class GridDistributionSetting
    {
        public int3 count = new(3, 3, 3);
        public float3 spacing = new(1.0f, 1.0f, 1.0f);
        public GridShape shape;
        [Range(0.0f, 1.0f)]
        public float fill = 1.0f;

        public int DistributionCount => count.x * count.y * count.z;
    }
}
