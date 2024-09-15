using System;

namespace Misaki.ArtToolEditor
{
    [Flags]
    public enum RandomnessType
    {
        None = 0,
        Position = 1 << 0,
        Rotation = 1 << 1,
        Scale = 1 << 2
    }
}