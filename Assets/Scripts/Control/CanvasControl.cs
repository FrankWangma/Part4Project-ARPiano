﻿using System.Collections.Generic;
using UnityEngine;
using generator;
using symbol;
using util;
using xmlParser;
using Pattern;
using summary;
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
        private ScoreView _scoreView;
        private NoteDatabase _noteDatabase = NoteDatabase.GetInstance();
        private ParamsGetter _paramsGetter = ParamsGetter.GetInstance();
        private SummaryMaster _summaryMaster = SummaryMaster.GetInstance();
        private float _secondsPerMeasure;
        private float _nextActionTime = 0;
        private int _numberOfPatterns = 1;
        public static bool isStarted = false;
        private bool addedTime = false;
        private int _paragraphNumber = 1;
        private int _measureNumber = 0;
        private int _measureTotal = 0;
        private int index = 0;
        private int _noteIndex = 0;
        private string _fifth = "";
        private float speed;
        private float timeRemaining = 4;
        private Text timeText;
        private float paragraphStartX = 67f / 2;
        private float paragraphStartY = Screen.height - 250;
        public static List<List<List<Note>>> _notes;
        private Dictionary<GameObject, Color> _oldKeys;
        private bool _reset = false;
        private float _patternSplitSeconds = 0;
        private int _patternIteration = 1;

        private void Update()
        {
            if (isStarted)
            {
                if (timeRemaining > 1)
                {
                    timeText.gameObject.SetActive(true);
                    timeRemaining -= Time.deltaTime;
                    DisplayTime(timeRemaining);
                }
                else
                {
                    if (timeText.gameObject.activeSelf)
                    {
                        timeText.gameObject.SetActive(false);
                    }
                    if (_sweeperLine == null)
                    {
                        _sweeperLine = GameObject.Find("Paragraph1 Sweeper");
                    }

                    _nextActionTime += Time.deltaTime;
                    RectTransform trans = _sweeperLine.GetComponent<RectTransform>();
                    trans.anchoredPosition = new Vector2(trans.anchoredPosition.x + (speed * Time.deltaTime), trans.anchoredPosition.y);

                    //Checks if measure needs to change
                    if (_measureNumber >= 3 && _nextActionTime >= _secondsPerMeasure)
                    {
                        // _nextActionTime -= _secondsPerMeasure;
                        _measureNumber = 0;
                        HandleParagraphChange();
                        // index++;
                        // HandlePianoColor();
                        _noteIndex = 0;
                        //_measureTotal++;
                        _reset = true;
                        if (_numberOfPatterns > 1)
                        {
                            _measureTotal++;
                        }
                    }
                    else if (_nextActionTime >= _secondsPerMeasure)
                    {
                        _measureNumber++;
                        //_measureTotal++;
                        _noteIndex = 0;
                        _reset = true;
                        if (_numberOfPatterns > 1)
                        {
                            _measureTotal++;
                        }
                    }

                    //Handles displaying different patterns
                    if (_nextActionTime >= _patternSplitSeconds * _patternIteration)
                    {
                        index++;
                        Debug.Log("measre " + index + " " + _noteIndex + " " + _measureTotal);
                        HandlePianoColor();
                        _noteIndex++;
                        //_numberOfPatterns--;
                        _patternIteration++;
                    }

                    //Handles reset
                    if (_reset)
                    {
                        if(_numberOfPatterns ==1){
                            _measureTotal++;
                        }
                        _nextActionTime -= _secondsPerMeasure;
                        _numberOfPatterns = _notes[_measureTotal].Count;
                        //Debug.Log("Hi " + _numberOfPatterns + _measureTotal);
                        _patternSplitSeconds = _secondsPerMeasure / _numberOfPatterns;
                        _patternIteration = 1;
                        _reset = false;
                    }
                }
            }
            else
            {
                timeRemaining = 4;
            }
        }

        private void HandleParagraphChange()
        {
            parentObject.transform.Find("Paragraph" + _paragraphNumber).gameObject.SetActive(false);
            _paragraphNumber++;
            
            Transform nextObject = parentObject.transform.Find("Paragraph" + (_paragraphNumber + 1));
            if (nextObject)
            {
                nextObject.gameObject.SetActive(true);
            }   
            
            if (parentObject.transform.Find("Paragraph" + (_paragraphNumber)) == null)
            {
                Button startButton = GameObject.Find("startButton").gameObject.GetComponent<Button>();
                startButton.onClick.Invoke();
                Button backButton = GameObject.Find("backButton").gameObject.GetComponent<Button>();
                Button helpButton = GameObject.Find("helpButton").gameObject.GetComponent<Button>();
                startButton.enabled = false;
                backButton.enabled = false;
                helpButton.enabled = false;

                DrawEndScreen();
            }
            _sweeperLine = GameObject.Find("Paragraph" + _paragraphNumber + " Sweeper");
        }

        private void HandlePianoColor()
        {
            Color color;
            if (_oldKeys != null)
            {
                foreach (GameObject key in _oldKeys.Keys)
                {
                    if (_oldKeys.TryGetValue(key, out color))
                    {
                        key.GetComponent<Image>().color = color;
                    }
                }
            }

            //Debug.Log(_noteDatabase.GetColorList(index));
            if (_notes[_measureTotal].Count != 0)
            {
                _oldKeys = _scoreView.changePianoKeyColor(_noteDatabase.GetColorList(index), _notes[_measureTotal][_noteIndex]);
            }
            else
            {
                _oldKeys = new Dictionary<GameObject, Color>();
            }

        }

        private void DisplayTime(float timeToDisplay)
        {
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);
            timeText.text = seconds.ToString();
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
            _noteDatabase.resetColorsList();
            _notes = new List<List<List<Note>>>();
            _oldKeys = new Dictionary<GameObject, Color>();
            _measureNumber = 0;
            index = 0;
            _noteIndex = 0;
            _measureTotal = 0;
            _paragraphNumber = 1;
            parentObject = GameObject.Find("Canvas_Score");
            DrawScore(scoreName);
            DrawTimerText();
            _fifth = _noteDatabase.GetFifth();
            _numberOfPatterns = _notes[_measureTotal].Count;
            _patternSplitSeconds = _secondsPerMeasure / _numberOfPatterns;
            HandlePianoColor();
            if (_numberOfPatterns <= 1)
            {
                _measureTotal++;
            }
            else
            {
                _noteIndex++;
            }
        }

        private void DrawTimerText()
        {
            GameObject textObject = GameObject.Instantiate(_commonParams.GetPrefabText(),
                this.transform.position,
                _commonParams.GetPrefabText().transform.rotation);
            textObject.name = "timerText";
            textObject.transform.SetParent(this.transform);
            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(500, 100);
            rect.position = new Vector3(Screen.width / 2, (Screen.height / 2) + 200, 0);
            timeText = textObject.GetComponent<Text>();
            timeText.text = "3";
            timeText.gameObject.SetActive(false);
        }

        private void DrawEndScreen() {
            GameObject panel = GameObject.Instantiate(_commonParams.GetPrefabPanel(),
                    parentObject.transform.position,
                    _commonParams.GetPrefabPanel().transform.rotation);
            panel.transform.SetParent(parentObject.transform);
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.offsetMin = new Vector2(Screen.width / 5, Screen.height / 5);
            rect.offsetMax = new Vector2(-Screen.width / 5,-Screen.height / 5);

            GameObject titleObject = panel.transform.Find("Title").gameObject;
            RectTransform titleRect = titleObject.GetComponent<RectTransform>();
            Text titleText = titleObject.GetComponent<Text>();
            titleText.text = "Summary";
            titleText.fontSize = 40;

            GameObject panelBackButtonObject = panel.transform.Find("PanelBackButton").gameObject;
            Text backText = panelBackButtonObject.GetComponentInChildren<Text>();
            backText.text = "Back to Main Menu";
            Button panelBackButton = panelBackButtonObject.GetComponent<Button>();
            panelBackButton.onClick.AddListener(delegate
            {
                Button backButton = GameObject.Find("backButton").gameObject.GetComponent<Button>();
                backButton.onClick.Invoke();
            });

            SummaryAlgorithm algorithm = new SummaryAlgorithm();
            algorithm.generateSummary();
            Dictionary<string, int> stats = _summaryMaster.GetSummary();
            int i = 0;

            float textOffset = -(titleRect.anchoredPosition.y) + titleRect.sizeDelta.y;
            foreach(string key in stats.Keys) {
                stats.TryGetValue(key, out int value);
                // Draw the text
                 GameObject textObject = GameObject.Instantiate(_commonParams.GetPrefabText());
                textObject.transform.SetParent(panel.transform);
                Text text = textObject.GetComponent<Text>();
                text.text = key + value;
                text.fontSize = 20;
                RectTransform textRect = textObject.GetComponent<RectTransform>();
                textRect.anchoredPosition = new Vector3(0,- textOffset - (-rect.sizeDelta.y /5) * i,0);
                textRect.anchorMin = new Vector2(0.5f, 1);
                textRect.anchorMax = new Vector2(0.5f, 1);
                i++;
            }
        }

        private void DrawScore(string filename)
        {
            // 解析MusicXml文件
            XmlFacade xmlFacade = new XmlFacade(filename);
            // 生成乐谱表
            //Debug.Log("Beat + " + xmlFacade.GetBPM());

            _secondsPerMeasure = CalculateSecondsPerMeasure(xmlFacade.GetBeat().GetBeatsPerMeasure(), xmlFacade.GetBPM());
            Debug.Log("seconds " + _secondsPerMeasure);

            // Draw Background
            GameObject backgroundPanel = GameObject.Instantiate(_commonParams.GetBackgroundPanel());
            backgroundPanel.name = "BackgroundPanel";
            backgroundPanel.transform.SetParent(parentObject.transform);
            RectTransform backgroundRect = backgroundPanel.GetComponent<RectTransform>();
            backgroundRect.offsetMin = new Vector2(0, 0);
            backgroundRect.offsetMax = new Vector2(0, 0);

            
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

            Debug.Log("Fifths + " + xmlFacade.GetFifths());
            //Debug.Log("Beat + " + xmlFacade.GetBPM());
            // Adds the created score to the note database so we can work with the notes
            _noteDatabase.AddScoreList(scoreList, xmlFacade.GetFifths());

            _scoreView = new ScoreView(scoreList, parentObject, screenSize, scoreInfo, _canvasScore, _loadScore, _overlayCanvas);
            // 更改乐符颜色
            //    Symbol symbol = scoreList[0][0].GetMeasureSymbolList()[0][1][2];
            //    SymbolControl symbolControl = new SymbolControl(symbol);
            //    symbolControl.SetColor(Color.red);
            speed = _paramsGetter.GetParagraphLength() / (_secondsPerMeasure * 4);
        }
        private float CalculateSecondsPerMeasure(string beatsPerMeasure, string BPM)
        {
            //Debug.Log("HI" + beatsPerMeasure);
            //Debug.Log("HI HI " + BPM);
            float secondsInMinute = 60.0f;
            float BPMeasure = float.Parse(beatsPerMeasure);
            float BPMinute = float.Parse(BPM);
            float secondsPerBeat = secondsInMinute / BPMinute;
            float secondsPerMeasure = secondsPerBeat * BPMeasure;

            return secondsPerMeasure;
        }
    }
}