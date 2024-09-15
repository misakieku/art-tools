using System;
using Unity.Mathematics;

namespace Misaki.ArtTool
{
    public class PushApartEffector : EffectorBase
    {
        public float radius = 1.0f;
        public uint iteration = 10;

        // TODO: Average the push direction and distance of each point for more consistence result
        public override void Operate(int index, float4x4 nodeWorldMatrix, ReadOnlySpan<PointData> points, ref float4x4 pointWorldMatrix, ref bool isValid)
        {
            var currentPoint = points[index];

            var weight = CalculateFieldsWeight(pointWorldMatrix.c3.xyz);
            if (weight == 0)
            {
                return;
            }

            for (var i = 0; i < iteration; i++)
            {
                for (var p = 0; p < points.Length; p++)
                {
                    var targetPoint = points[p];

                    if (ReferenceEquals(currentPoint, targetPoint))
                    {
                        continue;
                    }

                    var distance = math.distance(currentPoint.matrix.c3.xyz, targetPoint.matrix.c3.xyz);
                    if (distance < radius)
                    {
                        var direction = math.normalizesafe(currentPoint.matrix.c3.xyz - targetPoint.matrix.c3.xyz);
                        pointWorldMatrix.c3.xyz += distance * weight * direction;
                    }
                }
            }
        }
    }
}