using System;
using Unity.Mathematics;

namespace Misaki.ArtTool
{
    [Serializable]
    public struct PointData
    {
        public bool isValid;
        public float4x4 matrix;
    }
}