using Unity.Mathematics;

namespace Misaki.ArtTool
{
    internal static partial class ShapeHelper
    {
        internal static float3 GetCubePosition(int index, float3 size)
        {
            float3 localPosition;
            var yIndex = index / (size.x * size.z);
            var remain = index % (size.x * size.z);
            var zIndex = remain / size.x;
            var xIndex = remain % size.x;

            localPosition = new float3((int)xIndex, (int)yIndex, (int)zIndex);
            localPosition -= (size - 1) * 0.5f;
            return localPosition;
        }

        internal static float4x4 GetRadialMatrix(int index, RadialDistributionSetting setting)
        {
            var staringAngle = math.radians(setting.angleMinMax.x);
            var totalAngle = math.TAU / (360.0f / setting.angleMinMax.y) - staringAngle;
            var angleStep = totalAngle / setting.count;

            var angle = index * angleStep + staringAngle;
            var x = math.cos(-angle) * setting.radius;
            var y = math.sin(-angle) * setting.radius;

            var position = float3.zero;
            switch (setting.plane)
            {
                case PlaneDirection.XY:
                    position = new float3(x, y, 0);
                    break;
                case PlaneDirection.ZY:
                    position = new float3(0, x, y);
                    break;
                case PlaneDirection.XZ:
                    position = new float3(x, 0, y);
                    break;
                default:
                    break;
            }

            var rotation = quaternion.identity;
            if (setting.align)
            {
                switch (setting.plane)
                {
                    case PlaneDirection.XY:
                        rotation = quaternion.EulerXYZ(0.0f, 0.0f, angle);
                        break;
                    case PlaneDirection.ZY:
                        rotation = quaternion.EulerXYZ(angle, 0.0f, 0.0f);
                        break;
                    case PlaneDirection.XZ:
                        rotation = quaternion.EulerXYZ(0.0f, angle, 0.0f);
                        break;
                    default:
                        break;
                }
            }

            return float4x4.TRS(position, rotation, new float3(1.0f));
        }

        internal static bool TryGetMeshVertexMatrix(int index, bool alignNormal, ref MeshData meshData, out float4x4 outMatrix)
        {
            outMatrix = float4x4.identity;

            if (!meshData.vertices.IsCreated || meshData.vertices.Length <= index)
            {
                return false;
            }

            var meshScale = meshData.worldMatrix.GetScale();
            var meshPosition = meshData.worldMatrix.GetPosition();

            if (index >= meshData.vertices.Length)
            {
                return false;
            }

            var position = meshData.vertices[index] * meshScale + meshPosition;

            var rotation = quaternion.identity;
            if (alignNormal)
            {
                var upDirection = meshData.normals[index];
                var forwardDirection = math.normalize(math.cross(upDirection, new float3(-1.0f, 0.0f, 0.0f)));

                rotation = quaternion.LookRotation(forwardDirection, upDirection);
            }

            outMatrix = float4x4.TRS(position, rotation, new float3(1.0f));

            return true;
        }

        internal static bool TryGetMeshEdgeMatrix(int index, bool alignNormal, ref MeshData meshData, out float4x4 outMatrix)
        {
            outMatrix = float4x4.identity;

            if (!meshData.edges.IsCreated || index >= meshData.edges.Length)
            {
                return false;
            }

            var edge = meshData.edges[index];
            if (edge.x >= meshData.vertices.Length || edge.y >= meshData.vertices.Length)
            {
                return false;
            }

            var a = meshData.vertices[edge.x];
            var b = meshData.vertices[edge.y];

            var position = (a + b) / 2.0f;
            var forwardDirection = math.normalize(a - b);

            var meshScale = meshData.worldMatrix.GetScale();
            var meshPosition = meshData.worldMatrix.GetPosition();
            position = position * meshScale + meshPosition;

            var rotation = quaternion.identity;
            if (alignNormal)
            {
                var upDirection = math.normalize(meshData.normals[edge.x] + meshData.normals[edge.y]);
                rotation = quaternion.LookRotation(forwardDirection, upDirection);
            }

            outMatrix = float4x4.TRS(position, rotation, new float3(1.0f));

            return true;
        }

        //TODO : interpolate normal based on distance
        internal static bool TryGetMeshPolygonMatrix(int index, bool alignNormal, ref MeshData meshData, out float4x4 outMatrix)
        {
            outMatrix = float4x4.identity;

            if (!meshData.edges.IsCreated || meshData.edges.Length <= index)
            {
                return false;
            }

            var triangleIndex = index * 3;

            if (triangleIndex >= meshData.triangles.Length - 2)
            {
                return false;
            }

            var vertexIndexA = meshData.triangles[triangleIndex];
            var vertexIndexB = meshData.triangles[triangleIndex + 1];
            var vertexIndexC = meshData.triangles[triangleIndex + 2];

            var a = meshData.vertices[vertexIndexA];
            var b = meshData.vertices[vertexIndexB];
            var c = meshData.vertices[vertexIndexC];

            var position = (a + b + c) / 3.0f;

            var meshScale = meshData.worldMatrix.GetScale();
            var meshPosition = meshData.worldMatrix.GetPosition();
            position = position * meshScale + meshPosition;

            var rotation = quaternion.identity;
            if (alignNormal)
            {
                var upDirection = math.normalize(meshData.normals[vertexIndexA] + meshData.normals[vertexIndexB] + meshData.normals[vertexIndexC]);
                var forwardDirection = math.normalize(math.cross(upDirection, new float3(-1.0f, 0.0f, 0.0f)));

                rotation = quaternion.LookRotation(forwardDirection, upDirection);
            }

            outMatrix = float4x4.TRS(position, rotation, new float3(1.0f));

            return true;
        }

        //TODO : interpolate normal based on distance
        internal static bool TryGetMatrixOnMeshSurface(int index, uint seed, bool alignNormal, ref MeshData meshData, out float4x4 outMatrix)
        {
            outMatrix = float4x4.identity;

            if (!meshData.areas.IsCreated || !meshData.vertices.IsCreated || !meshData.triangles.IsCreated)
            {
                return false;
            }

            var random = Random.CreateFromIndex(seed + (uint)index);
            var randomValue = random.NextFloat(meshData.totalArea);

            var triangleIndex = -1;
            for (var j = 0; j < meshData.areas.Length; j++)
            {
                if (randomValue <= meshData.areas[j])
                {
                    triangleIndex = j * 3;
                    break;
                }
                randomValue -= meshData.areas[j];
            }

            if (triangleIndex >= meshData.triangles.Length - 2)
            {
                return false;
            }

            var vertexIndexA = meshData.triangles[triangleIndex];
            var vertexIndexB = meshData.triangles[triangleIndex + 1];
            var vertexIndexC = meshData.triangles[triangleIndex + 2];

            var a = meshData.vertices[vertexIndexA];
            var b = meshData.vertices[vertexIndexB];
            var c = meshData.vertices[vertexIndexC];

            var r1 = math.sqrt(random.NextFloat());
            var r2 = random.NextFloat();

            var position = (1 - r1) * a + (r1 * (1 - r2)) * b + (r1 * r2) * c;

            var meshScale = meshData.worldMatrix.GetScale();
            var meshPosition = meshData.worldMatrix.GetPosition();
            position = position * meshScale + meshPosition;

            var rotation = quaternion.identity;
            if (alignNormal)
            {
                var upDirection = math.normalize(meshData.normals[vertexIndexA] + meshData.normals[vertexIndexB] + meshData.normals[vertexIndexC]);
                var forwardDirection = math.normalize(math.cross(upDirection, new float3(-1.0f, 0.0f, 0.0f)));

                rotation = quaternion.LookRotation(forwardDirection, upDirection);
            }

            outMatrix = float4x4.TRS(position, rotation, new float3(1.0f));

            return true;
        }

        internal static bool TryGetPositionInMeshVolume(int index, uint seed, ref MeshData meshData, out float4x4 outMatrix)
        {
            var random = Random.CreateFromIndex(seed + (uint)index);
            var meshScale = meshData.worldMatrix.GetScale();
            var meshPosition = meshData.worldMatrix.GetPosition();

            var volumePosition = random.NextFloat3(-meshData.bounds.extents * meshScale + meshPosition, meshData.bounds.extents * meshScale + meshPosition);

            var isInsideMesh = IsPointInsideMesh(volumePosition, ref meshData);
            while (!isInsideMesh)
            {
                volumePosition = random.NextFloat3(-meshData.bounds.extents * meshScale + meshPosition, meshData.bounds.extents * meshScale + meshPosition);
                isInsideMesh = IsPointInsideMesh(volumePosition, ref meshData);
            }

            outMatrix = float4x4.TRS(volumePosition, quaternion.identity, new float3(1.0f));
            return true;
        }
    }
}