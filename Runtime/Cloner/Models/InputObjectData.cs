using System;
using UnityEngine;

namespace Misaki.ArtTool
{
    [Serializable]
    public class InputObjectData
    {
        public GameObject gameObject;
        public uint frequency = 1;

        private Mesh _mesh;
        public Mesh Mesh
        {
            get
            {
                if (_mesh == null)
                {
                    _mesh = gameObject.GetComponentInChildren<MeshFilter>().sharedMesh;
                }

                return _mesh;
            }
        }

        private Material _material;
        public Material Material
        {
            get
            {
                if (_material == null)
                {
                    _material = gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial;
                    _material.enableInstancing = true;
                }

                return _material;
            }
        }
    }
}