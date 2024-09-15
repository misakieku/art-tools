using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Misaki.ArtTool
{
    internal class ConverterInitializer
    {
        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            var boolToDisplayGroup = new ConverterGroup("BoolToDisplayConvertor");

            boolToDisplayGroup.AddConverter((ref bool v) => BoolToDisplayConverter.ConvertTo(v));
            boolToDisplayGroup.AddConverter((ref StyleEnum<DisplayStyle> v) => BoolToDisplayConverter.ConvertBack(v));

            ConverterGroups.RegisterConverterGroup(boolToDisplayGroup);


            var inverseBoolToDisplayGroup = new ConverterGroup("InverseBoolToDisplayConverter");

            inverseBoolToDisplayGroup.AddConverter((ref bool v) => InverseBoolToDisplayConverter.ConvertTo(v));
            inverseBoolToDisplayGroup.AddConverter((ref StyleEnum<DisplayStyle> v) => InverseBoolToDisplayConverter.ConvertBack(v));

            ConverterGroups.RegisterConverterGroup(inverseBoolToDisplayGroup);


            var float2ToVector2Group = new ConverterGroup("Float2ToVector2Converter");

            float2ToVector2Group.AddConverter((ref float2 v) => Float2ToVector2Converter.ConvertTo(v));
            float2ToVector2Group.AddConverter((ref Vector2 v) => Float2ToVector2Converter.ConvertBack(v));

            ConverterGroups.RegisterConverterGroup(float2ToVector2Group);


            var float3ToVector3Group = new ConverterGroup("Float3ToVector3Converter");

            float3ToVector3Group.AddConverter((ref float3 v) => Float3ToVector3Converter.ConvertTo(v));
            float3ToVector3Group.AddConverter((ref Vector3 v) => Float3ToVector3Converter.ConvertBack(v));

            ConverterGroups.RegisterConverterGroup(float3ToVector3Group);
        }
    }
}