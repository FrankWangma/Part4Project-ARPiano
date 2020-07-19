using System.Collections.Generic;
using UnityEngine;

namespace symbol
{
    public class ParagraphView
    {
        private List<Measure> _paragraphList;
        private GameObject _parentObject;

        //Test to add a single symbol
        private Symbol _symbol;

        public ParagraphView(List<Measure> paragraphList, GameObject parentObject)
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

                Debug.Log("Paragraph " + _paragraphList.Count);
                // 新建Measure对象作为目录
                string objName = "Measure" + (i + 1);
                GameObject measureObject = new GameObject(objName);
                measureObject.transform.SetParent(_parentObject.transform);
                measureObject.transform.localPosition = new Vector3(measurePosition.x,
                    measurePosition.y, measurePosition.z);

                // 将Measure对象对象赋为下一层的父对象
                // 绘制Measure视图
                MeasureView measureView = new MeasureView(_paragraphList[i], measureObject);

                Measure myMeasure = _paragraphList[i];
                List<List<List<Symbol>>> myMeasureSymbolList = myMeasure.GetMeasureSymbolList();
                

                //measureView.ModifyMeasure(myMeasure, measureObject);

                // 调整下一个小节的起始横坐标
                measurePosition.x += _paragraphList[i].GetMeasureLength();
            }

            //Test change first measure

            //Measure myMeasure = _paragraphList[0];

        }
    }
}