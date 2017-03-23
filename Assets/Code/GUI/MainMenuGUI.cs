using System;
using System.IO;
using System.Linq;
using Code.Network;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Code.Game.Data.GameRegulars;
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
        private readonly GUIElement _saveNameButton;
        private readonly GUIElement _saveServerButton;
        public string PlayerName;

        public static GameObject PlayerNameField;
        public static GameObject ServerAddressField;
        public static GameObject ServerPortField;

        public MainMenuGUI() {
            _netGameButton = new GUIElement {
                Name = NetGameButton,
                Action = NetGameClick
            };
            _rndNameButton = new GUIElement {
                Name = "RandomNameButton",
                Action = RandomNameClick
            };
            _saveNameButton = new GUIElement {
                Name = "SaveNameButton",
                Action = SaveName
            };
            _saveServerButton = new GUIElement {
                Name = "SaveServerButton",
                Action = SaveServer
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
            PlayerNameField = GameObject.Find(PlayerNameInputField);
            ServerAddressField = GameObject.Find(IPField);
            ServerPortField = GameObject.Find(PortField);

            SwitchPanels(PanelMultiplayer, PanelLobby);
            LoadData();
            AddListener(_rndNameButton);
            AddListener(_newGameButton);
            AddListener(_netGameButton);
            AddListener(_joinGameButton);
            AddListener(_hostGameButton);
            AddListener(_quitGameButton);
            AddListener(_quitGameButton);
            AddListener(_saveNameButton);
            AddListener(_saveServerButton);
            /*
            InitTextButton(_newGameButton);
            InitTextButton(_hostGameButton);
            InitTextButton(_joinGameButton);
            InitTextButton(_quitGameButton);*/
        }

        public static void LoadData() {
            string path;

            path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\karkas\\favName";
            if (File.Exists(path)) {
                var loadedName = File.ReadAllLines(path)[0];
                if (string.IsNullOrEmpty(loadedName)) GenerateName();
                else PlayerNameField.GetComponent<InputField>().text = loadedName;
            } else GenerateName();

            path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\karkas\\favServer";
            if (File.Exists(path)) {
                var loadedServerData = File.ReadAllLines(path);
                ServerAddressField.GetComponent<InputField>().text = loadedServerData[0];
                ServerPortField.GetComponent<InputField>().text = loadedServerData[1];
            }
        }

        public static void SaveName() {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\karkas\\favName";
            File.WriteAllText(path, PlayerNameField.GetComponent<InputField>().text);
        }

        public static void SaveServer() {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\karkas\\favServer";
            File.WriteAllText(path, ServerAddressField.GetComponent<InputField>().text + Environment.NewLine +
                                    ServerPortField.GetComponent<InputField>().text);
        }

        public static string GetRandomName() {
            var genName = string.Empty;
            var rnd = Random.value;
            var isMale = rnd > 0.5f;
            rnd = Random.value;
            var useNoblePart = rnd > 0.5f;
            string randTitle = string.Empty, randNobPart = " ", randName = string.Empty;
            switch (isMale) {
                case true:
                    randTitle = ngTitleM[Random.Range(0, ngTitleM.Length)];
                    if (useNoblePart) randNobPart = ngNobPart[Random.Range(0, ngNobPart.Length)];
                    randName = ngNameM[Random.Range(0, ngNameM.Length)];
                    break;
                case false:
                    randTitle = ngTitleF[Random.Range(0, ngTitleF.Length)];
                    if (useNoblePart) randNobPart = ngNobPart[Random.Range(0, ngNobPart.Length)];
                    randName = ngNameF[Random.Range(0, ngNameF.Length)];
                    break;
            }
            return randTitle + randNobPart + randName;
        }

        private static void GenerateName() {
            var genName = GetRandomName();
            PlayerNameField.GetComponent<InputField>().text = genName;
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
