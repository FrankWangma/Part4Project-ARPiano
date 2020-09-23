using System;
using System.Collections.Generic;
using util;
using UnityEngine;
using UnityEngine.UI;
using symbol;
using Pattern;

namespace summary
{
    public class SummaryMaster
    {

        private static SummaryMaster instance = new SummaryMaster();

        public static SummaryMaster GetInstance() { return instance; }

        private List<int> _notesPlayed = new List<int>();

        private Dictionary<String, int> _summary = new Dictionary<String, int>();
        SummaryAlgorithm _algorithm = new SummaryAlgorithm();

        public void AddNotePlayed(int note){
            _notesPlayed.Add(note);
        }

        public List<int> GetNotesPlayed(){
            return _notesPlayed;
        }

        public Dictionary<String, int> GetSummary(){
            return _summary;
        }

        public void UpdateSummary(Dictionary<String, int> summary){
            _summary = summary;
        }
    }
}