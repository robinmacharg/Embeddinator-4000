using System.Collections.Generic;
using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;

namespace MonoEmbeddinator4000.Generators
{
    public class JavaSources : CppSharp.Generators.CSharp.CSharpSources
    {
        public JavaSources(BindingContext context, TranslationUnit unit)
            : base(context, new List<TranslationUnit> { unit }, null, null)
        {
        }

        public override string FileExtension => "java";

        public string AssemblyId => CGenerator.AssemblyId(TranslationUnit);

        public override void Process()
        {
            //GenerateFilePreamble();

            PushBlock();
            PopBlock(NewLineKind.BeforeNextBlock);

            TranslationUnit.Visit(this);
        }

        public override bool VisitEnumDecl(Enumeration @enum)
        {
            return true;
        }

        public override bool VisitClassDecl(Class @class)
        {
            VisitDeclContext(@class);

            return true;
        }

        public override bool VisitMethodDecl(Method method)
        {
            return true;
        }

        public override bool VisitTypedefDecl(TypedefDecl typedef)
        {
            return true;
        }
    }
}