using Misaki.ArtTool;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Misaki.ArtToolEditor
{
    [UxmlElement]
    public partial class RemappingGraph : BindableElement, INotifyValueChanged<RemappingSetting>
    {
        internal static readonly BindingId valueProperty = "value";

        [DontCreateProperty]
        [SerializeField]
        private RemappingSetting _setting = new();

        [CreateProperty]
        public RemappingSetting value
        {
            get => _setting;
            set
            {
                if (EqualityComparer<RemappingSetting>.Default.Equals(_setting, value))
                    return;

                var previous = this.value;
                SetValueWithoutNotify(value);

                using var evt = ChangeEvent<object>.GetPooled(previous, value);
                evt.target = this;
                SendEvent(evt);

                NotifyPropertyChanged(in valueProperty);
            }
        }

        public void SetValueWithoutNotify(RemappingSetting newValue)
        {
            _setting.enable = newValue.enable;
            _setting.strength = newValue.strength;
            _setting.invert = newValue.invert;
            _setting.innerOffset = newValue.innerOffset;
            _setting.min = newValue.min;
            _setting.max = newValue.max;

            MarkDirtyRepaint();
        }

        public RemappingGraph()
        {
            style.height = 100;
            style.marginBottom = 6;
            style.marginLeft = 6;
            style.marginRight = 6;
            style.marginTop = 6;

            generateVisualContent += OnGenerateVisualContent;
        }

        private void OnGenerateVisualContent(MeshGenerationContext context)
        {
            Vector2 p0, p1, p2, p3, p4, p5, p6;

            if (_setting.invert)
            {
                p0 = CalculatePoint(0.0f, _setting.min * _setting.strength); // min
                p1 = new Vector2(0.0f, layout.height); // bottom left
                p2 = new Vector2(layout.width, layout.height); // bottom right
                p3 = CalculatePoint(1.0f, _setting.max * _setting.strength); // max
                p4 = CalculatePoint(1.0f - _setting.innerOffset, _setting.max * _setting.strength); // inner offset

                p5 = _setting.max > _setting.min ? p3 : p0;
                p6 = _setting.max > _setting.min ? CalculatePoint(0.0f, _setting.max * _setting.strength) : CalculatePoint(1.0f, _setting.min * _setting.strength);
            }
            else
            {
                p0 = CalculatePoint(0.0f, 1.0f - _setting.min * _setting.strength); // min
                p1 = new Vector2(0.0f, layout.height); // bottom left
                p2 = new Vector2(layout.width, layout.height); // bottom right
                p3 = CalculatePoint(1.0f, 1.0f - _setting.max * _setting.strength); // max
                p4 = CalculatePoint(1.0f - _setting.innerOffset, 1.0f - _setting.max * _setting.strength); // inner offset

                p5 = _setting.max > _setting.min ? p0 : p3;
                p6 = _setting.max > _setting.min ? CalculatePoint(1.0f, 1.0f - _setting.min * _setting.strength) : CalculatePoint(0.0f, 1.0f - _setting.max * _setting.strength);
            }

            var painter2D = context.painter2D;

            painter2D.fillColor = Color.gray;

            painter2D.BeginPath();
            painter2D.MoveTo(p0);
            painter2D.LineTo(p1);
            painter2D.LineTo(p2);
            painter2D.LineTo(p3);
            painter2D.LineTo(p4);
            painter2D.ClosePath();
            painter2D.Fill();

            painter2D.strokeColor = Color.black;

            painter2D.BeginPath();
            painter2D.MoveTo(p5);
            painter2D.LineTo(p6);
            painter2D.Stroke();
        }

        private Vector2 CalculatePoint(float xFactor, float yFactor)
        {
            return new Vector2(xFactor * layout.width, yFactor * layout.height);
        }
    }
}