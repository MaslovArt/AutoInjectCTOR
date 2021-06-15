using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace AutoInjectCTOR
{
    internal class InjectCtorReceiver : ISyntaxReceiver
    {
        public List<TypeDeclarationSyntax> Candidates { get; } = new List<TypeDeclarationSyntax>();
        private string _injectAttr = nameof(InjectCtorAttribute).Replace("Attribute", "");

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is TypeDeclarationSyntax typeDeclarationSyntax)
            {
                var attrs = typeDeclarationSyntax.AttributeLists.SelectMany(a => a.Attributes);
                var containsInjectAttr = attrs.Any(a => a.Name.ToString() == _injectAttr);

                if (containsInjectAttr) Candidates.Add(typeDeclarationSyntax);
            }
        }
    }
}
