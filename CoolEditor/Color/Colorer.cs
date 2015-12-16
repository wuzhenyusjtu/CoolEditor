using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.Classification;

namespace CoolEditor
{
    public enum CodeType
    {
        keywordType = 0,
        punctuationType = 1,
        operatorType = 2,
        numericType = 3,
        stringType = 4,
        variableType = 5,
        commentType = 6,
        classType = 7,
        defaultType = 8
    }

    public class Colorer
    {
        public Colorer() { }

        #region Public Methods

        /// <summary>
        /// Color all the text selected, return string in rtf format
        /// </summary>
        /// <param name="plainCode">
        /// Plain text of program, no rtf contained
        /// </param>
        /// <returns>
        /// Program text in rtf format, including coloring
        /// </returns>
        public string ExplicitColoring(string plainCode)
        {
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();
            SyntaxTree tree = VisualBasicSyntaxTree.ParseText(plainCode);
            SyntaxNode root = tree.GetRoot();
            SyntaxTree[] sourceTrees = { tree };

            // Add references
            MetadataReference mscorlib = new MetadataFileReference(typeof(object).Assembly.Location);
            MetadataReference NXOpen = new MetadataFileReference(typeof(NXOpen.Point3d).Assembly.Location);
            MetadataReference NXOpen_UF = new MetadataFileReference(typeof(NXOpen.UF.UF).Assembly.Location);
            MetadataReference NXOpen_Utilities = new MetadataFileReference(typeof(NXOpen.Utilities.BaseSession).Assembly.Location);
            MetadataReference NXOpenUI = new MetadataFileReference(typeof(NXOpenUI.FormUtilities).Assembly.Location);
            MetadataReference Snap = new MetadataFileReference(typeof(Snap.Color).Assembly.Location);
            MetadataReference[] references = { mscorlib, Snap, NXOpen, NXOpen_UF, NXOpen_Utilities, NXOpenUI };

            VisualBasicCompilation compilation = VisualBasicCompilation.Create("Syntax Coloring").AddSyntaxTrees(sourceTrees).AddReferences(references);
            // Get a collection of "ranges" from the program text ("range" = classifiedSpan + text)
            IEnumerable<Range> ranges = ColorDocument(compilation);

            // Cycle through ranges, building RTF output
            int offset = 0;

            #region highlight according to the classifications
            foreach (Range range in ranges)
            {
                if (range.ClassificationType != null)
                {
                    int start = range.TextSpan.Start;
                    start += offset;

                    // Insert different RTF code depending on this range's ClassificationType
                    switch (range.ClassificationType)
                    {
                        case "keyword":
                            plainCode = plainCode.Insert(start, @"\cf4 ");
                            offset += 5;
                            break;
                        case "preprocessor keyword":
                            plainCode = plainCode.Insert(start, @"\cf4 ");
                            offset += 5;
                            break;
                        case "class name":
                            plainCode = plainCode.Insert(start, @"\cf5 ");
                            offset += 5;
                            break;
                        case "struct name":
                            plainCode = plainCode.Insert(start, @"\cf5 ");
                            offset += 5;
                            break;
                        case "interface name":
                            plainCode = plainCode.Insert(start, @"\cf5");
                            offset += 5;
                            break;
                        case "enum name":
                            plainCode = plainCode.Insert(start, @"\cf5 ");
                            offset += 5;
                            break;
                        case "module name":
                            plainCode = plainCode.Insert(start, @"\cf5 ");
                            offset += 5;
                            break;
                        case "delegate name":
                            plainCode = plainCode.Insert(start, @"\cf5 ");
                            offset += 5;
                            break;
                        case "string":
                            plainCode = plainCode.Insert(start, @"\cf6 ");
                            offset += 5;
                            break;
                        case "punctuation":
                            plainCode = plainCode.Insert(start, @"\cf3 ");
                            offset += 5;
                            break;
                        case "identifier":
                            plainCode = plainCode.Insert(start, @"\cf3 ");
                            offset += 5;
                            break;
                        case "excluded code":
                            plainCode = plainCode.Insert(start, @"\cf3 ");
                            offset += 5;
                            break;
                        case "comment":
                            plainCode = plainCode.Insert(start, @"\cf1 ");
                            offset += 5;
                            break;
                        case "xml doc comment - delimiter":
                            plainCode = plainCode.Insert(start, @"\cf1");
                            offset += 5;
                            break;
                        case "xml doc comment - name":
                            plainCode = plainCode.Insert(start, @"\cf1");
                            offset += 5;
                            break;
                        case "xml doc comment - text":
                            plainCode = plainCode.Insert(start, @"\cf1");
                            offset += 5;
                            break;
                        case "xml doc comment - attribute name":
                            plainCode = plainCode.Insert(start, @"\cf1");
                            offset += 5;
                            break;
                        case "xml doc comment - attribute quotes":
                            plainCode = plainCode.Insert(start, @"\cf1");
                            offset += 5;
                            break;
                        case "xml literal - delimiter":
                            plainCode = plainCode.Insert(start, @"\cf3");
                            offset += 5;
                            break;
                        case "xml literal - name":
                            plainCode = plainCode.Insert(start, @"\cf3");
                            offset += 5;
                            break;
                        case "xml literal - text":
                            plainCode = plainCode.Insert(start, @"\cf3");
                            offset += 5;
                            break;
                        case "xml literal - entity reference":
                            plainCode = plainCode.Insert(start, @"\cf3");
                            offset += 5;
                            break;
                        case "operator":
                            plainCode = plainCode.Insert(start, @"\cf3 ");
                            offset += 5;
                            break;
                        case "number":
                            plainCode = plainCode.Insert(start, @"\cf3 ");
                            offset += 5;
                            break;
                        default:
                            plainCode = plainCode.Insert(start, @"\cf3 ");
                            offset += 5;
                            break;
                    }
                }
            }
            #endregion

            // Convert to RTF form (add header and newline stuff)
            string coloredRtf = PlainTextToRtf(plainCode);
            return coloredRtf;
        }


        /// <summary>
        /// Color the last syntax node or trivia, return string in rtf format
        /// </summary>
        /// <param name="plainCode">Text of program, from beginning up to cursor location</param>
        /// <returns>A SpanWithType object</returns>
        /// <remarks>
        /// We assume that the caller will color the returned span of text, using the
        /// returned type info and his choice of coloring scheme.
        /// </remarks>
        public SpanWithType DynamicColoring(string plainCode)
        {
            // Build syntax tree
            SyntaxTree tree = VisualBasicSyntaxTree.ParseText(plainCode);
            SyntaxTree[] sourceTrees = { tree };

            // Add references
            MetadataReference mscorlib = new MetadataFileReference(typeof(object).Assembly.Location);
            MetadataReference NXOpen = new MetadataFileReference(typeof(NXOpen.Point3d).Assembly.Location);
            MetadataReference NXOpen_UF = new MetadataFileReference(typeof(NXOpen.UF.UF).Assembly.Location);
            MetadataReference NXOpen_Utilities = new MetadataFileReference(typeof(NXOpen.Utilities.BaseSession).Assembly.Location);
            MetadataReference NXOpenUI = new MetadataFileReference(typeof(NXOpenUI.FormUtilities).Assembly.Location);
            MetadataReference Snap = new MetadataFileReference(typeof(Snap.Color).Assembly.Location);
            MetadataReference[] references = { mscorlib, Snap, NXOpen, NXOpen_UF, NXOpen_Utilities, NXOpenUI };

            // Build a compilation and semantic model
            VisualBasicCompilation compilation = VisualBasicCompilation.Create("Syntax Coloring").AddSyntaxTrees(sourceTrees).AddReferences(references);
            SemanticModel model = compilation.GetSemanticModel(tree);

            SpanWithType spanWithType = new SpanWithType();

            SyntaxNode root = tree.GetRoot();
            // to make sure it's not a blank text
            if (root.DescendantNodes().Any())    // May not be necessary
            {
                IEnumerable<SyntaxNode> childNodes = root.DescendantNodes();   // Does this return an empty collection or null ??
                foreach (SyntaxNode syntaxNode in childNodes.Reverse())        // Is the "Reverse" useful? The nodes are not in any specific order
                {
                    //Check that we have a span that is at the end of the input text
                    if (syntaxNode.Span.Length != 0 && syntaxNode.Span.End == plainCode.Length)
                    {
                        //Figure out the type of the syntaxNode
                        spanWithType = ColorCurrentNode(syntaxNode, model);
                        break;
                    }
                }
            }

            // Process a "SyntaxTrivia" (comments are the only ones we care about)
            if (root.DescendantTrivia().Any())
            {
                SyntaxTrivia syntaxTrivia = root.DescendantTrivia().Last();
                int rawkind = syntaxTrivia.RawKind;
                if (syntaxTrivia.Span.End == plainCode.Length)
                {
                    // Figure out the type of the SyntaxTrivia  (will always be "comment"?)
                    spanWithType = ColorCurrentTrivia(syntaxTrivia);
                }
            }
            return spanWithType;
        }
        #endregion

        #region Private methods

        #region methods for global ways of coloring
        /// <summary>
        /// Get "ranges" from program text (range = classifiedSpan + text), only deals with code(whitespace, tab, return...are excluded)
        /// </summary>
        /// <param name="compilation">The compilation</param>
        /// <returns>Collection of ranges</returns>
        private static IEnumerable<Range> ColorDocument(VisualBasicCompilation compilation)
        {
            // Get semantic model
            SemanticModel model = compilation.GetSemanticModel(compilation.SyntaxTrees[0]);

            // Create classifiedSpans
            SyntaxNode newSource = compilation.SyntaxTrees[0].GetRoot();
            SourceText text = newSource.GetText();
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();
            IEnumerable<ClassifiedSpan> classifiedSpans = Classifier.GetClassifiedSpans(model, TextSpan.FromBounds(0, text.Length), workspace);

            // Create new ranges
            IEnumerable<Range> ranges = classifiedSpans.Select(classifiedSpan =>
                new Range(classifiedSpan, text.GetSubText(classifiedSpan.TextSpan).ToString()));
            ranges = FillGaps(text, ranges);
            return ranges;
        }

        /// <summary>
        /// Converts text to RTF format
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns>Text converted to RTF format</returns>
        /// <remarks>
        /// Adds a header, and processes newlines
        /// </remarks>
        private static string PlainTextToRtf(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return "";
            string rtf = @"{\rtf1\ansi\ansicpg936\deff0\deflang1033\deflangfe2052{\fonttbl{\f0\fnil Consolas;}{\f1\fnil\fcharset0 Calibri;}}
{\colortbl ;\red0\green128\blue0;\red255\green255\blue255;\red0\green0\blue0;\red0\green0\blue255;\red43\green145\blue175;\red255\green0\blue255;}
\viewkind4\uc1\pard\sa200\sl276\slmult1\cf1\highlight2\lang9\f0\fs20 ";

            rtf += plainText.Replace("\n", "\\cf3\\par\n");
            rtf += " }";
            return rtf;
        }

        /// <summary>
        /// Fill the gaps between the words in the text, these gaps are whitespaces, tabs, returns...
        /// </summary>
        /// <param name="text">the program text</param>
        /// <param name="ranges">ranges without whitespaces, tabs and endofline being processed </param>
        /// <returns> ranges that represent the program text, including all trivia </returns>
        private static IEnumerable<Range> FillGaps(SourceText text, IEnumerable<Range> ranges)
        {
            const string WhitespaceClassification = null;
            int current = 0;
            Range previous = null;

            foreach (Range range in ranges)
            {
                int start = range.TextSpan.Start;
                if (start > current)
                {
                    yield return new Range(WhitespaceClassification, TextSpan.FromBounds(current, start), text);
                }

                if (previous == null || range.TextSpan != previous.TextSpan)
                {
                    yield return range;
                }

                previous = range;
                current = range.TextSpan.End;
            }

            if (current < text.Length)
            {
                yield return new Range(WhitespaceClassification, TextSpan.FromBounds(current, text.Length), text);
            }
        }
        #endregion

        #region methods for dynamic ways of coloring

        /// <summary>
        /// Determine the type of a syntaxNode, so that we can decide what color it should be
        /// </summary>
        /// <param name="syntaxNode">The syntaxnode to be analysed</param>
        /// <param name="model">Semantic model of the program (from start to cursor location)</param>
        /// <returns>A SpanWithType object (span + type)</returns>
        private SpanWithType ColorCurrentNode(SyntaxNode syntaxNode, SemanticModel model)
        {
            // Find type of current node
            SyntaxToken syntaxToken = syntaxNode.GetLastToken();
            SyntaxKind syntaxKind = syntaxToken.VisualBasicKind();

            // If type is "KeywordKind"
            if (SyntaxFacts.IsKeywordKind(syntaxKind))
            {
                // Create a suitable spanWithType and return it
                CodeType type = CodeType.keywordType;
                CodeSpan span = new CodeSpan(syntaxToken.Span.Start, syntaxToken.Span.Length);
                SpanWithType spanWithType = new SpanWithType(span, type);
                return spanWithType;
            }

            // If type is "Punctuation"
            else if (SyntaxFacts.IsPunctuation(syntaxKind))
            {
                CodeType type = CodeType.punctuationType;
                CodeSpan span = new CodeSpan(syntaxToken.Span.Start, syntaxToken.Span.Length);
                SpanWithType spanWithType = new SpanWithType(span, type);
                return spanWithType;
            }

            // If type is "Operator"
            else if (SyntaxFacts.IsOperator(syntaxKind))
            {
                CodeType type = CodeType.operatorType;
                CodeSpan span = new CodeSpan(syntaxToken.Span.Start, syntaxToken.Span.Length);
                SpanWithType spanWithType = new SpanWithType(span, type);
                return spanWithType;
            }

            // And so on
            else if (syntaxKind == SyntaxKind.NumericLiteralExpression)
            {
                CodeType type = CodeType.numericType;
                CodeSpan span = new CodeSpan(syntaxToken.Span.Start, syntaxToken.Span.Length);
                SpanWithType spanWithType = new SpanWithType(span, type);
                return spanWithType;
            }

            else if (syntaxKind == SyntaxKind.StringLiteralExpression)
            {
                CodeType type = CodeType.stringType;
                CodeSpan span = new CodeSpan(syntaxToken.Span.Start, syntaxToken.Span.Length);
                SpanWithType spanWithType = new SpanWithType(span, type);
                return spanWithType;
            }

            // If type is "Identifier" -- could be an identifier of a Class, Structure, Enum, Interface, or Module,
            // or it could be just a local variable
            else if (syntaxKind == SyntaxKind.IdentifierToken)
            {
                TypeInfo nodeTypeInfo = model.GetTypeInfo(syntaxNode);
                SymbolInfo nodeSymbolInfo = model.GetSymbolInfo(syntaxNode);

                #region if the identifier has a type
                if (nodeTypeInfo.Type != null)
                {
                    ITypeSymbol nodeType = nodeTypeInfo.Type;
                    TypeKind nodeTypeKind = nodeType.TypeKind;

                    #region if the identifier has a symbol
                    if (nodeSymbolInfo.Symbol != null)
                    {
                        // or the identifier of an *instance* of a Class or a Structure
                        ISymbol nodeSymbol = nodeSymbolInfo.Symbol;
                        SymbolKind nodeSymbolKind = nodeSymbol.Kind;
                        #region if identifier is an variable, event, field, property ...
                        if (nodeSymbolKind == SymbolKind.Local || nodeSymbolKind == SymbolKind.Event ||
                            nodeSymbolKind == SymbolKind.Field || nodeSymbolKind == SymbolKind.Label ||
                            nodeSymbolKind == SymbolKind.ArrayType || nodeSymbolKind == SymbolKind.Method ||
                            nodeSymbolKind == SymbolKind.Namespace || nodeSymbolKind == SymbolKind.Parameter ||
                            nodeSymbolKind == SymbolKind.PointerType || nodeSymbolKind == SymbolKind.PointerType ||
                            nodeSymbolKind == SymbolKind.Preprocessing || nodeSymbolKind == SymbolKind.Property ||
                            nodeSymbolKind == SymbolKind.RangeVariable || nodeSymbolKind == SymbolKind.TypeParameter)
                        {
                            CodeType type = CodeType.variableType;
                            CodeSpan span = new CodeSpan(syntaxToken.Span.Start, syntaxToken.Span.Length);
                            SpanWithType spanWithType = new SpanWithType(span, type);
                            return spanWithType;
                        }
                        #endregion

                        #region if identifier is a Class, Structure, Enum, Interface, Module, or Delegate
                        // If the node is the identifier of a Class, Structure, Enum, Interface, or Module
                        else if (nodeTypeKind == TypeKind.Class || nodeTypeKind == TypeKind.Structure ||
                            nodeTypeKind == TypeKind.Enum || nodeTypeKind == TypeKind.Interface ||
                            nodeTypeKind == TypeKind.Module || nodeTypeKind == TypeKind.Delegate)
                        {
                            // The following code tries to determine that the node is the name/identifier of a class or struct (like Integer or NXOpen.Point)
                            CodeType type = CodeType.classType;
                            CodeSpan span = new CodeSpan(syntaxToken.Span.Start, syntaxToken.Span.Length);
                            SpanWithType spanWithType = new SpanWithType(span, type);
                            return spanWithType;
                        }
                        #endregion

                        #region if identifier is in other cases, becomes "default" type
                        // What's left ... other items in TypeKind and SymbolKind enumeration
                        // These are not likely to be found in recorded journal code.
                        // Anything else that we can't figure out becomes "default" type
                        else
                        {
                            CodeType type = CodeType.defaultType;
                            CodeSpan span = new CodeSpan(syntaxToken.Span.Start, syntaxToken.Span.Length);
                            SpanWithType spanWithType = new SpanWithType(span, type);
                            return spanWithType;
                        }
                        #endregion
                    }
                    #endregion

                    #region if the identifier has no symbol
                    else
                    {
                        CodeType type = CodeType.defaultType;
                        CodeSpan span = new CodeSpan(syntaxToken.Span.Start, syntaxToken.Span.Length);
                        SpanWithType spanWithType = new SpanWithType(span, type);
                        return spanWithType;
                    }
                    #endregion
                }
                #endregion

                // nodeInfo.Type = null, so we don't know what to do. And it becomes "default" type
                #region if the identifier has no type
                else
                {
                    CodeType type = CodeType.defaultType;
                    CodeSpan span = new CodeSpan(syntaxToken.Span.Start, syntaxToken.Span.Length);
                    SpanWithType spanWithType = new SpanWithType(span, type);
                    return spanWithType;
                }
                #endregion
            }

            else
            {
                CodeType type = CodeType.defaultType;
                CodeSpan span = new CodeSpan(syntaxToken.Span.Start, syntaxToken.Span.Length);
                SpanWithType spanWithType = new SpanWithType(span, type);
                return spanWithType;
            }
        }


        /// <summary>
        /// Determine the type of a SyntaxTrivia, so that we can decide what color it should be
        /// </summary>
        /// <param name="syntaxTrivia">The SyntaxTrivia to be analysed</param>
        /// <returns>A SpanWithType object (span + type)</returns>
        /// <remarks>
        /// The only trivia that we need to color are comments
        /// </remarks>
        private SpanWithType ColorCurrentTrivia(SyntaxTrivia syntaxTrivia)
        {
            SyntaxKind syntaxKind = syntaxTrivia.VisualBasicKind();

            // If the SyntaxTrivia is a comment 
            if (syntaxKind == SyntaxKind.CommentTrivia)
            {
                CodeType type = CodeType.commentType;
                CodeSpan span = new CodeSpan(syntaxTrivia.Span.Start, syntaxTrivia.Span.Length);
                SpanWithType spanWithType = new SpanWithType(span, type);
                return spanWithType;
            }

            else  // Maybe not necessary
            {
                CodeType type = CodeType.keywordType;
                CodeSpan span = new CodeSpan(syntaxTrivia.Span.Start, syntaxTrivia.Span.Length);
                SpanWithType spanWithType = new SpanWithType(span, type);
                return spanWithType;
            }
        }

        #endregion

        #endregion
    }
}
