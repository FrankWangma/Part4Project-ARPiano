using System;
using System.Collections.Generic;
using util;
using UnityEngine;
using UnityEngine.UI;
using symbol;
using control;

namespace Pattern
{
    public class PatternAlgorithm
    {
        /**
Hashset - add notes up to three

If duplicates are attempted to be added, stored the duplicate in a temporary list

If a NEW note is found create a new hashset

If the hashset new hashset only has TWO notes, add the MOST RECENT duplicate to the new hashset

if the hashset only has ONE note, check if its is a 7th

if is it NOT a 7th, and there are TWO duplicate notes or greater, we add the most two recent, and check if its a pattern
        **/
        private List<List<Note>> _highNotes = new List<List<Note>>();
        private List<List<Note>> _lowNotes = new List<List<Note>>();
        string _fifth = "0";

        private NoteDatabase _noteDatabase = NoteDatabase.GetInstance();
        private static ChordDatabase _chordDatabase = ChordDatabase.GetInstance();

        public void DrawPatterns()
        {
            _lowNotes = _noteDatabase.GetLowNotes();
            _highNotes = _noteDatabase.GetHighNotes();
            _fifth = _noteDatabase.GetFifth();
            LoopThrough(_lowNotes);
            LoopThrough(_highNotes);
        }

        private void LoopThrough(List<List<Note>> notes)
        {
            foreach (List<Note> measure in notes)
            {
                List<Note> separateMeasure = SeparateNotes(measure);
                foreach (Note note in separateMeasure)
                {
                    //Debug.Log("Note " + note.GetStep());
                }
                FindPattern(separateMeasure);
            }
        }

        private List<Note> SeparateNotes(List<Note> measure)
        {
            List<Note> separateNotes = new List<Note>();

            foreach (Note note in measure)
            {
                if (note.GetChordList().Count > 0)
                {
                    separateNotes.Add(note);
                    foreach (Note chordNote in note.GetChordList())
                    {
                        separateNotes.Add(chordNote);
                    }
                }
                else
                {
                    separateNotes.Add(note);
                }
            }

            return separateNotes;
        }

        private void FindPattern(List<Note> measure)
        {
            List<HashSet<String>> chords = new List<HashSet<String>>();
            List<List<Note>> notes = new List<List<Note>>();

            HashSet<String> myChord = new HashSet<string>();
            List<Note> myNotes = new List<Note>();
            List<String> duplicates = new List<string>();
            List<Note> duplicateNotes = new List<Note>();
            String lastNote;

            foreach (Note note in measure)
            {
                String noteString = GetNote(note);

                if (myChord.Contains(noteString))
                {
                    duplicates.Add(noteString);
                    duplicateNotes.Add(note);
                    myNotes.Add(note);
                }
                else
                {
                    if (myChord.Count >= 3)
                    {
                        HashSet<String> copy = new HashSet<string>(myChord);
                        List<Note> notesCopy = new List<Note>(myNotes);
                        chords.Add(copy);
                        notes.Add(notesCopy);
                        myNotes.Clear();
                        myChord.Clear();
                    }
                    myChord.Add(noteString);
                    myNotes.Add(note);
                    lastNote = noteString;
                }
            }

            //If two notes, add most recent duplicate
            if (myChord.Count == 2)
            {
                if (duplicates.Count > 0)
                {
                    myChord.Add(duplicates[duplicates.Count - 1]);
                    myNotes.Add(duplicateNotes[duplicateNotes.Count - 1]);
                }
            }

            //Add to chords if the number is correct
            if (myChord.Count >= 3)
            {
                HashSet<String> copy = new HashSet<string>(myChord);
                List<Note> notesCopy = new List<Note>(myNotes);
                chords.Add(copy);
                notes.Add(notesCopy);
                myChord.Clear();
                duplicates.Clear();
            }

            if (myChord.Count == 1)
            {
                //If unique duplicates greater than two, creates a chord of three
                int count = 0;
                if (duplicates.Count > 0)
                {
                    for (int i = 0; i < duplicates.Count; i++)
                    {
                        myChord.Add(duplicates[duplicates.Count - 1 - i]);
                        myNotes.Add(duplicateNotes[duplicateNotes.Count - 1 - i]);
                        count = i + 1;
                        if (myChord.Count >= 3)
                        {
                            break;
                        }
                    }
                }


                Boolean notChord = true;

                if (myChord.Count >= 3)
                {
                    if (!(_chordDatabase.IdentifyChord(myChord) == null))
                    {
                        //Debug.Log("HI HI " + chords.Count + " " + notes.Count);
                        //Debug.Log("HI HI " + _chordDatabase.IdentifyChord(myChord));
                        HashSet<String> copy = new HashSet<string>(myChord);
                        List<Note> notesCopy = new List<Note>(myNotes);
                        chords.Add(copy);
                        notes.Add(notesCopy);
                        notChord = false;
                    }
                    else
                    {
                        //Undo notes that were added
                        for (int i = 0; i < count; i++)
                        {
                            //Debug.Log("Index size " + myNotes.Count + " " + i + " " + count);
                            //myNotes.RemoveAt(myNotes.Count - 1 - i);
                            myNotes.RemoveAt(myNotes.Count - 1);
                        }
                    }
                }

                //If still can't create new chord, checks if it's a seventh
                //Remember that this is only creating shallow copies, so if you want to work with chords afterwards, create deep copies
                if (notChord)
                {
                    if (chords.Count >= 1)
                    {
                        myChord.UnionWith(chords[chords.Count - 1]);
                        if (!(_chordDatabase.IdentifyChord(myChord) == null))
                        {
                            //If it is a seventh, assign it over the previous chord
                            chords[chords.Count - 1] = myChord;
                            notes[notes.Count - 1].AddRange(myNotes);
                            //Debug.Log("HI HI " + chords.Count + " " + notes.Count);
                        }
                    }
                }
            }

            //Rearrange notes if ending note of a chord is the same as the starting note of next chord
            if(chords.Count > 1){
                for(int i = 1; i < chords.Count; i++){
                    String chordName = _chordDatabase.IdentifyChord(chords[i]);
                    if(chordName != null){
                        String major = chordName.Substring(0, 1);
                        Note last = notes[i-1][notes[i-1].Count-1];
                        if(last.GetStep().Equals(major)){
                            notes[i].Add(last);
                            notes[i-1].RemoveAt(notes[i-1].Count -1);
                        }
                    }
                }
            }

            CheckPattern(chords, notes);
        }

        private void CheckPattern(List<HashSet<String>> chords, List<List<Note>> notes)
        {
            foreach (List<Note> note in notes)
            {
                String output = "";
                foreach (Note n in note)
                {
                    output = output + " " + n.GetStep();
                }
                //Debug.Log("notes" + output);
            }

            for (int i = 0; i < chords.Count; i++)
            {
                //Debug.Log("chordSize " + chords.Count);
                //Debug.Log("notes " + notes.Count);
                //Debug.Log("chord " + _chordDatabase.IdentifyChord(chords[i]));
                if (!(_chordDatabase.IdentifyChord(chords[i]) == null))
                {
                    //GET THE TYPE CHORD HERE AND DO SOMETHING WITH IT
                    //Debug.Log("SET CHORD COLOR");
                    Debug.Log("Chord + " + _chordDatabase.IdentifyChord(chords[i]));
                    //Debug.Log("notes " + notes[i].Count);
                    // String output = "";
                    // foreach (Note n in notes[i])
                    // {
                    //     output = output + " " + n.GetStep();
                    // }
                    // Debug.Log("notes" + output);
                    //Debug.Log("Index + " + i + "Notes " + notes.Count + "Chords " + chords.Count);
                    SetChordColor(notes[i], _chordDatabase.IdentifyChord(chords[i]));
                }
            }
        }

        private void SetChordColor(List<Note> notes, String major)
        {
            Color myColor = ColorHandler(major.Substring(0, 1));

            //default color for accidental chords
            if(major.Substring(2,5).Equals("Sharp")){
                myColor = Color.black;
            }
            List<String> diffNotes = _chordDatabase.IdentifyDiff(major);

            List<Color> colors = new List<Color>();
            control.CanvasControl._notes.Add(notes);
            foreach (Note note in notes)
            {
                note.SetColor(myColor);

                //Change color of non-major notes
                foreach (String diff in diffNotes){
                    if(GetNote(note).Equals(diff)){
                        note.SetColor(Color.cyan);
                    }
                }
                colors.Add(myColor);
            }

            _noteDatabase.AddColorsToList(colors);
        }

        private String GetNote(Note note)
        {
            String accidental = note.GetAccidental(_fifth);
            String step = note.GetStep();
            String output = step + accidental;
            if (accidental.Equals("flat"))
            {
                output = Transpose(step);
            }

            return output;
        }

        //Transposes flat to sharp
        private String Transpose(String step)
        {
            switch (step)
            {
                case "C": step = "B"; break;
                case "D": step = "C"; break;
                case "E": step = "D"; break;
                case "F": step = "E"; break;
                case "G": step = "F"; break;
                case "A": step = "G"; break;
                case "B": step = "H"; break;
            }

            step = step + "sharp";

            return step;
        }

        //handles color of chords
        private Color ColorHandler(String chordnote)
        {
            Color myColor = Color.black;
            switch (chordnote)
            {
                case "A": myColor = Color.yellow; break;
                case "B": myColor = Color.magenta; break; //Pink
                case "C": myColor = Color.green; break; //C is Green
                case "D": myColor = Color.blue; break; //D is blue
                case "E": myColor = new Color(0.52F, 0.16F, 0.89F); break; //Purple
                case "F": myColor = Color.red; break;
                case "G": myColor = new Color(1.0F, 0.4F, 0.0F); break; //Orange
            }

            return myColor;
        }
    }
}