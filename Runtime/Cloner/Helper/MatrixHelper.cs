using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Misaki.ArtTool
{
    internal static class MatrixHelper
    {
        internal static void DecomposeMatrix(float4x4 matrix, out float3 position, out quaternion rotation, out float3 scale)
        {
            position = matrix.c3.xyz;
            scale = new float3(
                math.length(matrix.c0.xyz),
                math.length(matrix.c1.xyz),
                math.length(matrix.c2.xyz)
            );

            rotation = quaternion.LookRotation(matrix.c2.xyz / scale.z, matrix.c1.xyz / scale.y);
        }

        internal static void DecomposeMatrixToVector(float4x4 matrix, out Vector3 position, out Quaternion rotation, out Vector3 scale)
        {
            position = matrix.c3.xyz;
            scale = new Vector3(
                math.length(matrix.c0.xyz),
                math.length(matrix.c1.xyz),
                math.length(matrix.c2.xyz)
            );

            rotation = Quaternion.LookRotation(matrix.c2.xyz / scale.z, matrix.c1.xyz / scale.y);
        }

        internal static void DecomposeMatrixListAsSpan(in List<Matrix4x4> matrixList, Span<Vector3> positions, Span<Quaternion> rotations, Span<Vector3> scales)
        {
            if (matrixList.Count != positions.Length || matrixList.Count != rotations.Length || matrixList.Count != scales.Length)
            {
                throw new ArgumentException("The length of the spans must match the number of matrices in the list.");
            }

            for (var i = 0; i < matrixList.Count; i++)
            {
                DecomposeMatrixToVector(matrixList[i], out positions[i], out rotations[i], out scales[i]);
            }
        }

        internal static void DecomposeMatrixListAsSpan(ReadOnlySpan<float4x4> matrixList, Span<Vector3> positions, Span<Quaternion> rotations, Span<Vector3> scales)
        {
            if (matrixList.Length != positions.Length || matrixList.Length != rotations.Length || matrixList.Length != scales.Length)
            {
                throw new ArgumentException("The length of the spans must match the number of matrices in the list.");
            }

            for (var i = 0; i < matrixList.Length; i++)
            {
                DecomposeMatrixToVector(matrixList[i], out positions[i], out rotations[i], out scales[i]);
            }
        }

        internal static void DecomposeMatrixListAsSpan(ReadOnlySpan<PointData> matrixList, Span<Vector3> positions, Span<Quaternion> rotations, Span<Vector3> scales)
        {
            if (matrixList.Length != positions.Length || matrixList.Length != rotations.Length || matrixList.Length != scales.Length)
            {
                throw new ArgumentException("The length of the spans must match the number of matrices in the list.");
            }

            for (var i = 0; i < matrixList.Length; i++)
            {
                DecomposeMatrixToVector(matrixList[i].matrix, out positions[i], out rotations[i], out scales[i]);
            }
        }

        internal static float3 GetScale(this float4x4 matrix)
        {
            return new float3(math.length(matrix.c0.xyz), math.length(matrix.c1.xyz), math.length(matrix.c2.xyz));
        }
    }
}
