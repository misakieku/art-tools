using UnityEngine.UIElements;

namespace Misaki.ArtTool
{
    public struct DistributionModeToDisplayStyleConverter
    {
        public static StyleEnum<DisplayStyle> ObjectModeConvertTo(DistributionMode mode)
        {
            if (mode == DistributionMode.Object)
            {
                return DisplayStyle.Flex;
            }

            return DisplayStyle.None;
        }

        public static StyleEnum<DisplayStyle> SplineModeConvertTo(DistributionMode mode)
        {
            if (mode == DistributionMode.Spline)
            {
                return DisplayStyle.Flex;
            }

            return DisplayStyle.None;
        }

        public static StyleEnum<DisplayStyle> LinearModeConvertTo(DistributionMode mode)
        {
            if (mode == DistributionMode.Linear)
            {
                return DisplayStyle.Flex;
            }

            return DisplayStyle.None;
        }

        public static StyleEnum<DisplayStyle> GridModeConvertTo(DistributionMode mode)
        {
            if (mode == DistributionMode.Grid)
            {
                return DisplayStyle.Flex;
            }

            return DisplayStyle.None;
        }

        public static StyleEnum<DisplayStyle> RadialModeConvertTo(DistributionMode mode)
        {
            if (mode == DistributionMode.Radial)
            {
                return DisplayStyle.Flex;
            }

            return DisplayStyle.None;
        }

        public static StyleEnum<DisplayStyle> HoneycombModeConvertTo(DistributionMode mode)
        {
            if (mode == DistributionMode.Honeycomb)
            {
                return DisplayStyle.Flex;
            }

            return DisplayStyle.None;
        }
    }
}
