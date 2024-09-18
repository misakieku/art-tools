using System;
using Unity.Mathematics;
using UnityEngine;

namespace Misaki.ArtTool
{
    [ExecuteInEditMode]
    public class PlainEffector : EffectorBase
    {
        public TransformSpace transformSpace;

        public bool isEnablePosition;
        public float3 position;

        public bool isEnableRotation;
        public float3 rotation;

        public bool isEnableScale;
        public bool isAbsoluteScale;
        public bool isUniformScale;
        public float3 scale;
        public float uniformScale;

        public override PointData Operate(int index, float4x4 nodeWorldMatrix, ReadOnlySpan<PointData> points)
        {
            var currentPoint = points[index];
            if (!isEnablePosition && !isEnableRotation && !isEnableScale)
            {
                return currentPoint;
            }


            MatrixHelper.DecomposeMatrix(currentPoint.matrix, out var position, out var rotation, out var scale);

            var weight = CalculateFieldsWeight(position);
            if (weight == 0)
            {
                return currentPoint;
            }

            if (isEnablePosition)
            {
                var newPosition = position;
                switch (transformSpace)
                {
                    case TransformSpace.Node:
                        newPosition += this.position;
                        break;
                    case TransformSpace.Effector:
                        newPosition += math.mul(effectorMatrix, new float4(this.position, 0)).xyz;
                        break;
                    case TransformSpace.Object:
                        newPosition += math.mul(currentPoint.matrix, new float4(this.position, 0)).xyz;
                        break;
                    default:
                        break;
                }
                position = math.lerp(position, newPosition, weight);
            }

            if (isEnableRotation)
            {
                var newRotation = rotation;
                switch (transformSpace)
                {
                    case TransformSpace.Node:
                        newRotation = math.mul(rotation, quaternion.EulerXYZ(math.radians(this.rotation)));
                        break;
                    case TransformSpace.Effector:
                        newRotation = math.mul(rotation,
                            quaternion.EulerXYZ(math.mul(effectorMatrix, new float4(math.radians(this.rotation), 0)).xyz));
                        break;
                    case TransformSpace.Object:
                        newRotation = math.mul(rotation,
                            quaternion.EulerXYZ(math.mul(currentPoint.matrix, new float4(math.radians(this.rotation), 0)).xyz));
                        break;
                    default:
                        break;
                }
                rotation = Quaternion.Lerp(rotation, newRotation, weight);
            }

            if (isEnableScale)
            {
                var newScale = scale;
                if (isUniformScale)
                {
                    newScale = isAbsoluteScale ? uniformScale : scale - uniformScale;
                }
                else
                {
                    newScale = isAbsoluteScale ? this.scale : scale - this.scale;
                }
                scale = math.lerp(scale, newScale, weight);
            }

            currentPoint.matrix = float4x4.TRS(position, rotation, scale);

            return currentPoint;
        }
    }
}