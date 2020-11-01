using UnityEngine;

namespace symbol
{
    public abstract class Symbol
    {
        protected int Duration;
        protected int StartTime;
        protected int StopTime;
        protected int Type;
        protected int Dot;
        protected int SymbolWidth;
        protected bool isChord; 
        protected bool isHigh; //Checks if the symbol is high or low
        protected SymbolView _symbolView = null;
        private Color _color = Color.black; //Controls the color which the note is rendered in

        public int GetDuration() { return Duration; }

        public void SetDuration(string divisions, string duration) { Duration = 64 * int.Parse(duration) / int.Parse(divisions); }

        public int GetStartTime() { return StartTime; }

        public void SetStartTime(int startTime) { StartTime = startTime * 100 / 12; }

        public int GetStopTime() { return StopTime; }

        public void SetStopTime(int stopTime) { StopTime = stopTime * 100 / 12; }

        public new int GetType() { return Type; }

        public int GetDot() { return Dot; }

        public void SetDot(int dot) { Dot = dot; }

        public int GetSymbolWidth() { return SymbolWidth; }

        public void SetSymbolWidth(int symbolWidth) { SymbolWidth = symbolWidth; }

        public bool IsChord() { return isChord; }

        public void SetChord(bool chord) { isChord = chord; }

        public bool IsHigh() { return isHigh; }

        public void SetHigh(bool high) { isHigh = high; }

        public void ChangeColor(Color color) { GetSymbolView().SetColor(color); }

        public SymbolView GetSymbolView() { return _symbolView; }

        public void SetSymbolView(SymbolView symbolview) { _symbolView = symbolview; }

        public abstract void SetType(string type);

        public abstract int GetRate();
        public void SetColor(Color color)
        {
            _color = color;
            if (_symbolView != null)
            {
                _symbolView.SetColor(_color); // Updates color when color is set (hopefully)
            }

        }
        public Color GetColor() { return _color; }

    }
}