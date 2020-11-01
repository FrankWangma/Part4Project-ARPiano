using System;
using System.Collections.Generic;

namespace symbol
{
    public class Measure
    {
        private List<List<List<Symbol>>> _measureSymbolList;
        private Beat _beat;
        private Head _highHead;
        private Head _lowHead;
        private bool _hasHead;
        private bool _hasBeat; 
        private int _maxCount;
        private int _measureLength; 

        public Measure(List<List<List<Symbol>>> measureSymbolList)
        {
            _measureSymbolList = measureSymbolList;
        }

        //Method to set the measure symbol list
        //Used to dynamically change the measure
        public void SetMeasureSymbolList(List<List<List<Symbol>>> measureSymbolList) { _measureSymbolList = measureSymbolList; }

        public List<List<List<Symbol>>> GetMeasureSymbolList() { return _measureSymbolList; }

        public bool HasHead() { return _hasHead; }

        public void SetHasHead(bool hasHead) { _hasHead = hasHead; }

        public bool HasBeat() { return _hasBeat; }

        public void SetHasBeat(bool hasBeat) { _hasBeat = hasBeat; }

        public Beat GetBeat() { return _beat; }

        public void SetBeat(Beat beat) { _beat = beat; }

        public List<Head> GetHead()
        {
            List<Head> headList = new List<Head>();
            headList.Add(_highHead);
            headList.Add(_lowHead);
            return headList;
        }

        public void SetHead(Head highHead, Head lowHead) { _highHead = highHead; _lowHead = lowHead; }

        public void SetMaxCount(int maxCount) { _maxCount = maxCount; }

        public int GetMaxCount() { return _maxCount; }

        public void SetMeasureLength(int measureLength) { _measureLength = measureLength; }

        public int GetMeasureLength() { return _measureLength; }
    }
}