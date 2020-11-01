using System;
using System.Collections.Generic;
using util;
using UnityEngine;
using UnityEngine.UI;

namespace symbol
{
    public abstract class SymbolView
    {
        protected Symbol symbol;
        protected int Width;
        protected int Start;
        protected int Type;
        protected Color color;
        protected int Num = 0;
        protected int ChordPlay = -1;
        protected bool Cursor = false;
        protected bool RightPlay;
        protected List<int> Chord = new List<int>();
        protected ParamsGetter ParamsGetter = ParamsGetter.GetInstance();
        protected CommonParams CommonParams = CommonParams.GetInstance();

        protected GameObject ParentObject;

        public SymbolView() { }

        public SymbolView(Symbol symbol, int width, int start, GameObject parentObject)
        {
            this.symbol = symbol;
            Width = width;
            Start = start;
            ParentObject = parentObject;
            Init();
        }

        private void Init()
        {
            Type = symbol.GetType();
        }

        public void SetColor(Color color)
        {
            if (ParentObject != null)
            {
                foreach (Transform childTransform in ParentObject.transform)
                {
                    CanvasRenderer canvasRenderer = childTransform.gameObject.GetComponent<CanvasRenderer>();
                    Image image = canvasRenderer.GetComponent<Image>();
                    if (image != null)
                    {
                        image.color = color;
                        continue;
                    }
                    Text text = canvasRenderer.GetComponent<Text>();
                    if (text != null)
                    {
                        text.color = color;
                        continue;
                    }
                }
            }
        }

        public void SetCursor(int num, bool cursor) { Num = num; Cursor = cursor; }

        public void SetChord(List<int> chord) { Chord = chord; }

        public bool IsRightPlay() { return RightPlay; }

        public void SetRightPlay(bool rightPlay) { RightPlay = rightPlay; }


        protected void DrawSymbol(string text, float x, float y)
        {
            GameObject symbolObject = GameObject.Instantiate(CommonParams.GetPrefabSymbol(),
                ParentObject.transform.position,
                CommonParams.GetPrefabSymbol().transform.rotation);
            symbolObject.transform.SetParent(ParentObject.transform);
            Text noteText = symbolObject.GetComponent<Text>();
            noteText.transform.localPosition = new Vector3(x, y, 0);
            noteText.text = text;
        }

        protected void DrawLine(float startX, float startY, float stopX, float stopY)
        {
            GameObject lineObject = GameObject.Instantiate(CommonParams.GetPrefabLine(),
                ParentObject.transform.position,
                CommonParams.GetPrefabLine().transform.rotation);
            lineObject.transform.SetParent(ParentObject.transform);
            RectTransform lineRect = lineObject.GetComponent<RectTransform>();
            float width = Math.Max(startX, stopX) - Math.Min(startX, stopX) + 1;
            float heigh = Math.Max(startY, stopY) - Math.Min(startY, stopY) + 1;
            lineRect.sizeDelta = new Vector2(width, heigh);
            lineRect.localPosition = new Vector3(Math.Min(startX, stopX), Math.Min(startY, stopY), 0);
        }

        protected void DrawLine(float startX, float startY, float stopX, float stopY, int strokeWidth)
        {
            GameObject lineObject = GameObject.Instantiate(CommonParams.GetPrefabLine(),
                ParentObject.transform.position,
                CommonParams.GetPrefabLine().transform.rotation);
            lineObject.transform.SetParent(ParentObject.transform);
            RectTransform lineRect = lineObject.GetComponent<RectTransform>();
            float width = Math.Max(startX, stopX) - Math.Min(startX, stopX) + strokeWidth;
            float heigh = Math.Max(startY, stopY) - Math.Min(startY, stopY) + strokeWidth;
            lineRect.sizeDelta = new Vector2(width, heigh);
            lineRect.localPosition = new Vector3(Math.Min(startX, stopX), Math.Min(startY, stopY), 0);
        }

        protected void DrawPoint(float x, float y)
        {

        }

        protected abstract void OnDraw();
    }
}
