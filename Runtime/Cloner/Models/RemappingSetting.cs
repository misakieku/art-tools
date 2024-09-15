using System;

namespace Misaki.ArtTool
{
    [Serializable]
    public class RemappingSetting
    {
        public bool enable = true;

        public float strength = 1.0f;
        public bool invert = false;

        public float innerOffset = 0.0f;

        public float min = 0.0f;
        public float max = 1.0f;
    }
}
