using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;


namespace CoolEditor
{
    class CodeFixRewriter : VisualBasicSyntaxRewriter
    {
        private readonly SyntaxTree tree;   // the syntax tree to be processed
        private bool isMatchAssignment;     // If true, indicates that declaration and assignment are on consecutive lines


        public CodeFixRewriter(SyntaxTree tree)
        {
            this.tree = tree;
            this.isMatchAssignment = false;
        }

        private LocalDeclarationStatementSyntax pnode;

        // Over-ride the VisualBasicSyntaxRewriter method that's called when we visit an Option Statement node in the tree
        public override SyntaxNode VisitOptionStatement(OptionStatementSyntax node)
        {
            // If there's an "Option" statement, change it to "Option Infer On"
            SyntaxToken inferToken = SyntaxFactory.Token(SyntaxKind.InferKeyword).WithTrailingTrivia(node.NameKeyword.GetAllTrivia());
            SyntaxToken onToken = SyntaxFactory.Token(SyntaxKind.OnKeyword).WithTrailingTrivia(node.ValueKeyword.GetAllTrivia());
            OptionStatementSyntax inferOnStatement = SyntaxFactory.OptionStatement(node.OptionKeyword, inferToken, onToken).WithLeadingTrivia(node.GetLeadingTrivia()).WithTrailingTrivia(node.GetTrailingTrivia());

            return inferOnStatement;
        }

        // Over-ride the VisualBasicSyntaxRewriter method that's called when we visit a LocalDeclaration statement node in the tree
        public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            // If we find a declaration with no initialization, mark it for processing
            if (node.Declarators[0].Names.Count > 1)
            { return node; }
            if (node.Declarators[0].Initializer == null)
            {
                pnode = node;
                this.isMatchAssignment = true;
                return node;
            }
            return node;
        }

        // Over-ride the VisualBasicSyntaxRewriter method that's called when we visit an Assignment statement node in the tree
        public override SyntaxNode VisitAssignmentStatement(AssignmentStatementSyntax node)
        {
            // If we find an assignment with a declaration on previous line, compress into one line
            if (this.isMatchAssignment == true)
            {
                if (pnode.Declarators[0].Names[0].ToString() == node.Left.ToString())
                {
                    if (pnode.Declarators[0].Names[0].ToString().Contains("(0)"))
                    {
                        ModifiedIdentifierSyntax name = pnode.Declarators[0].Names[0];
                        ModifiedIdentifierSyntax _name = SyntaxFactory.ModifiedIdentifier(name.Identifier.ToString()).WithTrailingTrivia(name.GetTrailingTrivia());
                        VariableDeclaratorSyntax newName = pnode.Declarators[0].ReplaceNode(name, _name);
                        SimpleAsClauseSyntax asClause = (SimpleAsClauseSyntax)pnode.Declarators[0].AsClause;
                        SimpleAsClauseSyntax _asClause = SyntaxFactory.SimpleAsClause(asClause.AsKeyword, asClause.AttributeLists, SyntaxFactory.ArrayType(asClause.Type.WithTrailingTrivia(null))).WithLeadingTrivia(asClause.GetLeadingTrivia()).WithTrailingTrivia(null);
                        SeparatedSyntaxList<ExpressionSyntax> creationList = new SeparatedSyntaxList<ExpressionSyntax>();
                        creationList = creationList.Add(node.Right.WithTrailingTrivia(null));
                        CollectionInitializerSyntax collectionInitializer = SyntaxFactory.CollectionInitializer(creationList);
                        EqualsValueSyntax _initializer = SyntaxFactory.EqualsValue(node.OperatorToken, collectionInitializer).WithLeadingTrivia(node.OperatorToken.GetAllTrivia()).WithTrailingTrivia(node.Right.GetTrailingTrivia());
                        VariableDeclaratorSyntax _declarator = SyntaxFactory.VariableDeclarator(newName.Names, _asClause, _initializer).WithLeadingTrivia(node.Right.GetLeadingTrivia()).WithTrailingTrivia(node.Right.GetTrailingTrivia());
                        this.isMatchAssignment = false;
                        return pnode.ReplaceNode(pnode.Declarators[0], _declarator);
                    }
                    else
                    {
                        SimpleAsClauseSyntax asClause = (SimpleAsClauseSyntax)pnode.Declarators[0].AsClause;
                        EqualsValueSyntax _initializer = SyntaxFactory.EqualsValue(node.OperatorToken, node.Right).WithLeadingTrivia(node.OperatorToken.GetAllTrivia());
                        SimpleAsClauseSyntax _asClause = SyntaxFactory.SimpleAsClause(asClause.AsKeyword, asClause.AttributeLists, asClause.Type).WithLeadingTrivia(asClause.GetLeadingTrivia()).WithTrailingTrivia(null);
                        VariableDeclaratorSyntax _declarator = SyntaxFactory.VariableDeclarator(pnode.Declarators[0].Names, _asClause, _initializer);
                        this.isMatchAssignment = false;
                        return pnode.ReplaceNode(pnode.Declarators[0], _declarator);
                    }
                }
                else
                {
                    this.isMatchAssignment = false;
                    return node;
                }
            }
            else
            {
                return node;
            }
        }

        public SyntaxNode RemoveNodes(VisualBasicCompilation compilation)
        {
            //remove options
            SyntaxRemoveOptions option = SyntaxRemoveOptions.KeepNoTrivia;

            //create new semantic model, since the visit act has changed the original syntax tree
            SemanticModel model = compilation.GetSemanticModel(compilation.SyntaxTrees[0]);

            //SyntaxNode sourceNode = newNode;
            SyntaxNode sourceNode = compilation.SyntaxTrees[0].GetRoot();

            //remove nodes with mark  
            #region 
            //assignment
            IEnumerable<AssignmentStatementSyntax> assignNode = sourceNode.DescendantNodes().OfType<AssignmentStatementSyntax>();
            List<AssignmentStatementSyntax> assignList = new List<AssignmentStatementSyntax>();
            foreach (AssignmentStatementSyntax node in assignNode)
            {
                string undoMark = "NXOpen.Session.UndoMarkId";
                if (node.Left.GetType().ToString() != "Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax")
                {
                    ITypeSymbol variableType = model.GetTypeInfo(node.Left).Type;
                    if (variableType != null)
                    {
                        if (variableType.ToString() == undoMark)
                        {
                            assignList.Add(node);
                        }
                    }
                }
            }
            sourceNode = SyntaxNodeExtensions.RemoveNodes(sourceNode, assignList, option);

            //declaration
            IEnumerable<LocalDeclarationStatementSyntax> decNode = sourceNode.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
            List<LocalDeclarationStatementSyntax> decList = new List<LocalDeclarationStatementSyntax>();
            foreach (LocalDeclarationStatementSyntax node in decNode)
            {
                string undoMark = "NXOpen.Session.UndoMarkId";
                if (node.Declarators[0].AsClause != null)
                {
                    TypeSyntax variableType = ((SimpleAsClauseSyntax)node.Declarators[0].AsClause).Type;

                    if (variableType.ToString().Equals(undoMark))
                    {
                        decList.Add(node);
                    }
                }
            }
            sourceNode = SyntaxNodeExtensions.RemoveNodes(sourceNode, decList, option);

            //expression
            IEnumerable<ExpressionStatementSyntax> expressionNode1 = sourceNode.DescendantNodes().OfType<ExpressionStatementSyntax>();
            List<ExpressionStatementSyntax> expressionList1 = new List<ExpressionStatementSyntax>();
            foreach (ExpressionStatementSyntax node in expressionNode1)
            {
                string expression = node.Expression.ToString();
                string removeMark = "SetUndoMarkName";
                if (expression.Contains(removeMark))
                {
                    expressionList1.Add(node);
                }
            }
            sourceNode = SyntaxNodeExtensions.RemoveNodes(sourceNode, expressionList1, option);
            #endregion

            //remove redundant declaration nodes
            #region 
            List<LocalDeclarationStatementSyntax> nullDecList = new List<LocalDeclarationStatementSyntax>();
            foreach (LocalDeclarationStatementSyntax node in sourceNode.DescendantNodes().OfType<LocalDeclarationStatementSyntax>())
            {
                if (node.Declarators[0].Initializer == null)
                {
                    nullDecList.Add(node);
                }
            }

            List<LocalDeclarationStatementSyntax> redundantDecList = new List<LocalDeclarationStatementSyntax>();
            foreach (LocalDeclarationStatementSyntax node in (from _node in sourceNode.DescendantNodes()
                                                             .OfType<LocalDeclarationStatementSyntax>()
                                                              where _node.Declarators[0].Initializer != null
                                                              select _node))
            {
                foreach (LocalDeclarationStatementSyntax nullNode in nullDecList)
                {
                    if (nullNode.Declarators[0].Names[0].Identifier.ToString() == node.Declarators[0].Names[0].ToString())
                    {
                        redundantDecList.Add(nullNode);
                    }
                }
            }

            sourceNode = SyntaxNodeExtensions.RemoveNodes(sourceNode, redundantDecList, option);
            #endregion

            //remove nodes with delete mark
            #region 
            IEnumerable<ExpressionStatementSyntax> expressionNode2 = sourceNode.DescendantNodes().OfType<ExpressionStatementSyntax>();
            List<ExpressionStatementSyntax> expressionList2 = new List<ExpressionStatementSyntax>();
            foreach (ExpressionStatementSyntax node in expressionNode2)
            {
                string expression = node.Expression.ToString();
                string removeMark = "DeleteUndoMark";
                if (expression.Contains(removeMark))
                {
                    expressionList2.Add(node);
                }
            }
            sourceNode = SyntaxNodeExtensions.RemoveNodes(sourceNode, expressionList2, option);
            #endregion

            //remove declaration & assignment nodes with null assignment
            #region
            List<string> nothingList = new List<string>();
            List<LocalDeclarationStatementSyntax> nothingDecList = new List<LocalDeclarationStatementSyntax>();
            foreach (LocalDeclarationStatementSyntax node in sourceNode.DescendantNodes().OfType<LocalDeclarationStatementSyntax>())
            {
                if (node.Declarators[0].Initializer != null)
                {
                    if (node.Declarators[0].Initializer.Value.ToString() == "Nothing")
                    {
                        nothingDecList.Add(node);
                        nothingList.Add(node.Declarators[0].Names[0].ToString());
                    }
                }
            }
            sourceNode = SyntaxNodeExtensions.RemoveNodes(sourceNode, nothingDecList, option);

            List<AssignmentStatementSyntax> nothingAssignList = new List<AssignmentStatementSyntax>();
            foreach (AssignmentStatementSyntax node in sourceNode.DescendantNodes().OfType<AssignmentStatementSyntax>())
            {
                if (node.Left.GetType().ToString() != "Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax")
                {
                    if (node.Right.ToString() == "Nothing")
                    {
                        nothingAssignList.Add(node);
                        nothingList.Add(node.Right.ToString());
                    }
                }
            }
            sourceNode = SyntaxNodeExtensions.RemoveNodes(sourceNode, nothingAssignList, option);

            List<IdentifierNameSyntax> nameList = new List<IdentifierNameSyntax>();
            foreach (IdentifierNameSyntax node in sourceNode.DescendantNodes().OfType<IdentifierNameSyntax>())
            {
                if (nothingList.Contains(node.Identifier.ToString()))
                {
                    nameList.Add(node);
                }
            }

            while (sourceNode.DescendantNodes().OfType<IdentifierNameSyntax>().Any())
            {
                bool foundNothingIdentifier = false;
                foreach (IdentifierNameSyntax node in sourceNode.DescendantNodes().OfType<IdentifierNameSyntax>())
                {
                    if (nothingList.Contains(node.Identifier.ToString()))
                    {
                        IdentifierNameSyntax newNode = SyntaxFactory.IdentifierName("Nothing").WithLeadingTrivia(node.GetLeadingTrivia()).WithTrailingTrivia(node.GetTrailingTrivia());
                        sourceNode = sourceNode.ReplaceNode(node, newNode);
                        foundNothingIdentifier = true;
                        break;
                    }
                }
                if (!foundNothingIdentifier)
                {
                    break;
                }
            }

            List<IdentifierNameSyntax> newNameList = new List<IdentifierNameSyntax>();
            foreach (IdentifierNameSyntax node in nameList)
            {
                IdentifierNameSyntax newNode = SyntaxFactory.IdentifierName("Nothing").WithLeadingTrivia(node.GetLeadingTrivia()).WithTrailingTrivia(node.GetTrailingTrivia());
                newNameList.Add(newNode);
            }

            #endregion

            return sourceNode;
        }

    }

    class CodeOptimizeRewriter : VisualBasicSyntaxRewriter
    {
        private List<string> sList;
        private readonly SyntaxTree tree;

        public CodeOptimizeRewriter(SyntaxTree tree)
        {
            this.tree = tree;
            this.sList = new List<string>();
        }

        public List<string> SList { get { return sList; } set { sList = value; } }

        public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            if (node.Declarators[0].Initializer == null)
            {
                sList.Add(node.Declarators[0].Names[0].ToString());

            }
            else
            {
                string name = node.Declarators[0].Names[0].ToString();
                sList.Add(name);
                if (node.Declarators[0].Initializer.Value.DescendantNodes().OfType<MemberAccessExpressionSyntax>().Any())
                {
                    MemberAccessExpressionSyntax memberAccess = node.Declarators[0].Initializer.Value.DescendantNodes().OfType<MemberAccessExpressionSyntax>().First();
                    if (memberAccess.GetLastToken().ToString() == "Commit")
                    {
                        sList.Remove(node.Declarators[0].Names[0].ToString());
                    }
                }

                if (node.Declarators[0].Initializer.DescendantNodes().OfType<CTypeExpressionSyntax>().Any())
                {
                    CTypeExpressionSyntax ctypeExpression = node.Declarators[0].Initializer.DescendantNodes().OfType<CTypeExpressionSyntax>().First();
                    if (sList.Contains(ctypeExpression.Expression.GetFirstToken().ToString()))
                    {
                        sList.Remove(ctypeExpression.Expression.GetFirstToken().ToString());
                    }
                }

                if (node.Declarators[0].Initializer.Value.DescendantNodes().OfType<ArgumentListSyntax>().Any())
                {
                    ArgumentListSyntax argumentList = node.Declarators[0].Initializer.Value.DescendantNodes().OfType<ArgumentListSyntax>().First();
                    foreach (SimpleArgumentSyntax argument in argumentList.DescendantNodes().OfType<SimpleArgumentSyntax>())
                    {
                        if (sList.Contains(argument.GetFirstToken().ToString()))
                        {
                            sList.Remove(argument.GetFirstToken().ToString());
                        }
                    }
                }

                if (node.Declarators[0].Initializer.DescendantNodes().OfType<CollectionInitializerSyntax>().Any())
                {
                    CollectionInitializerSyntax collectionInitializer = node.Declarators[0].Initializer.DescendantNodes().OfType<CollectionInitializerSyntax>().First();
                    foreach (ExpressionSyntax expression in collectionInitializer.Initializers)
                    {
                        if (sList.Contains(expression.GetFirstToken().ToString()))
                        {
                            sList.Remove(expression.GetFirstToken().ToString());
                        }
                    }
                }

                if (sList.Contains(node.Declarators[0].Initializer.Value.GetFirstToken().ToString()))
                {
                    sList.Remove(node.Declarators[0].Initializer.Value.GetFirstToken().ToString());
                }
            }
            return node;
        }

        public override SyntaxNode VisitAssignmentStatement(AssignmentStatementSyntax node)
        {
            if (sList.Contains(node.Right.GetFirstToken().ToString()))
            {
                sList.Remove(node.Right.GetFirstToken().ToString());
            }

            if (node.Right.DescendantNodes().OfType<MemberAccessExpressionSyntax>().Any())
            {
                MemberAccessExpressionSyntax memberAccess = node.Right.DescendantNodes().OfType<MemberAccessExpressionSyntax>().First();
                if (memberAccess.GetLastToken().ToString() == "Commit")
                {
                    if (sList.Contains(memberAccess.GetLastToken().ToString()))
                    {
                        sList.Remove(memberAccess.GetLastToken().ToString());
                    }
                }
            }

            if (node.Right.DescendantNodes().OfType<ArgumentListSyntax>().Any())
            {
                ArgumentListSyntax argumentList = node.Right.DescendantNodes().OfType<ArgumentListSyntax>().First();
                foreach (SimpleArgumentSyntax argument in argumentList.DescendantNodes().OfType<SimpleArgumentSyntax>())
                {
                    if (sList.Contains(argument.GetFirstToken().ToString()))
                    {
                        sList.Remove(argument.GetFirstToken().ToString());
                    }
                }
            }
            return node;
        }

        public override SyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            MemberAccessExpressionSyntax memberAccess = node.Expression.DescendantNodes().OfType<MemberAccessExpressionSyntax>().First();
            if (memberAccess.GetLastToken().ToString() == "Destroy" || memberAccess.GetLastToken().ToString() == "Commit")
            {
                if (sList.Contains(memberAccess.GetFirstToken().ToString()))
                {
                    sList.Remove(memberAccess.GetFirstToken().ToString());
                }
            }
            if (!memberAccess.GetLastToken().ToString().Contains("Delete"))
            {
                if (node.Expression.DescendantNodes().OfType<ArgumentListSyntax>().Any())
                {
                    ArgumentListSyntax argumentList = node.Expression.DescendantNodes().OfType<ArgumentListSyntax>().First();
                    foreach (SimpleArgumentSyntax argument in argumentList.DescendantNodes().OfType<SimpleArgumentSyntax>())
                    {
                        if (sList.Contains(argument.GetFirstToken().ToString()))
                        {
                            sList.Remove(argument.GetFirstToken().ToString());
                        }
                    }
                }
            }
            if (sList.Contains(node.Expression.GetFirstToken().ToString()))
            {
                sList.Remove(node.Expression.GetFirstToken().ToString());
            }
            return node;
        }

        public SyntaxNode RemoveNodes(VisualBasicCompilation compilation)
        {
            //remove options
            SyntaxRemoveOptions option = SyntaxRemoveOptions.KeepNoTrivia;

            //create new semantic model, since the visit act has changed the original syntax tree
            SemanticModel model = compilation.GetSemanticModel(compilation.SyntaxTrees[0]);

            //SyntaxNode sourceNode = newNode;
            SyntaxNode sourceNode = compilation.SyntaxTrees[0].GetRoot();

            //remove declared but unused nodes
            #region

            //local declaration
            List<LocalDeclarationStatementSyntax> noUseDecList = new List<LocalDeclarationStatementSyntax>();
            foreach (LocalDeclarationStatementSyntax node in sourceNode.DescendantNodes().OfType<LocalDeclarationStatementSyntax>())
            {
                //string typeInfo = model.GetTypeInfo(node.Declarators[0].Initializer.Value).Type.ToString();
                //if (typeInfo == "NXOpen.Point3d" || typeInfo == "NXOpen.Vector3d")
                //{
                if (sList.Contains(node.Declarators[0].Names[0].ToString()))
                {
                    noUseDecList.Add(node);
                }
                //}
            }
            sourceNode = SyntaxNodeExtensions.RemoveNodes(sourceNode, noUseDecList, option);

            //assignment statement
            List<AssignmentStatementSyntax> noUseAssignList = new List<AssignmentStatementSyntax>();
            foreach (AssignmentStatementSyntax node in sourceNode.DescendantNodes().OfType<AssignmentStatementSyntax>())
            {
                if (node.Left.GetType().ToString() != "Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax")
                {
                    //string typeInfo = model.GetTypeInfo(node.Left).Type.ToString();
                    //if (typeInfo == "NXOpen.Point3d" || typeInfo == "NXOpen.Vector3d")
                    //{
                    if (sList.Contains(node.Left.ToString()))
                    {
                        noUseAssignList.Add(node);
                    }
                    //}
                }
                else
                {
                    //string typeInfo = model.GetTypeInfo(node.Left.DescendantNodes().First()).Type.ToString();
                    //if (typeInfo == "NXOpen.Point3d" || typeInfo == "NXOpen.Vector3d")
                    //{
                    if (sList.Contains(node.Left.GetFirstToken().ToString()))
                    {
                        noUseAssignList.Add(node);
                    }
                    //}
                }
            }
            sourceNode = SyntaxNodeExtensions.RemoveNodes(sourceNode, noUseAssignList, option);

            //expression statement
            List<ExpressionStatementSyntax> noUseExpressionList = new List<ExpressionStatementSyntax>();
            foreach (ExpressionStatementSyntax node in sourceNode.DescendantNodes().OfType<ExpressionStatementSyntax>())
            {
                MemberAccessExpressionSyntax memberAccess = node.Expression.DescendantNodes().OfType<MemberAccessExpressionSyntax>().First();
                ArgumentListSyntax argumentList = node.Expression.DescendantNodes().OfType<ArgumentListSyntax>().First();
                if (memberAccess.GetLastToken().ToString().Contains("Delete"))
                {
                    foreach (SimpleArgumentSyntax argument in argumentList.DescendantNodes().OfType<SimpleArgumentSyntax>())
                    {
                        if (sList.Contains(argument.GetFirstToken().ToString()))
                        {
                            noUseExpressionList.Add(node);
                        }
                    }
                }
            }
            sourceNode = SyntaxNodeExtensions.RemoveNodes(sourceNode, noUseExpressionList, option);
            #endregion
            return sourceNode;
        }

    }
}
