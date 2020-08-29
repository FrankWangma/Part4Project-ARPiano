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