namespace Misaki.ArtToolEditor
{
    public class BrustSetting
    {
        public float BrushSize = 1.0f;
        public float SplatSpacing = 0.0f;
        public float Density = 1.0f;
        //public AnimationCurve DensityFalloff = new() { keys = new Keyframe[2] { new(0, 1, 0, 0f, 0.33f, 0.33f), new(1, 0, -2.71698f, -2.71698f, 0.04166669f, 0.3333333f) } };

        public bool GpuInstancing;
        public bool Gravity;
        public int PaintLayer = 1;
        public bool AvoidOverlap = true;
    }
}