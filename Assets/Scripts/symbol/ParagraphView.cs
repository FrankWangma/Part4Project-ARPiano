using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;
namespace symbol
{
    public class ParagraphView
    {
        private List<Measure> _paragraphList;
        private GameObject _parentObject;
        private ParamsGetter _paramsGetter = ParamsGetter.GetInstance();
        private CommonParams _commonParams = CommonParams.GetInstance();
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

                //Debug.Log("Paragraph " + _paragraphList.Count);
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

            float startY = _paramsGetter.GetTotalHeight() - _paramsGetter.GetStaffCenterPosition() - 2 * _paramsGetter.GetUnit();
            float stopY = _paramsGetter.GetTotalHeight() + _paramsGetter.GetStaffCenterPosition() + 2 * _paramsGetter.GetUnit();
            DrawLine(0, startY, 1, stopY);
            _paramsGetter.SetMeasureLength(_paragraphList[0].GetMeasureLength());
        }
         private void DrawLine(float startX, float startY, float stopX, float stopY)
        {
            // 实例化一个线段对象
            GameObject lineObject = GameObject.Instantiate(_commonParams.GetPrefabLine(),
                _parentObject.transform.position,
                _commonParams.GetPrefabLine().transform.rotation);
            lineObject.name = "Sweeper";
            lineObject.transform.SetParent(_parentObject.transform);
            RectTransform lineRect = lineObject.GetComponent<RectTransform>();
            float width = Math.Max(startX, stopX) - Math.Min(startX, stopX) + 1;
            float heigh = Math.Max(startY, stopY) - Math.Min(startY, stopY) + 1;
            lineRect.sizeDelta = new Vector2(width, heigh); // 设置线段长宽
            // 设置线段位置，从最小画到最大
            lineRect.localPosition = new Vector3(Math.Min(startX, stopX), Math.Min(startY, stopY), 0);
            lineObject.GetComponent<Image>().color = Color.red;
        }
    }
}