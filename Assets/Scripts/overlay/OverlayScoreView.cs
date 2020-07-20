using System.Collections.Generic;
using util;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using symbol;

namespace overlay
{
    public class OverlayScoreView
    {
        private List<List<Measure>> _scoreList;
        private GameObject _parentObject;
        private List<float> _screenSize;
        private List<string> _scoreInfo;
        private ParamsGetter _paramsGetter = ParamsGetter.GetInstance();
        private CommonParams _commonParams = CommonParams.GetInstance();
        private GameObject _canvasScore;
        private GameObject _loadScore;
        private OverlayParagraph _overlayParagraph;
        private OverlayMaster _overlayMaster = OverlayMaster.GetInstance();

        //testing purposes
        private Symbol _symbol;
        private int position = 0;

        public OverlayScoreView(List<List<Measure>> scoreList, GameObject parentObject, List<float> screenSize, List<string> scoreInfo, GameObject canvasScore, GameObject loadScore)
        {
            _parentObject = parentObject;
            _scoreList = scoreList;
            _screenSize = screenSize;
            _scoreInfo = scoreInfo;
            _canvasScore = canvasScore;
            _loadScore = loadScore;
            Init();
        }

        private void Init()
        {
            //_symbol =new Note("D", "5");
            _symbol =new Note("D", "4");
            _symbol.SetChord(false);
            _symbol.SetDuration("256", "256");
            _symbol.SetType("quarter");
            ((Note) _symbol).SetUpOrDown(true);
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
                rect.position = new Vector3(paragraphPosition.x,
                    paragraphPosition.y - 2 * _paramsGetter.GetTotalHeight() * i,
                    paragraphPosition.z);

                // 将paragraph画布对象赋为下一层的父对象
                // 绘制每一行的视图'
                //Debug.Log("ScoreList" + _scoreList.Count);
               _overlayParagraph = new OverlayParagraph(_scoreList[i], paragraphObject);
            }
            _overlayMaster.SetScoreView(this);
        }

        // 绘制乐谱信息
        private void DrawScoreInfo()
        {
            Vector2 worktitlePosition = new Vector2(_screenSize[0] / 2, _screenSize[1] - 50);
            Vector2 creatorPosition = new Vector2(_screenSize[0] - 50, _screenSize[1] - 75);

            DrawText(_scoreInfo[0], worktitlePosition, 30);
            DrawText(_scoreInfo[1], creatorPosition, 10);
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

        // 放置两个button按钮作为返回上一个场景，以及退出
        private void PlaceButton()
        {
            // 返回按钮
            GameObject backButtonObject = GameObject.Instantiate(_commonParams.GetPrefabFileButton(),
                _parentObject.transform.position, _commonParams.GetPrefabFileButton().transform.rotation);
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
            });

            // 退出按钮
            GameObject exitButtonObject = GameObject.Instantiate(_commonParams.GetPrefabFileButton(),
                _parentObject.transform.position, _commonParams.GetPrefabFileButton().transform.rotation);
            exitButtonObject.transform.SetParent(_parentObject.transform);
            RectTransform exitRect = exitButtonObject.GetComponent<RectTransform>();
            exitRect.position = new Vector3(_screenSize[0] - 50, _screenSize[1] - 50, 0);
            exitRect.sizeDelta = new Vector2(50, 30);
            Text exitText = exitButtonObject.GetComponentInChildren<Text>();
            exitText.text = "Exit";
            Button exitButton = exitButtonObject.GetComponent<Button>();
            exitButton.onClick.AddListener(delegate
            {
                Application.Quit();
            });

            //Button to create single note
            GameObject noteButtonObject = GameObject.Instantiate(_commonParams.GetPrefabFileButton(),
                _parentObject.transform.position, _commonParams.GetPrefabFileButton().transform.rotation);
            noteButtonObject.transform.SetParent(_parentObject.transform);
            RectTransform noteRect = noteButtonObject.GetComponent<RectTransform>();
            noteRect.position = new Vector3(_screenSize[0]/2, _screenSize[1] - 500, 0);
            noteRect.sizeDelta = new Vector2(50, 30);
            Text noteText = noteButtonObject.GetComponentInChildren<Text>();
            noteText.text = "Add Note";
            Button noteButton = noteButtonObject.GetComponent<Button>();
            noteButton.onClick.AddListener(delegate
            {
                _overlayMaster.ModifySetView(position, _symbol, true);
                position++;
            });
        }
    }
}