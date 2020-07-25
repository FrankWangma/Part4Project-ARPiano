using System;
using System.Collections.Generic;
using util;
using UnityEngine;
using UnityEngine.UI;
using symbol;

namespace overlay
{
    public class OverlayMaster
    {

        private static OverlayMaster instance = new OverlayMaster();

        //counter to increment across notes to modify
        int _toModify = 0;
        //Tracks which measure we are currently overlaying
        int _globalMeasureCounter = 0;

        private OverlayScoreView _overlayScoreView;
        private List<OverlayParagraph> _overlayParagraphViews = new List<OverlayParagraph>();
        private List<OverlayMeasure> _overlayMeasures = new List<OverlayMeasure>();
        private List<OverlaySetView> _overlaySetViews = new List<OverlaySetView>();
        private List<SetView> _scoreSetViews = new List<SetView>();

        public static OverlayMaster GetInstance() { return instance; }

        public void SetScoreView(OverlayScoreView scoreView) { _overlayScoreView = scoreView; }

        public void SetParagraphViews(List<OverlayParagraph> paragraphViews) { _overlayParagraphViews = paragraphViews; }

        public void SetMeasureViews(List<OverlayMeasure> measureViews) { _overlayMeasures = measureViews; }

        public void SetSetViews(List<OverlaySetView> setViews) { _overlaySetViews = setViews; }

        public void AddSetView(OverlaySetView setView) { _overlaySetViews.Add(setView); }

        public int GetGlobalMeasure() { return _globalMeasureCounter; }
        public void incrementGlobalMeasure() { _globalMeasureCounter++; }
        public void setGlobalMeasure(int count) { _globalMeasureCounter = count; }

        public List<OverlaySetView> GetSetViews() { return _overlaySetViews; }
        public OverlayMeasure GetMeasure(int i) { return _overlayMeasures[i]; }

        public void AddScoreSetView(SetView setView) { _scoreSetViews.Add(setView); }
        public SetView GetScoreSetView(int i) {return _scoreSetViews[i]; }

        public void ModifySetView(int position, Symbol symbol, Boolean upper, int startTime)
        {
            symbol = SetShift((Note)symbol);
            Debug.Log("To Modify " + _toModify);
            List<List<Symbol>> setList = _overlaySetViews[_toModify].GetSetList();
            if (upper)
            {
                setList[0].Add(symbol);
            }
            else
            {
                setList[1].Add(symbol);
            }
            _overlaySetViews[_toModify].ModifySetView(setList);

            int currentCount = Math.Max(setList[0].Count, setList[1].Count);

            Debug.Log("Max Count " + GetScoreSetView(_toModify).GetMaxCount());
            Debug.Log("Master Note length " + GetScoreSetView(_toModify).GetNoteLength());

            if(currentCount >= GetScoreSetView(_toModify).GetMaxCount())
            {
                _toModify++;
            }
        }

        private Note SetShift(Note note)
        {
            ParamsGetter paramsGetter = ParamsGetter.GetInstance();
            int shift = -(GetDigitizedPitch("B", "4") - GetDigitizedPitch(note.GetStep(), note.GetOctave())) *
                        paramsGetter.GetPitchPositionDiff();
            note.SetShift(shift);
            return note;
        }

        private int GetDigitizedPitch(string step, string octave)
        {
            int digitizedPitch = 1;

            switch (step)
            {
                case "C": digitizedPitch = 1; break;
                case "D": digitizedPitch = 2; break;
                case "E": digitizedPitch = 3; break;
                case "F": digitizedPitch = 4; break;
                case "G": digitizedPitch = 5; break;
                case "A": digitizedPitch = 6; break;
                case "B": digitizedPitch = 7; break;
            }

            return digitizedPitch + (int.Parse(octave) - 1) * 7;
        }
    }
}