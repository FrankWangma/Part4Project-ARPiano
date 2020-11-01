using UnityEngine;

namespace util
{
    public class ParamsGetter
    {
        private static ParamsGetter instance = new ParamsGetter();
        private int _unit = 8;
        private int? measureLength;

        public static ParamsGetter GetInstance() {
            return instance;
        }

        public int GetUnit() { return _unit; }

        public void SetUnit(int unit) { _unit = unit; }

        public int GetTotalHeight() { return 10 * _unit; }

        public int GetStaffHeight() { return 4 * _unit; }

        public int GetStaffPosition() { return 11 * _unit / 2; }

        public int GetStaffCenterPosition() { return GetTotalHeight() / 2; }

        public int GetHeadWidth() { return 6 * _unit; }

        public int GetBeatWidth() { return 2 * _unit; }

        public int GetSymbolSize() { return 4 * _unit; }

        public int GetPitchPositionDiff() { return _unit / 2; }

        public int GetSymbolStart() { return _unit; }

        public int GetNoteHeadWidth() { return _unit; }

        public int GetNoteStemHeight() { return 7 * _unit / 2; }

        public int GetNoteStemDownShift() { return _unit / 8; }

        public int GetNoteTailUpPortraitShift() { return 3 * _unit / 2; }

        public int GetNoteTailDownPortraitShift() { return 3 * _unit / 2; }

        public int GetNoteTailUpLandscapeShift() { return _unit / 8; }

        public int GetNoteTailDownLandscapeShift() { return _unit / 8; }

        public int GetNoteRightShift() { return _unit / 2; }

        public int GetNoteLeftShift() { return 3 * _unit / 4 ; }

        public int GetNoteBeamDiff(bool upOrDown) {
            if (upOrDown)
            {
                return 3 * _unit / 4;
            }
            else
            {
                return -3 * _unit / 4;
            }
        }

        public int GetDotePosition() { return GetNoteHeadWidth() + _unit / 2; }

        public float GetFirstFifthsPosition() { return (float) (3.6 * _unit); }

        public float GetSecondFifthsPosition() { return (float) (4.6 * _unit); }

        public int GetClefPortraitShift() { return 3 * _unit / 2; }

        public int GetBeatPortraitShift() { return 5 * _unit; }

        public void SetParagraphLength(int length) {
            if(!measureLength.HasValue) {
                measureLength = length;
            }
        }

        public int GetParagraphLength() {
            return measureLength.Value;
        }

        public Font GetSymbolFont()
        {
            Font scoreFont = (Font) Resources.Load("Fonts/mscore-20");
            return scoreFont;
        }
    }
}
