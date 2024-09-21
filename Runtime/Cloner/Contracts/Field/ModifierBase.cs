using System;

namespace Misaki.ArtTool.Packages
{
    public abstract class ModifierBase : IFieldLayer
    {
        public EventHandler PropertyChanged
        {
            get;
            set;
        }

        public abstract float Operate(float weight);
    }
}