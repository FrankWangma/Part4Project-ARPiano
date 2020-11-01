using System;
using UnityEngine;
using UnityEngine.UI;

namespace symbol
{
    public class NoteView : SymbolView
    {
        private Note _note;
        private int _stemX;
        private int _stemY;
        private int _beamX;
        private int _beamY;
        private int _tailX;
        private int _tailY;

        public NoteView(Symbol symbol, int width, int start, GameObject parentObject) : base(symbol, width, start, parentObject)
        {
            _note = (Note)symbol;
            Init();
        }

        private void Init()
        {
            OnDraw();
            SetColor(_note.GetColor());
        }

        protected override void OnDraw()
        {
            //This if statement isn't entered
            if (Cursor)
            {
                int tempStart = Start - 2 + Num * _note.GetSymbolWidth();
                DrawLine(tempStart, 0, tempStart, ParamsGetter.GetTotalHeight());
            }

            int yPosition =  ParamsGetter.GetStaffCenterPosition() + _note.GetShift();

            int shift = _note.GetShift() / ParamsGetter.GetPitchPositionDiff();
            DrawShiftLine(shift);
            DrawAccidental(_note.GetAccidental(), yPosition);

            if (_note.GetDot() == 1)
            {
                DrawPoint(Start + ParamsGetter.GetDotePosition(), yPosition);
            }

            if (Type == 1) // Whole Note
            {
                DrawSymbol("\uE12B", Start, yPosition);
                SetColor(_note.GetColor());
                if (_note.HasChord()) DrawChord();
            }
            else
            {
                if (Type == 2) // Half Note
                {
                    DrawSymbol("\uE12C", Start, yPosition);
                    SetColor(_note.GetColor());
                }
                else
                {
                    DrawSymbol("\uE12D", Start, yPosition);
                    SetColor(_note.GetColor());
                }

                if (_note.IsUpOrDown())
                {
                    _stemX = Start + ParamsGetter.GetNoteRightShift();
                    _stemY = yPosition; 
                    _tailX = Start + ParamsGetter.GetNoteTailUpLandscapeShift();
                    _tailY = _stemY + ParamsGetter.GetNoteStemHeight() - ParamsGetter.GetNoteTailDownPortraitShift();
                }
                else
                {
                    _stemX = Start - ParamsGetter.GetNoteLeftShift();
                    _stemY = yPosition - ParamsGetter.GetNoteStemDownShift();
                    _tailX = Start - ParamsGetter.GetNoteTailDownLandscapeShift();
                    _tailY = _stemY - ParamsGetter.GetNoteStemHeight() + ParamsGetter.GetNoteTailDownPortraitShift();
                }

                _beamX = _stemX;
                _beamY = _note.GetEnd();

                if (_note.HasChord())
                {
                    if (_note.IsUpOrDown())
                    {
                        int temp = _note.GetShift();
                        foreach (Note noteChord in _note.GetChordList())
                        {
                            if (temp > noteChord.GetShift())
                            {
                                temp = noteChord.GetShift();
                            }
                        }
                        _tailY = temp + ParamsGetter.GetNoteStemHeight();
                    }
                    DrawChord();
                }
                DrawStem();
                if (_note.IsSlur())
                {
                    DrawBeam();
                }
                else
                {
                    DrawTail();
                }
            }
        }

        private void DrawShiftLine(int shift)
        {
            if (shift > 4)
            {
                int num = (shift - 4) / 2 + 1;
                for (int i = 1; i < num; i++)
                {
                    int y = ParamsGetter.GetStaffCenterPosition() + (4 + 2 * i) * ParamsGetter.GetPitchPositionDiff();
                    DrawLine(Start - 4 - ParamsGetter.GetNoteLeftShift(), y, Start + 4 + ParamsGetter.GetNoteRightShift(), y);
                }
            }
            else if (shift < -4)
            {
                int num = (-4 - shift) / 2 + 1;
                for (int i = 1; i < num; i ++)
                {
                    int y = ParamsGetter.GetStaffCenterPosition() - (4 + 2 * i) * ParamsGetter.GetPitchPositionDiff();
                    DrawLine(Start - 4 - ParamsGetter.GetNoteLeftShift(), y, Start + 4 + ParamsGetter.GetNoteRightShift(), y);
                }
            }
        }

        private void DrawAccidental(string accidental, int position)
        {
            switch (accidental)
            {
                case "flat": DrawSymbol("\uE114", 0, position); break;
                case "sharp": DrawSymbol("\uE10E", 0, position); break;
                case "natural": DrawSymbol("\uE113", 0, position); break;
                default: break;
            }
        }

        private void DrawStem()
        {
            if (_note.IsSlur())
            {
                DrawLine(_stemX, _stemY, _stemX, _beamY);
            }
            else {
                if (_note.IsUpOrDown()) {
                    DrawLine(_stemX, _stemY, _stemX, _stemY + ParamsGetter.GetNoteStemHeight());
                } else {
                    DrawLine(_stemX, _stemY, _stemX, _stemY - ParamsGetter.GetNoteStemHeight());
                }
            }
        }

        private void DrawBeam()
        {
            int beamDiff = -ParamsGetter.GetNoteBeamDiff(_note.IsUpOrDown());
            if (_note.IsLast())
            {
                switch (_note.GetLastNote().GetType())
                {
                    case 8:
                        DrawLine(_beamX, _beamY, 0, _beamY, 2);
                        break;
                    case 16:
                        DrawLine(_beamX, _beamY, 0, _beamY, 2);
                        DrawLine(_beamX, _beamY + beamDiff, 0, _beamY + beamDiff, 2);
                        break;
                    case 64:
                        DrawLine(_beamX, _beamY, 0, _beamY, 2);
                        DrawLine(_beamX, _beamY + beamDiff, 0, _beamY + beamDiff, 2);
                        DrawLine(_beamX, _beamY + 2 * beamDiff, 0, _beamY + 2 * beamDiff, 2);
                        break;
                    default: break;
                }

                if (_note.GetLastNote().GetDot() != 0)
                {
                    switch ((int) (_note.GetLastNote().GetType() * Math.Pow(2, _note.GetLastNote().GetDot())))
                    {
                        case 16:
                            DrawLine(_beamX, _beamY + beamDiff, 0, _beamY + beamDiff, 2);
                            break;
                        case 32:
                            DrawLine(_beamX, _beamY + beamDiff, 0, _beamY + beamDiff, 2);
                            DrawLine(_beamX, _beamY + 2 * beamDiff, 0, _beamY + 2 * beamDiff, 2);
                            break;
                        default: break;
                    }
                }
            }

            if (_note.IsNext())
            {
                switch (_note.GetType())
                {
                    case 8:
                        DrawLine(_beamX - 1, _beamY, Width, _beamY, 2);
                        break;
                    case 16:
                        DrawLine(_beamX - 1, _beamY, Width, _beamY, 2);
                        DrawLine(_beamX - 1, _beamY + beamDiff, Width, _beamY + beamDiff, 2);
                        break;
                    case 32:
                        DrawLine(_beamX - 1, _beamY, Width, _beamY, 2);
                        DrawLine(_beamX - 1, _beamY + beamDiff, Width, _beamY + beamDiff, 2);
                        DrawLine(_beamX - 1, _beamY + 2 * beamDiff, Width, _beamY + 2 * beamDiff, 2);
                        break;
                    default: break;
                }

                if (_note.GetDot() != 0) {
                    int temp = (int) Math.Pow(2, _note.GetDot());
                    int start = (2 * temp - 1) * Width / (2 * temp);
                    switch (_note.GetType() * temp) {
                        case 16:
                            DrawLine(start, _beamY + beamDiff, Width, _beamY + beamDiff, 2);
                            break;
                        case 32:
                            DrawLine(start, _beamY + beamDiff, Width, _beamY + beamDiff, 2);
                            DrawLine(start, _beamY + 2 * beamDiff, Width, _beamY + 2 * beamDiff, 2);
                            break;
                        default: break;
                    }
                }
            }
        }

        private void DrawTail()
        {
            if (_note.IsUpOrDown())
            {
                switch (Type)
                {
                    case 8: DrawSymbol("\uE190", _tailX, _tailY); break;
                    case 16: DrawSymbol("\uE191", _tailX, _tailY); break;
                    case 32: DrawSymbol("\uE192", _tailX, _tailY); break;
                    case 64: DrawSymbol("\uE193", _tailX, _tailY); break;
                    default: break;
                }
            }
            else
            {
                switch (Type)
                {
                    case 8: DrawSymbol("\uE194", _tailX, _tailY); break;
                    case 16: DrawSymbol("\uE197", _tailX, _tailY); break;
                    case 32: DrawSymbol("\uE198", _tailX, _tailY); break;
                    case 64: DrawSymbol("\uE199", _tailX, _tailY); break;
                    default: break;
                }
            }
        }

        private void DrawChord()
        {
            bool leftOrRight = _note.IsUpOrDown();
            int headPosition = Start;
            int lastPosition = 0;

            for (int i = 0; i < _note.GetChordList().Count - 1; i++)
            {
                Note extraNote = _note.GetChordList()[i];
                DrawShiftLine(extraNote.GetShift() / ParamsGetter.GetPitchPositionDiff());
                int extraDuration = extraNote.GetType();
                int extraPosition = extraNote.GetShift() + ParamsGetter.GetStaffCenterPosition();
                if (Math.Abs(extraPosition - lastPosition) == ParamsGetter.GetPitchPositionDiff())
                {
                    if (leftOrRight)
                        headPosition += ParamsGetter.GetNoteHeadWidth();
                    else
                        headPosition -= ParamsGetter.GetNoteHeadWidth();
                    leftOrRight = !leftOrRight;
                }

                if (extraDuration == 1)
                {
                    DrawSymbol("\uE12B", headPosition, extraPosition);
                    continue;
                }
                else
                {
                    if (extraDuration == 2){
                        DrawSymbol("\uE12C", headPosition, extraPosition);
                        SetColor(Color.cyan);
                    }
                    else
                        DrawSymbol("\uE12D", headPosition, extraPosition);
                }

                lastPosition = extraPosition;
                int temp;
                if (_note.IsUpOrDown())
                    temp = extraPosition + ParamsGetter.GetNoteStemHeight();
                else
                    temp = extraPosition - ParamsGetter.GetNoteStemHeight();
                DrawLine(_stemX, extraPosition, _stemX, temp);
                SetColor(extraNote.GetColor());
            }
        }
    }
}