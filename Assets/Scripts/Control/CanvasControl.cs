using System.Collections.Generic;
using UnityEngine;
using generator;
using symbol;
using util;
using xmlParser;
using Pattern;

namespace control
{
    public class CanvasControl : MonoBehaviour
    {
        private CommonParams _commonParams = CommonParams.GetInstance();
        private GameObject parentObject;
        private GameObject _overlayObject;

        public GameObject _canvasScore;
        public GameObject _loadScore;

        private NoteDatabase _noteDatabase = NoteDatabase.GetInstance();



        // Called when the object is disabled 
        private void OnDisable()
        {
            // This allows for a new score to be generated without any overlap
            foreach (Transform child in parentObject.transform)
            {
                child.gameObject.SetActive(false);
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

            ScoreView scoreView = new ScoreView(scoreList, parentObject, screenSize, scoreInfo, _canvasScore, _loadScore);
            // 更改乐符颜色
            //    Symbol symbol = scoreList[0][0].GetMeasureSymbolList()[0][1][2];
            //    SymbolControl symbolControl = new SymbolControl(symbol);
            //    symbolControl.SetColor(Color.red);
        }
    }
}