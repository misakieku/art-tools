using UnityEngine.UIElements;

namespace Misaki.ArtTool
{
    public struct BoolToDisplayConverter
    {
        public static StyleEnum<DisplayStyle> ConvertTo(bool value)
        {
            return value ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public static bool ConvertBack(StyleEnum<DisplayStyle> value)
        {
            return value == DisplayStyle.Flex;
        }
    }

    public struct InverseBoolToDisplayConverter
    {
        public static StyleEnum<DisplayStyle> ConvertTo(bool value)
        {
            return value ? DisplayStyle.None : DisplayStyle.Flex;
        }

        public static bool ConvertBack(StyleEnum<DisplayStyle> value)
        {
            return value == DisplayStyle.None;
        }
    }
}