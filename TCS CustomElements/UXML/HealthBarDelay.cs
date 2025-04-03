using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace TCS_CustomElements.TCS_CustomElements.UXML {
    [UxmlElement]
    public partial class HealthBarDelay : BindableElement, INotifyValueChanged<float> {
        static readonly BindingId ShowTitleProperty = (BindingId)nameof(ShowTitle);
        static readonly BindingId LowValueProperty = (BindingId)nameof(LowValue);
        static readonly BindingId HighValueProperty = (BindingId)nameof(HighValue);
        static readonly BindingId ValueProperty = (BindingId)nameof(value);
        static readonly BindingId DelayedValueProperty = (BindingId)nameof(DelayedValue);
        static readonly BindingId ColorProperty = (BindingId)nameof(ColorValue);
        static readonly BindingId ColorDelayProperty = (BindingId)nameof(ColorDelayValue);

        public static readonly string USSClassName = "health-bar";
        public static readonly string ContainerUssClassName = USSClassName + "_container";
        public static readonly string TitleUssClassName = USSClassName + "_title";
        public static readonly string TitleContainerUssClassName = USSClassName + "_title-container";
        public static readonly string ProgressUssClassName = USSClassName + "_progress";
        public static readonly string DelayedProgressUssClassName = USSClassName + "_delayed-progress";
        public static readonly string BackgroundUssClassName = USSClassName + "_background";

        readonly VisualElement m_background;
        public VisualElement m_progress;
        public VisualElement m_delayedProgress;
        readonly Label m_title;

        float m_lowValue;
        float m_highValue = 100f;
        float m_value;
        float m_delayedValue;
        bool m_showTitle = true;

        public HealthBarDelay() {
            AddToClassList(USSClassName);
            var child1 = new VisualElement { name = "health-bar" };
            m_background = new VisualElement { name = "background" };
            m_background.AddToClassList(BackgroundUssClassName);
            m_delayedProgress = new VisualElement { name = "delayed-progress" };
            m_delayedProgress.AddToClassList(DelayedProgressUssClassName);
            m_progress = new VisualElement { name = "progress" };
            m_progress.AddToClassList(ProgressUssClassName);
            m_background.Add(m_delayedProgress);
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

            ColorValue = new Color(1f, 0f, 0f);
            ColorDelayValue = new Color(0f, 1f, 0f);
            LowValue = 0f;
            HighValue = 100f;
            value = 50f;
            DelayedValue = 50f;
        }

        void OnGeometryChanged(GeometryChangedEvent e) {
            SetProgress(value);
            SetDelayedProgress(DelayedValue);
        }

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
            if (width >= 0f) m_progress.style.right = width;
            UpdateTitle();
        }

        public void SetDelayedProgress(float p) {
            m_delayedProgress.style.display = p <= LowValue ? DisplayStyle.None : DisplayStyle.Flex;
            float clamped = Mathf.Clamp(p, LowValue, HighValue);
            float width = CalculateProgressWidth(clamped);
            if (width >= 0f) m_delayedProgress.style.right = width;
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

        [CreateProperty, UxmlAttribute("delayed-value")]
        public float DelayedValue {
            get => m_delayedValue;
            set {
                if (EqualityComparer<float>.Default.Equals(m_delayedValue, value)) return;
                if (panel != null) {
                    using var pooled = ChangeEvent<float>.GetPooled(m_delayedValue, value);
                    pooled.target = this;
                    SetDelayValueWithoutNotify(value);
                    SendEvent(pooled);
                    NotifyPropertyChanged(DelayedValueProperty);
                } else SetDelayValueWithoutNotify(value);
            }
        }

        public void SetValueWithoutNotify(float newValue) {
            m_value = newValue;
            SetProgress(m_value);
        }

        public void SetDelayValueWithoutNotify(float newDelayedValue) {
            m_delayedValue = newDelayedValue;
            SetDelayedProgress(m_delayedValue);
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

        [CreateProperty, UxmlAttribute("color-delay-value")]
        public Color ColorDelayValue {
            get => m_delayedProgress.resolvedStyle.backgroundColor;
            set {
                if (m_delayedProgress == null) return;
                m_delayedProgress.style.backgroundColor = value;
                NotifyPropertyChanged(ColorDelayProperty);
            }
        }

        void UpdateTitle() => m_title.text = ShowTitle ? $"{m_value}/{m_highValue}" : string.Empty;
    }
}