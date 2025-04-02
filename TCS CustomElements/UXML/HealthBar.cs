using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
namespace Source.UIToolkit.Elements {
    [UxmlElement] public partial class HealthBar : BindableElement, INotifyValueChanged<float> {
        static readonly BindingId TitleProperty = (BindingId)nameof(Title);
        static readonly BindingId LowValueProperty = (BindingId)nameof(LowValue);
        static readonly BindingId HighValueProperty = (BindingId)nameof(HighValue);
        static readonly BindingId ValueProperty = (BindingId)nameof(value);
        static readonly BindingId ColorProperty = (BindingId)nameof(ColorValue);

        public static readonly string USSClassName = "health-bar";
        public static readonly string ContainerUssClassName = USSClassName + "_container";
        public static readonly string TitleUssClassName = USSClassName + "_title";
        public static readonly string TitleContainerUssClassName = USSClassName + "_title-container";
        public static readonly string ProgressUssClassName = USSClassName + "_progress";
        public static readonly string BackgroundUssClassName = USSClassName + "_background";

        readonly VisualElement m_background;
        public VisualElement m_progress;
        readonly Label m_title;

        float m_lowValue;
        float m_highValue = 100f;
        float m_value;

        public HealthBar() {
            AddToClassList(USSClassName);
            var child1 = new VisualElement {
                name = "health-bar",
            };
            m_background = new VisualElement();
            m_background.AddToClassList(BackgroundUssClassName);
            m_background.name = "background";
            child1.Add(m_background);
            m_progress = new VisualElement();
            m_progress.AddToClassList(ProgressUssClassName);
            m_progress.name = "progress";
            m_background.Add(m_progress);
            var child2 = new VisualElement {
                name = "title-container",
            };
            child2.AddToClassList(TitleContainerUssClassName);
            m_background.Add(child2);
            m_title = new Label();
            m_title.AddToClassList(TitleUssClassName);
            m_title.name = "title";
            child2.Add(m_title);
            child1.AddToClassList(ContainerUssClassName);
            hierarchy.Add(child1);
            RegisterCallback(new EventCallback<GeometryChangedEvent>(OnGeometryChanged));
            
            ColorValue = new Color(1.0f, 0.0f, 0.0f);
            LowValue = 0f;
            HighValue = 100f;
            value = 50f;
        }
        
        void OnGeometryChanged(GeometryChangedEvent e) {
            SetProgress(value);
        }
        
        [CreateProperty, UxmlAttribute("title")] public string Title {
            get => m_title.text;
            set {
                string title = Title;
                m_title.text = value;
                if (string.CompareOrdinal(title, Title) == 0) {
                    return;
                }

                NotifyPropertyChanged(TitleProperty);
            }
        }

        [CreateProperty, UxmlAttribute("low-value")] public float LowValue {
            get => m_lowValue;
            set {
                float lowValue = LowValue;
                m_lowValue = value;
                SetProgress(m_value);
                if (Mathf.Approximately(lowValue, LowValue)) {
                    return;
                }

                NotifyPropertyChanged(LowValueProperty);
            }
        }

        [CreateProperty, UxmlAttribute("high-value")] public float HighValue {
            get => m_highValue;
            set {
                float highValue = HighValue;
                m_highValue = value;
                SetProgress(m_value);
                if (Mathf.Approximately(highValue, HighValue)) {
                    return;
                }

                NotifyPropertyChanged(HighValueProperty);
            }
        }

        void SetProgress(float p) {
            if (p <= LowValue) {
                m_progress.style.display = DisplayStyle.None;
            }
            else {
                m_progress.style.display = DisplayStyle.Flex;
                float progressWidth = CalculateProgressWidth(p >= LowValue ? (p <= HighValue ? p : HighValue) : LowValue);
                if (progressWidth < 0.0) {
                    return;
                }

                m_progress.style.right = progressWidth;
            }
        }

        float CalculateProgressWidth(float width) {
            if (m_background == null || m_progress == null) {
                return 0.0f;
            }

            var backgroundLayout = m_background.layout;
            if (float.IsNaN(backgroundLayout.width)) {
                return 0.0f;
            }

            backgroundLayout = m_background.layout;
            float num = backgroundLayout.width - 2f;
            return num - Mathf.Max(num * width / HighValue, 1f);
        }

        [CreateProperty, UxmlAttribute("value")] public float value {
            get => m_value;
            set {
                if (EqualityComparer<float>.Default.Equals(m_value, value)) {
                    return;
                }

                if (panel != null) {
                    using ChangeEvent<float> pooled = ChangeEvent<float>.GetPooled(m_value, value);
                    pooled.target = this;
                    SetValueWithoutNotify(value);
                    SendEvent(pooled);
                    NotifyPropertyChanged(ValueProperty);
                }
                else {
                    SetValueWithoutNotify(value);
                }
            }
        }

        public void SetValueWithoutNotify(float newValue) {
            m_value = newValue;
            SetProgress(value);
        }

        [CreateProperty, UxmlAttribute("color-value")] public Color ColorValue {
            get => m_progress.resolvedStyle.backgroundColor;
            set {
                if (m_progress == null) {
                    return;
                }

                m_progress.style.backgroundColor = value;
                NotifyPropertyChanged(ColorProperty);
            }
        }
    }
}