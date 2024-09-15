using System;
using UnityEngine;

namespace Misaki.ArtTool
{
    [Serializable]
    public struct ObjectDistributionSetting
    {
        public MeshFilter meshFilter;
        public int count;
        public bool alignNormal;
    }
}
