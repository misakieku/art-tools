using System;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Misaki.ArtTool
{
    public class RandomEffector : EffectorBase
    {
        public float2 minMax = new(-1.0f, 1.0f);

        public bool synchronized;
        public uint seed = 123456;

        public TransformSpace transformSpace;

        public bool isEnablePosition;
        public float3 positionMinMax;

        public bool isEnableRotation;
        public float3 rotationMinMax;

        public bool isEnableScale;
        public bool isAbsoluteScale;
        public bool isUniformScale;
        public float3 scaleMinMax;
        public float uniformScaleMinMax;

        public override void Operate(int index, float4x4 nodeWorldMatrix, Span<PointData> points)
        {
            if (!isEnablePosition && !isEnableRotation && !isEnableScale)
            {
                return;
            }

            var currentPoint = points[index];

            Random random;
            if (synchronized)
            {
                random = Random.CreateFromIndex(seed);
            }
            else
            {
                if (index > uint.MaxValue - seed)
                {
                    random = Random.CreateFromIndex(seed - (uint)index);
                }
                else
                {
                    random = Random.CreateFromIndex(seed + (uint)index);
                }
            }

            MatrixHelper.DecomposeMatrix(currentPoint.matrix, out var position, out var rotation, out var scale);

            var weight = CalculateFieldsWeight(position);
            if (weight == 0)
            {
                return;
            }

            if (isEnablePosition)
            {
                var newPosition = random.NextFloat3(positionMinMax * minMax.x, positionMinMax * minMax.y);

                switch (transformSpace)
                {
                    case TransformSpace.Effector:
                        newPosition = math.mul(effectorMatrix, new float4(newPosition, 0.0f)).xyz;
                        break;
                    case TransformSpace.Object:
                        newPosition = math.mul(currentPoint.matrix, new float4(newPosition, 0.0f)).xyz;
                        break;
                    default:
                        break;
                }
                position = math.lerp(position, position + newPosition, weight);
            }

            if (isEnableRotation)
            {
                var angle = random.NextFloat3(rotationMinMax * minMax.x, rotationMinMax * minMax.y);

                switch (transformSpace)
                {
                    case TransformSpace.Effector:
                        angle = math.mul(effectorMatrix, new float4(angle, 0.0f)).xyz;
                        break;
                    case TransformSpace.Object:
                        angle = math.mul(currentPoint.matrix, new float4(angle, 0.0f)).xyz;
                        break;
                    default:
                        break;
                }

                var newRotation = quaternion.Euler(math.radians(angle));
                rotation = Quaternion.Lerp(rotation, math.mul(rotation, newRotation), weight);
            }

            if (isEnableScale)
            {
                float3 newScale;
                if (isUniformScale)
                {
                    var minScale = isAbsoluteScale ? uniformScaleMinMax : 1.0f + (uniformScaleMinMax - 1.0f) * minMax.x;
                    var maxScale = isAbsoluteScale ? 0.0f : 1.0f + (uniformScaleMinMax - 1.0f) * minMax.y;

                    newScale = random.NextFloat(minScale, maxScale);
                }
                else
                {
                    var minScale = isAbsoluteScale ? scaleMinMax : 1.0f + ((scaleMinMax - 1.0f) * minMax.x);
                    var maxScale = isAbsoluteScale ? 0.0f : 1.0f + ((scaleMinMax - 1.0f) * minMax.y);

                    newScale = random.NextFloat3(minScale, maxScale);
                }

                scale = math.lerp(scale, scale + newScale, weight);
            }

            currentPoint.matrix = float4x4.TRS(position, rotation, scale);

            points[index] = currentPoint;
        }
    }
}