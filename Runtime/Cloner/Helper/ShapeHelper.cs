using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Misaki.ArtTool
{
    internal static class ShapeHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float Linear01DistanceToSphereCenter(float3 pointPosition, float3 spherePosition, float3 sphereSize)
        {
            var x = (pointPosition.x - spherePosition.x) / sphereSize.x;
            var y = (pointPosition.y - spherePosition.y) / sphereSize.y;
            var z = (pointPosition.z - spherePosition.z) / sphereSize.z;

            var normalizedDistance = math.sqrt(x * x + y * y + z * z);

            return math.saturate(normalizedDistance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsInsideSphere(float3 pointPosition, float3 spherePosition, float3 sphereSize)
        {
            sphereSize /= 2.0f;

            var x = (pointPosition.x - spherePosition.x) / sphereSize.x;
            var y = (pointPosition.y - spherePosition.y) / sphereSize.y;
            var z = (pointPosition.z - spherePosition.z) / sphereSize.z;

            return (x * x + y * y + z * z) <= 1.0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsInsideCylinder(float3 pointPosition, float3 cylinderPosition, float3 cylinderSize)
        {
            cylinderSize /= 2.0f;

            var dx = (pointPosition.x - cylinderPosition.x) / cylinderSize.x;
            var dz = (pointPosition.z - cylinderPosition.z) / cylinderSize.z;
            var distanceSquared = dx * dx + dz * dz;

            var withinRadius = distanceSquared <= 1.0f;

            var withinHeight = pointPosition.y >= (cylinderPosition.y - cylinderSize.y) && pointPosition.y <= (cylinderPosition.y + cylinderSize.y);

            return withinRadius && withinHeight;
        }

        internal static bool IsInsideMesh(float3 pointPosition, float3 meshPosition, Mesh mesh)
        {
            throw new NotImplementedException();
        }
    }
}