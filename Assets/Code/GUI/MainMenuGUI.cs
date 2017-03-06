using System;
using Code.Network;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Code.GameRegulars;
using Random = UnityEngine.Random;

namespace Code.GUI {
    public class MainMenuGUI : CoreGUI {
        private const float ButtonWidth = 300.0f;
        private const float ButtonHeight = 80.0f;
        private readonly float _indent = ButtonHeight + PercHeight(4.0f);
        private readonly GUIElement _rndNameButton;
        private readonly GUIElement _newGameButton;
        private readonly GUIElement _netGameButton;
        private readonly GUIElement _hostGameButton;
        private readonly GUIElement _joinGameButton;
        private readonly GUIElement _quitGameButton;
        public string PlayerName;

        public MainMenuGUI() {
            _netGameButton = new GUIElement {
                Name = NetGameButton,
                Action = NetGameClick
            };
            _rndNameButton = new GUIElement {
                Name = "RandomNameButton",
                Action = RandomNameClick
            };
            _newGameButton = new GUIElement {
                Name = NewGameButton,
                ChildName = NewGameButton + ButtonText,
                Parent = OverlayMiddle,
                Texture = "button",
                Text = LN_NewGameButton,
                Font = "lb_font",
                FontSize = 36,
                Layer = 5,
                Width = ButtonWidth,
                Height = ButtonHeight,
                X = - Screen.width / 2.0f + ButtonWidth / 2.0f + PercWidth(2.0f),
                Y = Screen.height / 2.0f - ButtonHeight / 2.0f - PercHeight(10.0f),
                Action = NewGameClick
            };
            _hostGameButton = new GUIElement {
                Name = HostGameButton,
                ChildName = HostGameButton + ButtonText,
                Parent = OverlayMiddle,
                Texture = "button",
                Text = LN_HostGameButton,
                Font = "lb_font",
                FontSize = 36,
                Layer = 5,
                Width = ButtonWidth,
                Height = ButtonHeight,
                X = _newGameButton.X,
                Y = _newGameButton.Y - _indent,
                Action = HostGameClick
            };
            _joinGameButton = new GUIElement {
                Name = JoinGameButton,
                ChildName = JoinGameButton + ButtonText,
                Parent = OverlayMiddle,
                Texture = "button",
                Text = LN_JoinGameButton,
                Font = "lb_font",
                FontSize = 36,
                Layer = 5,
                Width = ButtonWidth,
                Height = ButtonHeight,
                X = _hostGameButton.X,
                Y = _hostGameButton.Y - _indent,
                Action = JoinGameClick
            };
            _quitGameButton = new GUIElement {
                Name = QuitGameButton,
                ChildName = QuitGameButton + ButtonText,
                Parent = OverlayMiddle,
                Texture = "button",
                Text = LN_QuitGameButton,
                Font = "lb_font",
                FontSize = 36,
                Layer = 5,
                Width = ButtonWidth,
                Height = ButtonHeight,
                X = _joinGameButton.X,
                Y = _joinGameButton.Y - _indent,
                Action = QuitGameClick
            };
        }
        public static void SwitchPanels(string active, string inactive) {
            SetVisible(active);
            SetInvisible(inactive);
        }
        private bool GetPlayerName() {
            string n = GameObject.Find(PlayerNameInputField).GetComponent<InputField>().text;
            if (n == string.Empty) return false;
            PlayerName = n;
            return true;
        }
        public static void LogJoin(string msg) {
            GameObject log = GameObject.Find(JoinLogField);
            log.GetComponent<Text>().text = msg;
        }
        public static void LogHost(string msg) {
            GameObject log = GameObject.Find(HostLogField);
            log.GetComponent<Text>().text = msg;
        }

        public void Make() {
            SwitchPanels(PanelMultiplayer, PanelLobby);
            GenerateName();
            AddListener(_rndNameButton);
            AddListener(_newGameButton);
            AddListener(_netGameButton);
            AddListener(_joinGameButton);
            AddListener(_hostGameButton);
            AddListener(_quitGameButton);
            /*
            InitTextButton(_newGameButton);
            InitTextButton(_hostGameButton);
            InitTextButton(_joinGameButton);
            InitTextButton(_quitGameButton);*/
        }

        private void GenerateName() {
            var genName = string.Empty;
            var rnd = Random.value;
            var IsMale = rnd > 0.5f;
            rnd = Random.value;
            var UseNoblePart = rnd > 0.5f;
            string randTitle = string.Empty, randNobPart = " ", randName = string.Empty;
            switch (IsMale) {
                case true:
                    randTitle = ngTitleM[Random.Range(0, ngTitleM.Length)];
                    if (UseNoblePart) randNobPart = ngNobPart[Random.Range(0, ngNobPart.Length)];
                    randName = ngNameM[Random.Range(0, ngNameM.Length)];
                    break;
                case false:
                    randTitle = ngTitleF[Random.Range(0, ngTitleF.Length)];
                    if (UseNoblePart) randNobPart = ngNobPart[Random.Range(0, ngNobPart.Length)];
                    randName = ngNameF[Random.Range(0, ngNameF.Length)];
                    break;
            }
            genName = randTitle + randNobPart + randName;
            GameObject.Find(PlayerNameInputField).GetComponent<InputField>().text = genName;
        }

        private void RandomNameClick() {
            GenerateName();
        }

        private void NewGameClick() {
            SceneManager.LoadScene(SceneGame);
            Net.Game.SetOffline();
        }
        private void NetGameClick() {
            SetVisibility(PanelNetGame, 22.0f);
        }
        private void HostGameClick() {
            if (!GetPlayerName()) {
                LogHost(NoPlayerNameError);
                return;
            }

            LogHost(CreatingServer);
            SwitchPanels(PanelLobby, PanelMultiplayer);

        }
        private void JoinGameClick() {
            if (!GetPlayerName()) {
                LogJoin(NoPlayerNameError);
                return;
            }

            LogJoin(JoiningServer);
        }
        private void QuitGameClick() {
            Application.Quit();
        }
    }
}
