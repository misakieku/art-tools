using System;
using Unity.Mathematics;

namespace Misaki.ArtTool
{
    [Serializable]
    public struct PointData : IEquatable<PointData>
    {
        public bool isValid;
        public float4x4 matrix;

        public bool Equals(PointData other)
        {
            return isValid == other.isValid && Equals(matrix, other.matrix);
        }

        public override bool Equals(object obj)
        {
            if (obj is PointData other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(isValid, matrix);
        }

        public static bool operator ==(PointData left, PointData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PointData left, PointData right)
        {
            return !(left == right);
        }
    }
}