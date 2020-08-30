using System;
using System.Collections.Generic;
using util;
using UnityEngine;
using UnityEngine.UI;
using symbol;

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

        private NoteDatabase _noteDatabase = NoteDatabase.GetInstance();
        private static ChordDatabase _chordDatabase = ChordDatabase.GetInstance();

        private int _color = 0;

        public void DrawPatterns()
        {
            _lowNotes = _noteDatabase.GetLowNotes();
            _highNotes = _noteDatabase.GetHighNotes();
            LoopThrough(_lowNotes);
        }

        private void LoopThrough(List<List<Note>> notes)
        {
            foreach (List<Note> measure in notes)
            {
                List<Note> separateMeasure = SeparateNotes(measure);
                FindPattern(separateMeasure);
            }
        }

        private List<Note> SeparateNotes(List<Note> measure)
        {
            List<Note> separateNotes = new List<Note>();

            foreach (Note note in separateNotes)
            {

                //CURRENT BUG IN CHORD FUNCTIONALITY. IS CHORD NOT WORKING PROPERLY
                if (note.IsChord())
                {
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
                        chords.Add(myChord);
                        notes.Add(myNotes);
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
                chords.Add(myChord);
                notes.Add(myNotes);
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
                        chords.Add(myChord);
                        notChord = false;
                    }
                    else
                    {
                        //Undo notes that were added
                        for (int i = 0; i < count; i++)
                        {
                            myNotes.RemoveAt(myNotes.Count - 1 - i);
                        }
                    }
                }

                //If still can't create new chord, checks if it's a seventh
                if (notChord)
                {
                    myChord.UnionWith(chords[chords.Count - 1]);
                    if (!(_chordDatabase.IdentifyChord(myChord) == null))
                    {
                        //If it is a seventh, assign it over the previous chord
                        chords[chords.Count - 1] = myChord;
                        notes[notes.Count - 1].AddRange(myNotes);
                    }
                }
            }

            CheckPattern(chords, notes);
        }

        private void CheckPattern(List<HashSet<String>> chords, List<List<Note>> notes)
        {
            for (int i = 0; i < chords.Count; i++)
            {
                if (!(_chordDatabase.IdentifyChord(chords[i]) == null))
                {
                    //GET THE TYPE CHORD HERE AND DO SOMETHING WITH IT
                    SetChordColor(notes[i]);
                }
            }
        }

        private void SetChordColor(List<Note> notes)
        {
            Color myColor = ColorHandler();

            foreach (Note note in notes)
            {
                note.SetColor(myColor);
            }
        }

        private String GetNote(Note note)
        {
            String accidental = note.GetAccidental();
            String step = note.GetStep();
            String output = accidental + step;
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
        private Color ColorHandler()
        {
            Color myColor = Color.black;
            switch (_color)
            {
                case 0: myColor = Color.blue; break;
                case 1: myColor = Color.cyan; break;
                case 2: myColor = Color.yellow; break;
            }

            _color ++;
            if(_color > 2){
                _color = 0;
            }

            return myColor;
        }
    }
}