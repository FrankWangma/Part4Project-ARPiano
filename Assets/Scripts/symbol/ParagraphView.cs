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
            int paragraphLength = 0;
            for (int i = 0; i < _paragraphList.Count; i++)
            {
                string objName = "Measure" + (i + 1);
                GameObject measureObject = new GameObject(objName);
                measureObject.transform.SetParent(_parentObject.transform);
                measureObject.transform.localPosition = new Vector3(measurePosition.x,
                    measurePosition.y, measurePosition.z);

                MeasureView measureView = new MeasureView(_paragraphList[i], measureObject);

                Measure myMeasure = _paragraphList[i];
                List<List<List<Symbol>>> myMeasureSymbolList = myMeasure.GetMeasureSymbolList();
                
                measurePosition.x += _paragraphList[i].GetMeasureLength();
                paragraphLength += _paragraphList[i].GetMeasureLength();
            }

            float startY = _paramsGetter.GetTotalHeight() - _paramsGetter.GetStaffCenterPosition() - 2 * _paramsGetter.GetUnit();
            float stopY = _paramsGetter.GetTotalHeight() + _paramsGetter.GetStaffCenterPosition() + 2 * _paramsGetter.GetUnit();
            DrawLine(0, startY, 1, stopY);
            _paramsGetter.SetParagraphLength(paragraphLength);
        }
         private void DrawLine(float startX, float startY, float stopX, float stopY)
        {
            GameObject lineObject = GameObject.Instantiate(_commonParams.GetPrefabLine(),
                _parentObject.transform.position,
                _commonParams.GetPrefabLine().transform.rotation);
            lineObject.name = _parentObject.name + " Sweeper";
            lineObject.transform.SetParent(_parentObject.transform);
            RectTransform lineRect = lineObject.GetComponent<RectTransform>();
            float width = Math.Max(startX, stopX) - Math.Min(startX, stopX) + 1;
            float heigh = Math.Max(startY, stopY) - Math.Min(startY, stopY) + 1;
            lineRect.sizeDelta = new Vector2(width, heigh);
            lineRect.localPosition = new Vector3(Math.Min(startX, stopX), Math.Min(startY, stopY), 0);
            lineObject.GetComponent<Image>().color = Color.red;
        }
    }
}