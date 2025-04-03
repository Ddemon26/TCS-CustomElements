using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace TCS_CustomElements.TCS_CustomElements.UXML {
    [UxmlElement]
    public partial class ProgressBar : BindableElement, INotifyValueChanged<float> {
        static readonly BindingId ShowTitleProperty = (BindingId)nameof(ShowTitle);
        static readonly BindingId ShowPercentageProperty = (BindingId)nameof(ShowPercentage);
        static readonly BindingId LowValueProperty = (BindingId)nameof(LowValue);
        static readonly BindingId HighValueProperty = (BindingId)nameof(HighValue);
        static readonly BindingId ValueProperty = (BindingId)nameof(value);
        static readonly BindingId ColorProperty = (BindingId)nameof(ColorValue);

        public static readonly string USSClassName = "progress-bar";
        public static readonly string ContainerUssClassName = USSClassName + "_container";
        public static readonly string TitleUssClassName = USSClassName + "_title";
        public static readonly string TitleContainerUssClassName = USSClassName + "_title-container";
        public static readonly string ProgressUssClassName = USSClassName + "_progress";
        public static readonly string BackgroundUssClassName = USSClassName + "_background";

        readonly VisualElement m_background;
        readonly VisualElement m_progress;
        readonly Label m_title;

        float m_lowValue;
        float m_highValue = 100f;
        float m_value;
        bool m_showTitle = true;
        bool m_showPercentage = false;

        public ProgressBar() {
            AddToClassList(USSClassName);
            var child1 = new VisualElement { name = "progress-bar" };
            m_background = new VisualElement { name = "background" };
            m_background.AddToClassList(BackgroundUssClassName);
            m_progress = new VisualElement { name = "progress" };
            m_progress.AddToClassList(ProgressUssClassName);
            m_background.Add(m_progress);

            var child2 = new VisualElement { name = "title-container" };
            child2.AddToClassList(TitleContainerUssClassName);
            m_title = new Label { name = "title" };
            m_title.AddToClassList(TitleUssClassName);
            child2.Add(m_title);
            m_background.Add(child2);

            child1.Add(m_background);
            child1.AddToClassList(ContainerUssClassName);
            hierarchy.Add(child1);

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);

            ColorValue = new Color(1.0f, 0.0f, 0.0f);
            LowValue = 0f;
            HighValue = 100f;
            value = 50f;
        }

        void OnGeometryChanged(GeometryChangedEvent e) => SetProgress(value);

        [CreateProperty, UxmlAttribute("show-title")]
        public bool ShowTitle {
            get => m_showTitle;
            set {
                if (m_showTitle == value) return;
                m_showTitle = value;
                m_title.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
                UpdateTitle();
                NotifyPropertyChanged(ShowTitleProperty);
            }
        }

        [CreateProperty, UxmlAttribute("show-percentage")]
        public bool ShowPercentage {
            get => m_showPercentage;
            set {
                if (m_showPercentage == value) return;
                m_showPercentage = value;
                UpdateTitle();
                NotifyPropertyChanged(ShowPercentageProperty);
            }
        }

        [CreateProperty, UxmlAttribute("low-value")]
        public float LowValue {
            get => m_lowValue;
            set {
                float old = m_lowValue;
                m_lowValue = value;
                SetProgress(m_value);
                if (Mathf.Approximately(old, m_lowValue)) return;
                NotifyPropertyChanged(LowValueProperty);
            }
        }

        [CreateProperty, UxmlAttribute("high-value")]
        public float HighValue {
            get => m_highValue;
            set {
                float old = m_highValue;
                m_highValue = value;
                SetProgress(m_value);
                if (Mathf.Approximately(old, m_highValue)) return;
                NotifyPropertyChanged(HighValueProperty);
            }
        }

        void SetProgress(float p) {
            m_progress.style.display = p <= LowValue ? DisplayStyle.None : DisplayStyle.Flex;
            float clamped = Mathf.Clamp(p, LowValue, HighValue);
            float width = CalculateProgressWidth(clamped);
            if (width >= 0.0f) m_progress.style.right = width;
            UpdateTitle();
        }

        float CalculateProgressWidth(float width) {
            if (m_background == null || m_progress == null) return 0f;
            var bgWidth = m_background.layout.width;
            if (float.IsNaN(bgWidth)) return 0f;
            float full = bgWidth - 2f;
            return full - Mathf.Max(full * width / HighValue, 1f);
        }

        [CreateProperty, UxmlAttribute("value")]
        public float value {
            get => m_value;
            set {
                if (EqualityComparer<float>.Default.Equals(m_value, value)) return;
                if (panel != null) {
                    using var pooled = ChangeEvent<float>.GetPooled(m_value, value);
                    pooled.target = this;
                    SetValueWithoutNotify(value);
                    SendEvent(pooled);
                    NotifyPropertyChanged(ValueProperty);
                } else SetValueWithoutNotify(value);
            }
        }

        public void SetValueWithoutNotify(float newValue) {
            m_value = newValue;
            SetProgress(m_value);
        }

        [CreateProperty, UxmlAttribute("color-value")]
        public Color ColorValue {
            get => m_progress.resolvedStyle.backgroundColor;
            set {
                if (m_progress == null) return;
                m_progress.style.backgroundColor = value;
                NotifyPropertyChanged(ColorProperty);
            }
        }

        void UpdateTitle() {
            if (!ShowTitle) {
                m_title.text = string.Empty;
                return;
            }

            m_title.text = ShowPercentage
                ? $"{Mathf.RoundToInt(Mathf.Clamp01(m_highValue <= 0f ? 0f : m_value / m_highValue) * 100f)}%"
                : $"{m_value}/{m_highValue}";
        }
    }
}