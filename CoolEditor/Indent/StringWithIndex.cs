using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolEditor
{
    public class CodeWithPosition
    {
        private string code;
        private int position;

        public string Code
        {
            get { return code; }
            set { code = value; }
        }

        // The position to set the cursor, after the text is indented
        public int Position
        {
            get { return position; }
            set { position = value; }
        }

        public CodeWithPosition() { }

        public CodeWithPosition(string _code, int _position)
        {
            code = _code;
            position = _position;
        }
    }
}
