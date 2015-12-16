using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CoolEditor
{

    /// <summary>
    /// Object returned by intellisense processing.
    /// List of these will be displayed at cursor position.
    /// </summary>
    public class IntellisenseItem : Object
    {
        #region Members
        private Color forecolor = Color.FromKnownColor(KnownColor.Transparent);
        private Image image = null;
        private List<IMethodSymbol> methodInfo = new List<IMethodSymbol>();
        private string tag = null;
        private string text = "";
        #endregion

        #region Constructors
        public IntellisenseItem() { }
        #endregion

        #region Properties
        public Color ForeColor
        { get; set; }

        public Image Image
        { get; set; }

        public List<IMethodSymbol> MethodInfoList
        {
            get
            {
                return methodInfo;
            }
            set
            {
                methodInfo = value;
            }
        }
        public string Tag
        { get; set; }
        public string Text
        { get;set;}

        #endregion

    }
}
