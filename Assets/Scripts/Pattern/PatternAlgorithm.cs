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
        private List<Note> _notes = new List<Note>();

        private static NoteDatabase instance = new NoteDatabase();

        public PatternAlgorithm(List<Note> measure)
        {
            Init();
        }

        private void Init()
        {

        }
    }
}