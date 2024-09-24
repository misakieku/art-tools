using System;
using Unity.Mathematics;

namespace Misaki.ArtTool
{
    [Serializable]
    public struct RadialDistributionSetting
    {
        public uint count;
        public float radius;
        public PlaneDirection plane;
        public float2 angleMinMax;
        public bool align;
    }
}