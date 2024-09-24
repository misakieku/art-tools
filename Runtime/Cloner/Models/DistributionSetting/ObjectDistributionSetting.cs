using System;

namespace Misaki.ArtTool
{
    [Serializable]
    public struct ObjectDistributionSetting
    {
        public MeshData meshData;
        public ObjectDistributionMode mode;
        public uint count;
        public uint seed;
        public bool isAlignNormal;

        public int DistributionCount
        {
            get
            {
                var result = 0;
                switch (mode)
                {
                    case ObjectDistributionMode.Surface:
                    case ObjectDistributionMode.Volume:
                        result = (int)count;
                        break;

                    case ObjectDistributionMode.Vertex:
                        result = meshData.vertexCount;
                        break;

                    case ObjectDistributionMode.Edge:
                        result = meshData.edges.Length;
                        break;

                    case ObjectDistributionMode.PolygonCenter:
                        result = meshData.triangles.Length / 3;
                        break;

                    default:
                        break;
                }

                return result;
            }
        }
    }
}