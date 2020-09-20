using System.Collections.Generic;
using util;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using control;
using Pattern;
namespace symbol
{
    public class ScoreView
    {
        private List<List<Measure>> _scoreList;
        private GameObject _parentObject;
        private List<float> _screenSize;
        private List<string> _scoreInfo;
        private ParamsGetter _paramsGetter = ParamsGetter.GetInstance();
        private CommonParams _commonParams = CommonParams.GetInstance();
        private NoteDatabase _noteDatabase = NoteDatabase.GetInstance();
        private GameObject _canvasScore;
        private GameObject _loadScore;
        private List<GameObject> _paragraphs = new List<GameObject>();
        private GameObject _overlayCanvas;
        private List<string> _whiteKeyText;
        private List<string> _blackKeyText;
        private GameObject _panel;
        private static List<GameObject> _pianoKeys = new List<GameObject>();
        public ScoreView(List<List<Measure>> scoreList, GameObject parentObject, List<float> screenSize, List<string> scoreInfo, GameObject canvasScore, GameObject loadScore, GameObject overlayCanvas)
        {
            _parentObject = parentObject;
            _scoreList = scoreList;
            _screenSize = screenSize;
            _scoreInfo = scoreInfo;
            _canvasScore = canvasScore;
            _loadScore = loadScore;
            _overlayCanvas = overlayCanvas;
            string[] whiteKeyInput = {"C", "D", "E", "F", "G", "A", "B"};
            _whiteKeyText = new List<string>();
            _whiteKeyText.AddRange(whiteKeyInput);

            string[] blackKeyInput = {"Db\nC#", "Eb\nD#", "Gb\nF#", "Ab\nG#", "Bb\nA#",};
            _blackKeyText = new List<string>();
            _blackKeyText.AddRange(blackKeyInput);
            _pianoKeys.Clear();
            Init();
        }

        private void Init()
        {
            OnDraw();
        }

        private void OnDraw()
        {
            // 放置两个按钮
            PlaceButton();
            // 绘制乐谱信息
            DrawScoreInfo();

            // 绘制乐谱内容
            float startX = 67f / 2;
            float startY = _screenSize[1] - 250;
            Vector3 paragraphPosition = new Vector3(startX, startY, 0);
            // 遍历scoreList，对每一行来绘制
            for (int i = 0; i < _scoreList.Count; i++)
            {
                // 新建paragraph画布，每一行有自己的画布
                string objName = "Paragraph" + (i + 1);
                GameObject paragraphObject = new GameObject(objName);
                Canvas paragraphCanvas = paragraphObject.AddComponent<Canvas>();
                paragraphCanvas.transform.SetParent(_parentObject.transform);
                RectTransform rect = paragraphCanvas.GetComponent<RectTransform>();
                // 设置位置为以画布左下角为坐标原点
                rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.zero; rect.pivot = new Vector2(0.5f, 0.5f);
                int offset = 0;
                if(i > 0) {
                    offset = -2 * _paramsGetter.GetTotalHeight() * 1;
                }
                rect.position = new Vector3(paragraphPosition.x,
                    paragraphPosition.y + offset,
                    paragraphPosition.z);

                // 将paragraph画布对象赋为下一层的父对象
                // 绘制每一行的视图'
                //Debug.Log("ScoreList" + _scoreList.Count);
                ParagraphView paragraphView = new ParagraphView(_scoreList[i], paragraphObject);
                _paragraphs.Add(paragraphObject);
            }

            DisableParagraphs();
            DrawPianoKeys();
            DrawHelperCanvas();
        }

        public Dictionary<GameObject,Color> changePianoKeyColor(List<Color> colors, List<Note> notes) {
            int lowestOctave = 100;
            foreach(Note note in notes) {
                if(int.Parse(note.GetOctave())  < lowestOctave) {
                    lowestOctave = int.Parse(note.GetOctave());
                }
            }
            Dictionary<GameObject,Color> keysAndColors = new Dictionary<GameObject,Color>();
            Debug.Log("colors: " + colors.Count + " Notes: " + notes.Count);
            // foreach(Color color in colors){
            //     Debug.Log("My Color " + color);
            // }
            // foreach(Note note in notes){
            //     Debug.Log("Note " + note.GetStep());
            // }
            for(int i = 0; i < colors.Count; i++) {
                Note note = notes[i];
                GameObject key;
                if(int.Parse(note.GetOctave()) == lowestOctave) {
                    key = _pianoKeys.Find(x => x.gameObject.name.Equals(note.GetStep()));
                } else {
                    key = _pianoKeys.Find(x => {
                            string name = note.GetStep();
                            if(x.gameObject.name.Equals(name + "1")) {
                                return x;
                            }
                            return false;
                        }
                    );
                }
                Image image = key.GetComponent<Image>();
                if(!keysAndColors.ContainsKey(key)) {
                    keysAndColors.Add(key, image.color);
                }
                image.color = colors[i];
            }

            return keysAndColors;
        }

        private void DisableParagraphs() {
            // Disable every paragraph, except for first 2
            for(int i = 2; i < _paragraphs.Count; i++) {
                if(_paragraphs[i] != false) {
                    _paragraphs[i].SetActive(false);
                }
            }
        }

        // 绘制乐谱信息
        private void DrawScoreInfo()
        {
            Vector2 worktitlePosition = new Vector2(_screenSize[0] / 2, _screenSize[1] - 50);
            Vector2 creatorPosition = new Vector2(_screenSize[0] - 50, _screenSize[1] - 75);

            DrawText(_scoreInfo[0], worktitlePosition, 30);
        }

        private void DrawText(string text, Vector2 position, int fontSize)
        {
            GameObject textObject = GameObject.Instantiate(_commonParams.GetPrefabText(),
                _parentObject.transform.position,
                _commonParams.GetPrefabText().transform.rotation);
            textObject.transform.SetParent(_parentObject.transform);
            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(500, 100);
            rect.position = new Vector3(position.x, position.y, 0);
            Text objectText = textObject.GetComponent<Text>();
            objectText.fontSize = fontSize;
            objectText.text = text;
        }

        private void DrawPianoKeys() {
            int width = 100;
            int height = 400;
            Vector2 position = new Vector2(_screenSize[0] / 2, _screenSize[1] - 250);
            GameObject piano = new GameObject();
            piano.transform.SetParent(_parentObject.transform);
            piano.gameObject.name = "Piano";
            Vector3 pianoKeyPositioning = new Vector3((position.x - 6.5f * width), position.y - 2 * _paramsGetter.GetTotalHeight() * 3, 0);
            for(int i = 0; i < 14; i++) {
                GameObject whiteKey = GameObject.Instantiate(_commonParams.GetPrefabPianoKey(),
                    _parentObject.transform.position,
                    _commonParams.GetPrefabPianoKey().transform.rotation);

                Text whiteKeyText = whiteKey.transform.GetChild(0).gameObject.GetComponent<Text>();
                RectTransform textRect = whiteKeyText.gameObject.GetComponent<RectTransform>();
                textRect.position = new Vector3(textRect.position.x, textRect.position.y - height * 0.75f, 0);
                textRect.sizeDelta = new Vector2(width,height);
                whiteKeyText.color = Color.black;
                whiteKeyText.fontSize = 30;

                if(i >= 7) {
                    whiteKey.gameObject.name =  _whiteKeyText[i % _whiteKeyText.Count] + "1";
                } else {
                    whiteKey.gameObject.name =  _whiteKeyText[i % _whiteKeyText.Count];
                }
                whiteKeyText.text = _whiteKeyText[i % _whiteKeyText.Count];

                whiteKey.transform.SetParent(piano.transform);
                RectTransform rect = whiteKey.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(width,height);
                rect.position = new Vector3(pianoKeyPositioning.x + width * i, pianoKeyPositioning.y, 0);
                
                _pianoKeys.Add(whiteKey);
            }

            int offset = width / 2;
            for(int i = 0; i < 10; i++) {
                 GameObject blackKey = GameObject.Instantiate(_commonParams.GetPrefabPianoKey(),
                    _parentObject.transform.position,
                    _commonParams.GetPrefabPianoKey().transform.rotation);
                blackKey.transform.SetParent(piano.transform);
                Image image = blackKey.GetComponent<Image>();
                image.color = Color.black;
                RectTransform rect = blackKey.GetComponent<RectTransform>();

                Text blackKeyText = blackKey.transform.GetChild(0).gameObject.GetComponent<Text>();
                blackKeyText.color = Color.white;
                blackKeyText.fontSize = 30;
                RectTransform textRect = blackKeyText.gameObject.GetComponent<RectTransform>();
                textRect.position = new Vector3(textRect.position.x, textRect.position.y - height / 2, 0);
                textRect.sizeDelta = new Vector2(width,height);

                string name = _blackKeyText[i % _blackKeyText.Count];
                if(i >= 5) {
                     blackKey.gameObject.name = name.Substring(3,2) + "1";
                } else {
                     blackKey.gameObject.name = name.Substring(3,2);
                }
                blackKeyText.text = name;

                if (name.Contains("#")) {
                    blackKey.gameObject.name = blackKey.gameObject.name.Replace("#", "sharp");
                }   

                rect.sizeDelta = new Vector2(width - 20, height - 170);
                if (i == 2 || i == 5 || i == 7) {
                    offset += width;
                }
                rect.position = new Vector3((pianoKeyPositioning.x + width * i) + offset, pianoKeyPositioning.y + (rect.sizeDelta.y / 3), 0);           
                _pianoKeys.Add(blackKey);
            }
        }

        private void DrawHelperCanvas() {
            _panel = GameObject.Instantiate(_commonParams.GetPrefabPanel(),
                    _parentObject.transform.position,
                    _commonParams.GetPrefabPanel().transform.rotation);
            _panel.transform.SetParent(_parentObject.transform);
            RectTransform rect = _panel.GetComponent<RectTransform>();
            rect.offsetMin = new Vector2(_screenSize[0] / 3, _screenSize[1] / 3);
            rect.offsetMax = new Vector2(-_screenSize[0] / 3, -_screenSize[1] / 3);

            for(int i = 0; i < _whiteKeyText.Count; i++) {
                Color myColor = Color.black;
                string chord = _whiteKeyText[i];

                // Determine color of text
                switch (chord)
                {
                    case "A": myColor = Color.yellow; break;
                    case "B": myColor = Color.magenta; break; //Pink
                    case "C": myColor = Color.green; break; //C is Green
                    case "D": myColor = Color.blue; break; //D is blue
                    case "E": myColor = new Color(0.52F, 0.16F, 0.89F); break; //Purple
                    case "F": myColor = Color.red; break;
                    case "G": myColor = new Color(1.0F, 0.4F, 0.0F); break; //Orange
                }
                
                // Draw the text
                 GameObject textObject = GameObject.Instantiate(_commonParams.GetPrefabText());
                textObject.transform.SetParent(_panel.transform);
                Text text = textObject.GetComponent<Text>();
                text.text = chord + " Chord";
                text.fontSize = 20;
                RectTransform textRect = textObject.GetComponent<RectTransform>();
                textRect.anchoredPosition = new Vector3(0,-72 - 30 * i,0);
                textRect.anchorMin = new Vector2(0.5f, 1);
                textRect.anchorMax = new Vector2(0.5f, 1);
                textRect.sizeDelta = new Vector2(100, 25);

                // Draw the square representing the color
                GameObject imageObject = GameObject.Instantiate(_commonParams.GetPrefabPianoKey());
                imageObject.transform.SetParent(_panel.transform);
                Image image = imageObject.GetComponent<Image>();
                image.color = myColor;
                RectTransform imageRect = imageObject.GetComponent<RectTransform>();
                imageRect.anchoredPosition = new Vector3(-textRect.sizeDelta.x / (3.0f/2.0f),-72 - 30 * i,0);
                imageRect.anchorMin = new Vector2(0.5f, 1);
                imageRect.anchorMax = new Vector2(0.5f, 1);
                imageRect.sizeDelta = new Vector2(25, 25);
                imageObject.transform.GetChild(0).gameObject.SetActive(false);

                _panel.SetActive(false);
                //Take Care of Button
                GameObject backButtonObject = _panel.transform.Find("Button").gameObject;
                Button backButton = backButtonObject.GetComponent<Button>();
                backButton.onClick.AddListener(delegate
                {
                    _panel.gameObject.SetActive(false);
                });
            }
        }

        // 放置两个button按钮作为返回上一个场景，以及退出
        private void PlaceButton()
        {
            // 返回按钮
            //Not working?
            GameObject backButtonObject = GameObject.Instantiate(_commonParams.GetPrefabFileButton(),
                _parentObject.transform.position, _commonParams.GetPrefabFileButton().transform.rotation);
            backButtonObject.gameObject.name = "backButton";
            backButtonObject.transform.SetParent(_parentObject.transform);
            RectTransform backRect = backButtonObject.GetComponent<RectTransform>();
            backRect.position = new Vector3(50, _screenSize[1] - 50, 0);
            backRect.sizeDelta = new Vector2(50, 30);
            Text backText = backButtonObject.GetComponentInChildren<Text>();
            backText.text = "Back";
            Button backButton = backButtonObject.GetComponent<Button>();
            backButton.onClick.AddListener(delegate
            {
                //SceneManager.LoadScene("LoadScore", LoadSceneMode.Additive);
                //SceneManager.UnloadSceneAsync("DrawScore");
                _canvasScore.SetActive(false);
                _loadScore.SetActive(true);
                _overlayCanvas.SetActive(false);
                _noteDatabase.Clear();
            });

            GameObject helpButtonObject = GameObject.Instantiate(_commonParams.GetPrefabFileButton(),
                _parentObject.transform.position, _commonParams.GetPrefabFileButton().transform.rotation);
            helpButtonObject.transform.SetParent(_parentObject.transform);
            RectTransform helpRect = helpButtonObject.GetComponent<RectTransform>();
            helpRect.position = new Vector3(_screenSize[0] - 50, _screenSize[1] - 50, 0);
            helpRect.sizeDelta = new Vector2(50, 30);
            Text helpText = helpButtonObject.GetComponentInChildren<Text>();
            helpText.text = "Help";
            Button helpButton = helpButtonObject.GetComponent<Button>();
            helpButton.onClick.AddListener(delegate
            {
                _panel.gameObject.SetActive(true);
            });

            GameObject startButtonObject = GameObject.Instantiate(_commonParams.GetPrefabFileButton(),
                _parentObject.transform.position, _commonParams.GetPrefabFileButton().transform.rotation);
            startButtonObject.name = "startButton";
            startButtonObject.transform.SetParent(_parentObject.transform);
            RectTransform startRect = startButtonObject.GetComponent<RectTransform>();
            startRect.position = new Vector3(100, _screenSize[1] - 50, 0);
            startRect.sizeDelta = new Vector2(50, 30);
            Text startText = startButtonObject.GetComponentInChildren<Text>();
            startText.text = "Start";
            Button startButton = startButtonObject.GetComponent<Button>();
            startButton.onClick.AddListener(delegate
            {
                if(control.CanvasControl.isStarted) {
                    startText.text = "Start";
                    control.CanvasControl.isStarted = false;
                    backButton.enabled = true;
                    helpButton.enabled = true;
                } else {
                    control.CanvasControl.isStarted = true;
                    startText.text = "Pause";
                    backButton.enabled = false;
                    helpButton.enabled = false;
                }
            });
        }
    }
}