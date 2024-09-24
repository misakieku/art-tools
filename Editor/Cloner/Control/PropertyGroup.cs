using UnityEngine;
using UnityEngine.UIElements;

namespace Misaki.ArtToolEditor
{
    public enum HeaderSize
    {
        Small,
        Medium,
        Large
    }

    [UxmlElement]
    public partial class PropertyGroup : VisualElement
    {
        private string _headerText = "Property Header";

        [UxmlAttribute]
        public string HeaderText
        {
            get => _headerText;
            set
            {
                _headerText = value;
                header.text = value;
            }
        }

        private HeaderSize _headerSize = HeaderSize.Medium;

        [UxmlAttribute]
        public HeaderSize HeaderSize
        {
            get => _headerSize;
            set
            {
                _headerSize = value;
                SetHeaderStyle(value);
            }
        }

        private readonly Label header;
        private readonly VisualElement propertyContainer;

        public PropertyGroup()
        {
            header = new Label(_headerText) { name = "header" };

            SetHeaderStyle(_headerSize);

            propertyContainer = new VisualElement() { name = "property-content" };

            propertyContainer.style.marginLeft = 8;

            hierarchy.Add(header);
            hierarchy.Add(propertyContainer);
        }

        public override VisualElement contentContainer => propertyContainer;

        private void SetHeaderStyle(HeaderSize size)
        {
            switch (size)
            {
                case HeaderSize.Small:
                    header.style.marginTop = 6;
                    header.style.marginBottom = 4;
                    header.style.fontSize = 11;
                    break;
                case HeaderSize.Medium:
                    header.style.marginTop = 8;
                    header.style.marginBottom = 6;
                    header.style.fontSize = 12;
                    break;
                case HeaderSize.Large:
                    header.style.marginTop = 16;
                    header.style.marginBottom = 12;
                    header.style.fontSize = 16;
                    break;
                default:
                    break;
            }

            header.style.unityFontStyleAndWeight = FontStyle.Bold;
        }
    }
}