using System.Collections.Generic;
using UnityEngine;
using symbol;

namespace overlay
{
    public class OverlayParagraph
    {
        private List<Measure> _paragraphList;
        private GameObject _parentObject;

        //Test to add a single symbol
        private Symbol _symbol;
        private OverlayMaster _overlayMaster = OverlayMaster.GetInstance();

        public OverlayParagraph(List<Measure> paragraphList, GameObject parentObject)
        {
            _paragraphList = paragraphList;
            _parentObject = parentObject;
            Init();
        }

        private void Init()
        {
            OnDraw();
        }

        private void OnDraw()
        {
            Vector3 measurePosition = Vector3.zero;
            // 遍历一行中的所有小节，绘制每个小节
            //Debug.Log("Paragraph " + _paragraphList.Count);
            for (int i = 0; i < _paragraphList.Count; i++)
            {

                // Debug.Log("Paragraph " + _paragraphList.Count);
                // 新建Measure对象作为目录
                string objName = "Measure" + (i + 1);
                GameObject measureObject = new GameObject(objName);
                measureObject.transform.SetParent(_parentObject.transform);
                measureObject.transform.localPosition = new Vector3(measurePosition.x,
                    measurePosition.y, measurePosition.z);

                //Create blank measures
                List<List<List<Symbol>>> symbolList = _paragraphList[i].GetMeasureSymbolList();

                //Debug.Log("Before " + symbolList.Count);

                foreach (List<List<Symbol>> measure in symbolList)
                {
                    for (int j = 0; j < measure.Count; j++)
                    {
                        measure[j] = new List<Symbol>();
                    }
                }

                //Debug.Log("After " + symbolList.Count);
                //Debug.Log("Paragraph count " + _paragraphList.Count);
                _paragraphList[i].SetMeasureSymbolList(symbolList);

                float noteLength = _overlayMaster.GetScoreSetView(_overlayMaster.GetGlobalMeasure()).GetNoteLength();
                _overlayMaster.incrementGlobalMeasure();

                // 将Measure对象对象赋为下一层的父对象
                // 绘制Measure视图
                OverlayMeasure measureView = new OverlayMeasure(_paragraphList[i], measureObject, noteLength);

                // 调整下一个小节的起始横坐标
                measurePosition.x += _paragraphList[i].GetMeasureLength();
            }

        }
    }
}