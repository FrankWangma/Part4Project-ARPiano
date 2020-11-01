using symbol;
using UnityEngine;

namespace control
{
    public class SymbolControl
    {
        private Symbol _symbol;
        public SymbolControl(Symbol symbol)
        {
            _symbol = symbol;
        }

        public void SetColor(Color color)
        {
            _symbol.ChangeColor(color);
        }

        public void Rebound()
        {

        }
    }
}