using System;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

namespace Misaki.ArtTool
{
    public class InstanceDrawer : MonoBehaviour
    {
        private BatchRendererGroup _batchRendererGroup;

        private BatchMeshID _batchMeshID;
        private BatchMaterialID _batchMaterialID;

        public Mesh mesh;
        public Material material;

        void Start()
        {
            _batchRendererGroup = new BatchRendererGroup(OnPerformCulling, IntPtr.Zero);
            _batchMeshID = _batchRendererGroup.RegisterMesh(mesh);
            _batchMaterialID = _batchRendererGroup.RegisterMaterial(material);
        }

        void Update()
        {

        }

        private void OnDisable()
        {
            _batchRendererGroup.Dispose();
        }

        public JobHandle OnPerformCulling(BatchRendererGroup rendererGroup, BatchCullingContext cullingContext, BatchCullingOutput cullingOutput, IntPtr userContext)
        {
            return new JobHandle();
        }
    }
}