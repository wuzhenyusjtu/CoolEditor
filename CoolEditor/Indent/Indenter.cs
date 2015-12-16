using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.VisualBasic;

namespace CoolEditor
{
    public class Indenter
    {
        public Indenter() { }

        /// <summary>
        /// Adds an "End" block when the user types a return after a "Begin" statement
        /// </summary>
        /// <param name="text">Entire text of program</param>
        /// <param name="position">Character index locating current cursor position</param>
        /// <returns>A CodeWithPosition object containing new program text and cursor location</returns>
        /// <remarks>
        /// This function is expected to be called from a key event handler in an editor.
        /// Specifically, it should be called when the user types a "return".
        /// For example usage, see key event handlers in MyTextBox.cs
        /// </remarks>
        public CodeWithPosition DynamicIndenting(string text, int position)
        {
            //create syntax tree
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();
            SyntaxTree tree = VisualBasicSyntaxTree.ParseText(text);
            SyntaxNode root = tree.GetRoot();
            
           // Create a new SyntaxTrivia.
           // We're creating a "fake" trivia that we add to the end of the program,
           // after the cursor, to trigger the formatting function to work.
           SyntaxTrivia endoflineTrivia = SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "");

           // Some of sort of cycling looking for endoflineTrivia, the trivia should be at the postition of the current cursor
            foreach (SyntaxTrivia trivia in root.DescendantTrivia())
            {
                if (trivia.Span.End == position)
                {
                    endoflineTrivia = trivia;
                }
            }


            SyntaxTrivia commentTrivia = SyntaxFactory.CommentTrivia("'comment");
            List<SyntaxTrivia> commentList = new List<SyntaxTrivia>();
            commentList.Add(commentTrivia);

           // Insert our fake trivia at the cursor position
            root = root.InsertTriviaAfter(endoflineTrivia, commentList);

           // Various cases of code completion (adding "End" blocks)
           // The following 200 lines of code are fairly repetitive, and follow the same pattern

            #region Case of "Module"
 
           // Check if we have any Module statements.
           // This code assumes that there is only one Module in the file (true for recorded journals)+
            if (root.DescendantNodes().OfType<ModuleBlockSyntax>().Any())
            {
                ModuleBlockSyntax moduleBlock = root.DescendantNodes().OfType<ModuleBlockSyntax>().Last();
                if (moduleBlock.DescendantNodes().OfType<EndBlockStatementSyntax>().Any())
                {
                    EndBlockStatementSyntax endblockStatement = moduleBlock.DescendantNodes().OfType<EndBlockStatementSyntax>().Last();
                    if (endblockStatement.VisualBasicKind() == SyntaxKind.EndModuleStatement && endblockStatement.BlockKeyword.ToString().ToLower() != "module")
                    {
                        SyntaxToken endToken = SyntaxFactory.Token(SyntaxKind.EndKeyword);
                        SyntaxToken moduleToken = SyntaxFactory.Token(SyntaxKind.ModuleKeyword);
                        SyntaxTrivia endofline = SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "");

                        // Create an "End Module" statement
                        EndBlockStatementSyntax endmoduleStatement = SyntaxFactory
                           .EndBlockStatement(SyntaxKind.EndModuleStatement, endToken, moduleToken)
                           .WithLeadingTrivia(endofline)
                           .WithTrailingTrivia(endblockStatement.GetTrailingTrivia());
                       
                        // In the syntax tree, replace the placeholder "End Block" statement by an "End Module" statement 
                        root = root.ReplaceNode(endblockStatement, endmoduleStatement);
                    }
                }
            }
            #endregion

            #region Case of "Sub" or "Function"

            // Check if we have any "Sub" or "Function" statements.
            if (root.DescendantNodes().OfType<MethodBlockSyntax>().Any())   // ### Is this needed ??? ###
            {
                foreach (MethodBlockSyntax methodBlock in root.DescendantNodes().OfType<MethodBlockSyntax>())
                {
                    if (methodBlock.DescendantNodes().OfType<EndBlockStatementSyntax>().Any())
                    {
                        EndBlockStatementSyntax endblockStatement = methodBlock.DescendantNodes().OfType<EndBlockStatementSyntax>().Last();

                        // The "Function" case
                        if (endblockStatement.VisualBasicKind() == SyntaxKind.EndFunctionStatement && endblockStatement.BlockKeyword.ToString().ToLower() != "function")
                        {
                            SyntaxToken endToken = SyntaxFactory.Token(SyntaxKind.EndKeyword);
                            SyntaxToken functionToken = SyntaxFactory.Token(SyntaxKind.FunctionKeyword);
                            SyntaxTrivia endofline = SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "");

                            // Create an "End Function" statement
                            EndBlockStatementSyntax endfunctionStatement = SyntaxFactory
                               .EndBlockStatement(SyntaxKind.EndFunctionStatement, endToken, functionToken)
                               .WithLeadingTrivia(endofline)
                               .WithTrailingTrivia(endblockStatement.GetTrailingTrivia());

                            // In the syntax tree, replace the placeholder "End Block" statement by an "End Function" statement 
                            root = root.ReplaceNode(endblockStatement, endfunctionStatement);
                        }

                        // The "Sub" case
                        if (endblockStatement.VisualBasicKind() == SyntaxKind.EndSubStatement && endblockStatement.BlockKeyword.ToString().ToLower() != "sub")
                        {
                            SyntaxToken endToken = SyntaxFactory.Token(SyntaxKind.EndKeyword);
                            SyntaxToken subToken = SyntaxFactory.Token(SyntaxKind.SubKeyword);
                            SyntaxTrivia endofline = SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "");
                            EndBlockStatementSyntax endsubStatement = SyntaxFactory
                               .EndBlockStatement(SyntaxKind.EndSubStatement, endToken, subToken)
                               .WithLeadingTrivia(endofline)
                               .WithTrailingTrivia(endblockStatement.GetTrailingTrivia());
                            root = root.ReplaceNode(endblockStatement, endsubStatement);
                        }
                    }
                }
            }
            #endregion

            #region case of the if block
            if (root.DescendantNodes().OfType<MultiLineIfBlockSyntax>().Any())
            {
                foreach (MultiLineIfBlockSyntax ifBlock in root.DescendantNodes().OfType<MultiLineIfBlockSyntax>())
                {
                    if (ifBlock.DescendantNodes().OfType<EndBlockStatementSyntax>().Any())
                    {
                        EndBlockStatementSyntax endblockStatement = ifBlock.DescendantNodes().OfType<EndBlockStatementSyntax>().Last();
                        if (endblockStatement.VisualBasicKind() == SyntaxKind.EndIfStatement && endblockStatement.BlockKeyword.ToString().ToLower() != "if")
                        {
                            SyntaxToken endToken = SyntaxFactory.Token(SyntaxKind.EndKeyword);
                            SyntaxToken ifToken = SyntaxFactory.Token(SyntaxKind.IfKeyword);
                            SyntaxTrivia endofline = SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "");
                            EndBlockStatementSyntax endifStatement = SyntaxFactory.EndBlockStatement(SyntaxKind.EndIfStatement, endToken, ifToken).WithLeadingTrivia(endofline).WithTrailingTrivia(endblockStatement.GetTrailingTrivia());
                            root = root.ReplaceNode(endblockStatement, endifStatement);
                        }
                    }
                }
            }

            #endregion

            #region case of the while block
            if (root.DescendantNodes().OfType<WhileBlockSyntax>().Any())
            {
                foreach (WhileBlockSyntax whileBlock in root.DescendantNodes().OfType<WhileBlockSyntax>())
                {
                    if (whileBlock.DescendantNodes().OfType<EndBlockStatementSyntax>().Any())
                    {
                        EndBlockStatementSyntax endblockStatement = whileBlock.DescendantNodes().OfType<EndBlockStatementSyntax>().Last();
                        if (endblockStatement.VisualBasicKind() == SyntaxKind.EndWhileStatement && endblockStatement.BlockKeyword.ToString().ToLower() != "while")
                        {
                            SyntaxToken endToken = SyntaxFactory.Token(SyntaxKind.EndKeyword);
                            SyntaxToken whileToken = SyntaxFactory.Token(SyntaxKind.WhileKeyword);
                            SyntaxTrivia endofline = SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "");
                            EndBlockStatementSyntax endwhileStatement = SyntaxFactory.EndBlockStatement(SyntaxKind.EndWhileStatement, endToken, whileToken).WithLeadingTrivia(endofline).WithTrailingTrivia(endblockStatement.GetTrailingTrivia());
                            root = root.ReplaceNode(endblockStatement, endwhileStatement);
                        }
                    }
                }
            }
            #endregion

            #region case of the for block
            if (root.DescendantNodes().OfType<ForBlockSyntax>().Any())
            {
                foreach (ForBlockSyntax forBlock in root.DescendantNodes().OfType<ForBlockSyntax>())
                {
                    if (forBlock.DescendantNodes().OfType<NextStatementSyntax>().Any())
                    {
                        NextStatementSyntax next = forBlock.DescendantNodes().OfType<NextStatementSyntax>().Last();
                        if (next.VisualBasicKind() == SyntaxKind.NextStatement && next.NextKeyword.ToString().ToLower() != "next")
                        {
                            SyntaxTrivia endofline = SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "");
                            NextStatementSyntax nextStatement = SyntaxFactory.NextStatement().WithLeadingTrivia(endofline).WithTrailingTrivia(next.GetTrailingTrivia());
                            root = root.ReplaceNode(next, nextStatement);
                        }
                    }
                }
            }
            #endregion

            #region case of the for each block
            if (root.DescendantNodes().OfType<ForEachStatementSyntax>().Any())
            {
                foreach (ForEachStatementSyntax foreachBlock in root.DescendantNodes().OfType<ForEachStatementSyntax>())
                {
                    if (foreachBlock.DescendantNodes().OfType<NextStatementSyntax>().Any())
                    {
                        NextStatementSyntax next = foreachBlock.DescendantNodes().OfType<NextStatementSyntax>().Last();
                        if (next.VisualBasicKind() == SyntaxKind.NextStatement && next.NextKeyword.ToString().ToLower() != "next")
                        {
                            SyntaxTrivia endofline = SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "");
                            NextStatementSyntax nextStatement = SyntaxFactory.NextStatement().WithLeadingTrivia(endofline).WithTrailingTrivia(next.GetTrailingTrivia());
                            root = root.ReplaceNode(next, nextStatement);
                        }
                    }
                }
            }
            #endregion

            #region case of the class
            if (root.DescendantNodes().OfType<ClassBlockSyntax>().Any())
            {
                foreach (ClassBlockSyntax classBlock in root.DescendantNodes().OfType<ClassBlockSyntax>())
                {
                    if (classBlock.DescendantNodes().OfType<EndBlockStatementSyntax>().Any())
                    {
                        EndBlockStatementSyntax endblockStatement = classBlock.DescendantNodes().OfType<EndBlockStatementSyntax>().Last();
                        if (endblockStatement.VisualBasicKind() == SyntaxKind.EndClassStatement && endblockStatement.BlockKeyword.ToString().ToLower() != "class")
                        {
                            SyntaxToken endToken = SyntaxFactory.Token(SyntaxKind.EndKeyword);
                            SyntaxToken classToken = SyntaxFactory.Token(SyntaxKind.ClassKeyword);
                            SyntaxTrivia endofline = SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "");
                            EndBlockStatementSyntax endclassStatement = SyntaxFactory.EndBlockStatement(SyntaxKind.EndClassStatement, endToken, classToken).WithLeadingTrivia(endofline).WithTrailingTrivia(endblockStatement.GetTrailingTrivia());
                            root = root.ReplaceNode(endblockStatement, endclassStatement);
                        }
                    }
                }
            }
            #endregion

            #region case of the structure
            if (root.DescendantNodes().OfType<StructureBlockSyntax>().Any())
            {
                foreach (StructureBlockSyntax structBlock in root.DescendantNodes().OfType<StructureBlockSyntax>())
                {
                    if (structBlock.DescendantNodes().OfType<EndBlockStatementSyntax>().Any())
                    {
                        EndBlockStatementSyntax endblockStatement = structBlock.DescendantNodes().OfType<EndBlockStatementSyntax>().Last();
                        if (endblockStatement.VisualBasicKind() == SyntaxKind.EndStructureStatement && endblockStatement.BlockKeyword.ToString().ToLower() != "structure")
                        {
                            SyntaxToken endToken = SyntaxFactory.Token(SyntaxKind.EndKeyword);
                            SyntaxToken structureToken = SyntaxFactory.Token(SyntaxKind.StructureKeyword);
                            SyntaxTrivia endofline = SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "");
                            EndBlockStatementSyntax endstructStatement = SyntaxFactory.EndBlockStatement(SyntaxKind.EndStructureStatement, endToken, structureToken).WithLeadingTrivia(endofline).WithTrailingTrivia(endblockStatement.GetTrailingTrivia());
                            root = root.ReplaceNode(endblockStatement, endstructStatement);
                        }
                    }
                }
            }
            #endregion

            #region case of the enum
            if (root.DescendantNodes().OfType<EnumBlockSyntax>().Any())
            {
                foreach (EnumBlockSyntax enumBlock in root.DescendantNodes().OfType<EnumBlockSyntax>())
                {
                    if (enumBlock.DescendantNodes().OfType<EndBlockStatementSyntax>().Any())
                    {
                        EndBlockStatementSyntax endblockStatement = enumBlock.DescendantNodes().OfType<EndBlockStatementSyntax>().Last();
                        if (endblockStatement.VisualBasicKind() == SyntaxKind.EndEnumStatement && endblockStatement.BlockKeyword.ToString().ToLower() != "enum")
                        {
                            SyntaxToken endToken = SyntaxFactory.Token(SyntaxKind.EndKeyword);
                            SyntaxToken structureToken = SyntaxFactory.Token(SyntaxKind.EnumKeyword);
                            SyntaxTrivia endofline = SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "");
                            EndBlockStatementSyntax endenumStatement = SyntaxFactory.EndBlockStatement(SyntaxKind.EndEnumStatement, endToken, structureToken).WithLeadingTrivia(endofline).WithTrailingTrivia(endblockStatement.GetTrailingTrivia());
                            root = root.ReplaceNode(endblockStatement, endenumStatement);
                        }
                    }
                }
            }
            #endregion

            #region case of the namespace

            // This assumes that there will be only one namespace block in the file.
            // Actually, in a recorded journal, there won't be any namespaces blocks at all.

            if (root.DescendantNodes().OfType<NamespaceBlockSyntax>().Any())
            {
                NamespaceBlockSyntax namespaceBlock = root.DescendantNodes().OfType<NamespaceBlockSyntax>().Last();
                if (namespaceBlock.DescendantNodes().OfType<EndBlockStatementSyntax>().Any())
                {
                    EndBlockStatementSyntax endblockStatement = namespaceBlock.DescendantNodes().OfType<EndBlockStatementSyntax>().Last();
                    if (endblockStatement.VisualBasicKind() == SyntaxKind.EndNamespaceStatement && endblockStatement.BlockKeyword.ToString().ToLower() != "namespace")
                    {
                        SyntaxToken endToken = SyntaxFactory.Token(SyntaxKind.EndKeyword);
                        SyntaxToken namespaceToken = SyntaxFactory.Token(SyntaxKind.NamespaceKeyword);
                        SyntaxTrivia endofline = SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "");
                        EndBlockStatementSyntax endnamespaceStatement = SyntaxFactory.EndBlockStatement(SyntaxKind.EndNamespaceStatement, endToken, namespaceToken).WithLeadingTrivia(endofline).WithTrailingTrivia(endblockStatement.GetTrailingTrivia());
                        root = root.ReplaceNode(endblockStatement, endnamespaceStatement);
                    }
                }
            }
            #endregion

            SyntaxNode formattedNode = Formatter.Format(root, workspace);
            string formattedCode = formattedNode.ToFullString();
            int index = formattedCode.IndexOf("'comment") - 1;
            formattedCode = formattedNode.ToFullString().Replace("'comment", "");
            CodeWithPosition CodeWithPosition = new CodeWithPosition(formattedCode,index);
            return CodeWithPosition;
        }

       /// <summary>
       /// Indents selected text from a VB source file
       /// </summary>
       /// <param name="text">Input program (as a single text string)</param>
       /// <returns>Same program, but with spaces to provide indenting </returns>
        public string ExplicitIndenting(string programText)
        {
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();
            SyntaxTree tree = VisualBasicSyntaxTree.ParseText(programText);
            SyntaxNode root = tree.GetRoot();

            // Format the code (using default options)
            SyntaxNode formattedNode = Formatter.Format(root, workspace);
            string formattedCode = formattedNode.ToFullString();
            return formattedCode;
        }
    }
}
