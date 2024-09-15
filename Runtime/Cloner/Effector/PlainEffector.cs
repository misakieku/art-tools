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

        public override void Operate(int index, float4x4 nodeWorldMatrix, ReadOnlySpan<PointData> points, ref float4x4 pointWorldMatrix, ref bool isValid)
        {
            if (!isEnablePosition && !isEnableRotation && !isEnableScale)
            {
                return;
            }

            MatrixHelper.DecomposeMatrix(pointWorldMatrix, out var position, out var rotation, out var scale);

            var weight = CalculateFieldsWeight(pointWorldMatrix.c3.xyz);
            if (weight == 0)
            {
                return;
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
                        newPosition += math.mul(pointWorldMatrix, new float4(this.position, 0)).xyz;
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
                            quaternion.EulerXYZ(math.mul(pointWorldMatrix, new float4(math.radians(this.rotation), 0)).xyz));
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

            pointWorldMatrix = float4x4.TRS(position, rotation, scale);
        }
    }
}