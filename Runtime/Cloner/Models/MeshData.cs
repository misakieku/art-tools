using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Misaki.ArtTool
{
    public struct MeshData : IDisposable
    {
        public Bounds bounds;
        public NativeArray<int> triangles;
        public NativeArray<float4> tangents;
        public NativeArray<float3> vertices;
        public NativeList<int2> edges;
        public int vertexCount;

        public float4x4 worldMatrix;

        public MeshData(MeshFilter meshFilter, Allocator allocator)
        {
            var mesh = meshFilter.sharedMesh;

            bounds = mesh.bounds;

            triangles = new(mesh.triangles.Length, allocator);
            for (var i = 0; i < triangles.Length; i++)
            {
                triangles[i] = mesh.triangles[i];
            }

            tangents = new(mesh.tangents.Length, allocator);
            for (var i = 0; i < tangents.Length; i++)
            {
                tangents[i] = mesh.tangents[i];
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
            tangents.Dispose();
            vertices.Dispose();
            edges.Dispose();
        }
    }
}