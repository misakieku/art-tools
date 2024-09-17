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


            var int2ToVector2IntGroup = new ConverterGroup("int2ToVector2IntConverter");
            int2ToVector2IntGroup.AddConverter((ref int2 v) => Int2ToVector2IntConverter.ConvertTo(v));
            int2ToVector2IntGroup.AddConverter((ref Vector2Int v) => Int2ToVector2IntConverter.ConvertBack(v));

            ConverterGroups.RegisterConverterGroup(int2ToVector2IntGroup);


            var int3ToVector3IntGroup = new ConverterGroup("int3ToVector3IntConverter");
            int3ToVector3IntGroup.AddConverter((ref int3 v) => Int3ToVector3IntConverter.ConvertTo(v));
            int3ToVector3IntGroup.AddConverter((ref Vector3Int v) => Int3ToVector3IntConverter.ConvertBack(v));

            ConverterGroups.RegisterConverterGroup(int3ToVector3IntGroup);

            // Converter in ui-toolkit does not support converter parameters right now, we have to register all the types one by one
            var objectModeToDisplayStyleGroup = new ConverterGroup("ObjectModeToDisplayStyleConverter");
            objectModeToDisplayStyleGroup.AddConverter((ref DistributionMode v) => DistributionModeToDisplayStyleConverter.ObjectModeConvertTo(v));

            var splineModeToDisplayStyleGroup = new ConverterGroup("SplineModeToDisplayStyleConverter");
            splineModeToDisplayStyleGroup.AddConverter((ref DistributionMode v) => DistributionModeToDisplayStyleConverter.SplineModeConvertTo(v));

            var linearModeToDisplayStyleGroup = new ConverterGroup("LinearModeToDisplayStyleConverter");
            linearModeToDisplayStyleGroup.AddConverter((ref DistributionMode v) => DistributionModeToDisplayStyleConverter.LinearModeConvertTo(v));

            var gridModeToDisplayStyleGroup = new ConverterGroup("GridModeToDisplayStyleConverter");
            gridModeToDisplayStyleGroup.AddConverter((ref DistributionMode v) => DistributionModeToDisplayStyleConverter.GridModeConvertTo(v));

            var radialModeToDisplayStyleGroup = new ConverterGroup("RadialModeToDisplayStyleConverter");
            radialModeToDisplayStyleGroup.AddConverter((ref DistributionMode v) => DistributionModeToDisplayStyleConverter.RadialModeConvertTo(v));

            var honeycombModeToDisplayStyleGroup = new ConverterGroup("HoneycombModeToDisplayStyleConverter");
            honeycombModeToDisplayStyleGroup.AddConverter((ref DistributionMode v) => DistributionModeToDisplayStyleConverter.HoneycombModeConvertTo(v));

            ConverterGroups.RegisterConverterGroup(objectModeToDisplayStyleGroup);
            ConverterGroups.RegisterConverterGroup(splineModeToDisplayStyleGroup);
            ConverterGroups.RegisterConverterGroup(linearModeToDisplayStyleGroup);
            ConverterGroups.RegisterConverterGroup(gridModeToDisplayStyleGroup);
            ConverterGroups.RegisterConverterGroup(radialModeToDisplayStyleGroup);
            ConverterGroups.RegisterConverterGroup(honeycombModeToDisplayStyleGroup);
        }
    }
}