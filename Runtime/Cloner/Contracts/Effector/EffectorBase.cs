using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Misaki.ArtTool
{
    public abstract class EffectorBase : MonoBehaviour
    {
        public float strength = 1.0f;

        public List<FieldData> fieldDataList = new();

        public EventHandler propertyChanged;

        protected float4x4 effectorMatrix;

        private void OnEnable()
        {
            foreach (var item in fieldDataList)
            {
                if (item == null)
                {
                    return;
                }

                item.field.propertyChanged += OnPropertyChanged;
            }
        }

        private void OnDisable()
        {
            foreach (var item in fieldDataList)
            {
                if (item == null)
                {
                    return;
                }

                item.field.propertyChanged -= OnPropertyChanged;
            }
        }

        private void OnPropertyChanged(object sender, EventArgs e)
        {
            propertyChanged?.Invoke(sender, e);
        }

        public virtual void Initialize()
        {
            var fieldCount = fieldDataList.Count;
            for (var i = fieldCount - 1; i >= 0; i--)
            {
                var fieldData = fieldDataList[i];
                if (fieldData.field == null || !fieldData.enable || fieldData.opacity <= 0.0f)
                {
                    continue;
                }

                fieldDataList[i].field.Initialize();
            }

            effectorMatrix = transform.localToWorldMatrix;
        }

        public abstract PointData Operate(int index, float4x4 nodeWorldMatrix, ReadOnlySpan<PointData> points);

        protected float CalculateFieldsWeight(float3 worldPosition)
        {
            var weight = 1.0f;
            var fieldCount = fieldDataList.Count;
            for (var i = fieldCount - 1; i >= 0; i--)
            {
                var fieldData = fieldDataList[i];
                if (fieldData.field == null || !fieldData.enable || fieldData.opacity <= 0.0f)
                {
                    continue;
                }

                var bValue = fieldData.field.Operate(worldPosition, weight);
                weight = FieldHelper.BlendField(weight, bValue, fieldData.opacity, fieldData.blending);
            }

            weight *= strength;

            return weight;
        }
    }
}