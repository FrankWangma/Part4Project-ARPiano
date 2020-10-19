using System;
using System.Collections.Generic;
using Pattern;
using symbol;
using util;
using UnityEngine;
using UnityEngine.UI;

namespace summary {
    public class SummaryMaster {

        private static SummaryMaster instance = new SummaryMaster ();

        public static SummaryMaster GetInstance () { return instance; }

        private List<int> _notesPlayed = new List<int> ();

        private Dictionary<String, int> _summary = new Dictionary<String, int> ();
        private int _notesIncorrect = 0;
        private int _highNotesMissed = 0;
        private int _lowNotesMissed = 0;
        private int _paragraphNumber = 1;
        private int _highNotesCorrect = 0;
        private int _lowNotesCorrect = 0;
        private int _highNotesTooFast = 0;
        private int _lowNotesTooFast = 0;
        private bool _reset = false;

        private List<int> _noteNumberHigh = new List<int>();
        private List<int> _noteNumberLow = new List<int>();
        private int _highPointer;
        private int _lowPointer;

        public void SetHighNotesTooFast (int highNotesTooFast){
            _highNotesTooFast = highNotesTooFast;
        }

        public void SetLowNotesTooFast (int lowNotesTooFast){
            _lowNotesTooFast = lowNotesTooFast;
        }

        public void SetHighNotesCorrect (int highNotesCorrect){
            _highNotesCorrect = highNotesCorrect;
        }

        public void SetLowNotesCorrect (int lowNotesCorrect){
            _lowNotesCorrect = lowNotesCorrect;
        }

        public void SetHighPointer (int highPointer){
            _highPointer = highPointer;
        }

        public void SetLowPointer (int lowPointer){
            _lowPointer = lowPointer;
        }

        public void SetNumberHigh (List<int> noteNumberHigh){
            _noteNumberHigh = noteNumberHigh;
        }

        public void SetNumberLow (List<int> noteNumberLow){
            _noteNumberLow = noteNumberLow;
        }

        public void AddNotePlayed (int note) {
            _notesPlayed.Add (note);
        }

        public List<int> GetNotesPlayed () {
            return _notesPlayed;
        }

        public Dictionary<String, int> GetSummary () {
            UpdateSummary ();
            return _summary;
        }

        public void UpdateSummary () {
            Debug.Log("number " + _paragraphNumber);
            if(_paragraphNumber -2 >= _noteNumberHigh.Count){
                _paragraphNumber = _noteNumberHigh.Count + 1; 
            }
            int expectedHigh = _noteNumberHigh[_paragraphNumber - 2];
            int expectedLow = _noteNumberLow[_paragraphNumber - 2];
            AddHighNotesMissed (expectedHigh - (_highPointer-1));
            AddLowNotesMissed (expectedLow - (_lowPointer - 1));
            Dictionary<String, int> summary = new Dictionary<String, int> ();
            summary.Add("High Notes Correct: ", _highNotesCorrect);
            summary.Add("Low Notes Correct: ", _lowNotesCorrect);
            summary.Add ("Notes Incorrect: ", _notesIncorrect);
            summary.Add ("High Notes Missed: ", _highNotesMissed);
            summary.Add ("Low Notes Missed: ", _lowNotesMissed);
            summary.Add("High Notes Too Fast: ", _highNotesTooFast);
            summary.Add("Low Notes Too Fast: ", _lowNotesTooFast);
            _summary = summary;
        }

        public void SetNotesIncorrect (int notesIncorrect) {
            _notesIncorrect = notesIncorrect;
        }

        public void SetParagraphNumber (int paragraphNumber) {
            _paragraphNumber = paragraphNumber;
        }

        public int GetParagraphNumber () {
            return _paragraphNumber;
        }

        public void AddHighNotesMissed (int missed) {
            _highNotesMissed += missed;
        }

        public void AddLowNotesMissed (int missed) {
            _lowNotesMissed += missed;
        }

        public void Reset () {
            _notesIncorrect = 0;
            _highNotesMissed = 0;
            _lowNotesMissed = 0;
            _paragraphNumber = 0;
            _reset = false;
        }

        public void SetReset () {
            _reset = true;
        }

        public bool GetReset () {
            return _reset;
        }
    }
}