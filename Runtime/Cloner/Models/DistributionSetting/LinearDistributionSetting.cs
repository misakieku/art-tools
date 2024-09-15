using System;
using Unity.Mathematics;

namespace Misaki.ArtTool
{
    [Serializable]
    public class LinearDistributionSetting
    {
        public uint count = 10;
        public uint indexOffset = 0;

        public float3 positionSpacing = new(0.0f, 1.0f, 0.0f);
        public float3 rotationSpacing = float3.zero;
        public float3 scaleSpacing = new(1.0f, 1.0f, 1.0f);

        public float3 stepRotation = float3.zero;
    }
}
