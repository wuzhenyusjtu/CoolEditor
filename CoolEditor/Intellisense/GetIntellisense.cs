using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;


namespace CoolEditor
{
    public class GetIntellisense
    {
        #region Constructors
        public GetIntellisense()
        {
        }
        #endregion

        /// <summary>
        /// Shows the intellisense box.
        /// </summary>
        #region Methods

        public List<IntellisenseItem> GetMethodInfo(IntellisenseItem item)
        {
            List<IntellisenseItem> itemList = new List<IntellisenseItem>();
            int overloadCount = 0;
            foreach (IMethodSymbol methodInfo in item.MethodInfoList)
            {
                overloadCount++;
                IntellisenseItem countItem = new IntellisenseItem();
                countItem.Text = "Overload: " + overloadCount.ToString();
                itemList.Add(countItem);
                //IntellisenseItem item = (IntellisenseItem)myTextBox.IntellisenseBox.SelectedItem;
                if (!methodInfo.ReturnsVoid)
                {
                    IntellisenseItem returnItem = new IntellisenseItem();
                    returnItem.Text = "Return: " + methodInfo.ReturnType.ToString();
                    itemList.Add(returnItem);
                }
                else
                {
                    IntellisenseItem returnItem = new IntellisenseItem();
                    returnItem.Text = "Return: void";
                    itemList.Add(returnItem);
                }

                if (methodInfo.Parameters.Count() > 0)
                {
                    int paramCount = 0;
                    foreach (IParameterSymbol param in methodInfo.Parameters)
                    {
                        paramCount++;
                        IntellisenseItem paramItem = new IntellisenseItem();
                        paramItem.Text = "param" + paramCount.ToString() + ": " + param.Type.ToString() + " ";
                        paramItem.Text += param.Name.ToString();
                        itemList.Add(paramItem);
                    }
                }
            }
            return itemList;
        }

        /// <summary>
        /// Get the members of the "current" object, or Class, Structure, Enum or Namespace...
        /// </summary>
        /// <param name="code">The code</param>
        /// <param name="position">Cursor position</param>
        /// <returns>List of members of the current object</returns>
        /// <remarks>
        /// We assume the user just typed a period. 
        /// We want to get the members of the thing before the period
        /// (this thing could be a namespace, class, structure, enum, or instance)
        /// </remarks>
        public List<IntellisenseItem> GetMembers(string code, int position)
        {
            // Create syntax tree
            SyntaxTree tree = VisualBasicSyntaxTree.ParseText(code);
            SyntaxNode root = tree.GetRoot();

            // Add references
            MetadataReference mscorlib = new MetadataFileReference(typeof(object).Assembly.Location);
            MetadataReference Snap = new MetadataFileReference(typeof(Snap.Color).Assembly.Location);
            MetadataReference NXOpen = new MetadataFileReference(typeof(NXOpen.Point3d).Assembly.Location);
            MetadataReference NXOpen_UF = new MetadataFileReference(typeof(NXOpen.UF.UF).Assembly.Location);
            MetadataReference NXOpen_Utilities = new MetadataFileReference(typeof(NXOpen.Utilities.BaseSession).Assembly.Location);
            MetadataReference NXOpenUI = new MetadataFileReference(typeof(NXOpenUI.FormUtilities).Assembly.Location);
            MetadataReference[] references = { mscorlib, Snap, NXOpen, NXOpen_UF, NXOpen_Utilities, NXOpenUI };

            //Create compilation and get semantic model
            VisualBasicCompilation compilation = VisualBasicCompilation.Create("Intellisense").AddReferences(references).AddSyntaxTrees(tree);
            SemanticModel model = compilation.GetSemanticModel(tree);

            // Make a new list of intellisense items
            List<IntellisenseItem> itemList = new List<IntellisenseItem>();

            if (root.DescendantNodes().Any())
            {
                // Create TextSpan that consists of the last "dot" (period) you have typed
                TextSpan dotTextSpan = new TextSpan(position - 1, 1);

                // Get the type of the last word before the period
                SyntaxNode prevNode = root.DescendantNodes(dotTextSpan).Last();
                string typeInfo = prevNode.GetType().ToString();

                #region MemberAccessExpression
                if (typeInfo == "Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax")
                {
                    MemberAccessExpressionSyntax node = (MemberAccessExpressionSyntax)prevNode;
                    TypeInfo nodeTypeInfo = model.GetTypeInfo(node.Expression);
                    SymbolInfo nodeSymbolInfo = model.GetSymbolInfo(node.Expression);
                    if (nodeTypeInfo.Type != null)
                    {
                        ITypeSymbol nodeType = nodeTypeInfo.Type;
                        TypeKind nodeTypeKind = nodeType.TypeKind;
                        #region if the identifier has a symbol
                        if (nodeSymbolInfo.Symbol != null)
                        {
                            ISymbol nodeSymbol = nodeSymbolInfo.Symbol;
                            SymbolKind nodeSymbolKind = nodeSymbol.Kind;

                            #region if identifier is an variable...
                            if (nodeSymbolKind == SymbolKind.Local)
                            {
                                #region Get the name of the non-static methods
                                // Can we make this list once, instead of twice
                                // var myList = type.GetMembers().OfType<IMethodSymbol>()where method.DeclaredAccessibility == Accessibility.Public && method.IsStatic == true;

                                // Getting a list of distinct static method names, which we use to organise
                                // our list of intellisense items
                                IEnumerable<string> methodNameLinq = (from method in nodeType.GetMembers().OfType<IMethodSymbol>()
                                where method.DeclaredAccessibility == Accessibility.Public && method.IsStatic == false
                                select method.Name).Distinct();
                                foreach (string name in methodNameLinq)
                                {
                                    IntellisenseItem item = new IntellisenseItem();
                                    item.Text = name;
                                    //item.ForeColor = Color.Black;
                                    itemList.Add(item);
                                }
                                #endregion

                                #region Get the method info of the non-static methods

                                // Getting full information about static methods (name, signature, return type)
                                // Organise into a list using the list of distinct method names from above.
                                IEnumerable<IMethodSymbol> methodInfoLinq = from method in nodeType.GetMembers().OfType<IMethodSymbol>()
                                                                      where method.DeclaredAccessibility == Accessibility.Public && method.IsStatic == false
                                                                      select method;
                                foreach (IMethodSymbol method in methodInfoLinq)
                                {
                                    foreach (IntellisenseItem item in itemList)
                                    {
                                        // item.Tag = method.MethodKind.ToString();   // Does this work. Can it replace the logic below.

                                        if (item.Text == method.Name)
                                        {
                                            #region initialize items
                                            switch (method.MethodKind)
                                            {
                                                case MethodKind.AnonymousFunction:
                                                    item.Tag = MethodKind.AnonymousFunction.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    break;
                                                case MethodKind.Constructor:
                                                    item.Tag = MethodKind.Constructor.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    item.MethodInfoList.Add(method);
                                                    break;
                                                case MethodKind.Conversion:
                                                    item.Tag = MethodKind.Conversion.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    break;
                                                case MethodKind.DelegateInvoke:
                                                    item.Tag = MethodKind.DelegateInvoke.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    break;
                                                case MethodKind.Destructor:
                                                    item.Tag = MethodKind.Destructor.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    item.MethodInfoList.Add(method);
                                                    break;
                                                case MethodKind.EventAdd:
                                                    item.Tag = MethodKind.EventAdd.ToString();
                                                    item.Image = MyTextBox.eventImage;
                                                    break;
                                                case MethodKind.EventRaise:
                                                    item.Tag = MethodKind.EventRaise.ToString();
                                                    item.Image = MyTextBox.eventImage;
                                                    break;
                                                case MethodKind.EventRemove:
                                                    item.Tag = MethodKind.EventRemove.ToString();
                                                    item.Image = MyTextBox.eventImage;
                                                    break;
                                                case MethodKind.ExplicitInterfaceImplementation:
                                                    item.Tag = MethodKind.ExplicitInterfaceImplementation.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    break;
                                                case MethodKind.UserDefinedOperator:
                                                    item.Tag = MethodKind.UserDefinedOperator.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    break;
                                                case MethodKind.Ordinary:
                                                    item.Tag = MethodKind.Ordinary.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    item.MethodInfoList.Add(method);
                                                    break;
                                                case MethodKind.PropertyGet:
                                                    item.Tag = MethodKind.PropertyGet.ToString();
                                                    item.Image = MyTextBox.propertyImage;
                                                    break;
                                                case MethodKind.PropertySet:
                                                    item.Tag = MethodKind.PropertySet.ToString();
                                                    item.Image = MyTextBox.propertyImage;
                                                    break;
                                                case MethodKind.ReducedExtension:
                                                    item.Tag = MethodKind.ReducedExtension.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    break;
                                                case MethodKind.SharedConstructor:
                                                    item.Tag = MethodKind.SharedConstructor.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    break;
                                                case MethodKind.BuiltinOperator:
                                                    item.Tag = MethodKind.BuiltinOperator.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    break;
                                                case MethodKind.DeclareMethod:
                                                    item.Tag = MethodKind.DeclareMethod.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    item.MethodInfoList.Add(method);
                                                    break;
                                                default:
                                                    item.Tag = "Unknown";
                                                    item.Image = MyTextBox.methodImage;
                                                    break;
                                            }
                                            #endregion
                                        }
                                    }
                                }
                                #endregion
                            }
                            #endregion

                            #region if identifier is a Class or Structure type
                            else if (nodeTypeKind == TypeKind.Class || nodeTypeKind == TypeKind.Structure ||
                            nodeTypeKind == TypeKind.Enum)
                            {

                                #region Get the name of the static methods
                                // Getting a list of distinct static method names, which we use to organise
                                // our list of intellisense items
                                IEnumerable<string> methodNameLinq = (from method in nodeType.GetMembers().OfType<IMethodSymbol>()
                                                                      where method.DeclaredAccessibility == Accessibility.Public && method.IsStatic == true
                                                                      select method.Name).Distinct();
                                foreach (string name in methodNameLinq)
                                {
                                    IntellisenseItem item = new IntellisenseItem();
                                    item.Text = name;
                                    //item.ForeColor = Color.Black;
                                    itemList.Add(item);
                                }
                                #endregion

                                #region Get the method info of the static methods

                                // Getting full information about static methods (name, signature, return type)
                                // Organise into a list using the list of distinct method names from above.
                                IEnumerable<IMethodSymbol> methodInfoLinq = from method in nodeType.GetMembers().OfType<IMethodSymbol>()
                                                                            where method.DeclaredAccessibility == Accessibility.Public && method.IsStatic == true
                                                                            select method;
                                foreach (IMethodSymbol method in methodInfoLinq)
                                {
                                    foreach (IntellisenseItem item in itemList)
                                    {
                                        // item.Tag = method.MethodKind.ToString();   // Does this work. Can it replace the logic below.

                                        if (item.Text == method.Name)
                                        {
                                            #region initialize items
                                            switch (method.MethodKind)
                                            {
                                                case MethodKind.AnonymousFunction:
                                                    item.Tag = MethodKind.AnonymousFunction.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    break;
                                                case MethodKind.Constructor:
                                                    item.Tag = MethodKind.Constructor.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    item.MethodInfoList.Add(method);
                                                    break;
                                                case MethodKind.Conversion:
                                                    item.Tag = MethodKind.Conversion.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    break;
                                                case MethodKind.DelegateInvoke:
                                                    item.Tag = MethodKind.DelegateInvoke.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    break;
                                                case MethodKind.Destructor:
                                                    item.Tag = MethodKind.Destructor.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    item.MethodInfoList.Add(method);
                                                    break;
                                                case MethodKind.EventAdd:
                                                    item.Tag = MethodKind.EventAdd.ToString();
                                                    item.Image = MyTextBox.eventImage;
                                                    break;
                                                case MethodKind.EventRaise:
                                                    item.Tag = MethodKind.EventRaise.ToString();
                                                    item.Image = MyTextBox.eventImage;
                                                    break;
                                                case MethodKind.EventRemove:
                                                    item.Tag = MethodKind.EventRemove.ToString();
                                                    item.Image = MyTextBox.eventImage;
                                                    break;
                                                case MethodKind.ExplicitInterfaceImplementation:
                                                    item.Tag = MethodKind.ExplicitInterfaceImplementation.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    break;
                                                case MethodKind.UserDefinedOperator:
                                                    item.Tag = MethodKind.UserDefinedOperator.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    break;
                                                case MethodKind.Ordinary:
                                                    item.Tag = MethodKind.Ordinary.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    item.MethodInfoList.Add(method);
                                                    break;
                                                case MethodKind.PropertyGet:
                                                    item.Tag = MethodKind.PropertyGet.ToString();
                                                    item.Image = MyTextBox.propertyImage;
                                                    break;
                                                case MethodKind.PropertySet:
                                                    item.Tag = MethodKind.PropertySet.ToString();
                                                    item.Image = MyTextBox.propertyImage;
                                                    break;
                                                case MethodKind.ReducedExtension:
                                                    item.Tag = MethodKind.ReducedExtension.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    break;
                                                case MethodKind.SharedConstructor:
                                                    item.Tag = MethodKind.SharedConstructor.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    break;
                                                case MethodKind.BuiltinOperator:
                                                    item.Tag = MethodKind.BuiltinOperator.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    break;
                                                case MethodKind.DeclareMethod:
                                                    item.Tag = MethodKind.DeclareMethod.ToString();
                                                    item.Image = MyTextBox.methodImage;
                                                    item.MethodInfoList.Add(method);
                                                    break;
                                                default:
                                                    item.Tag = "Unknown";
                                                    item.Image = MyTextBox.methodImage;
                                                    break;
                                            }
                                            #endregion
                                        }
                                    }
                                }
                                #endregion

                            }
                            #endregion
                        }
                        #endregion
                    }
                    else
                    {
                        SymbolInfo symbolInfo = model.GetSymbolInfo(node.Expression);
                        INamespaceSymbol symbol = (INamespaceSymbol)symbolInfo.Symbol;

                        #region Get the subnamespaces
                        if (symbol.GetNamespaceMembers().Any())
                        {
                            foreach (INamespaceSymbol name in symbol.GetNamespaceMembers().Distinct())
                            {
                                IntellisenseItem item = new IntellisenseItem();
                                item.Text = name.Name;
                                item.Tag = "Namespace";
                                item.Image = MyTextBox.namespaceImage;
                                //item.ForeColor = Color.Black;
                                itemList.Add(item);
                            }
                        }
                        #endregion

                        #region Get the members of the namespace
                        if (symbol.GetMembers().OfType<INamedTypeSymbol>().Any())
                        {
                            foreach (INamedTypeSymbol member in symbol.GetMembers().OfType<INamedTypeSymbol>().Distinct())
                            {
                                IntellisenseItem item = new IntellisenseItem();
                                item.Tag = member.TypeKind.ToString();
                                item.Text = member.Name;
                                switch (member.TypeKind)
                                {
                                    case TypeKind.Class:
                                        item.Image = MyTextBox.classImage;
                                        break;
                                    case TypeKind.Enum:
                                        item.Image = MyTextBox.classImage;
                                        break;
                                    case TypeKind.Interface:
                                        item.Image = MyTextBox.interfaceImage;
                                        break;
                                    case TypeKind.Structure:
                                        item.Image = MyTextBox.classImage;
                                        break;
                                    default:
                                        item.Image = MyTextBox.classImage;
                                        break;
                                }
                                //item.ForeColor = Color.Black;
                                itemList.Add(item);
                            }
                        }
                        #endregion

                    }
                }
                #endregion

                #region PredefinedType
                else
                {
                    // Get the last word user has typed
                    string lastWord = GetLastWord(code.Substring(0, position));

                    // Construct a syntax tree, use the last word only
                    SyntaxTree newTree = VisualBasicSyntaxTree.ParseText("Module intellisense\n\tSub Main()\n\t\t" + lastWord
        + "\n\tEnd Sub\nEnd Module");
                    root = newTree.GetRoot();

                    // Build compilation and get semantic model
                    compilation = compilation.ReplaceSyntaxTree(compilation.SyntaxTrees[0], newTree);
                    model = compilation.GetSemanticModel(newTree);
                    if (root.DescendantNodes().OfType<MemberAccessExpressionSyntax>().Any())
                    {
                        MemberAccessExpressionSyntax node = root.DescendantNodes().OfType<MemberAccessExpressionSyntax>().First();
                        if (node.Expression != null)
                        {
                            TypeInfo nodeTypeInfo = model.GetTypeInfo(node.Expression);
                            SymbolInfo nodeSymbolInfo = model.GetSymbolInfo(node.Expression);
                            // If the identifier is an object, not a namesapce
                            if (nodeTypeInfo.Type != null)
                            {
                                ITypeSymbol nodeType = nodeTypeInfo.Type;
                                TypeKind nodeTypeKind = nodeType.TypeKind;
                                #region if the identifier has a symbol
                                if (nodeSymbolInfo.Symbol != null)
                                {
                                    ISymbol nodeSymbol = nodeSymbolInfo.Symbol;
                                    SymbolKind nodeSymbolKind = nodeSymbol.Kind;

                                    #region if identifier is an variable...
                                    if (nodeSymbolKind == SymbolKind.Local)
                                    {
                                        #region Get the name of the non-static methods
                                        // Can we make this list once, instead of twice
                                        // var myList = type.GetMembers().OfType<IMethodSymbol>()where method.DeclaredAccessibility == Accessibility.Public && method.IsStatic == true;

                                        // Getting a list of distinct static method names, which we use to organise
                                        // our list of intellisense items
                                        foreach (string name in (from method in nodeType.GetMembers().OfType<IMethodSymbol>()
                                                                 where method.DeclaredAccessibility == Accessibility.Public && method.IsStatic == false
                                                                 select method.Name).Distinct())
                                        {
                                            IntellisenseItem item = new IntellisenseItem();
                                            item.Text = name;
                                            //item.ForeColor = Color.Black;
                                            itemList.Add(item);
                                        }
                                        #endregion

                                        #region Get the method info of the non-static methods

                                        // Getting full information about static methods (name, signature, return type)
                                        // Oranise into a list using the list of distinct method names from above.
                                        foreach (IMethodSymbol method in (from method in nodeType.GetMembers().OfType<IMethodSymbol>()
                                                                          where method.DeclaredAccessibility == Accessibility.Public && method.IsStatic == false
                                                                          select method))
                                        {
                                            foreach (IntellisenseItem item in itemList)
                                            {
                                                // item.Tag = method.MethodKind.ToString();   // Does this work. Can it replace the logic below.

                                                if (item.Text == method.Name)
                                                {
                                                    #region initialize items
                                                    switch (method.MethodKind)
                                                    {
                                                        case MethodKind.AnonymousFunction:
                                                            item.Tag = MethodKind.AnonymousFunction.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            break;
                                                        case MethodKind.Constructor:
                                                            item.Tag = MethodKind.Constructor.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            item.MethodInfoList.Add(method);
                                                            break;
                                                        case MethodKind.Conversion:
                                                            item.Tag = MethodKind.Conversion.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            break;
                                                        case MethodKind.DelegateInvoke:
                                                            item.Tag = MethodKind.DelegateInvoke.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            break;
                                                        case MethodKind.Destructor:
                                                            item.Tag = MethodKind.Destructor.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            item.MethodInfoList.Add(method);
                                                            break;
                                                        case MethodKind.EventAdd:
                                                            item.Tag = MethodKind.EventAdd.ToString();
                                                            item.Image = MyTextBox.eventImage;
                                                            break;
                                                        case MethodKind.EventRaise:
                                                            item.Tag = MethodKind.EventRaise.ToString();
                                                            item.Image = MyTextBox.eventImage;
                                                            break;
                                                        case MethodKind.EventRemove:
                                                            item.Tag = MethodKind.EventRemove.ToString();
                                                            item.Image = MyTextBox.eventImage;
                                                            break;
                                                        case MethodKind.ExplicitInterfaceImplementation:
                                                            item.Tag = MethodKind.ExplicitInterfaceImplementation.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            break;
                                                        case MethodKind.UserDefinedOperator:
                                                            item.Tag = MethodKind.UserDefinedOperator.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            break;
                                                        case MethodKind.Ordinary:
                                                            item.Tag = MethodKind.Ordinary.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            item.MethodInfoList.Add(method);
                                                            break;
                                                        case MethodKind.PropertyGet:
                                                            item.Tag = MethodKind.PropertyGet.ToString();
                                                            item.Image = MyTextBox.propertyImage;
                                                            break;
                                                        case MethodKind.PropertySet:
                                                            item.Tag = MethodKind.PropertySet.ToString();
                                                            item.Image = MyTextBox.propertyImage;
                                                            break;
                                                        case MethodKind.ReducedExtension:
                                                            item.Tag = MethodKind.ReducedExtension.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            break;
                                                        case MethodKind.SharedConstructor:
                                                            item.Tag = MethodKind.SharedConstructor.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            break;
                                                        case MethodKind.BuiltinOperator:
                                                            item.Tag = MethodKind.BuiltinOperator.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            break;
                                                        case MethodKind.DeclareMethod:
                                                            item.Tag = MethodKind.DeclareMethod.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            item.MethodInfoList.Add(method);
                                                            break;
                                                        default:
                                                            item.Tag = "Unknown";
                                                            item.Image = MyTextBox.methodImage;
                                                            break;
                                                    }
                                                    #endregion
                                                }
                                            }
                                        }
                                        #endregion

                                    }
                                    #endregion

                                    #region if identifier is a Class, Structure, Enum type
                                    else if (nodeTypeKind == TypeKind.Class || nodeTypeKind == TypeKind.Structure ||
                                    nodeTypeKind == TypeKind.Enum)
                                    {

                                        #region Get the name of the non-static methods
                                        foreach (string name in (from method in nodeType.GetMembers().OfType<IMethodSymbol>()
                                                                 where method.DeclaredAccessibility == Accessibility.Public && method.IsStatic == true
                                                                 select method.Name).Distinct())
                                        {
                                            IntellisenseItem item = new IntellisenseItem();
                                            item.Text = name;
                                            //item.ForeColor = Color.Black;
                                            itemList.Add(item);
                                        }
                                        #endregion

                                        #region Get the method info of the non-static methods
                                        foreach (IMethodSymbol method in (from method in nodeType.GetMembers().OfType<IMethodSymbol>()
                                                                          where method.DeclaredAccessibility == Accessibility.Public && method.IsStatic == true
                                                                          select method))
                                        {
                                            foreach (IntellisenseItem item in itemList)
                                            {
                                                if (item.Text == method.Name)
                                                {
                                                    #region initialize items
                                                    switch (method.MethodKind)
                                                    {
                                                        case MethodKind.AnonymousFunction:
                                                            item.Tag = MethodKind.AnonymousFunction.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            break;
                                                        case MethodKind.Constructor:
                                                            item.Tag = MethodKind.Constructor.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            item.MethodInfoList.Add(method);
                                                            break;
                                                        case MethodKind.Conversion:
                                                            item.Tag = MethodKind.Conversion.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            break;
                                                        case MethodKind.DelegateInvoke:
                                                            item.Tag = MethodKind.DelegateInvoke.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            break;
                                                        case MethodKind.Destructor:
                                                            item.Tag = MethodKind.Destructor.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            item.MethodInfoList.Add(method);
                                                            break;
                                                        case MethodKind.EventAdd:
                                                            item.Tag = MethodKind.EventAdd.ToString();
                                                            item.Image = MyTextBox.eventImage;
                                                            break;
                                                        case MethodKind.EventRaise:
                                                            item.Tag = MethodKind.EventRaise.ToString();
                                                            item.Image = MyTextBox.eventImage;
                                                            break;
                                                        case MethodKind.EventRemove:
                                                            item.Tag = MethodKind.EventRemove.ToString();
                                                            item.Image = MyTextBox.eventImage;
                                                            break;
                                                        case MethodKind.ExplicitInterfaceImplementation:
                                                            item.Tag = MethodKind.ExplicitInterfaceImplementation.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            break;
                                                        case MethodKind.UserDefinedOperator:
                                                            item.Tag = MethodKind.UserDefinedOperator.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            break;
                                                        case MethodKind.Ordinary:
                                                            item.Tag = MethodKind.Ordinary.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            item.MethodInfoList.Add(method);
                                                            break;
                                                        case MethodKind.PropertyGet:
                                                            item.Tag = MethodKind.PropertyGet.ToString();
                                                            item.Image = MyTextBox.propertyImage;
                                                            break;
                                                        case MethodKind.PropertySet:
                                                            item.Tag = MethodKind.PropertySet.ToString();
                                                            item.Image = MyTextBox.propertyImage;
                                                            break;
                                                        case MethodKind.ReducedExtension:
                                                            item.Tag = MethodKind.ReducedExtension.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            break;
                                                        case MethodKind.SharedConstructor:
                                                            item.Tag = MethodKind.SharedConstructor.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            break;
                                                        case MethodKind.BuiltinOperator:
                                                            item.Tag = MethodKind.BuiltinOperator.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            break;
                                                        case MethodKind.DeclareMethod:
                                                            item.Tag = MethodKind.DeclareMethod.ToString();
                                                            item.Image = MyTextBox.methodImage;
                                                            item.MethodInfoList.Add(method);
                                                            break;
                                                        default:
                                                            item.Tag = "Unknown";
                                                            item.Image = MyTextBox.methodImage;
                                                            break;
                                                    }
                                                    #endregion
                                                    break;
                                                }
                                            }
                                        }
                                        #endregion

                                    }
                                    #endregion
                                }
                                #endregion
                            }

                            // If the identifier is a namesapce, not an object
                            else
                            {
                                SymbolInfo symbolInfo = model.GetSymbolInfo(node.Expression);
                                INamespaceSymbol symbol = (INamespaceSymbol)symbolInfo.Symbol;

                                #region Get the subnamespaces
                                foreach (INamespaceSymbol name in symbol.GetNamespaceMembers().Distinct())
                                {
                                    IntellisenseItem item = new IntellisenseItem();
                                    item.Text = name.Name;
                                    item.Tag = "Namespace";
                                    item.Image = MyTextBox.namespaceImage;
                                    //item.ForeColor = Color.Black;
                                    itemList.Add(item);
                                }
                                #endregion

                                #region Get the members of the namespace
                                foreach (INamedTypeSymbol member in symbol.GetMembers().OfType<INamedTypeSymbol>().Distinct())
                                {
                                    IntellisenseItem item = new IntellisenseItem();
                                    item.Tag = member.TypeKind.ToString();
                                    item.Text = member.Name;
                                    switch (member.TypeKind)
                                    {
                                        case TypeKind.Class:
                                            item.Image = MyTextBox.classImage;
                                            break;
                                        case TypeKind.Enum:
                                            item.Image = MyTextBox.classImage;
                                            break;
                                        case TypeKind.Interface:
                                            item.Image = MyTextBox.interfaceImage;
                                            break;
                                        case TypeKind.Structure:
                                            item.Image = MyTextBox.classImage;
                                            break;
                                        default:
                                            item.Image = MyTextBox.classImage;
                                            break;
                                    }
                                    //item.ForeColor = Color.Black;
                                    itemList.Add(item);
                                }
                                #endregion

                            }
                        }
                    }
                }
                #endregion
            }
            return itemList;
        }

       /// <summary>
       /// Get local variables that are in scope at current cursor position
       /// </summary>
       /// <param name="code">Code text (all the text of the current program)</param>
       /// <param name="position">Position of cursor within text (character index)</param>
       /// <returns>List of intellisense items</returns>
       public List<IntellisenseItem> GetLocalVariables(string code, int position)
        {
            // Build syntax tree
            SyntaxTree tree = VisualBasicSyntaxTree.ParseText(code);
            SyntaxNode root = tree.GetRoot();
            
            // Add references
            MetadataReference mscorlib = new MetadataFileReference(typeof(object).Assembly.Location);
            MetadataReference Snap = new MetadataFileReference(typeof(Snap.Color).Assembly.Location);
            MetadataReference NXOpen = new MetadataFileReference(typeof(NXOpen.Point3d).Assembly.Location);
            MetadataReference NXOpen_UF = new MetadataFileReference(typeof(NXOpen.UF.UF).Assembly.Location);
            MetadataReference NXOpen_Utilities = new MetadataFileReference(typeof(NXOpen.Utilities.BaseSession).Assembly.Location);
            MetadataReference NXOpenUI = new MetadataFileReference(typeof(NXOpenUI.FormUtilities).Assembly.Location);
            MetadataReference[] references = { mscorlib, Snap, NXOpen, NXOpen_UF, NXOpen_Utilities, NXOpenUI };

            //Build compilation
            VisualBasicCompilation compilation = VisualBasicCompilation.Create("intellisense").AddReferences(references).AddSyntaxTrees(tree);
            SemanticModel model = compilation.GetSemanticModel(tree);
            bool showLocalVar = false;
            TextSpan span = new TextSpan(position - 1, 1);

            // Check if it is the right occasion to show local variables
            if (root.DescendantNodes(span).Any())
            {
                string typeInfo = root.DescendantNodes(span).Last().GetType().ToString();
                switch (typeInfo)
                {
                    // The following are two occasions where local variables should be showed
                    case "Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax":
                        EqualsValueSyntax equalNode = (EqualsValueSyntax)root.DescendantNodes(span).Last();
                        if (equalNode.Value.ToString().Length == 0)
                        {
                            showLocalVar = true;
                        }
                        break;
                    case "Microsoft.CodeAnalysis.VisualBasic.Syntax.AssignmentStatementSyntax":
                        AssignmentStatementSyntax assignNode = (AssignmentStatementSyntax)root.DescendantNodes(span).Last();
                        if (assignNode.Right.ToString().Length == 0)
                        {
                            showLocalVar = true;
                        }
                        break;
                    default:
                        break;
                }
            }

            // Add accessible variable items to the variable list
            List<IntellisenseItem> variableList = new List<IntellisenseItem>();
            if (showLocalVar)
            {
                // Look up symbols at this position
                ImmutableArray<ISymbol> symbols = model.LookupSymbols(position);
                foreach (ISymbol symbol in symbols)
                {
                    if (symbol.Kind == SymbolKind.Local)
                    {
                        IntellisenseItem item = new IntellisenseItem();
                        item.Text = symbol.ToString();
                        item.Tag = "Local variable";
                        variableList.Add(item);
                    }
                }
            }
            return variableList;
        }

        /// <summary>
        /// Get the last "word" the user has just typed (before the period)
        /// </summary>
        /// <param name="code">Code from beginning of file to cursor position</param>
        /// <returns>The word before the cursor</returns>
        private string GetLastWord(string code)
        {
            int pos = code.Length - 1;

            while (pos >= 1)
            {
                string substr = code.Substring(pos - 1, 1);

                // Search backwards, looking for beginning of the word
                if (substr[0] == ' ' || substr[0] == '\t' || substr[0] == '\n')
                {
                    return code.Substring(pos, code.Length - pos);
                }

                pos--;
            }

            return code;
        }

        #endregion
    }
}
