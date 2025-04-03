using System.Collections.Generic;
using Unity.Properties;
using UnityEngine.UIElements;
namespace TCS_CustomElements.TCS_CustomElements.UXML {
    [UxmlElement] public partial class TextInput : BindableElement, INotifyValueChanged<string> {
        static readonly BindingId ValueProperty = (BindingId)nameof(value);
        
        public static readonly string USSClassName = "textInput";
        public static readonly string ContainerUss = USSClassName + "_container";
        public static readonly string TextInputUss = USSClassName + "_text-input";
        public static readonly string TextInputContainerUss = TextInputUss + "_container";
        
        VisualElement m_container;
        public TextField m_textInput;
        public VisualElement m_textInputContainer;
        
        public TextInput() {
            BuildUi();
        }
        public TextInput(string header) {
            BuildUi();
        }
        void BuildUi() {
            AddToClassList(USSClassName);
            m_container = new VisualElement {
                name = ContainerUss,
            };
            m_container.AddToClassList(ContainerUss);
            m_textInputContainer = new VisualElement {
                name = "textInput_text-container",
            };
            m_textInputContainer.AddToClassList(TextInputContainerUss);
            m_container.Add(m_textInputContainer);
            m_textInput = new TextField {
                name = "text-input",
            };
            m_textInput.AddToClassList(TextInputUss);
            m_textInputContainer.Add(m_textInput);
            hierarchy.Add(m_container);
            
            value = "";
            
            m_textInput.label = "Text Input";
        }
        
        [CreateProperty, UxmlAttribute("value")] public string value {
            get => m_textInput.value;
            set {
                if (EqualityComparer<string>.Default.Equals(m_textInput.value, value)) {
                    return;
                }

                if (panel != null) {
                    using ChangeEvent<string> pooled = ChangeEvent<string>.GetPooled(m_textInput.value, value);
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
        
        public void SetValueWithoutNotify(string newValue) {
            m_textInput.value = newValue;
            NotifyPropertyChanged(ValueProperty);
        }
    }
}