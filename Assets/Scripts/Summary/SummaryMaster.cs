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
        private int _paragraphNumber = 0;
        private bool _reset = false;

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
            Dictionary<String, int> summary = new Dictionary<String, int> ();
            summary.Add ("Notes Incorrect: ", _notesIncorrect);
            summary.Add ("High Notes Missed: ", _highNotesMissed);
            summary.Add ("Low Notes Missed: ", _lowNotesMissed);
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

        public void SetReset(){
            _reset = true;
        }

        public bool GetReset(){
            return _reset;
        }
    }
}