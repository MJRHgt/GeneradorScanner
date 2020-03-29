using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneradorScanner
{
    public class Set
    {        
        public List<Symbol> set = new List<Symbol>();
    }
    public class Symbol
    {
        public string symbol { get; set; }
        public bool isNonTerminal { get; set; }
        public Symbol()
        {
            isNonTerminal = false;
        }        
    }
    public class transitions
    {
        public string symbol { get; set; }
        public char destiny { get; set; }
        public transitions()
        {
            symbol = default;
            destiny = default;
        }
    }
    public class Elemen
    {
        public string symbol { get; set; }
        public List<int> numbers;
        public Elemen()
        {
            numbers = new List<int>();
        }
    }
    public class Token
    {
        public List<Symbol> ListSymbols { get; set; }
        public int Number { get; set; }
        public Token()
        {
            ListSymbols = new List<Symbol>();
        }        
    }
    public class Node
    {
        public bool isFather { get; set; }
        public int Num { get; set; }
        public Node Izquierdo { get; set; }
        public Node Derecho { get; set; }
        public Symbol Simbolo { get; set; }
        public List<int> First { get; set; }
        public List<int> Last { get; set; }
        public bool Nullable { get; set; }
        public Node()
        {
            First = new List<int>();
            Last = new List<int>();
            Nullable = false;
            isFather = false;
            Num = 0;
        }        
    }
    public class Entry
    {
        public object Key;
        public object Value;
        public Entry()
        {
        }

        public Entry(object key, object value)
        {
            Key = key;
            Value = value;
        }
    }
}
