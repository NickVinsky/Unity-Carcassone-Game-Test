using UnityEngine;
using UnityEngine.UI;

namespace Code.GUI {
    public class CoreGUI {
        private const float InvisibleZ = 0.0f;
        private const float VisibleZ = 22.0f;

        protected static void InitRectPos(GameObject element, float posX, float posY) {
            element.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);
            element.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        protected static void InitRectSize(GameObject element, float sizeX, float sizeY) {
            element.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeX, sizeY);
            element.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        protected static void InitRect(GameObject element, float sizeX, float sizeY, float posX, float posY) {
            element.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);
            element.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeX, sizeY);
            element.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }

        protected static float PercWidth(float p) {
            return Screen.width * p / 100;
        }
        protected static float PercHeight(float p) {
            return Screen.height * p / 100;
        }

        #region Init Objects
        protected void InitLabel(GUIElement e) {
            GameObject newLabel = new GameObject(e.Name);
            newLabel.transform.SetParent(GameObject.Find(e.Parent).transform);

            newLabel.AddComponent<RectTransform>();
            newLabel.layer = e.Layer;
            InitRect(newLabel, e.Width, e.Height, e.X, e.Y);

            newLabel.AddComponent<Text>();
            newLabel.GetComponent<Text>().alignByGeometry = true;
            newLabel.GetComponent<Text>().alignment = TextAnchor.MiddleRight;
            newLabel.GetComponent<Text>().font = Resources.Load<Font>(e.Font);
            newLabel.GetComponent<Text>().fontSize = e.FontSize;
            newLabel.GetComponent<Text>().color = Color.black;
            newLabel.GetComponent<Text>().fontStyle = FontStyle.Normal;

            //if (e.FormatedText != null) newLabel.GetComponent<Text>().text = e.FormatedText();
            newLabel.GetComponent<Text>().text = e.Text;

            if (e.Action != null) {
                newLabel.AddComponent<Button>();
                newLabel.GetComponent<Button>().onClick.AddListener(e.Action);
            }

            e.PostInitEvent?.Invoke(null, null);
        }
        protected void InitButton(GUIElement e) {
            GameObject newButton = new GameObject(e.Name);
            newButton.transform.SetParent(GameObject.Find(e.Parent).transform);

            newButton.AddComponent<RectTransform>();
            newButton.layer = e.Layer;
            InitRect(newButton, e.Width, e.Height, e.X, e.Y);

            newButton.AddComponent<Image>();
            newButton.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>(e.Texture);

            newButton.AddComponent<Button>();
            newButton.GetComponent<Button>().onClick.AddListener(e.Action);

            e.PostInitEvent?.Invoke(null, null);
        }
        protected void InitTextButton(GUIElement e) {
            GameObject newButton = new GameObject(e.Name);
            GameObject newText = new GameObject(e.ChildName);
            newButton.transform.SetParent(GameObject.Find(e.Parent).transform);
            newText.transform.SetParent(newButton.transform);

            newButton.AddComponent<RectTransform>();
            newText.AddComponent<RectTransform>();
            newButton.layer = e.Layer;
            InitRect(newButton, e.Width, e.Height, e.X, e.Y);
            InitRectSize(newText, e.Width, e.Height);

            newButton.AddComponent<Image>();
            newButton.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>(e.Texture);

            newButton.AddComponent<Button>();
            ColorBlock cb = newButton.GetComponent<Button>().colors;
            cb.highlightedColor = new Color(0.8f, 0.8f, 1.0f, 1.0f);
            newButton.GetComponent<Button>().colors = cb;
            newButton.GetComponent<Button>().onClick.AddListener(e.Action);

            newText.AddComponent<Text>();
            newText.GetComponent<Text>().alignByGeometry = true;
            newText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            newText.GetComponent<Text>().font = Resources.Load<Font>(e.Font);
            newText.GetComponent<Text>().fontSize = e.FontSize;
            newText.GetComponent<Text>().color = new Color(0.93f, 0.85f, 0.56f, 0.9f);
            newText.GetComponent<Text>().fontStyle = FontStyle.Normal;

            newText.GetComponent<Text>().text = e.Text;

            e.PostInitEvent?.Invoke(null, null);
        }
        protected void AddListener(GUIElement e) {
            GameObject newButton = GameObject.Find(e.Name);
            newButton.GetComponent<Button>().onClick.AddListener(e.Action);
        }
        #endregion

        protected static void SetVisible(string obj) {
            var o = GameObject.Find(obj);
            if (o == null) return;
            o.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            //var o = GameObject.Find(obj);
            //if (o == null) return;
            //var p = o.GetComponent<RectTransform>().anchoredPosition3D;
            //o.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(p.x, p.y, VisibleZ);
        }
        protected static void SetInvisible(string obj) {
            var o = GameObject.Find(obj);
            if (o == null) return;
            o.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 0.0f, 0.0f);
        }
        protected static void SetVisibility(string obj, float v) {
            var o = GameObject.Find(obj);
            if (o == null) return;
            var p = o.GetComponent<RectTransform>().anchoredPosition3D;
            o.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(p.x, p.y, v);
        }
    }
}
