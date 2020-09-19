using System;
using System.Collections.Generic;
using util;
using UnityEngine;
using UnityEngine.UI;
using symbol;
using control;
using Pattern;

namespace summary
{
    public class SummaryAlgorithm
    {
        private NoteDatabase _noteDatabase = NoteDatabase.GetInstance();
        private SummaryMaster _summaryMaster = SummaryMaster.GetInstance();
        private List<Note> _highNotes = new List<Note>();
        private List<Note> _lowNotes = new List<Note>();
        private List<Note> _playedNotes = new List<Note>();

        private int _highNotesCorrect = 0;
        private int _highNotesMissed = 0;
        private int _lowNotesCorrect = 0;
        private int _lowNotesMissed = 0;
        private int _highNotesIncorrect = 0;
        private int _lowNotesIncorrect = 0;


        public void generateSummary()
        {
            List<List<Note>> highNotes = _noteDatabase.GetHighNotes();
            List<List<Note>> lowNotes = _noteDatabase.GetLowNotes();
            _highNotes = SmoothList(highNotes);
            _lowNotes = SmoothList(lowNotes);

            _playedNotes = _summaryMaster.GetNotesPlayed();

            RunSummary();

            Dictionary<String, int> summary = new Dictionary<String, int>();
            summary.Add("HighNotesCorrect", _highNotesCorrect);
            summary.Add("HighNotesMissed", _highNotesMissed);
            summary.Add("LowNotesCorrect", _lowNotesCorrect);
            summary.Add("LowNotesMissed", _lowNotesMissed);
            summary.Add("HighNotesIncorrect", _highNotesIncorrect);
            summary.Add("LowNotesIncorrect", _lowNotesIncorrect);
            _summaryMaster.UpdateSummary(summary);
        }

        private void RunSummary()
        {
            int highPointer = 0;
            int lowPointer = 0;

            bool highFinished = false;
            bool lowFinished = false;

            foreach (Note note in _playedNotes)
            {
                Note highNote;
                Note lowNote;

                if(!highFinished)
                {
                    highNote = _highNotes[highPointer];
                }
                else
                {
                    highNote = null;
                }

                if(!lowFinished)
                {
                    lowNote = _lowNotes[lowPointer];
                } 
                else
                {
                    lowNote = null;
                }

                bool closestToHigh = findClosest(note, highNote, lowNote);

                if (!highFinished)
                {
                    if (note.GetStep().Equals(_highNotes[highPointer].GetStep()))
                    {
                        if (note.GetOctave().Equals(_highNotes[highPointer].GetOctave()))
                        {
                            highPointer++;
                            _highNotesCorrect++;
                        }
                    }
                }
                else if (!lowFinished)
                {
                    if (note.GetStep().Equals(_lowNotes[lowPointer].GetStep()))
                    {
                        if (note.GetOctave().Equals(_lowNotes[lowPointer].GetOctave()))
                        {
                            lowPointer++;
                            _lowNotesCorrect++;
                        }
                    }
                }
                else
                {
                    if(closestToHigh)
                    {
                        _highNotesIncorrect++;
                        highPointer++;
                    }
                    else
                    {
                        _lowNotesIncorrect++;
                        lowPointer++;
                    }
                }


                //If we reach the end of the list, no longer loops in
                if (highPointer >= _highNotes.Count)
                {
                    highFinished = true;
                }

                if (lowPointer >= _lowNotes.Count)
                {
                    lowFinished = true;
                }

                if(highFinished && lowFinished){

                }

            }

            _highNotesMissed = _highNotes.Count - highPointer;
            _lowNotesMissed = _lowNotes.Count - lowPointer;
        }

        private List<Note> SmoothList(List<List<Note>> notes)
        {
            List<Note> smoothedList = new List<Note>();

            foreach (List<Note> measure in notes)
            {
                foreach (Note note in measure)
                {
                    smoothedList.Add(note);
                }
            }

            return smoothedList;
        }

        //Method which takes three notes. The first note is a played note, and it finds the note which the played note is closest to.
        //Returns true if the note is closer to highnote
        private bool findClosest(Note note, Note highNote, Note lowNote)
        {
            if(highNote == null){
                return false;
            }

            if(lowNote == null){
                return true;
            }

            return false;
        }
    }
}