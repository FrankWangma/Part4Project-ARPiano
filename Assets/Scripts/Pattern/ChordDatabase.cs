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

        private void addChord(String firstNote, String secondNote, String thirdNote, String chordName){
            HashSet<String> chord = new HashSet<string>();
            chord.Add(firstNote);
            chord.Add(secondNote);
            chord.Add(thirdNote);

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
    }
}