using System;
using System.Collections.Generic;
using util;
using UnityEngine;
using UnityEngine.UI;
using symbol;

namespace Pattern
{
    public class NoteDatabase
    {
        private List<List<Measure>> _scoreList;

        private List<List<Note>> _highNotes = new List<List<Note>>();
        private List<List<Note>> _lowNotes = new List<List<Note>>();
        private List<Measure> _measures = new List<Measure>();

        private static NoteDatabase instance = new NoteDatabase();

        public static NoteDatabase GetInstance() { return instance; }

        public void AddScoreList(List<List<Measure>> scoreList)
        {
            _scoreList = scoreList;
            foreach (List<Measure> measures in _scoreList)
            {
                _measures.AddRange(measures);
            }
            SortScoreList();

            PatternAlgorithm pattern = new PatternAlgorithm();
            pattern.DrawPatterns();

            //Test code to check method works
            ///ChangeNoteColor(1, 0, Color.green, false);
            //When we are looking at a chord, check if is chord using .HasChord(), then use .GetChordList() to get list of notes that make up the chord
            //Debug.Log("note " + _lowNotes[1][0].GetStep() + " " + _lowNotes[1][0].IsChord() + " " + _lowNotes[1][0].GetChordList().Count + " " + _lowNotes[4][0].GetChordList()[1].GetStep());
            //foreach(Note note in _lowNotes[5][0].GetChordList()){
                //Debug.Log("note chord " + note.GetStep());
            //}
            //Debug.Log("note " + _lowNotes[3][0].GetStep() + " " + _lowNotes[3][0].IsChord() + " " + _lowNotes[3][0].GetChordList().Count);
            //Debug.Log("note " + _lowNotes[10][0].GetStep() + _lowNotes[10][0].GetChordList()[0].GetStep() + " " + _lowNotes[10][0].GetChordList()[1].GetStep()  + " " + _lowNotes[10][0].IsChord() + " " + _lowNotes[10][0].GetChordList().Count);
            //GetChordList()[0].GetStep()
             //Debug.Log("note " + _highNotes[2][3].GetStep() );
            //Accidental is empty if no accidental
            //Debug.Log("note " + _highNotes[3][0].GetStep());
            //Debug.Log("Accidental " + _highNotes[3][0].GetAccidental() );
        }

        //Adds notes to their corresponding bar, in order and based on measure
        private void SortScoreList()
        {
            for (int i = 0; i < _measures.Count; i++)
            {
                List<Note> measureHighNotes = new List<Note>();
                List<Note> measureLowNotes = new List<Note>();

                List<List<List<Symbol>>> measureSymbolList = new List<List<List<Symbol>>>();
                measureSymbolList = _measures[i].GetMeasureSymbolList();

                foreach (List<List<Symbol>> setList in measureSymbolList)
                {
                    List<Symbol> highSymbolList = setList[0];
                    List<Symbol> lowSymbolList = setList[1];
                    foreach (Symbol symbol in highSymbolList)
                    {
                        if (symbol is Note)
                        {
                            measureHighNotes.Add((Note)symbol);
                        }
                    }
                    foreach (Symbol symbol in lowSymbolList)
                    {
                        if (symbol is Note)
                        {
                            measureLowNotes.Add((Note)symbol);
                        }
                    }
                }
                _highNotes.Add(measureHighNotes);
                _lowNotes.Add(measureLowNotes);
            }
        }

        public List<List<Note>> GetHighNotes(){ return _highNotes; }

        public List<List<Note>> GetLowNotes(){ return _lowNotes; }

        public void ChangeNoteColor(int measurePosition, int notePosition, Color color, Boolean highNote){
            if(highNote){
                Note toChange = _highNotes[measurePosition][notePosition];
                toChange.SetColor(color);
            } 
            else{
                Note toChange = _lowNotes[measurePosition][notePosition];
                toChange.SetColor(color);
            }
        }
    }
}