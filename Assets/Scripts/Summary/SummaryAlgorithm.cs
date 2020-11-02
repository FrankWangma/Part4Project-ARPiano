using System;
using System.Collections.Generic;
using Pattern;
using symbol;
using UnityEngine;

namespace summary {
    public class SummaryAlgorithm {
        private NoteDatabase _noteDatabase = NoteDatabase.GetInstance ();
        private SummaryMaster _summaryMaster = SummaryMaster.GetInstance ();
        private List<int> _highNotes = new List<int> ();
        private List<int> _lowNotes = new List<int> ();
        private List<int> _playedNotes = new List<int> ();
        private string _fifth = "0";

        private int _highNotesCorrect = 0;
        private int _highNotesMissed = 0;
        private int _lowNotesCorrect = 0;
        private int _lowNotesMissed = 0;
        private int _highNotesIncorrect = 0;
        private int _lowNotesIncorrect = 0;

        public void generateSummary () {
            List<Note> highNotes = SmoothList (_noteDatabase.GetHighNotes ());
            List<Note> lowNotes = SmoothList (_noteDatabase.GetLowNotes ());
            _fifth = _noteDatabase.GetFifth ();
            _highNotes = ConvertToNumber (highNotes);
            _lowNotes = ConvertToNumber (lowNotes);

            _playedNotes = _summaryMaster.GetNotesPlayed ();

            RunSummary ();

            Dictionary<String, int> summary = new Dictionary<String, int> ();
            summary.Add ("HighNotesCorrect", _highNotesCorrect);
            // summary.Add ("HighNotesIncorrect", _highNotesIncorrect);
            // summary.Add ("HighNotesMissed", _highNotesMissed);
            // summary.Add ("LowNotesCorrect", _lowNotesCorrect);
            // summary.Add ("LowNotesMissed", _lowNotesMissed);
            // summary.Add ("LowNotesIncorrect", _lowNotesIncorrect);
        }

        private void RunSummary () {
            int highPointer = 0;
            int lowPointer = 0;

            bool highFinished = false;
            bool lowFinished = false;

            foreach (int note in _playedNotes) {
                int highNote;
                int lowNote;

                if (!highFinished) {
                    highNote = _highNotes[highPointer];
                } else {
                    highNote = -999;
                }

                if (!lowFinished) {
                    lowNote = _lowNotes[lowPointer];
                } else {
                    lowNote = -999;
                }

                string closestToHigh = findClosest (note, highNote, lowNote);

                if (note == highNote) {
                    highPointer++;
                    _highNotesCorrect++;
                } else if (note == lowNote) {
                    lowPointer++;
                    _lowNotesCorrect++;
                } else {
                    if (closestToHigh.Equals ("highIncorrect")) {
                        _highNotesIncorrect++;
                        highPointer++;
                    } else if (closestToHigh.Equals ("highFail")) {
                        _highNotesIncorrect++;
                    } else if (closestToHigh.Equals ("lowIncorrect")) {
                        _lowNotesIncorrect++;
                        lowPointer++;
                    } else {
                        _lowNotesIncorrect++;
                    }
                }

                //If we reach the end of the list, no longer loops in
                if (highPointer >= _highNotes.Count) {
                    highFinished = true;
                }

                if (lowPointer >= _lowNotes.Count) {
                    lowFinished = true;
                }

                if (highFinished && lowFinished) {
                    break;
                }

            }

            _highNotesMissed = _highNotes.Count - highPointer;
            _lowNotesMissed = _lowNotes.Count - lowPointer;
        }

        public List<Note> SmoothList (List<List<Note>> notes) {
            List<Note> smoothedList = new List<Note> ();

            foreach (List<Note> measure in notes) {
                foreach (Note note in measure) {
                    smoothedList.Add (note);
                }
            }

            return smoothedList;
        }

        private List<int> ConvertToNumber (List<Note> notes) {

            List<int> output = new List<int> ();
            foreach (Note note in notes) {
                int step = StepToInt (note);
                int octave = Int32.Parse (note.GetOctave ()) * 12;
                int noteNumber = octave + step;
                output.Add (noteNumber);
            }

            return output;
        }

        //Method which takes three notes. The first note is a played note, and it finds the note which the played note is closest to.
        //Provides a 3 note birth as to whether the note played was intended
        private string findClosest (int note, int highNote, int lowNote) {

            int highDiff = Math.Abs (note - highNote);
            int lowDiff = Math.Abs (note - lowNote);

            if (highDiff <= lowDiff) {
                if (highDiff <= 4) {
                    return "highIncorrect";
                } else {
                    return "highFail";
                }
            } else {
                if (lowDiff <= 3) {
                    return "lowIncorrect";
                } else {
                    return "lowFail";
                }
            }
        }

        private int StepToInt (Note note) {
            string step = note.GetStep ();
            string accidental = note.GetAccidental (_fifth);

            int number = 0;
            switch (step) {
                case "C":
                    number = 0;
                    break;
                case "D":
                    number = 2;
                    break;
                case "E":
                    number = 4;
                    break;
                case "F":
                    number = 5;
                    break;
                case "G":
                    number = 7;
                    break;
                case "A":
                    number = 9;
                    break;
                case "B":
                    number = 11;
                    break;
            }

            if (accidental.Equals ("sharp")) {
                number++;
            }

            if (accidental.Equals ("flat")) {
                number--;
                if (number < 0) {
                    number = 12;
                }
            }

            return number;
        }
    }
}