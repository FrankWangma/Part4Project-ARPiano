﻿using System.Collections.Generic;
using util;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        private GameObject _canvasScore;
        private GameObject _loadScore;

        public ScoreView(List<List<Measure>> scoreList, GameObject parentObject, List<float> screenSize, List<string> scoreInfo, GameObject canvasScore, GameObject loadScore)
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
            OnDraw();
            changeChildren(_parentObject.transform);
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
                rect.anchorMin = new Vector2(0.5f, 0.5f); rect.anchorMax = new Vector2(0.5f, 0.5f); rect.pivot = new Vector2(0.0f, 0.0f);
                rect.anchoredPosition3D = new Vector3(700, i * -150 + 100, 0);
                rect.localEulerAngles = new Vector3(0,180,0);
                // 将paragraph画布对象赋为下一层的父对象
                // 绘制每一行的视图
                ParagraphView para = new ParagraphView(_scoreList[i], paragraphObject);
                foreach(Transform child in paragraphObject.transform)
                {
                    child.transform.localEulerAngles = new Vector3(0,0,0);
                }
            }
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
            //rect.sizeDelta = new Vector2(500, 100);
            rect.anchoredPosition3D = new Vector3(800,700, 0);
            textObject.transform.localEulerAngles = new Vector3(0,180,0);

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
            //backRect.position = new Vector3(50, _screenSize[1] - 50, 0);
            //backRect.sizeDelta = new Vector2(50, 30);
            backRect.anchoredPosition3D = new Vector3(1100, 700, 0);
            Text backText = backButtonObject.GetComponentInChildren<Text>();
            backText.text = "Back";
            backButtonObject.transform.localEulerAngles = new Vector3(0,180,0);
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
            //exitRect.position = new Vector3(_screenSize[0] - 50, _screenSize[1] - 50, 0);
            //exitRect.sizeDelta = new Vector2(50, 30);
            exitRect.anchoredPosition3D = new Vector3(400, 700, 0);
            Text exitText = exitButtonObject.GetComponentInChildren<Text>();
            exitText.text = "Exit";
            exitButtonObject.transform.localEulerAngles = new Vector3(0,180,0);
            Button exitButton = exitButtonObject.GetComponent<Button>();
            exitButton.onClick.AddListener(delegate
            {
                Application.Quit();
            });
        }

        private void changeChildren(Transform parent) 
        {
            if (parent.childCount != 0)
            {
                foreach(Transform child in parent.transform)
                {
                    child.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                    changeChildren(child);
                }
            }
        }
    }
}