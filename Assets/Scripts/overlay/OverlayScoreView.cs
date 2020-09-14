﻿using System.Collections.Generic;
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
        private ParamsGetter _paramsGetter = ParamsGetter.GetInstance();
        private CommonParams _commonParams = CommonParams.GetInstance();
        private GameObject _canvasScore;
        private GameObject _loadScore;
        private OverlayParagraph _overlayParagraph;
        private OverlayMaster _overlayMaster = OverlayMaster.GetInstance();

        //testing purposes
        private Symbol _symbol;
        private int position = 0;

        public OverlayScoreView(List<List<Measure>> scoreList, GameObject parentObject, List<float> screenSize, GameObject canvasScore, GameObject loadScore)
        {
            _parentObject = parentObject;
            _scoreList = scoreList;
            _screenSize = screenSize;
            _canvasScore = canvasScore;
            _loadScore = loadScore;
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

        // 放置两个button按钮作为返回上一个场景，以及退出
        private void PlaceButton()
        {
            //Button to create single note
            GameObject noteButtonObject = GameObject.Instantiate(_commonParams.GetPrefabFileButton(),
                _parentObject.transform.position, _commonParams.GetPrefabFileButton().transform.rotation);
            noteButtonObject.transform.SetParent(_parentObject.transform);
            RectTransform noteRect = noteButtonObject.GetComponent<RectTransform>();
            noteRect.position = new Vector3(_screenSize[0] / 2, _screenSize[1] - 500, 0);
            noteRect.sizeDelta = new Vector2(50, 30);
            Text noteText = noteButtonObject.GetComponentInChildren<Text>();
            noteText.text = "Add Note";
            Button noteButton = noteButtonObject.GetComponent<Button>();
            noteButton.onClick.AddListener(delegate
            {
                _symbol = new Note("C", "4");
                _symbol.SetChord(false);
                _symbol.SetDuration("256", "256");
                _symbol.SetType("quarter");
                ((Note)_symbol).SetUpOrDown(false);
                _overlayMaster.ModifySetView(position, _symbol, false);
                //position++;
                //startTime = startTime + 8;
                //if (startTime >= 64)
                //{
                //    position++;
                //    startTime = 0;
                //}
            });
        }
    }
}