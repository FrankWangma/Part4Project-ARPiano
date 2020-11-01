using System.Collections.Generic;
using symbol;
using util;

namespace generator
{
    public class ScoreGenerator
    {
        private int _beats;
        private int _beatType;
        ParamsGetter _paramsGetter = ParamsGetter.GetInstance();

        public ScoreGenerator(string beats, string beatType)
        {
            _beats = int.Parse(beats);
            _beatType = int.Parse(beatType);
        }

        public List<List<Measure>> Generate(List<Measure> measureList, int scoreWidth)
        {
            List<List<Measure>> scoreList = new List<List<Measure>>(); 

            int maxMeasureInLine = 4;

            for (int i = 0; i < measureList.Count; i += maxMeasureInLine)
            {
               
                List<Head> headList = measureList[0].GetHead();
                if (headList != null)
                {
                    measureList[i].SetHead(headList[0], headList[1]);
                    measureList[i].SetHasHead(true);
                }

                List<Measure> paragraphList = new List<Measure>();
                int measureRest = scoreWidth;
                for (int j = i; j < i + 4 && j < measureList.Count; j++)
                {
                    if (measureList[j].HasHead())
                    {
                        measureRest -= _paramsGetter.GetHeadWidth();
                    }
                    if (measureList[j].HasBeat()) 
                    {
                        measureRest -= _paramsGetter.GetBeatWidth();
                    }
                    measureRest -= measureList[j].GetMaxCount() * _paramsGetter.GetUnit(); 
                }
               
                int averageRest = measureRest / maxMeasureInLine;
                for (int j = i; j < i + 4 && j < measureList.Count; j++)
                {
                    int measureLength = measureList[j].GetMaxCount() * _paramsGetter.GetUnit() + averageRest;
                    if (measureList[j].HasHead()) 
                    {
                        measureLength += _paramsGetter.GetHeadWidth();
                    }
                    if (measureList[j].HasBeat()) 
                    {
                        measureLength += _paramsGetter.GetBeatWidth();
                    }
                    measureList[j].SetMeasureLength(measureLength);
                    paragraphList.Add(measureList[j]);
                }
                scoreList.Add(paragraphList);
            }

            return scoreList;
        }

        public List<List<List<Symbol>>> Generate(List<Symbol> symbolList)
        {
            int barDuration = _beats * 256 / _beatType;
            int tempBarDuration = 0;
            int barNum = 4;
            int tempBarNum = 0;
            List<Symbol> barList = new List<Symbol>();
            List<List<Symbol>> paragraphList = new List<List<Symbol>>();
            List<List<List<Symbol>>> scoreList = new List<List<List<Symbol>>>();

            int size = symbolList.Count;
            for (int i = 0; i < size; i++)
            {
                barList.Add(symbolList[i]);
                tempBarDuration += symbolList[i].GetDuration();

                if (tempBarDuration == barDuration)
                {
                    paragraphList.Add(barList);
                    barList = new List<Symbol>();
                    tempBarNum++;
                    tempBarDuration = 0;
                }

                if (tempBarNum == barNum || i == size - 1)
                {
                    scoreList.Add(paragraphList);
                    paragraphList = new List<List<Symbol>>();
                    tempBarNum = 0;
                }
            }

            return scoreList;
        }

        public List<List<List<List<List<Symbol>>>>> Generate(List<Symbol> highSymbolList, List<Symbol> lowSymbolList)
        {
            List<Symbol> highList = new List<Symbol>();
            List<Symbol> lowList = new List<Symbol>();
            List<List<Symbol>> setList = new List<List<Symbol>>(); 
            List<List<List<Symbol>>> measureList = new List<List<List<Symbol>>>(); 
            List<List<List<List<Symbol>>>> paragraphList = new List<List<List<List<Symbol>>>>();
            List<List<List<List<List<Symbol>>>>> scoreList = new List<List<List<List<List<Symbol>>>>>(); 
            int barDuration = _beats * 256 / _beatType;
            int tempBarDuration = 0;
            int measureNum = 4;
            int tempBarNum = 0;
            int tempHighDuration = 0;
            int tempLowDuration = 0;
            int i = 0;
            int j = 0;
            while (i < highSymbolList.Count && j < lowSymbolList.Count)
            {
                highList.Add(highSymbolList[i]);
                lowList.Add(lowSymbolList[j]);
                tempHighDuration += highSymbolList[i].GetDuration();
                tempLowDuration += lowSymbolList[j].GetDuration();
                i++;
                j++;
                while (tempHighDuration != tempLowDuration)
                {
                    if (tempHighDuration > tempLowDuration)
                    {
                        lowList.Add(lowSymbolList[j]);
                        tempLowDuration += lowSymbolList[j].GetDuration();
                        j++;
                    }
                    else if (tempHighDuration < tempLowDuration)
                    {
                        highList.Add(highSymbolList[i]);
                        tempHighDuration += highSymbolList[i].GetDuration();
                        i++;
                    }
                }

                setList.Add(highList);
                setList.Add(lowList);
                measureList.Add(setList);

                tempBarDuration += tempHighDuration;
                tempHighDuration = 0;
                tempLowDuration = 0;
                highList = new List<Symbol>();
                lowList = new List<Symbol>();
                setList = new List<List<Symbol>>();

                if (tempBarDuration == barDuration)
                {
                    paragraphList.Add(measureList);

                    tempBarNum++;
                    tempBarDuration = 0;
                    measureList = new List<List<List<Symbol>>>();
                }

                if (tempBarNum == measureNum || (i == highSymbolList.Count && j == lowSymbolList.Count))
                {
                    scoreList.Add(paragraphList);

                    tempBarNum = 0;
                    paragraphList = new List<List<List<List<Symbol>>>>();
                }
            }

            return scoreList;
        }
    }
}