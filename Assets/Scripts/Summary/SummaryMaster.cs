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

        private List<Note> _notesPlayed = new List<Note>();

        private Dictionary<String, int> _summary = new Dictionary<String, int>();

        public void AddNotePlayed(Note note){
            _notesPlayed.Add(note);
        }

        public List<Note> GetNotesPlayed(){
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