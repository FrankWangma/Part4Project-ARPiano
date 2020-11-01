using System.IO;
using util;
using UnityEngine;
using UnityEngine.UI;

namespace control
{
    public class LoadScoreControl : MonoBehaviour
    {
        private CommonParams _commonParams = CommonParams.GetInstance();
        private GameObject _prefabSymbol;
        private GameObject _prefabText;
        private GameObject _prefabLine;
        private GameObject _prefabFileButton;
        private GameObject _prefabPianoKey;
        private GameObject _prefabPanel;
        private GameObject _background;
        public GameObject _canvasScore;
        public GameObject _loadScore;
        //Test canvas to show dynamically changing template. Merge with _canvasScore later for overlay
        public GameObject _testCanvas;

        private void Awake()
        {
            _prefabSymbol = (GameObject)Resources.Load("Prefabs/Prefab_Symbol");
            _prefabText = (GameObject)Resources.Load("Prefabs/Prefab_Text");
            _prefabLine = (GameObject)Resources.Load("Prefabs/Prefab_Line");
            _prefabFileButton = (GameObject)Resources.Load("Prefabs/Prefab_FileButton");
            _prefabPianoKey = (GameObject)Resources.Load("Prefabs/Prefab_PianoKey");
            _prefabPanel = (GameObject)Resources.Load("Prefabs/Prefab_Panel");
            _background = (GameObject)Resources.Load("Prefabs/BackgroundPanel");

            _commonParams.SetPrefabSymbol(_prefabSymbol);
            _commonParams.SetPrefabText(_prefabText);
            _commonParams.SetPrefabLine(_prefabLine);
            _commonParams.SetPrefabFileButton(_prefabFileButton);
            _commonParams.SetPrefabPianoKey(_prefabPianoKey);
            _commonParams.SetPrefabPanel(_prefabPanel);
            _commonParams.SetBackgroundPanel(_background);
        }

        // Use this for initialization
        void Start()
        {
            _commonParams.SetPrefabSymbol(_prefabSymbol);
            _commonParams.SetPrefabText(_prefabText);
            _commonParams.SetPrefabLine(_prefabLine);
            _commonParams.SetPrefabFileButton(_prefabFileButton);
            _commonParams.SetPrefabPianoKey(_prefabPianoKey);
            _commonParams.SetPrefabPanel(_prefabPanel);
            _commonParams.SetBackgroundPanel(_background);
            LoadScore();
        }

        private void LoadScore()
        {
            GameObject canvasObject = GameObject.Find("Canvas_Menu");

            DirectoryInfo xmlFolder = new DirectoryInfo(_commonParams.GetXmlFolderPath());

            GameObject backgroundPanel = GameObject.Instantiate(_commonParams.GetBackgroundPanel());
            backgroundPanel.name = "BackgroundPanel";
            backgroundPanel.transform.SetParent(canvasObject.transform);
            RectTransform backgroundRect = backgroundPanel.GetComponent<RectTransform>();
            backgroundRect.offsetMin = new Vector2(0, 0);
            backgroundRect.offsetMax = new Vector2(0, 0);

            int xmlFileCount = 0;
            Vector3 buttonPosition = new Vector3(Screen.width / 2, Screen.height - 100, 0);
            foreach (FileInfo xmlFile in xmlFolder.GetFiles())
            {
                if (xmlFile.Extension == ".xml")
                {
                    if (xmlFileCount >= 9) //TODO If theres too many button, not all buttons show
                    {
                        break;
                    }

                    xmlFileCount += 1;

                    string buttonName = "Button" + xmlFileCount;
                    GameObject buttonObject = GameObject.Instantiate(_commonParams.GetPrefabFileButton(),
                        canvasObject.transform.position, _commonParams.GetPrefabFileButton().transform.rotation);
                    buttonObject.name = buttonName;
                    buttonObject.transform.SetParent(canvasObject.transform);
                    RectTransform rect = buttonObject.GetComponent<RectTransform>();

                    rect.position = new Vector3(buttonPosition.x,
                        buttonPosition.y - 50 * xmlFileCount,
                        buttonPosition.z);
                    Text btnText = buttonObject.GetComponentInChildren<Text>();
                    btnText.text = xmlFile.Name.Replace(xmlFile.Extension, "");

                    Button button = buttonObject.GetComponent<Button>();
                    button.onClick.AddListener(delegate
                    {
                        _commonParams.SetScoreName(xmlFile.FullName);
                        _canvasScore.SetActive(true);
                        _loadScore.SetActive(false);
                    });
                }
            }
        }
    }
}