using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Misaki.ArtTool
{
    [Serializable]
    public struct SplineDistributionSetting
    {
        public SplineContainer spline;

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
                if (spline == null)
                {
                    return 0;
                }

                if (isSpacingMode)
                {
                    return Mathf.FloorToInt(spline.CalculateLength() / spacing) + 1;
                }
                else
                {
                    return (int)count;
                }
            }
        }
    }
}