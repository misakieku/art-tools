using System;

namespace Misaki.ArtTool
{
    [Serializable]
    public class FieldData
    {
        public bool enable = true;
        public FieldBase field;
        public BlendingMode blending;
        public float opacity = 1.0f;
    }
}
