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

        private OverlayScoreView _overlayScoreView;
        private List<OverlayParagraph> _overlayParagraphViews = new List<OverlayParagraph>();
        private List<OverlayMeasure> _overlayMeasures = new List<OverlayMeasure>();
        private List<OverlaySetView> _overlaySetViews = new List<OverlaySetView>();

        public static OverlayMaster GetInstance() { return instance; }

        public void SetScoreView (OverlayScoreView scoreView) { _overlayScoreView = scoreView; }

        public void SetParagraphViews (List<OverlayParagraph> paragraphViews) { _overlayParagraphViews = paragraphViews; }

        public void SetMeasureViews (List<OverlayMeasure> measureViews ) { _overlayMeasures = measureViews; }

        public void SetSetViews (List<OverlaySetView> setViews ) { _overlaySetViews = setViews; }

        public void AddSetView ( OverlaySetView setView ) { _overlaySetViews.Add(setView); }

        public OverlayMeasure GetMeasure(int i) { return _overlayMeasures[i]; }

        public void ModifySetView(int position, Symbol symbol, Boolean upper){
            symbol.SetStartTime(0);
            symbol.SetStopTime(symbol.GetDuration());
            symbol = SetShift((Note) symbol);
            List<List<Symbol>> setList = _overlaySetViews[position].GetSetList();
            Debug.Log("total size " + setList.Count);
            if (upper){
                setList[0].Add(symbol);
                Debug.Log("upper " + setList[0].Count);
            } 
            else
            {
                setList[1].Add(symbol);
                Debug.Log("lower " + setList[1].Count);
            }
            Debug.Log("high measure size " + setList[0].Count);
            Debug.Log("low measure size " + setList[1].Count);
            _overlaySetViews[position].ModifySetView(setList);
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