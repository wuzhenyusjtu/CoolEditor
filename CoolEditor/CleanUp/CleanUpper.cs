using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.VisualBasic;

namespace CoolEditor
{
    public class CleanUpper
    {
        public CleanUpper() { }

        /// <summary>
        /// Applies clean-up operations to a selected set of VB code
        /// </summary>
        /// <param name="code">The code, before cleaning</param>
        /// <returns>The code after cleaning</returns>
        public string ExplicitCleaning(string code)
        {
            // Create compilation
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();
            SyntaxTree tree = VisualBasicSyntaxTree.ParseText(code);

            // Add syntaxtree and add references
            SyntaxTree[] sourceTrees = { tree };

            MetadataReference mscorlib = new MetadataFileReference(typeof(object).Assembly.Location);
            MetadataReference NXOpen = new MetadataFileReference(typeof(NXOpen.Point3d).Assembly.Location);
            MetadataReference NXOpen_UF = new MetadataFileReference(typeof(NXOpen.UF.UF).Assembly.Location);
            MetadataReference NXOpen_Utilities = new MetadataFileReference(typeof(NXOpen.Utilities.BaseSession).Assembly.Location);
            MetadataReference NXOpenUI = new MetadataFileReference(typeof(NXOpenUI.FormUtilities).Assembly.Location);
            MetadataReference Snap = new MetadataFileReference(typeof(Snap.Color).Assembly.Location);
            MetadataReference[] references = { mscorlib, Snap, NXOpen, NXOpen_UF, NXOpen_Utilities, NXOpenUI };

            // Create a compilation
            VisualBasicCompilation compilation = VisualBasicCompilation.Create("CleaningUp").AddSyntaxTrees(sourceTrees).AddReferences(references);

            // Clean up
            compilation = CleanUpDocument(compilation);

            // Get root node and convert back to textual form
            SyntaxNode cleanedRoot = compilation.SyntaxTrees[0].GetRoot();
            return cleanedRoot.ToFullString();
        }

        /// <summary>
        /// Do all the hard work of cleaning up the code
        /// </summary>
        /// <param name="compilation">Compilation before cleanup</param>
        /// <returns>Compilation after cleanup</returns>
        private VisualBasicCompilation CleanUpDocument(VisualBasicCompilation compilation)
        {
            // Create semantic model
            SyntaxTree sourceTree = compilation.SyntaxTrees[0];
            SemanticModel model = compilation.GetSemanticModel(sourceTree);

            // Create a new CodeFixRewriter
            CodeFixRewriter fixRewriter = new CodeFixRewriter(sourceTree);

            // Start "visiting" (walking through tree nodes) at the root
            // Depending on the node type, some processing function gets called in each visit.
            // See "visiting" functions in CodeFixRewriter class
            // The "newSource" code is only partially cleaned up
            SyntaxNode newSource = fixRewriter.Visit(sourceTree.GetRoot());

            // Build a new syntax tree
            SyntaxTree tree = VisualBasicSyntaxTree.ParseText(newSource.ToFullString());

            // Replace the syntax tree in the compilation by the new one
            compilation = compilation.ReplaceSyntaxTree(compilation.SyntaxTrees[0], tree);

            //remove nodes with specified pattern
            newSource = fixRewriter.RemoveNodes(compilation);

            //update compilation
            tree = VisualBasicSyntaxTree.ParseText(newSource.ToFullString());
            compilation = compilation.ReplaceSyntaxTree(compilation.SyntaxTrees[0], tree);
            model = compilation.GetSemanticModel(compilation.SyntaxTrees[0]);

            //rewrite syntax tree
            CodeOptimizeRewriter optRewriter = new CodeOptimizeRewriter(compilation.SyntaxTrees[0]);

            #region remove unused objects
            newSource = compilation.SyntaxTrees[0].GetRoot();
            while (true)
            {
                newSource = optRewriter.Visit(newSource);
                //remove nodes with specified pattern
                if (optRewriter.SList.Count != 0)
                {
                    newSource = optRewriter.RemoveNodes(compilation);
                    optRewriter.SList.Clear();
                    //update compilation
                    tree = VisualBasicSyntaxTree.ParseText(newSource.ToFullString());
                    compilation = compilation.ReplaceSyntaxTree(compilation.SyntaxTrees[0], tree);
                }
                else { break; }
            }
            #endregion
            return compilation;
        }
    }
}
