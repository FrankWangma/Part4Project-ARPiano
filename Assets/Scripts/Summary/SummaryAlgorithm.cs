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
        private int _notesIncorrect = 0;


        public void generateSummary()
        {
            List<List<Note>> highNotes = _noteDatabase.GetHighNotes();
            List<List<Note>> lowNotes = _noteDatabase.GetLowNotes();
            _highNotes = SmoothList(highNotes);
            _lowNotes = SmoothList(lowNotes);

            _playedNotes = _summaryMaster.GetNotesPlayed();

            RunSummary();

            Dictionary<String, int> summary = new Dictionary<String, int>();
            summary.Add("High Notes Correct: ", _highNotesCorrect);
            summary.Add("High Notes Missed: ", _highNotesMissed);
            summary.Add("Low Notes Correct: ", _lowNotesCorrect);
            summary.Add("Low Notes Missed: ",  _lowNotesMissed);
            summary.Add("Notes Incorrect: ", _notesIncorrect);
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
                    _notesIncorrect++;
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
        private bool findClosest(Note note, Note highNote, Note lowNote)
        {
            return false;
        }
    }
}