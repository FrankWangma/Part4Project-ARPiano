﻿using System.Collections.Generic;
using UnityEngine;
using generator;
using symbol;
using util;
using xmlParser;
using Pattern;
using UnityEngine.UI;

namespace control
{
    public class CanvasControl : MonoBehaviour
    {
        private CommonParams _commonParams = CommonParams.GetInstance();
        private GameObject parentObject;
        private GameObject _overlayObject;
        private GameObject _sweeperLine;
        public GameObject _overlayCanvas;
        public GameObject _canvasScore;
        public GameObject _loadScore;
        private NoteDatabase _noteDatabase = NoteDatabase.GetInstance();
        private ParamsGetter _paramsGetter = ParamsGetter.GetInstance();
        private float _secondsPerMeasure;
        private float _nextActionTime;
        public static bool isStarted = false;
        private bool addedTime = false;
        private int paragraphNumber = 1;
        private int measureNumber = 0;
        private float speed;

        private void Update() {
            if(isStarted) {
                if(!addedTime) {
                    addedTime = true;
                    _nextActionTime += Time.time;
                    _sweeperLine = GameObject.Find("Sweeper");
                }
            
                RectTransform trans = _sweeperLine.GetComponent<RectTransform>();
                trans.anchoredPosition = new Vector2(trans.anchoredPosition.x + (speed * Time.deltaTime), trans.anchoredPosition.y);
                if(measureNumber >= 3 && Time.time > _nextActionTime) {
                    _nextActionTime += _secondsPerMeasure;
                    measureNumber = 0;
                    parentObject.transform.Find("Paragraph" + paragraphNumber).gameObject.SetActive(false);
                    paragraphNumber++;
                    Transform nextObject = parentObject.transform.Find("Paragraph" + paragraphNumber);
                    if(nextObject) {
                       nextObject.gameObject.SetActive(true);
                    } else {
                        Button startButton = GameObject.Find("startButton").gameObject.GetComponent<Button>();
                        startButton.onClick.Invoke();
                        paragraphNumber = 1;
                        parentObject.transform.Find("Paragraph1").gameObject.SetActive(true);
                    }
                    _sweeperLine = GameObject.Find("Sweeper");
                }

                if (Time.time > _nextActionTime ) {
                    _nextActionTime += _secondsPerMeasure;
                    measureNumber++;
                    Debug.Log(measureNumber);
                }
                
            }
        }

        // Called when the object is disabled 
        private void OnDisable()
        {
            // This allows for a new score to be generated without any overlap
            foreach (Transform child in parentObject.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        // Called when the object is enabled 
        private void OnEnable()
        {
            string scoreName = _commonParams.GetScoreName();
            parentObject = GameObject.Find("Canvas_Score");
            DrawScore(scoreName);
        }

        private void DrawScore(string filename)
        {
            // 解析MusicXml文件
            XmlFacade xmlFacade = new XmlFacade(filename);
            // 生成乐谱表

            _secondsPerMeasure = CalculateSecondsPerMeasure(xmlFacade.GetBeat().GetBeatsPerMeasure(), xmlFacade.GetBPM());
            _nextActionTime = _secondsPerMeasure;

            ScoreGenerator scoreGenerator =
                new ScoreGenerator(xmlFacade.GetBeat().GetBeats(), xmlFacade.GetBeat().GetBeatType());
            List<List<Measure>> scoreList = scoreGenerator.Generate(xmlFacade.GetMeasureList(), Screen.width - 67);
            // 准备绘制乐谱对象及其他参数
            List<float> screenSize = new List<float>();
            screenSize.Add(Screen.width);
            screenSize.Add(Screen.height);
            List<string> scoreInfo = new List<string>();
            // 乐谱名称和作者信息
            scoreInfo.Add(xmlFacade.GetWorkTitle()); // 0
            scoreInfo.Add(xmlFacade.GetCreator()); // 1

            // 绘制乐谱视图
            _noteDatabase.AddScoreList(scoreList);

            ScoreView scoreView = new ScoreView(scoreList, parentObject, screenSize, scoreInfo, _canvasScore, _loadScore, _overlayCanvas);
            // 更改乐符颜色
            //    Symbol symbol = scoreList[0][0].GetMeasureSymbolList()[0][1][2];
            //    SymbolControl symbolControl = new SymbolControl(symbol);
            //    symbolControl.SetColor(Color.red);
            speed = _paramsGetter.GetParagraphLength() / (_secondsPerMeasure * 4);
        }
        private float CalculateSecondsPerMeasure(string beatsPerMeasure, string BPM) {
            float secondsInMinute = 60.0f;
            float BPMeasure = float.Parse(beatsPerMeasure);
            float BPMinute = float.Parse(BPM);
            float secondsPerBeat = secondsInMinute / BPMinute;
            float secondsPerMeasure = secondsPerBeat * BPMeasure;

            return secondsPerMeasure;
        }
    }
}