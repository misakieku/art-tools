using System;
using Unity.Collections;

namespace Misaki.ArtTool
{
    public struct ObjectIdInfo : IDisposable
    {
        public NativeArray<int> instanceIdArray;
        public NativeArray<int> transformIdArray;

        public bool IsCreated
        {
            get
            {
                return instanceIdArray.IsCreated && transformIdArray.IsCreated;
            }
        }

        public ObjectIdInfo(int size)
        {
            instanceIdArray = new(size, Allocator.Persistent);
            transformIdArray = new(size, Allocator.Persistent);
        }

        public void Dispose()
        {
            instanceIdArray.Dispose();
            transformIdArray.Dispose();
        }
    }
}