using System;
using System.Collections.Generic;
using util;
using UnityEngine;
using UnityEngine.UI;
using symbol;

namespace overlay
{
    public class OverlayMeasure
    {
        private Measure _measure;
        private GameObject _parentObject;
        private GameObject _measureLines;
        private ParamsGetter _paramsGetter = ParamsGetter.GetInstance();
        private CommonParams _commonParams = CommonParams.GetInstance();
        private OverlayMaster _overlayMaster = OverlayMaster.GetInstance();
        private int _shift = 0;
        private float _noteLength;


        public OverlayMeasure(Measure measure, GameObject parentObject, float noteLength)
        {
            _measure = measure;
            _parentObject = parentObject;
            _noteLength = noteLength;
            Init();
        }

        private void Init()
        {
            OnDraw();
        }

        private void OnDraw()
        {

            DrawMeasureLines();

            if (_measure.HasHead())
            {
                DrawClef();
                _shift += _paramsGetter.GetHeadWidth();
            }

            if (_measure.HasBeat())
            {
                DrawBeat();
                _shift += _paramsGetter.GetBeatWidth();
            }


            float setLength = _measure.GetMeasureLength() - _shift;
            Vector3 setPosition = Vector3.zero;
            if (_measure.GetMeasureSymbolList().Count != 0)
            {
                setLength /= _measure.GetMeasureSymbolList().Count;
            }

            for (int i = 0; i < _measure.GetMeasureSymbolList().Count; i++)
            {
                string objName = "Set" + (i + 1);
                GameObject setObject = new GameObject(objName);
                setObject.transform.SetParent(_parentObject.transform);
                setObject.transform.localPosition = new Vector3(setPosition.x + setLength * i + _shift,
                    setPosition.y, setPosition.z);

                float noteLength = _overlayMaster.GetScoreSetView(i).GetNoteLength();

                OverlaySetView setView = new OverlaySetView(_measure.GetMeasureSymbolList()[i], setObject, setLength, _noteLength);
                _overlayMaster.AddSetView(setView);

            }
        }

        private void DrawMeasureLines()
        {

            _measureLines = new GameObject("Lines");
            _measureLines.transform.SetParent(_parentObject.transform);
            _measureLines.transform.localPosition = new Vector3(0, 0, 0);

            int measureLength = _measure.GetMeasureLength();

            for (int i = 0; i < 5; i++)
            {
                float yPosition = _paramsGetter.GetTotalHeight() + _paramsGetter.GetStaffCenterPosition() + (i - 2) * _paramsGetter.GetUnit();
                DrawLine(0, yPosition, measureLength, yPosition);
            }

            for (int i = 0; i < 2; i++)
            {

                float startY = _paramsGetter.GetTotalHeight() + _paramsGetter.GetStaffCenterPosition() - 2 * _paramsGetter.GetUnit();
                float stopY = _paramsGetter.GetTotalHeight() + _paramsGetter.GetStaffCenterPosition() + 2 * _paramsGetter.GetUnit();
                DrawLine(measureLength * i, startY, measureLength * i, stopY);
            }

            for (int i = 0; i < 5; i++)
            {
                float yPosition = _paramsGetter.GetTotalHeight() - _paramsGetter.GetStaffCenterPosition() + (i - 2) * _paramsGetter.GetUnit();
                DrawLine(0, yPosition, measureLength, yPosition);
            }

            for (int i = 0; i < 2; i++)
            {
                float startY = _paramsGetter.GetTotalHeight() - _paramsGetter.GetStaffCenterPosition() - 2 * _paramsGetter.GetUnit();
                float stopY = _paramsGetter.GetTotalHeight() - _paramsGetter.GetStaffCenterPosition() + 2 * _paramsGetter.GetUnit();
                DrawLine(measureLength * i, startY, measureLength * i, stopY);
            }
        }

        private void DrawClef()
        {
            GameObject highHeadObject = new GameObject("HighHead");
            highHeadObject.transform.SetParent(_parentObject.transform);
            highHeadObject.transform.localPosition = Vector3.zero;
            Head highHead = _measure.GetHead()[0];
            HeadView highHeadView = new HeadView(highHead, highHeadObject);

            GameObject lowHeadObject = new GameObject("LowHead");
            lowHeadObject.transform.SetParent(_parentObject.transform);
            lowHeadObject.transform.localPosition = Vector3.zero;
            Head lowHead = _measure.GetHead()[1];
            HeadView lowHeadView = new HeadView(lowHead, lowHeadObject);
        }

        private void DrawBeat()
        {
            GameObject beatObject = new GameObject("Beat");
            beatObject.transform.SetParent(_parentObject.transform);
            beatObject.transform.localPosition = Vector3.zero;
            Beat beat = _measure.GetBeat();
            BeatView beatView = new BeatView(beat, beatObject);
        }

        private void DrawLine(float startX, float startY, float stopX, float stopY)
        {
            GameObject lineObject = GameObject.Instantiate(_commonParams.GetPrefabLine(),
                _measureLines.transform.position,
                _commonParams.GetPrefabLine().transform.rotation);
            lineObject.transform.SetParent(_measureLines.transform);
            RectTransform lineRect = lineObject.GetComponent<RectTransform>();
            float width = Math.Max(startX, stopX) - Math.Min(startX, stopX) + 1;
            float heigh = Math.Max(startY, stopY) - Math.Min(startY, stopY) + 1;
            lineRect.sizeDelta = new Vector2(width, heigh);
            lineRect.localPosition = new Vector3(Math.Min(startX, stopX), Math.Min(startY, stopY), 0);
        }
    }
}