using System;
using System.Collections.Generic;
using System.Xml;
using symbol;
using util;
using UnityEngine;

namespace xmlParser
{
    public class XmlParser
    {
        private static string[] PITCH_NAME = {"C", "D", "E", "F", "G", "A", "B"};
        private int _highTime = 0; //  Sheet music playing time
        private int _lowTime = 0; //  Sheet music playing time
        private bool _isChord = false; //  Whether it is a chord
        private string _filename; //  musicXML document content
        private string _workTitle = ""; //  Title of work
        private string _creator = ""; 
        //256 default?
        private string _divisions = ""; 
        private string _fifths = ""; 
        private string _beats = ""; 
        private string _beatType = ""; 
        private string _clef = "";
        private string _sign = "";
        private string _line = "";
        //The note (E.g. a C, or D)
        private string _step = ""; 
        //The octave the note is on
        private string _octave = ""; 
        //256 equals a quarter
        private string _duration = "";
        //quarter, half, whole
        private string _type = "";
        //Sharp, flats, etc
        private string _accidental = "";
        private string _staff = ""; 
        private string _stem = ""; 
        private string _beam = ""; 
        private string _beamNum = ""; 
        private string _bpm = "";
        private Symbol _symbol;
        private Beat _beat;
        private Head _highHead;
        private Head _lowHead;
        private Slur _slur;
        private string _highStandardStep = "F";
        private string _highStandardOctave = "4";
        private string _lowStandardStep = "F";
        private string _lowStandardOctave = "4";
        private List<Symbol> _highSymbolList = new List<Symbol>(); 
        private List<Symbol> _lowSymbolList = new List<Symbol>(); 

        private List<Symbol> _highSymbolMeasure = new List<Symbol>();
        private List<Symbol> _lowSymbolMeasure = new List<Symbol>(); 
        private List<List<List<Symbol>>> _measureSymbolList = new List<List<List<Symbol>>>(); 
        private List<Measure> _measureList = new List<Measure>(); 
        private Beat _measureBeat; 
        private Head _measureHighHead;
        private Head _measureLowHead; 

        public XmlParser(string filename)
        {
            _filename = filename;
            Init();
        }

        private void Init()
        {
            try
            {
                _highSymbolList = new List<Symbol>();
                _lowSymbolList = new List<Symbol>();

                XmlReaderSettings readerSettings = new XmlReaderSettings();
                readerSettings.ProhibitDtd = false;
//                readerSettings.DtdProcessing = DtdProcessing.Ignore;
                XmlReader xmlReader = XmlReader.Create(_filename, readerSettings);
//                XmlReader xmlReader = XmlReader.Create(_filename);

                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        switch (xmlReader.Name)
                        {
                            case "work-title": _workTitle = xmlReader.ReadString(); break;
                            case "creator": _creator = xmlReader.ReadString(); break;
                            case "divisions": _divisions = xmlReader.ReadString(); break; //Debug.Log("Divisions " + _divisions); break;
                            case "fifths": _fifths = xmlReader.ReadString(); break;
                            case "beats": _beats = xmlReader.ReadString(); break;
                            case "beat-type": _beatType = xmlReader.ReadString(); break;
                            case "clef": _clef = xmlReader.GetAttribute("number"); break;
                            case "sign": _sign = xmlReader.ReadString(); break;
                            case "line": _line = xmlReader.ReadString(); break;
                            case "step": _step = xmlReader.ReadString(); break; //Debug.Log("Step " + _step); break;
                            case "octave": _octave = xmlReader.ReadString(); break; //Debug.Log("Octave " + _octave); break;
                            case "duration": _duration = xmlReader.ReadString(); break; //Debug.Log("Duration " + _duration);break;
                            case "type": _type = xmlReader.ReadString(); break; //Debug.Log("Type " + _type);break;
                            case "accidental": _accidental = xmlReader.ReadString(); break; //Debug.Log("Accidental " + _accidental); break;
                            case "staff": _staff = xmlReader.ReadString(); break;
                            case "stem": _stem = xmlReader.ReadString(); break;
                            case "per-minute":_bpm = xmlReader.ReadString(); break;
                            case "beam":
                                _beamNum = xmlReader.GetAttribute("number");
                                if (_beamNum.Equals("1"))
                                {
                                    _beam = xmlReader.ReadString();
                                }
                                break;
                            case "rest": // 休止符，由于休止符是self closing <rest /> 的，所以放在这里
                                _symbol = new Rest();
                                _symbol.SetChord(false);
                                break;
                            case "chord": // 和弦，由于和弦也是self closing <chord /> 的，所以也放在这里
                                _isChord = true; break;
                            
                        }
                    }
                    //EndElement is returned when xml reader gets to the end of an element, therefore, when it is just
                    //Element the xmlreader continues reading
                    else if (xmlReader.NodeType == XmlNodeType.EndElement)
                    {
                        if (xmlReader.Name.Equals("time"))
                        {
                            // 节拍
                            _beat = new Beat(_beats, _beatType);
                            _measureBeat = new Beat(_beats, _beatType);
                        }

                        if (xmlReader.Name.Equals("clef"))
                        {
                            // 谱号
                            if (_clef.Equals("1"))
                            {
                                _highHead = new Head(_fifths, _sign, _line);
                                _measureHighHead = new Head(_fifths, _sign, _line);
                                SetHighStandard();
                            }
                            else if (_clef.Equals("2"))
                            {
                                _lowHead = new Head(_fifths, _sign, _line);
                                _measureLowHead = new Head(_fifths, _sign, _line);
                                SetLowStandard();
                            }
                        }

                        if (xmlReader.Name.Equals("pitch"))
                        {
                            //  音高
                            _symbol = new Note(_step, _octave);
                            _symbol.SetChord(_isChord);
                            _isChord = false;
                        }

                        if (xmlReader.Name.Equals("dot"))
                        {
                            _symbol.SetDot(1); // 附点
                        }

                        if (xmlReader.Name.Equals("note"))
                        {
                            //  音符，包括音符及休止符
                            //Debug.Log("If divisions " + _divisions);
                            _symbol.SetDuration(_divisions, _duration);
                            _symbol.SetType(_type);

                            bool isNote = _symbol is Note;
                            if (isNote)
                            {
                                //Debug.Log("If acccidental " + _accidental);
                                ((Note) _symbol).SetAccidental(_accidental);
                                _accidental = "";
                                if (_stem.Equals("up")) ((Note) _symbol).SetUpOrDown(true);
                                else if (_stem.Equals("down")) ((Note) _symbol).SetUpOrDown(false);
                            }

                            if (_staff.Equals("1"))
                            {
                                if (isNote)
                                {
                                    SetShift((Note) _symbol, _highStandardStep, _highStandardOctave);
                                    SetBeam(_highSymbolList);
                                    SetBeam(_highSymbolMeasure);
                                }
                                if (_symbol.IsChord())
                                {
                                    SetChord(_highSymbolList);
                                    SetChord(_highSymbolMeasure);
                                }
                                else
                                {
                                    _symbol.SetStartTime(_highTime);
                                    _symbol.SetStopTime((_highTime += _symbol.GetDuration()));
                                    _highSymbolList.Add(_symbol);
                                    _highSymbolMeasure.Add(_symbol);
                                }
                            }
                            else
                            {
                                if (isNote)
                                {
                                    SetShift((Note) _symbol, _lowStandardStep, _lowStandardOctave);
                                    SetBeam(_lowSymbolList);
                                    SetBeam(_lowSymbolMeasure);
                                }
                                if (_symbol.IsChord())
                                {
                                    SetChord(_lowSymbolList);
                                    SetChord(_lowSymbolMeasure);
                                }
                                else
                                {
                                    _symbol.SetStartTime(_lowTime);
                                    _symbol.SetStopTime((_lowTime += _symbol.GetDuration()));
                                    _lowSymbolList.Add(_symbol);
                                    _lowSymbolMeasure.Add(_symbol);
                                }
                            }
                        }
                        if (xmlReader.Name.Equals("measure")) // 小节结束
                        {
                            // 整合这一个小节中的乐符
                            int maxCount = ArrangeMeasure();

                            // 存入小节
                            Measure measure = new Measure(_measureSymbolList);
                            measure.SetMaxCount(maxCount);
                            if (_measureBeat == null)
                            {
                                measure.SetHasBeat(false);
                            }
                            else
                            {
                                measure.SetHasBeat(true);
                                measure.SetBeat(GetBeat());
                            }
                            if (_measureHighHead == null || _measureLowHead == null)
                            {
                                measure.SetHasHead(false);
                            }
                            else
                            {
                                measure.SetHasHead(true);
                                measure.SetHead(GetHighHead(), GetLowHead());
                            }

                            _measureList.Add(measure);

                            // 将相应List清零，下一个小节再重新赋值
                            //Clear measure list and get ready for next section
                            _highSymbolMeasure = new List<Symbol>();
                            _lowSymbolMeasure = new List<Symbol>();
                            _measureSymbolList = new List<List<List<Symbol>>>();

                            _measureBeat = null;
                            _measureHighHead = null;
                            _measureLowHead = null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public string GetWorkTitle() { return _workTitle; }

        public string GetCreator() { return _creator; }

        public string GetBPM() { return _bpm; }

        public Beat GetBeat() { return _beat; }

        public Head GetHighHead()  { return _highHead; }

        public Head GetLowHead() { return _lowHead; }

        public List<Symbol> GetHighSymbolList() { return _highSymbolList; }

        public List<Symbol> GetLowSymbolList() { return _lowSymbolList; }

        public List<List<List<Symbol>>> GetMeasureSymbolList() { return _measureSymbolList; }

        public List<Measure> GetMeasureList() { return _measureList; }

        private int GetStandard() {
            int standard = 0;
            switch (_sign)
            {
                case "G": standard = 4; break;
                case "F": standard = 3; break;
            }
            return standard;
        }

        private void SetHighStandard()
        {
            int standard = GetStandard();
            int temp = 2 * (3 - int.Parse(_line));
            _highStandardStep = PITCH_NAME[(temp + standard) % 7];
            _highStandardOctave = standard.ToString();
        }

        private void SetLowStandard()
        {
            int standard = GetStandard();
            int temp = 2 * (3 - int.Parse(_line));
            _lowStandardStep = PITCH_NAME[(temp + standard) % 7];
            _lowStandardOctave = standard.ToString();
        }

        private void SetShift(Note note, string standardStep, string standardOctave)
        {
            ParamsGetter paramsGetter = ParamsGetter.GetInstance();
            int shift = -(GetDigitizedPitch(standardStep, standardOctave) - GetDigitizedPitch(_step, _octave)) *
                        paramsGetter.GetPitchPositionDiff();
            note.SetShift(shift);
        }

        private int GetDigitizedPitch(string step, string octave)
        {
            int digitizedPitch = 1;

            switch (step)
            {
                case "C": digitizedPitch = 1; break;
                case "D": digitizedPitch = 2; break;
                case "E": digitizedPitch = 3; break;
                case "F": digitizedPitch = 4; break;
                case "G": digitizedPitch = 5; break;
                case "A": digitizedPitch = 6; break;
                case "B": digitizedPitch = 7; break;
            }

            return digitizedPitch + (int.Parse(octave) - 1) * 7;
        }

        private void SetChord(List<Symbol> symbolList)
        {
            Note lastNote = (Note) symbolList[symbolList.Count - 1];
            lastNote.SetHasChord(true);
            lastNote.GetChordList().Add((Note) _symbol);
        }

        private void SetBeam(List<Symbol> symbolList)
        {
            switch (_beam)
            {
                case "begin":
                {
                    _slur = new Slur();
                    _slur.GetList().Add((Note) _symbol);
                    ((Note) _symbol).SetSlur(true);
                    ((Note) _symbol).SetNext(true);
                }
                    break;
                case "continue":
                {
                    _slur.GetList().Add((Note) _symbol);
                    ((Note) _symbol).SetLastNote((Note) symbolList[symbolList.Count - 1]);
                    ((Note) _symbol).SetSlur(true);
//                    ((Note) _symbol).SetLast(true);
                    ((Note) _symbol).SetNext(true);
                }
                    break;
                case "end":
                {
                    Note lastNote = (Note) symbolList[symbolList.Count - 1];
                    _slur.GetList().Add((Note) _symbol);
                    ((Note) _symbol).SetLastNote(lastNote);
                    ((Note) _symbol).SetSlur(true);
                    ((Note) _symbol).SetLast(true);

                    if (_slur != null)
                    {
                        _slur.Operate();
                    }
                }
                    break;
                default: break;
            }

            ((Note) _symbol).SetBeam(_beam);
            _beam = "";
        }

        // 将小节中按照音符时长分组，返回整个小节的总duration
        private int ArrangeMeasure()
        {
            int i = 0;
            int j = 0;
            int tempHighDuration = 0;
            int tempLowDuration = 0;
            while (i < _highSymbolMeasure.Count && j < _lowSymbolMeasure.Count)
            {
                List<List<Symbol>> setList = new List<List<Symbol>>();
                List<Symbol> highList = new List<Symbol>();
                List<Symbol> lowList = new List<Symbol>();

                highList.Add(_highSymbolMeasure[i]);
                tempHighDuration += _highSymbolMeasure[i].GetDuration();
                i++;
                lowList.Add(_lowSymbolMeasure[j]);
                tempLowDuration += _lowSymbolMeasure[j].GetDuration();
                j++;

                while (tempHighDuration != tempLowDuration)
                {
                    if (tempHighDuration > tempLowDuration)
                    {
                        lowList.Add(_lowSymbolMeasure[j]);
                        tempLowDuration += _lowSymbolMeasure[j].GetDuration();
                        j++;
                    }
                    else if (tempHighDuration < tempLowDuration)
                    {
                        highList.Add(_highSymbolMeasure[i]);
                        tempHighDuration += _highSymbolMeasure[i].GetDuration();
                        i++;
                    }
                }
                setList.Add(highList);
                setList.Add(lowList);
                _measureSymbolList.Add(setList);
            }
            return i >= j ? i : j;
        }
    }
}