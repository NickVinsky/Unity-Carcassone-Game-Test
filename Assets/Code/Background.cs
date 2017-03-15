using System;
using Code.Game.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code {
    public class Background : MonoBehaviour {


        // Use this for initialization
        void Start () {
            float sW = 1920f; //Screen.width;
            float sH = 1080f; //Screen.height;
            float bgW = GetComponent<RectTransform>().sizeDelta.x;
            float bgH = GetComponent<RectTransform>().sizeDelta.y;
            int maxX;
            int maxY;

            maxX = (int) Math.Ceiling(sW / bgW);
            //if (Math.Abs(sW % bgW) > 0) maxX++;
            maxY = (int) Math.Ceiling(sH / bgH);
            //if (Math.Abs(sH % bgH) > 0) maxY++;

            for (int i = 0; i < maxX; i++) {
                for (int j = 0; j < maxY; j++) {
                    if (i != 0 || j != 0) {
                        GameObject bg = new GameObject("Background#" + i + ":" + j);
                        bg.AddComponent<RectTransform>();
                        bg.GetComponent<RectTransform>().SetParent(GameObject.Find(GameRegulars.OverlayBackground).transform);
                        bg.GetComponent<RectTransform>().anchorMin = new Vector2(0.0f, 1.0f);
                        bg.GetComponent<RectTransform>().anchorMax = new Vector2(0.0f, 1.0f);
                        bg.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        bg.GetComponent<RectTransform>().sizeDelta = new Vector2(bgW, bgH);
                        bg.AddComponent<Image>();
                        bg.GetComponent<Image>().sprite = Resources.Load<Sprite>("formBg");
                        Vector3 p = new Vector3 {
                            x = bgW * i + bgW / 2,
                            y = - (bgH * j + bgH / 2),
                            z = 0.0f
                        };
                        bg.GetComponent<RectTransform>().anchoredPosition3D = p;
                    }
                }
            }

            if (SceneManager.GetActiveScene().name == "game")
                GameObject.Find("bgPanel").transform.SetAsLastSibling();


            /*
	    while (sH > drawnY * bgH) {
	        while (sW > drawnX * bgW) {
            	        GameObject Bg = new GameObject("Background#" + drawnX + ":" + drawnY);
            	        Bg.AddComponent<RectTransform>();
            	        Bg.GetComponent<RectTransform>().SetParent(GameObject.Find(GameRegulars.OverlayBackground).transform);
            	        Bg.GetComponent<RectTransform>().anchorMin = new Vector2(0.0f, 1.0f);
            	        Bg.GetComponent<RectTransform>().anchorMax = new Vector2(0.0f, 1.0f);
            	        Bg.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            	        Bg.GetComponent<RectTransform>().sizeDelta = new Vector2(bgW, bgH);
            	        Bg.AddComponent<Image>();
            	        Bg.GetComponent<Image>().sprite = Resources.Load<Sprite>("formBg");
            	        Vector3 p = new Vector3 {
            	            x = bgW * drawnX + bgW / 2,
            	            y = - bgH / 2 * drawnY,
            	            z = 0.0f
            	        };
            	        Bg.GetComponent<RectTransform>().anchoredPosition3D = p;

            	        drawnX++;
            	    }
	        Debug.Log(drawnY);
	        drawnY++;
	    }
*/
        }

        // Update is called once per frame
        void Update () {

        }
    }
}
