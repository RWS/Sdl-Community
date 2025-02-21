using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.InvoiceAndQuotes.OpenXML
{
    public class TokensProvider
    {
        public List<Token> Tokens { get; set; }

        public TokensProvider()
        {
        }

        public virtual void GenerateTokens()
        {
            
        }
        public bool HasToken(string tokenText)
        {
            return Tokens.Any(token => token.Text.ToUpperInvariant() == tokenText.ToUpperInvariant());
        }

        public IEnumerable<Token> GetAllTokensFromString(string text)
        {
            return Tokens.Where(token => text.ToUpperInvariant().IndexOf(token.Text.ToUpperInvariant(), System.StringComparison.Ordinal) != -1);
        }

        public String ReplaceTokensInString(string text, IEnumerable<Token> tokens)
        {
            foreach (var token in tokens)
            {
                text = text.Replace(token.Text.ToUpperInvariant(), token.Value.ToString());
            }
            return text;
        }

        public object GetTokenValue(string tokenText)
        {
            Token token = Tokens.FirstOrDefault(tk => tk.Text.ToUpperInvariant() == tokenText.ToUpperInvariant());
            return token == null ? null : token.Value;
        }

        public void UpdateTokenValue(string tokenText, object value)
        {
            if (Tokens == null)
                Tokens = new List<Token>();
            Token token = Tokens.FirstOrDefault(tk => tk.Text.ToUpperInvariant() == tokenText.ToUpperInvariant());
            if (token != null)
                token.Value = value;
            else 
                Tokens.Add(new Token(tokenText, value));
        }
    }
}
