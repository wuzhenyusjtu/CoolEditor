using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Text;

namespace CoolEditor
{
    // The postition of the text
    public class CodeSpan
    {
        private int start;
        private int length;

        public int Start
        {
            get { return start; }
            set { start = value; }
        }

        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        public CodeSpan() { }

        public CodeSpan(int _start, int _length)
        {
            start = _start;
            length = _length;
        }
    }

    // All the information needed to color a substring in the text
    public class SpanWithType
    {
        private CodeSpan span;
        private CodeType type;
        
        // Text indices
        public CodeSpan Span
        {
            get { return span; }
            set { span = value; }
        }

        //Type of the text indices
        public CodeType Type
        {
            get { return type; }
            set { type = value; }
        }

        public SpanWithType() { }

        public SpanWithType(CodeSpan _span, CodeType _type)
        {
            span = _span;
            type = _type;
        }
    }
}
