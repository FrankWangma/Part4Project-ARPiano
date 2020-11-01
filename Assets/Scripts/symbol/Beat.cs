namespace symbol
{
    public class Beat
    {
        private string _beats;
        private string _beatType;

        public Beat(string beats, string beatType)
        {
            _beats = beats;
            _beatType = beatType;
        }

        public string GetBeats() { return _beats; }

        public string GetBeatType() { return _beatType; }

        public string GetBeatsPerMeasure() { 
            double beats = double.Parse(_beats);
            double beatType = double.Parse(_beatType);
            double bPerMeasure = beats / (beatType / 4);
            
            return bPerMeasure.ToString();
        }
    }
}