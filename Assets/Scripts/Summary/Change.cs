using System;
using System.Collections.Generic;
using MidiJack;
using Pattern;
using symbol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace summary {
    public class Change : MonoBehaviour {
        //Gets note text object from canvas
        //public GameObject note;

        //Get overlay master
        private SummaryMaster _summaryMaster = SummaryMaster.GetInstance ();
        private NoteDatabase _noteDatabase = NoteDatabase.GetInstance ();
        private List<int> _highNotes = new List<int> ();
        private List<int> _lowNotes = new List<int> ();
        private string _fifth = "0";

        private List<Note> _smoothedHighNotes = new List<Note> ();
        private List<Note> _smoothedLowNotes = new List<Note> ();

        private int highPointer = 0;
        private int lowPointer = 0;
        private int incorrectNotes = 0;
        private int _paragraphNumber = 1;

        private List<int> _noteNumberHigh = new List<int> ();
        private List<int> _noteNumberLow = new List<int> ();

        private Symbol _symbol;

        // Start is called before the first frame update
        void Start () {
            _symbol = new Note ("D", "4");
            _symbol.SetChord (false);
            _symbol.SetDuration ("256", "256");
            _symbol.SetType ("quarter");
            _noteNumberHigh = FindNotesPerParagraph (_noteDatabase.GetHighNotes ());
            _noteNumberLow = FindNotesPerParagraph (_noteDatabase.GetLowNotes ());
            _smoothedHighNotes = SmoothList (_noteDatabase.GetHighNotes ());
            _smoothedLowNotes = SmoothList (_noteDatabase.GetLowNotes ());
            _highNotes = ConvertToNumber (_smoothedHighNotes);
            _lowNotes = ConvertToNumber (_smoothedLowNotes);
            highPointer = 0;
            lowPointer = 0;
            incorrectNotes = 0;
            _paragraphNumber = 1;
        }

        int noteStatus;
        public int noteNumber;

        //Release and press status values for notes
        int release = 128;
        int press = 144;
        //HashSet to keep track of current notes pressed
        HashSet<int> notesPressed = new HashSet<int> ();

        // Update is called once per frame
        void Update () {
            //HashSet to keep track of current notes pressed    
            //Reset the list each time
            //HashSet<int> notesPressed = new HashSet<int>();

            //Get queue of notes played
            Queue<MidiJack.MidiMessage> myQueue = MidiDriver.Instance.History;

            //Change queue to array so we can get last note pressed or released
            MidiMessage[] messageArray = myQueue.ToArray ();

            if (messageArray.Length > 0) {
                //Get the last note played (represented as message)
                MidiMessage message = messageArray[messageArray.Length - 1];

                noteNumber = message.data1;
                noteStatus = message.status;

                //If notes status indicates press, add the note to the hashset
                //Otherwise, if it detects the note is released, then remove the note from the hashset
                if (noteStatus == press) {
                    if (!notesPressed.Contains (noteNumber)) {
                        updateSheetMusic (noteNumber);
                    }
                    notesPressed.Add (noteNumber);
                } else if (noteStatus == release) {
                    notesPressed.Remove (noteNumber);
                }
            }

            CheckNotesMissed ();
            CheckReset ();

        }

        private void CheckNotesMissed () {
            if (_paragraphNumber < _summaryMaster.GetParagraphNumber ()) {

                int expectedHigh = _noteNumberHigh[_paragraphNumber - 1];
                int expectedLow = _noteNumberLow[_paragraphNumber - 1];

                if (highPointer < expectedHigh) {
                    _summaryMaster.AddHighNotesMissed (expectedHigh - highPointer);
                    highPointer = expectedHigh;
                }

                if (lowPointer < expectedLow) {
                    _summaryMaster.AddLowNotesMissed (expectedLow - lowPointer);
                    lowPointer = expectedLow;
                }

                _paragraphNumber = _summaryMaster.GetParagraphNumber ();
            }
        }

        private void CheckReset () {
            if (_summaryMaster.GetReset ()) {
                highPointer = 0;
                lowPointer = 0;
                incorrectNotes = 0;
                _paragraphNumber = 1;
            }
        }

        //Method to pass in notes played
        public void updateSheetMusic (int noteNumber) {
            int octave = (noteNumber - 12) / 12;
            int stepNum = noteNumber - 12 - (octave * 12);
            string step = "";

            switch (stepNum) {
                case 0:
                    step = "C";
                    break;
                case 2:
                    step = "D";
                    break;
                case 4:
                    step = "E";
                    break;
                case 5:
                    step = "F";
                    break;
                case 7:
                    step = "G";
                    break;
                case 9:
                    step = "A";
                    break;
                case 11:
                    step = "B";
                    break;
            }

            ((Note) _symbol).SetOctave (octave.ToString ());
            ((Note) _symbol).SetStep (step);

            //Pass in notes played
            noteNumber = noteNumber - 12;
            _summaryMaster.AddNotePlayed (noteNumber);

            bool isHighChord = false;
            bool isLowChord = false;
            bool noMatch = true;
            int highNoteNumber;
            if (highPointer < _highNotes.Count) {
                highNoteNumber = _highNotes[highPointer];
                if (_smoothedHighNotes[highPointer].IsChord ()) {
                    isHighChord = true;
                }
            } else {
                highNoteNumber = -999;
            }

            int lowNoteNumber;
            if (lowPointer < _lowNotes.Count) {
                lowNoteNumber = _lowNotes[lowPointer];
                if (_smoothedLowNotes[lowPointer].IsChord ()) {
                    isLowChord = true;
                }
            } else {
                lowNoteNumber = -999;
            }

            if (noteNumber == highNoteNumber) {
                _smoothedHighNotes[highPointer].ChangeColor (Color.green);
                highPointer++;
                noMatch = false;
            }
            if (noteNumber == lowNoteNumber) {
                _smoothedLowNotes[lowPointer].ChangeColor (Color.green);
                lowPointer++;
                noMatch = false;
            }

            if (noMatch) {
                incorrectNotes++;
                if (ClosestHigh (noteNumber, highNoteNumber, lowNoteNumber)) {
                    _smoothedHighNotes[highPointer].ChangeColor (Color.red);
                } else {
                    _smoothedLowNotes[lowPointer].ChangeColor (Color.red);
                }
                _summaryMaster.SetNotesIncorrect (incorrectNotes);
            }

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

        private int ConvertToNumberSingle (Note note) {

            int step = StepToInt (note);
            int octave = Int32.Parse (note.GetOctave ()) * 12;
            int noteNumber = octave + step;

            return noteNumber;
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

        private List<int> FindNotesPerParagraph (List<List<Note>> measures) {
            List<int> notesPerParagraph = new List<int> ();

            int noteCount = 0;
            int measureCount = 0;
            foreach (List<Note> notes in measures) {
                measureCount++;
                if (measureCount >= 5) {
                    measureCount = 1;
                    notesPerParagraph.Add (noteCount);
                    //noteCount = 0;
                }
                foreach (Note note in notes) {
                    noteCount++;
                }
            }

            return notesPerParagraph;
        }

        private bool ClosestHigh (int note, int highNote, int lowNote) {

            int highDiff = Math.Abs (note - highNote);
            int lowDiff = Math.Abs (note - lowNote);

            if (highDiff <= lowDiff) {
                return true;

            } else {
                return false;
            }
        }
    }
}