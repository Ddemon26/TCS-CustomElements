using Unity.Properties;
using UnityEngine.UIElements;
namespace Source.UIToolkit.Classes {
    [UxmlElement] public partial class TextDisplay : BindableElement, INotifyValueChanged<string> {
        static readonly BindingId ContextValueProperty = (BindingId)nameof(TextContext);
        static readonly BindingId HeaderValueProperty = (BindingId)nameof(TextHeader);
        
        public static readonly string Name = "textDisplay";
        public static readonly string Header = Name + "_header";
        public static readonly string Context = Name + "_context";

        public Label TextHeader;
        public Label TextContext;

        public TextDisplay() => BuildUI();

        public TextDisplay(string header) {
            BuildUI();
            
            HeaderValue = header + ":";
            RegisterCallback(new EventCallback<GeometryChangedEvent>(OnGeometryChanged));
        }
        
        void BuildUI() {
            AddToClassList(Name);
            TextHeader = new Label();
            TextHeader.AddToClassList(Header);
            TextContext = new Label();
            TextContext.AddToClassList(Context);
            Add(TextHeader);    
            Add(TextContext);
        }
        
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
        
        void OnGeometryChanged(GeometryChangedEvent evt) {
            if (evt.newRect.width == 0 || evt.newRect.height == 0) {
                return;
            }
            UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            NotifyPropertyChanged(HeaderValueProperty);
        }
        
        public void SetContext(string contextValue) {
            TextContext.text = contextValue;
        }
        
        public void SetText(string header, string contextValue) {
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
    }
}