using System;
using System.Collections.Generic;
using System.Text;
using LSharp.Tokens;

namespace LSharp.Scanner
{
    public class Scanner
    {
        private readonly string source;
        private readonly List<Token> tokens = new();

        public Scanner(string source)
        {
            this.source = source;
        }

    }
}
