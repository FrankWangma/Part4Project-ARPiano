using System;
using System.Collections.Generic;
using util;
using UnityEngine;
using UnityEngine.UI;
using symbol;

namespace Pattern
{
    public class ChordDatabase
    {
        private Dictionary<HashSet<String>, String> _chordDatabase = new Dictionary<HashSet<String>, String>();

        private static ChordDatabase instance = new ChordDatabase();

        public static ChordDatabase GetInstance() { return instance;}

        private ChordDatabase(){
            addMajorChords();
            addMinorChords();
            addDiminishedChords();
            addSeventhChords();
            addMajorSeventhChords();
        }

        private void addChord(String firstNote, String secondNote, String thirdNote, String chordName){
            HashSet<String> chord = new HashSet<string>();
            chord.Add(firstNote);
            chord.Add(secondNote);
            chord.Add(thirdNote);

            _chordDatabase.Add(chord, chordName);
        }

        
        private void addChord(String firstNote, String secondNote, String thirdNote, String fourthNote, String chordName){
            HashSet<String> chord = new HashSet<string>();
            chord.Add(firstNote);
            chord.Add(secondNote);
            chord.Add(thirdNote);
            chord.Add(fourthNote);

            _chordDatabase.Add(chord, chordName);
        }      

        public String IdentifyChord(HashSet<String> notes){
            String output;
            //Check if key exists, if it does give output string the key value
            //If key does not exist, method returns null
            if (!_chordDatabase.TryGetValue(notes, out output)){
                return null;
            }
            return output;
        }

        public void addMajorChords() {
            addChord("C", "E", "G", "C Major");
            addChord("C", "Fsharp", "A", "D Major");
            addChord("E", "Gsharp", "B", "E Major");
            addChord("F", "G", "C", "F Major");
            addChord("G", "B", "D", "D Major");
            addChord("A", "Csharp", "E", "A Major");
            addChord("B", "Dsharp", "Fsharp", "B Major");
            addChord("Csharp", "F", "Gsharp", "C Sharp Major");
            addChord("Dsharp", "G", "Asharp", "D Sharp Major");
            addChord("Fsharp", "Asharp", "Csharp", "F Sharp");
            addChord("Gsharp", "C", "Dsharp", "G Sharp Major");
            addChord("Asharp", "D", "F", "A Sharp Major");
        }

        public void addMinorChords() {
            addChord("C", "Dsharp", "G", "C Minor");
            addChord("D", "F", "A", "D Minor");
            addChord("E", "G", "B", "E Minor");
            addChord("F", "Gsharp", "C", "F Minor");
            addChord("G", "Asharp", "D", "G Minor");
            addChord("A", "C", "E", "A Minor");
            addChord("B", "D", "Fsharp", "B Minor");
            addChord("Csharp", "E", "Asharp", "C Sharp Minor");
            addChord("Dsharp","Fsharp","Asharp", "D Sharp Minor");
            addChord("Fsharp", "A", "Csharp", "F Sharp Minor");
            addChord("Gsharp", "B", "Dsharp", "G Sharp Minor");
            addChord("Asharp", "Csharp", "F", "A Sharp Minor");
        }

        public void addDiminishedChords() {
            addChord("C", "Dsharp", "Fsharp", "C Diminished");
            addChord("D", "F", "Gsharp", "D Diminished");
            addChord("E", "G", "Asharp", "E Diminished");
            addChord("F", "Gsharp", "B", "F Diminished");
            addChord("G", "Asharp", "Csharp", "G Diminished");
            addChord("A", "C", "Dsharp", "A Diminished");
            addChord("B", "D", "F", "B Diminshed");
            addChord("Csharp", "E", "G", "C Sharp Diminished");
            addChord("Dsharp", "Fsharp", "A", "D Sharp Diminished");
            addChord("Fsharp", "A", "C", "F Sharp Diminished");
            addChord("Gsharp", "B", "D", "G Sharp Diminished");
            addChord("Asharp", "Csharp", "E", "A Sharp Diminished");
        }

        public void addSeventhChords() {
            addChord("C", "E", "G", "Asharp", "C 7");
            addChord("D", "Fsharp", "A", "C", "D 7");
            addChord("E", "Gsharp", "B", "D", "E 7");
            addChord("F", "A", "C", "Dsharp", "F 7");
            addChord("G", "B", "D", "Fsharp", "G 7");
            addChord("A", "Csharp", "E", "G", "A 7");
            addChord("B", "Dsharp", "Fsharp", "A", "B 7");
            addChord("Csharp", "F", "Gsharp", "B", "C Sharp 7");
            addChord("Dsharp", "G", "Asharp", "Csharp", "D Sharp 7");
            addChord("Fsharp", "Asharp", "Csharp", "E", "F Sharp 7");
            addChord("Gsharp", "C", "Bsharp", "Fsharp", "G Sharp 7");
            addChord("Asharp", "D", "F", "Gsharp", "A Sharp 7");
        }

        public void addMajorSeventhChords() {
            addChord("C", "E", "G", "B", "C Major 7");
            addChord("D", "Fsharp", "A", "Csharp", "D Major 7");
            addChord("E", "Gsharp", "B", "Dsharp", "E Major 7");
            addChord("F", "A", "C", "E", "F Major 7");
            addChord("G", "B", "D", "Fsharp", "G Major 7");
            addChord("A", "Csharp", "E", "Gsharp", "A Major 7");
            addChord("B", "Dsharp", "Fsharp", "Asharp", "B Major 7");
            addChord("Csharp", "F", "Gsharp", "C", "C Sharp Major 7");
            addChord("Dsharp", "G", "Asharp", "D", "D Sharp Major 7");
            addChord("Fsharp", "Asharp", "Dsharp", "F", "F Sharp Major 7");
            addChord("Gsharp", "C", "Dsharp", "G", "G Sharp Major 7");
            addChord("Asharp", "D", "F", "A", "A Sharp Major 7");
        }
    }
}