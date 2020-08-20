using System;
using System.Collections.Generic;
using util;
using UnityEngine;

namespace symbol
{
    public class SetView
    {
        private List<List<Symbol>> _setList;
        private List<Symbol> _highSymbolList;
        private List<Symbol> _lowSymbolList;
        private GameObject _parentObject;
        private float _setLength;
        private ParamsGetter _paramsGetter = ParamsGetter.GetInstance();
        private float _noteLength;
        private int _maxCount;

        public SetView(List<List<Symbol>> setList, GameObject parentObject, float setLength)
        {
            _setList = setList;
            _parentObject = parentObject;
            _highSymbolList = setList[0];
            _lowSymbolList = setList[1];
            _setLength = setLength;
            Init();
        }

        private void Init()
        {
            OnDraw();
        }

        private void OnDraw()
        {
            //Debug.Log("Set List" + _setList.Count);
            //Debug.Log("HighSymbolList " + _highSymbolList.Count);
            //Debug.Log("LowSymbolList " + _lowSymbolList.Count);

            Vector3 notePosition = Vector3.zero;
            _noteLength = _setLength;
            _maxCount = Math.Max(_highSymbolList.Count, _lowSymbolList.Count);
            if (_setList.Count != 0)
            {
                //Note length dynamically changes based on number of notes in a measure
                _noteLength = _setLength / _maxCount;
            }

            //Debug.Log("Note Length " + _noteLength);

            for (int i = 0; i < _highSymbolList.Count; i++)
            {
                //新建HighNote对象作为目录
                string objName = "HighNote" + (i + 1);
                GameObject highNoteObj = new GameObject(objName);
                highNoteObj.transform.SetParent(_parentObject.transform);
                highNoteObj.transform.localPosition = new Vector3(
                    notePosition.x + _paramsGetter.GetBeatWidth() + _noteLength * i,
                    notePosition.y + _paramsGetter.GetTotalHeight(),
                    notePosition.z);

                //Debug.Log("Noteposition " + notePosition.x);

                // 将Set对象赋为下一层的父对象
                if (_highSymbolList[i] is Note)
                {
                    NoteView noteView = new NoteView(_highSymbolList[i], (int)_noteLength, _paramsGetter.GetSymbolStart(), highNoteObj);
                    _highSymbolList[i].SetSymbolView(noteView);
                }
                else if (_highSymbolList[i] is Rest)
                {
                    RestView restView = new RestView(_highSymbolList[i], (int)_noteLength, _paramsGetter.GetSymbolStart(), highNoteObj);
                    _highSymbolList[i].SetSymbolView(restView);
                }
            }
            for (int i = 0; i < _lowSymbolList.Count; i++)
            {
                // 新建LowNote对象作为目录
                string objName = "LowNote" + (i + 1);
                GameObject lowNoteObj = new GameObject(objName);
                lowNoteObj.transform.SetParent(_parentObject.transform);
                lowNoteObj.transform.localPosition = new Vector3(
                    notePosition.x + _paramsGetter.GetBeatWidth() + _noteLength * i,
                    notePosition.y,
                    notePosition.z);

                // 将Set对象赋为下一层的父对象
                if (_lowSymbolList[i] is Note)
                {
                    NoteView noteView = new NoteView(_lowSymbolList[i], (int)_noteLength, _paramsGetter.GetSymbolStart(), lowNoteObj);
                    _lowSymbolList[i].SetSymbolView(noteView);
                }
                else if (_lowSymbolList[i] is Rest)
                {
                    RestView restView = new RestView(_lowSymbolList[i], (int)_noteLength, _paramsGetter.GetSymbolStart(), lowNoteObj);
                    _lowSymbolList[i].SetSymbolView(restView);
                }
            }
        }

        public float GetNoteLength() { return _noteLength; }
        public int GetMaxCount() { return _maxCount; }
    }
}