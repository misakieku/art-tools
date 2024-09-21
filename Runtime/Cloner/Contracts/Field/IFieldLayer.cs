using System;
using Unity.Mathematics;

namespace Misaki.ArtTool
{
    public interface IFieldLayer
    {
        public EventHandler PropertyChanged
        {
            get;
            set;
        }

        public virtual void Initialize()
        {
        }

        public virtual float Operate(float3 position)
        {
            return 0.0f;
        }

        public virtual float Operate(float weight)
        {
            return weight;
        }
    }
}
