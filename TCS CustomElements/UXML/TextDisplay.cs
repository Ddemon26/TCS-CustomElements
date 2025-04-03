using Unity.Properties;
using UnityEngine.UIElements;
namespace TCS_CustomElements.TCS_CustomElements.UXML {
    [UxmlElement] public partial class TextDisplay : BindableElement, INotifyValueChanged<string> {
        static readonly BindingId ContextValueProperty = (BindingId)nameof(TextContext);
        static readonly BindingId HeaderValueProperty = (BindingId)nameof(TextHeader);
        static readonly BindingId AllCapsProperty = (BindingId)nameof(AllCaps);

        public static readonly string ClassNameUSS = "textDisplay";
        public static readonly string ContainerUSS = ClassNameUSS + "_container";
        public static readonly string HeaderUSS = ClassNameUSS + "_header";
        public static readonly string ContextUSS = ClassNameUSS + "_context";

        public VisualElement Container;
        public Label TextHeader;
        public Label TextContext;

        #region Element Constructor
        public TextDisplay() => BuildUI();

        public TextDisplay(string header) {
            BuildUI();

            HeaderValue = header + ":";
            RegisterCallback(new EventCallback<GeometryChangedEvent>(OnGeometryChanged));
        }

        void BuildUI() {
            AddToClassList(ClassNameUSS);
            Container = new VisualElement {
                name = "container",
            };
            Container.AddToClassList(ContainerUSS);
            Add(Container);
            
            TextHeader = new Label {
                name = "textHeader",
            };
            TextHeader.AddToClassList(HeaderUSS);
            TextContext = new Label {
                name = "textContext",
            };
            TextContext.AddToClassList(ContextUSS);
            Container.Add(TextHeader);
            Container.Add(TextContext);

            HeaderValue = "Header:";
            value = "Context";
            AllCaps = true;
        }

        void OnGeometryChanged(GeometryChangedEvent evt) {
            if (evt.newRect.width == 0 || evt.newRect.height == 0) {
                return;
            }

            UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            NotifyPropertyChanged(HeaderValueProperty);
        }
        #endregion

        #region Element Variables
        [CreateProperty, UxmlAttribute("value")] public string value {
            get => TextContext.text;
            set {
                string header = TextContext.text;
                TextContext.text = value;
                if (string.CompareOrdinal(header, TextContext.text) == 0) {
                    return;
                }

                NotifyPropertyChanged(ContextValueProperty);
            }
        }

        [CreateProperty, UxmlAttribute("headerValue")] public string HeaderValue {
            get => TextHeader.text;
            set {
                string context = TextHeader.text;
                TextHeader.text = value;
                if (string.CompareOrdinal(context, TextHeader.text) == 0) {
                    return;
                }

                NotifyPropertyChanged(HeaderValueProperty);
            }
        }

        [CreateProperty, UxmlAttribute("allCaps")] public bool AllCaps {
            get => TextContext.text == TextContext.text.ToUpper();
            set {
                if (value) {
                    TextHeader.text = TextHeader.text.ToUpper();
                    TextContext.text = TextContext.text.ToUpper();
                }
                else {
                    TextHeader.text = TextHeader.text.ToLower();
                    TextContext.text = TextContext.text.ToLower();
                }

                NotifyPropertyChanged(AllCapsProperty);
            }
        }
        #endregion

        #region Element Methods
        public void SetHeader(string header) {
            TextHeader.text = header + ":";
        }
        
        public void SetContext(string contextValue) {
            TextContext.text = contextValue;
        }

        public void SetHeaderAndContext(string header, string contextValue) {
            TextHeader.text = header + ":";
            TextContext.text = contextValue;
        }

        public void SetValueWithoutNotify(string newValue) {
            string header = TextContext.text;
            TextContext.text = newValue;
            if (string.CompareOrdinal(header, TextContext.text) == 0) {
                return;
            }

            NotifyPropertyChanged(HeaderValueProperty);
        }
        #endregion
    }
}