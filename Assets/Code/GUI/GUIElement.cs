
using UnityEngine.Events;

namespace Code.GUI {
    public struct GUIElement {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string Name { get; set; }
        public string ChildName { get; set; }
        public string Parent { get; set; }
        public string Texture { get; set; }
        public string Text { get; set; }
        public string Font { get; set; }
        public int FontSize { get; set; }
        public int Layer { get; set; }
        public UnityAction Action { get; set; }
        public GameRegulars.IntDelegate Delegate { get; set; }
        public GameRegulars.StringDelegate FormatedText { get; set; }
        public GameRegulars.EventDelegate PostInitEvent { get; set; }
    }
}