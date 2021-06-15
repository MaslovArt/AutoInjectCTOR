using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AutoInjectCTOR
{
    [Generator]
    public class InjectCtorSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is InjectCtorReceiver receiver)
            {
				Console.WriteLine(receiver.Candidates.Count);
                foreach (var candidateTypeNode in receiver.Candidates)
                {
                    var candidateTypeSymbol = context.Compilation
                        .GetSemanticModel(candidateTypeNode.SyntaxTree)
                        .GetDeclaredSymbol(candidateTypeNode) as ITypeSymbol;

                    var source = GenerateInjects(candidateTypeSymbol);
                    context.AddSource(
                        $"{candidateTypeSymbol.Name}.InjectCtor.cs",
                        SourceText.From(source, Encoding.UTF8));
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif
            context.RegisterForSyntaxNotifications(() => new InjectCtorReceiver());
        }

        private string GenerateInjects(ITypeSymbol typeSymbol)
        {
            var (parameters, initializations) = BuildCtor(typeSymbol);

            return $@"
                using System;
                namespace {typeSymbol.ContainingNamespace}
                {{
                    public partial class {typeSymbol.Name} 
                    {{
                        public {typeSymbol.Name}({parameters})
                        {{
                            {initializations}
                        }}
                    }}
                }}";
        }

        /// <summary>
        /// Build ctor source
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <returns>(params, initialization)</returns>
        private (string, string) BuildCtor(ITypeSymbol typeSymbol)
        {
            var attrMarkedClass = typeSymbol
                .GetAttributes()
                .Any(t => t.AttributeClass.Name == nameof(InjectCtorAttribute));

            var fieldsSymbols = typeSymbol.GetMembers().OfType<IFieldSymbol>();

            var fields = attrMarkedClass
                ? fieldsSymbols.Where(f =>
                    f.IsReadOnly &&
                    !f.IsStatic &&
                    !f.GetAttributes()
                        .Any(fa => fa.AttributeClass.Name == nameof(IgnoreCtorAttribute)))
                : fieldsSymbols
                    .Where(f => f.GetAttributes()
                        .Any(fa => fa.AttributeClass.Name == nameof(InjectCtorAttribute)));

            var parameters = new StringBuilder();
            var initializations = new StringBuilder();

            foreach (var field in fields)
            {
                parameters.Append($"{field.Type} {field.Name},");
                initializations.AppendLine($"this.{field.Name} = {field.Name};");
            }

            return (parameters.ToString().TrimEnd(','), initializations.ToString());
        }
    }
}
