using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Misaki.ArtTool
{
    public struct MeshData : IDisposable
    {
        public Bounds bounds;

        [ReadOnly]
        public NativeArray<int> triangles;
        [ReadOnly]
        public NativeArray<float3> normals;
        [ReadOnly]
        public NativeArray<float3> vertices;
        [ReadOnly]
        public NativeList<int2> edges;

        public int vertexCount;

        public float4x4 worldMatrix;

        public MeshData(Allocator allocator)
        {
            bounds = default;

            triangles = new(0, allocator);
            normals = new(0, allocator);
            vertices = new(0, allocator);
            edges = new(0, allocator);

            vertexCount = 0;
            worldMatrix = float4x4.identity;
        }

        public MeshData(MeshFilter meshFilter, Allocator allocator)
        {
            var mesh = meshFilter.sharedMesh;

            bounds = mesh.bounds;

            triangles = new(mesh.triangles.Length, allocator);
            for (var i = 0; i < triangles.Length; i++)
            {
                triangles[i] = mesh.triangles[i];
            }

            normals = new(mesh.tangents.Length, allocator);
            for (var i = 0; i < normals.Length; i++)
            {
                normals[i] = mesh.normals[i];
            }

            vertices = new(mesh.vertices.Length, allocator);
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] = mesh.vertices[i];
            }

            vertexCount = mesh.vertexCount;

            edges = new((int)(vertexCount * 1.5f), allocator);
            for (var i = 0; i < triangles.Length; i += 3)
            {
                AddEdge(edges, triangles[i], triangles[i + 1]);
                AddEdge(edges, triangles[i + 1], triangles[i + 2]);
                AddEdge(edges, triangles[i + 2], triangles[i]);
            }

            worldMatrix = meshFilter.transform.localToWorldMatrix;

            static void AddEdge(NativeList<int2> edges, int a, int b)
            {
                if (a < b)
                {
                    edges.Add(new int2(a, b));
                }
                else
                {
                    edges.Add(new int2(b, a));
                }
            }
        }

        public void Dispose()
        {
            triangles.Dispose();
            normals.Dispose();
            vertices.Dispose();
            edges.Dispose();
        }
    }
}