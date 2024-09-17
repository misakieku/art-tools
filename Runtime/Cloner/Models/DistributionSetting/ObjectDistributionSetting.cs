using System;

namespace Misaki.ArtTool
{
    [Serializable]
    public struct ObjectDistributionSetting
    {
        public MeshData meshData;
        public ObjectDistributionMode mode;
        public int count;
        public uint seed;
        public bool alignNormal;
    }
}