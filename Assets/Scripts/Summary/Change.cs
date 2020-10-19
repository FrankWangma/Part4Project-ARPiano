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
        private int _highNotesCorrect = 0;
        private int _lowNotesCorrect = 0;

        private List<int> _noteNumberHigh = new List<int> ();
        private List<int> _noteNumberLow = new List<int> ();

        HashSet<int> _notesRecorded = new HashSet<int> ();

        private Symbol _symbol;

        bool _highIncorrect = false;
        bool _lowIncorrect = false;

        int _highNotesTooFast = 0;
        int _lowNotesTooFast = 0;

        private bool highTooFast = false;
        private bool lowTooFast = false;

        // Start is called before the first frame update
        void OnEnable () {
            _symbol = new Note ("D", "4");
            _symbol.SetChord (false);
            _symbol.SetDuration ("256", "256");
            _symbol.SetType ("quarter");
            _fifth = _noteDatabase.GetFifth ();
            _noteNumberHigh = FindNotesPerParagraph (_noteDatabase.GetHighNotes ());
            _noteNumberLow = FindNotesPerParagraph (_noteDatabase.GetLowNotes ());
            _summaryMaster.SetNumberHigh (_noteNumberHigh);
            _summaryMaster.SetNumberLow (_noteNumberLow);
            _smoothedHighNotes = SmoothList (_noteDatabase.GetHighNotes ());
            _smoothedLowNotes = SmoothList (_noteDatabase.GetLowNotes ());
            _highNotes = ConvertToNumber (_smoothedHighNotes);
            _lowNotes = ConvertToNumber (_smoothedLowNotes);
            highPointer = 0;
            lowPointer = 0;
            incorrectNotes = 0;
            _paragraphNumber = 1;
            _highIncorrect = false;
            _lowIncorrect = false;
            _highNotesTooFast = 0;
            _lowNotesTooFast = 0;
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
            Queue<MidiJack.MidiMessage> myQueue = MidiDriver.Instance.GetHistory ();

            //Change queue to array so we can get last note pressed or released
            MidiMessage[] messageArray = myQueue.ToArray ();

            if (messageArray.Length > 0) {
                foreach (MidiMessage message in messageArray) {
                    //Get the last note played (represented as message)
                    //MidiMessage message = messageArray[messageArray.Length - 1];

                    noteNumber = message.data1;
                    noteStatus = message.status;
                    noteNumber = AdjustNote (noteNumber);

                    //If notes status indicates press, add the note to the hashset
                    //Otherwise, if it detects the note is released, then remove the note from the hashset
                    if (noteStatus == press) {
                        if (!notesPressed.Contains (noteNumber)) {
                            notesPressed.Add (noteNumber);
                            updateSheetMusic (noteNumber, notesPressed);
                            _summaryMaster.AddNotePlayed (noteNumber);
                        }
                    } else if (noteStatus == release) {
                        notesPressed.Remove (noteNumber);
                        _notesRecorded.Remove (noteNumber);
                    }
                }
            }
            CheckReset ();
            CheckNotesMissed ();

            MidiDriver.Instance.ClearHistory ();

        }

        private void CheckNotesMissed () {
            if (_paragraphNumber < _summaryMaster.GetParagraphNumber ()) {
                _paragraphNumber = _summaryMaster.GetParagraphNumber ();
                int expectedHigh = _noteNumberHigh[_paragraphNumber - 2];
                int expectedLow = _noteNumberLow[_paragraphNumber - 2];

                if (highPointer < expectedHigh) {
                    _summaryMaster.AddHighNotesMissed (expectedHigh - highPointer);
                    for (int i = highPointer; i < expectedHigh; i++) {
                        _smoothedHighNotes[i].ChangeColor (Color.red);
                    }
                    highPointer = expectedHigh;
                    _summaryMaster.SetHighPointer (highPointer);
                }

                if (lowPointer < expectedLow) {
                    _summaryMaster.AddLowNotesMissed (expectedLow - lowPointer);
                    for (int i = lowPointer; i < expectedLow; i++) {
                        _smoothedLowNotes[i].ChangeColor (Color.red);
                    }
                    lowPointer = expectedLow;
                    _summaryMaster.SetLowPointer (lowPointer);
                }
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

        //Fix notenumber to keyboard
        private int AdjustNote (int notenumber) {
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
            return noteNumber;
        }

        //Method to pass in notes played
        public void updateSheetMusic (int noteNumber, HashSet<int> notesPressed) {

            highTooFast = false;
            lowTooFast = false;
            bool noMatch = true;

            if (highPointer >= _noteNumberHigh[_paragraphNumber - 1]) {
                _highNotesTooFast++;
                _summaryMaster.SetHighNotesTooFast (_highNotesTooFast);
                highTooFast = true;
            }


            if (lowPointer >= _noteNumberLow[_paragraphNumber - 1]) {
                _lowNotesTooFast++;
                _summaryMaster.SetLowNotesTooFast (_lowNotesTooFast);
                lowTooFast = true;
            }

            HashSet<int> highNoteNumber = new HashSet<int> ();
            //List<int> highNoteNumber = new List<int>();
            if (highPointer < _highNotes.Count) {
                highNoteNumber.Add (_highNotes[highPointer]);
                if (_smoothedHighNotes[highPointer].GetChordList ().Count > 1) {
                    foreach (Note note in _smoothedHighNotes[highPointer].GetChordList ()) {
                        highNoteNumber.Add (ConvertToNumberSingle (note));
                    }
                }
            } else {
                highNoteNumber.Add (-999);
            }

            HashSet<int> lowNoteNumber = new HashSet<int> ();
            //List<int> lowNoteNumber = new List<int> ();
            if (lowPointer < _lowNotes.Count) {
                lowNoteNumber.Add (_lowNotes[lowPointer]);
                if (_smoothedLowNotes[lowPointer].GetChordList ().Count > 1) {
                    foreach (Note note in _smoothedLowNotes[lowPointer].GetChordList ()) {
                        lowNoteNumber.Add (ConvertToNumberSingle (note));
                    }
                }
            } else {
                lowNoteNumber.Add (-999);
            }

            if (!highTooFast) {
                if (highNoteNumber.IsSubsetOf (notesPressed)) {
                    //if (highNoteNumber.Contains(noteNumber)){
                    if (!highNoteNumber.IsSubsetOf (_notesRecorded)) {
                        _notesRecorded.UnionWith (highNoteNumber);
                        _smoothedHighNotes[highPointer].ChangeColor (Color.green);
                        highPointer++;
                        _highNotesCorrect++;
                        _summaryMaster.SetHighNotesCorrect (_highNotesCorrect);
                        _summaryMaster.SetHighPointer (highPointer);
                        noMatch = false;
                        _highIncorrect = false;
                    }
                }
            }
            if (!lowTooFast) {
                if (lowNoteNumber.IsSubsetOf (notesPressed)) {
                    if (!lowNoteNumber.IsSubsetOf (_notesRecorded)) {
                        _notesRecorded.UnionWith (lowNoteNumber);
                        _smoothedLowNotes[lowPointer].ChangeColor (Color.green);
                        lowPointer++;
                        _lowNotesCorrect++;
                        _summaryMaster.SetLowNotesCorrect (_lowNotesCorrect);
                        _summaryMaster.SetLowPointer (lowPointer);
                        noMatch = false;
                        _lowIncorrect = false;
                    }
                }
            }

            if (!highTooFast) {
                if (_highIncorrect) {
                    highNoteNumber.Clear ();
                    if (highPointer + 1 < _highNotes.Count) {
                        highNoteNumber.Add (_highNotes[highPointer + 1]);
                        if (_smoothedHighNotes[highPointer + 1].GetChordList ().Count > 1) {
                            foreach (Note note in _smoothedHighNotes[highPointer + 1].GetChordList ()) {
                                highNoteNumber.Add (ConvertToNumberSingle (note));
                            }
                        }
                    } else {
                        highNoteNumber.Add (-999);
                    }

                    if (highNoteNumber.IsSubsetOf (notesPressed)) {
                        if (!highNoteNumber.IsSubsetOf (_notesRecorded)) {
                            _notesRecorded.UnionWith (highNoteNumber);
                            _smoothedHighNotes[highPointer + 1].ChangeColor (Color.green);
                            highPointer += 2;
                            _highNotesCorrect++;
                            _summaryMaster.SetHighNotesCorrect (_highNotesCorrect);
                            _summaryMaster.SetHighPointer (highPointer);
                            noMatch = false;
                            _highIncorrect = false;
                        }
                    }
                }
            }

            if (!lowTooFast) {
                if (_lowIncorrect) {
                    if (lowPointer + 1 < _noteNumberLow[_paragraphNumber - 1]) {
                        lowNoteNumber.Clear ();
                        if (lowPointer + 1 < _lowNotes.Count) {
                            lowNoteNumber.Add (_lowNotes[lowPointer + 1]);
                            if (_smoothedLowNotes[lowPointer + 1].GetChordList ().Count > 1) {
                                foreach (Note note in _smoothedLowNotes[lowPointer + 1].GetChordList ()) {
                                    lowNoteNumber.Add (ConvertToNumberSingle (note));
                                }
                            }
                        } else {
                            lowNoteNumber.Add (-999);
                        }

                        if (lowNoteNumber.IsSubsetOf (notesPressed)) {
                            if (!lowNoteNumber.IsSubsetOf (_notesRecorded)) {
                                _notesRecorded.UnionWith (lowNoteNumber);
                                _smoothedLowNotes[lowPointer + 1].ChangeColor (Color.green);
                                lowPointer += 2;
                                _lowNotesCorrect++;
                                _summaryMaster.SetLowNotesCorrect (_lowNotesCorrect);
                                _summaryMaster.SetLowPointer (lowPointer);
                                noMatch = false;
                                _lowIncorrect = false;
                            }
                        }
                    }
                }
            }

            if (noMatch) {
                incorrectNotes++;
                if (ClosestHigh (noteNumber, _highNotes[highPointer], _lowNotes[lowPointer])) {
                    _smoothedHighNotes[highPointer].ChangeColor (Color.red);
                    if (!highTooFast) {
                        _highIncorrect = true;
                    }
                } else {
                    _smoothedLowNotes[lowPointer].ChangeColor (Color.red);
                    if (!lowTooFast) {
                        _lowIncorrect = true;
                    }
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
            //int measureCount = 0;
            foreach (List<Note> notes in measures) {
                // measureCount++;
                // if (measureCount >= 5) {
                //     measureCount = 1;
                //     notesPerParagraph.Add (noteCount);

                //     //noteCount = 0;
                // }
                foreach (Note note in notes) {
                    noteCount++;
                }
                notesPerParagraph.Add (noteCount);
            }

            notesPerParagraph.Add (noteCount);

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