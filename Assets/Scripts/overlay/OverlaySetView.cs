using System;
using System.Collections.Generic;
using util;
using UnityEngine;
using symbol;

namespace overlay
{
    public class OverlaySetView
    {
        private List<List<Symbol>> _setList;
        private List<Symbol> _highSymbolList;
        private List<Symbol> _lowSymbolList;
        private GameObject _parentObject;
        private float _setLength;
        private ParamsGetter _paramsGetter = ParamsGetter.GetInstance();
        private float _noteLength;

        public OverlaySetView(List<List<Symbol>> setList, GameObject parentObject, float setLength, float noteLength)
        {
            _setList = setList;
            _parentObject = parentObject;
            _highSymbolList = setList[0];
            _lowSymbolList = setList[1];
            _setLength = setLength;
            _noteLength = noteLength;
        }

        public GameObject GetParentObject()
        {
            return _parentObject;
        }

        public float GetSetLength()
        {
            return _setLength;
        }

        public List<List<Symbol>> GetSetList()
        {
            return _setList;
        }

        public void ModifySetView(List<List<Symbol>> setList)
        {
            _setList = setList;
            OnModify();
        }

        private void Init()
        {
            OnDraw();
        }

        private void OnModify()
        {
            Vector3 notePosition = Vector3.zero;
            int maxCount = Math.Max(_highSymbolList.Count, _lowSymbolList.Count);
            if (_setList.Count != 0)
            {
                //noteLength = _setLength / maxCount;
            }


            for (int i = 0; i < _highSymbolList.Count; i++)
            {
                string objName = "HighNote" + (i + 1);
                GameObject highNoteObj = new GameObject(objName);
                highNoteObj.transform.SetParent(_parentObject.transform);
                highNoteObj.transform.localPosition = new Vector3(
                    notePosition.x + _paramsGetter.GetBeatWidth() + _noteLength * i,
                    notePosition.y + _paramsGetter.GetTotalHeight(),
                    notePosition.z);

                if (_highSymbolList[i] is Note)
                {
                    NoteView noteView = new NoteView(_highSymbolList[i], (int)_noteLength, _paramsGetter.GetSymbolStart(), highNoteObj);
                    //Changes overlay to red
                    noteView.SetColor(Color.red);
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
                string objName = "LowNote" + (i + 1);
                GameObject lowNoteObj = new GameObject(objName);
                lowNoteObj.transform.SetParent(_parentObject.transform);
                lowNoteObj.transform.localPosition = new Vector3(
                    notePosition.x + _paramsGetter.GetBeatWidth() + _noteLength * i,
                    notePosition.y,
                    notePosition.z);

                if (_lowSymbolList[i] is Note)
                {
                    NoteView noteView = new NoteView(_lowSymbolList[i], (int)_noteLength, _paramsGetter.GetSymbolStart(), lowNoteObj);
                    noteView.SetColor(Color.red);
                    _lowSymbolList[i].SetSymbolView(noteView);
                }
                else if (_lowSymbolList[i] is Rest)
                {
                    RestView restView = new RestView(_lowSymbolList[i], (int)_noteLength, _paramsGetter.GetSymbolStart(), lowNoteObj);
                    _lowSymbolList[i].SetSymbolView(restView);
                }
            }
        }


        private void OnDraw()
        {

            Vector3 notePosition = Vector3.zero;
            float noteLength = _setLength;
            int maxCount = Math.Max(_highSymbolList.Count, _lowSymbolList.Count);
            if (_setList.Count != 0)
            {
                noteLength = _setLength / maxCount;
            }

            for (int i = 0; i < _highSymbolList.Count; i++)
            {
                string objName = "HighNote" + (i + 1);
                GameObject highNoteObj = new GameObject(objName);
                highNoteObj.transform.SetParent(_parentObject.transform);
                highNoteObj.transform.localPosition = new Vector3(
                    notePosition.x + _paramsGetter.GetBeatWidth() + noteLength * i,
                    notePosition.y + _paramsGetter.GetTotalHeight(),
                    notePosition.z);

                if (_highSymbolList[i] is Note)
                {
                    NoteView noteView = new NoteView(_highSymbolList[i], (int)noteLength, _paramsGetter.GetSymbolStart(), highNoteObj);
                    _highSymbolList[i].SetSymbolView(noteView);
                }
                else if (_highSymbolList[i] is Rest)
                {
                    RestView restView = new RestView(_highSymbolList[i], (int)noteLength, _paramsGetter.GetSymbolStart(), highNoteObj);
                    _highSymbolList[i].SetSymbolView(restView);
                }
            }
            for (int i = 0; i < _lowSymbolList.Count; i++)
            {
                string objName = "LowNote" + (i + 1);
                GameObject lowNoteObj = new GameObject(objName);
                lowNoteObj.transform.SetParent(_parentObject.transform);
                lowNoteObj.transform.localPosition = new Vector3(
                    notePosition.x + _paramsGetter.GetBeatWidth() + noteLength * i,
                    notePosition.y,
                    notePosition.z);
                    
                if (_lowSymbolList[i] is Note)
                {
                    NoteView noteView = new NoteView(_lowSymbolList[i], (int)noteLength, _paramsGetter.GetSymbolStart(), lowNoteObj);
                    _lowSymbolList[i].SetSymbolView(noteView);
                }
                else if (_lowSymbolList[i] is Rest)
                {
                    RestView restView = new RestView(_lowSymbolList[i], (int)noteLength, _paramsGetter.GetSymbolStart(), lowNoteObj);
                    _lowSymbolList[i].SetSymbolView(restView);
                }
            }
        }
    }
}
