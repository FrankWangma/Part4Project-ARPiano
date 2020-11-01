using System.Collections.Generic;
using UnityEngine;

namespace symbol
{
    public class Note : Symbol
    {
        private string _step;
        private string _octave;
        private string _beam;
        private string _accidental;
        private int _shift;
        private int _end;
        private Note _lastNote;      
        private List<Note> _chordList = new List<Note>();
        private bool _slur;
        private bool _last;
        private bool _next;
        private bool _hasChord;
        private bool _upOrDown;

        public Note(string step, string octave)
        {
            _step = step;
            _octave = octave;
        }

        public void SetStep(string step) { _step = step; }

        public void SetOctave(string octave) { _octave = octave; }

        public string GetStep() { return _step; }

        public string GetOctave() { return _octave; }

        public string GetBeam() { return _beam; }

        public void SetBeam(string beam) { _beam = beam; }

        public string GetAccidental() { return _accidental; }

        public string GetAccidental(string fifth)
        {
            if (_accidental.Equals(""))
            {
                return GetAdjustedAccidental(fifth);
            }
            return _accidental;
        }

        public void SetAccidental(string accidental) { _accidental = accidental; }

        public int GetShift() { return _shift; }

        public void SetShift(int shift) { _shift = shift; }

        public int GetEnd() { return _end; }

        public void SetEnd(int end) { _end = end; }

        public Note GetLastNote() { return _lastNote; }

        public void SetLastNote(Note lastNote) { _lastNote = lastNote; }

        public List<Note> GetChordList() { return _chordList; }

        public bool IsSlur() { return _slur; }

        public void SetSlur(bool slur) { _slur = slur; }

        public bool IsLast() { return _last; }

        public void SetLast(bool last) { _last = last; }

        public bool IsNext() { return _next; }

        public void SetNext(bool next) { _next = next; }

        public bool HasChord() { return _hasChord; }

        public void SetHasChord(bool hasChord) { _hasChord = hasChord; }

        public bool IsUpOrDown()
        {
            if (IsSlur())
            {
                return _upOrDown;
            }
            //            return GetShift() < 0;
            return _upOrDown;
        }

        public void SetUpOrDown(bool upOrDown) { _upOrDown = upOrDown; }

        public override void SetType(string type)
        {
            switch (type)
            {
                case "whole": base.Type = 1; break;
                case "half": base.Type = 2; break;
                case "quarter": base.Type = 4; break;
                case "eighth": base.Type = 8; break;
                case "16th": base.Type = 16; break;
                default: break;
            }
        }

        private string GetAdjustedAccidental(string fifth)
        {
            string output = "";
            int fifthInt = int.Parse(fifth);

            if (fifthInt > 0)
            {
                if (OrderOfSharps() <= fifthInt)
                {
                    output = "sharp";
                }
            }
            else if (fifthInt < 0)
            {
                if (OrderOfFlats() >= fifthInt)
                {
                    output = "flat";
                }
            }

            return output;
        }

        private int OrderOfSharps()
        {
            int output = 0;
            switch (_step)
            {
                case "C": output = 2; break;
                case "D": output = 4; break;
                case "E": output = 6; break;
                case "F": output = 1; break;
                case "G": output = 3; break;
                case "A": output = 5; break;
                case "B": output = 7; break;
                default: break;
            }

            return output;
        }

        private int OrderOfFlats()
        {
            int output = 0;
            switch (_step)
            {
                case "C": output = -6; break;
                case "D": output = -4; break;
                case "E": output = -2; break;
                case "F": output = -7; break;
                case "G": output = -5; break;
                case "A": output = -3; break;
                case "B": output = -1; break;
                default: break;
            }

            return output;
        }

        public override int GetRate()
        {
            int rate = 1;
            switch (_step)
            {
                case "C": rate = 33; break;
                case "D": rate = 37; break;
                case "E": rate = 41; break;
                case "F": rate = 44; break;
                case "G": rate = 49; break;
                case "A": rate = 55; break;
                case "B": rate = 62; break;
                default: break;
            }
            switch (_octave)
            {
                case "2": rate *= 2; break;
                case "3": rate *= 4; break;
                case "4": rate *= 8; break;
                case "5": rate *= 16; break;
                case "6": rate *= 32; break;
                case "7": rate *= 64; break;
                default: break;
            }
            return rate;
        }
    }
}