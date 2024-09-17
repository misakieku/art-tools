using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Misaki.ArtTool
{
    [Serializable]
    public struct SplineDistributionSetting
    {
        public NativeSpline nativeSpline;

        public int indexOffset;

        public uint count;
        public float spacing;

        public bool isSpacingMode;

        [HideInInspector]
        public float4x4 splineWorldMatrix;
        [HideInInspector]
        public float splineLength;

        public int DistributionCount
        {
            get
            {
                if (isSpacingMode)
                {
                    return Mathf.FloorToInt(splineLength / spacing) + 1;
                }
                else
                {
                    return (int)count;
                }
            }
        }
    }
}