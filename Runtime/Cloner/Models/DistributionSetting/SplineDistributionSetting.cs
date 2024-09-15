using System;
using UnityEngine;
using UnityEngine.Splines;

namespace Misaki.ArtTool
{
    [Serializable]
    public class SplineDistributionSetting
    {
        public SplineContainer spline;

        public int indexOffset;

        public uint count = 10;
        public float spacing = 1.0f;

        public bool isSpacingMode;

        public int DistributionCount
        {
            get
            {
                if (isSpacingMode)
                {
                    return Mathf.RoundToInt(spline.CalculateLength() / spacing) + 1;
                }
                else
                {
                    return (int)count;
                }
            }
        }
    }
}