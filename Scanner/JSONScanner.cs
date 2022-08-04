using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSharp.Tokens;

namespace LSharp.Scanner
{
    class JSONScanner
    {
        private readonly string source;
        private readonly List<Token> tokens;
        private int start;
        private int current;
        private int line = 1;

        public JSONScanner(string source)
        {
            this.source = source;
        }

        private void scanToken()
        {
            var c = advance();
            switch (c)
            {
                case '"': addToken(TokenType.STRING); break; 
            }
        }

        private void addToken(TokenType token) { }

        public char advance()
        {
            return source[current++];
        }
    }
}
