using System;
using Unity.Mathematics;

namespace Misaki.ArtTool
{
    [Serializable]
    public struct LinearDistributionSetting
    {
        public uint count;
        public uint indexOffset;

        public float3 positionSpacing;
        public float3 rotationSpacing;
        public float3 scaleSpacing;

        public float3 stepRotation;
    }
}
