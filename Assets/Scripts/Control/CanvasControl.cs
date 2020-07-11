using System.Collections.Generic;
using UnityEngine;
using generator;
using symbol;
using util;
using xmlParser;

namespace control
{
    public class CanvasControl : MonoBehaviour
    {
        private CommonParams _commonParams = CommonParams.GetInstance();
        private GameObject parentObject;

        public GameObject _canvasScore;
        public GameObject _loadScore;

        // Use this for initialization
        private void Start()
        {
            //        DrawScore("Assets/Materials/example.xml");
            string scoreName = _commonParams.GetScoreName();
            parentObject = GameObject.Find("Canvas_Score");
            DrawScore(scoreName);

            //            DrawScore("Assets/Materials/MusicXml/印第安鼓.xml");
        }

        // Update is called once per frame
        private void Update()
        {

        }

        private void OnDisable()
        {
            foreach (Transform child in parentObject.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

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
            Debug.Log(filename);
            // 准备绘制乐谱对象及其他参数
            List<float> screenSize = new List<float>();
            screenSize.Add(Screen.width);
            screenSize.Add(Screen.height);
            List<string> scoreInfo = new List<string>();
            // 乐谱名称和作者信息
            scoreInfo.Add(xmlFacade.GetWorkTitle()); // 0
            scoreInfo.Add(xmlFacade.GetCreator()); // 1

            // 绘制乐谱视图
            ScoreView scoreView = new ScoreView(scoreList, parentObject, screenSize, scoreInfo, _canvasScore, _loadScore);

            // 更改乐符颜色
//        Symbol symbol = scoreList[0][0].GetMeasureSymbolList()[0][1][2];
//        SymbolControl symbolControl = new SymbolControl(symbol);
//        symbolControl.SetColor(Color.red);
        }
    }
}