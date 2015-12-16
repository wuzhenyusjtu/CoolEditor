using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Text;

namespace CoolEditor
{
    public class Range
    {
        public ClassifiedSpan ClassifiedSpan { get; private set; }
        public string Text { get; private set; }

        #region Constructor
        /// <summary>
        /// A constructor of the Range Class
        /// </summary>
        /// <param name="classification"> the type of the classifiedSpan</param>
        /// <param name="span"> the position of the text, by showing the beginning position & count of characters</param>
        /// <param name="text"> contains all the information of the original text</param>
        public Range(string classification, TextSpan span, SourceText text) :
            this(classification, span, text.GetSubText(span).ToString())
        {
        }

        public Range(string classification, TextSpan span, string text) :
            this(new ClassifiedSpan(classification, span), text)
        {
        }

        public Range(ClassifiedSpan classifiedSpan, string text)
        {
            this.ClassifiedSpan = classifiedSpan; 
            this.Text = text;
        }
        #endregion

        public string ClassificationType
        {
            get { return ClassifiedSpan.ClassificationType; }
        }

        public TextSpan TextSpan
        {
            get { return ClassifiedSpan.TextSpan; }
        }
    }
}