using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneradorScanner
{
    public class ExpressionTree
    {
        private Dictionary<string, Set> Sets;
        private Stack<Token> Tokens;
        private List<Symbol> SimbolsList;
        private Node raiz;
        public ExpressionTree(Stack<Token> Tokens, Dictionary<string, Set> sets)
        {
            this.Tokens = Tokens;
            this.Sets = sets;
        }
        public void Concatenate()
        {
            //-- test
            List<Token> temp = Tokens.ToList<Token>();
            Tokens = new Stack<Token>();
            foreach (var item in temp)
            {
                Tokens.Push(item);
            }
            //--
            SimbolsList = new List<Symbol>();
            SimbolsList.Add(new Symbol { symbol = "(", isNonTerminal = true });            
            int n = Tokens.Count;
            for (int i = 0; i < n; i++)
            {
                Token TempTok = new Token();
                TempTok = Tokens.Pop();
                SimbolsList.Add(new Symbol { symbol = "(", isNonTerminal = true });
                int contador = 0;
                foreach (Symbol symbol in TempTok.ListSymbols)
                {
                    if (!symbol.isNonTerminal)
                    {
                        if (contador == 0)
                        {
                            SimbolsList.Add(symbol);
                            contador++;
                        }
                        else if (SimbolsList[SimbolsList.Count - 1].isNonTerminal)
                        {
                            SimbolsList.Add(symbol);
                            contador = 0;
                            if (symbol.symbol == "'" && !symbol.isNonTerminal)
                            {
                                contador = 1;
                            }
                        }
                        else if (contador == 1)
                        {
                            SimbolsList.Add(new Symbol { symbol = ".", isNonTerminal = true });
                            SimbolsList.Add(symbol);
                        }
                    }
                    else
                    {
                        if (symbol.symbol == "*" || symbol.symbol == "|" || symbol.symbol == ")")
                        {
                            SimbolsList.Add(symbol);
                        }
                        else
                        {
                            if (contador == 1)
                            {
                                SimbolsList.Add(new Symbol { symbol = ".", isNonTerminal = true });
                                SimbolsList.Add(symbol);

                            }
                        }
                    }
                }
                SimbolsList.Add(new Symbol { symbol = ")", isNonTerminal = true });
                SimbolsList.Add(new Symbol { symbol = "|", isNonTerminal = true });
            }
            SimbolsList.RemoveAt(SimbolsList.Count - 1);//ver si funciona este cambio             
            SimbolsList.Add(new Symbol { symbol = ")", isNonTerminal = true });
            SimbolsList.Add(new Symbol { symbol = ".", isNonTerminal = true });
            SimbolsList.Add(new Symbol { symbol = "#" });
            SimbolsList = Polish(SimbolsList);
            raiz = Tree(SimbolsList);

        }
        //calculadora polaca, recorrido de orden al arbol
        public List<Symbol> Polish(List<Symbol> Symbol)
        {
            //ReplaceSets(ref Symbol);
            List<Symbol> resultado = new List<Symbol>();
            Stack<Symbol> stack = new Stack<Symbol>();
            int n = Symbol.Count;
            for (int i = 0; i < n; ++i)
            {
                Symbol c = new Symbol();
                c = Symbol[i];

                if (!c.isNonTerminal)
                {
                    resultado.Add(c);
                }
                else if (c.symbol == "(")
                {
                    stack.Push(c);
                }
                else if (c.symbol == ")")
                {
                    while (stack.Count > 0 && stack.Peek().symbol != "(")
                    {
                        resultado.Add(stack.Pop());
                    }
                    if (stack.Count > 0 && stack.Peek().symbol != "(")
                    {
                        return null;
                    }
                    else
                    {
                        stack.Pop();
                    }
                }
                else
                {
                    while (stack.Count > 0 && Importance(c.symbol) <= Importance(stack.Peek().symbol))
                    {
                        resultado.Add(stack.Pop());
                    }
                    stack.Push(c);
                }
            }

            while (stack.Count > 0)
            {
                resultado.Add(stack.Pop());
            }
            return resultado;
        }
        private int Importance(string Symbol)
        {
            switch (Symbol)
            {
                case "|":
                    return 1;
                case ".":
                    return 2;
                case "*":
                    return 3;
                case "?":
                    return 3;
            }
            return -1;
        }
        private void ReplaceSets(ref List<Symbol> Symbol)
        {
            List<string> keyList = new List<string>(Sets.Keys);
            foreach (var item in keyList)
            {
                int result = -1;
                result = Symbol.FindIndex(index => index.symbol == item);
                if (result != -1)
                {
                    List<Symbol> setsym = Sets[item].set;
                    List<Symbol> tem = new List<Symbol>();
                    tem.Add(new Symbol { symbol = "(" ,isNonTerminal= true}); 
                    foreach (var item2 in setsym)
                    {
                        tem.Add(item2);
                        tem.Add(new Symbol { symbol = "|", isNonTerminal = true });
                    }
                    tem.RemoveAt(tem.Count-1);
                    tem.Add(new Symbol { symbol = ")", isNonTerminal = true });                    
                    Symbol.InsertRange(result + 1, tem);
                    Symbol.RemoveAt(result);
                }
            }
        }
        public Node Tree (List<Symbol> Symbol)
        {
            Stack<Node> pila = new Stack<Node>();
            Node nodoTemp;
            for (int i = 0; i < Symbol.Count; i++)
            {
                if (!Symbol[i].isNonTerminal)
                {
                    nodoTemp = new Node();
                    nodoTemp.Simbolo = Symbol[i];
                    pila.Push(nodoTemp);
                }
                else if (Symbol[i].symbol == "|" || Symbol[i].symbol == ".")
                {
                   nodoTemp = new Node();
                   nodoTemp.Simbolo = Symbol[i];
                   Node nodoTemp1 = pila.Pop();
                   Node nodoTemp2 = pila.Pop();
                   nodoTemp.Izquierdo = nodoTemp2;
                   nodoTemp.Derecho = nodoTemp1;
                   nodoTemp.isFather = true;
                   pila.Push(nodoTemp);                    

                }
                else if (Symbol[i].symbol == "*" || Symbol[i].symbol == "+" || Symbol[i].symbol == "?")
                {
                    nodoTemp = new Node();
                    nodoTemp.Simbolo = Symbol[i];
                    Node nodoTemp1 = pila.Pop();
                    nodoTemp.Izquierdo = nodoTemp1;
                    nodoTemp.isFather = true;
                    pila.Push(nodoTemp);
                }
            }

            return pila.Pop();
        }
        public Node getRaiz()
        {
            return raiz;
        }
        public int NodeNum = 0;
        public void SetNumber(Node raiz)
        {
            if (raiz != null)
            {
                SetNumber(raiz.Izquierdo);
                SetNumber(raiz.Derecho);
                if (!raiz.isFather)
                {
                    raiz.Num = 1+NodeNum++;
                    raiz.Last.Add(NodeNum);
                    raiz.First.Add(NodeNum);
                }
            }
        }
        private List<Node> ListNodes = new List<Node>();
        public List<Node> ListNode(Node sub)
        {
            if (sub != null)
            {
                ListNode(sub.Izquierdo);
                ListNode(sub.Derecho);
                if (sub.Num != 0)
                {
                    ListNodes.Add(sub);
                }
            }
            return ListNodes;
        }
        private List<Node> total = new List<Node>();
        public List<Node> ListInter(Node sub)
        {
            if (sub != null)
            {
                ListInter(sub.Izquierdo);
                ListInter(sub.Derecho);
                total.Add(sub);
                
            }
            return total;
        }
        public Node Search(int i,Node SubRoot)
        {
            if (SubRoot!=null)
            {
                if (SubRoot.Num==i)
                {
                    return SubRoot;                    
                }
                Search(i,SubRoot.Izquierdo);
                Search(i, SubRoot.Derecho);
            }
            return null;
        }

        public int ContNodes()
        {
            return NodeNum;
        }
        public void Nullability(Node root)
        {
            if (root == null)
            {
                return;
            }
            Nullability(root.Izquierdo);
            Nullability(root.Derecho);
            if (root.Simbolo.symbol == "*")
            {
                root.Nullable = true;

            }
            else if (root.Simbolo.symbol == "|")
            {
                if (root.Izquierdo.Nullable == true || root.Derecho.Nullable == true)
                {
                    root.Nullable = true;
                }
            }
            else if (root.Simbolo.symbol == ".")
            {
                if (root.Izquierdo.Nullable == true && root.Derecho.Nullable == true)
                {
                    root.Nullable = true;
                }
            }
        }
        public void AddFirst(Node raiz)
        {
            if (raiz == null)
            {
                return;
            }

            AddFirst(raiz.Izquierdo);
            AddFirst(raiz.Derecho);

            if (raiz.Simbolo.symbol == ".")
            {
                if (raiz.Izquierdo.Nullable == true)
                {
                    if (!(raiz.Izquierdo.Simbolo.isNonTerminal) && !(raiz.Derecho.Simbolo.isNonTerminal))
                    {
                        raiz.First.AddRange(raiz.Izquierdo.First);
                        raiz.First.AddRange(raiz.Derecho.First);
                    }
                    else
                    {
                        raiz.First.AddRange(raiz.Izquierdo.First);
                        raiz.First.AddRange(raiz.Derecho.First);
                    }
                }
                else
                {
                    raiz.First.AddRange(raiz.Izquierdo.First);
                }
            }
            else if (raiz.Simbolo.symbol == "*" && raiz.Simbolo.isNonTerminal)
            {
                raiz.First.AddRange(raiz.Izquierdo.First);
            }
            else if (raiz.Simbolo.symbol == "|")
            {
                if (!(raiz.Izquierdo.Simbolo.isNonTerminal) && !(raiz.Derecho.Simbolo.isNonTerminal))
                {
                    raiz.First.AddRange(raiz.Izquierdo.First);
                    raiz.First.AddRange(raiz.Derecho.First);
                }
                else
                {
                    raiz.First.AddRange(raiz.Izquierdo.First);
                    raiz.First.AddRange(raiz.Derecho.First);
                }
            }
        }
        public void AddLast(Node raiz)
        {
            if (raiz == null)
            {
                return;
            }

            AddLast(raiz.Izquierdo);
            AddLast(raiz.Derecho);

            if (raiz.Simbolo.symbol == ".")
            {
                if (raiz.Derecho.Nullable == true)
                {
                    if (!(raiz.Izquierdo.Simbolo.isNonTerminal) && !(raiz.Derecho.Simbolo.isNonTerminal))
                    {
                        raiz.Last.AddRange(raiz.Izquierdo.Last);
                        raiz.Last.AddRange(raiz.Derecho.Last);
                    }
                    else
                    {
                        raiz.Last.AddRange(raiz.Izquierdo.Last);
                        raiz.Last.AddRange(raiz.Derecho.Last);
                    }
                }
                else
                {
                    raiz.Last.AddRange(raiz.Derecho.Last);
                }
            }
            else if (raiz.Simbolo.symbol == "*" && raiz.Simbolo.isNonTerminal)
            {
                raiz.Last.AddRange(raiz.Izquierdo.Last);
            }
            else if (raiz.Simbolo.symbol == "|")
            {
                try
                {
                    if (!(raiz.Izquierdo.Simbolo.isNonTerminal) && !(raiz.Derecho.Simbolo.isNonTerminal))
                    {
                        raiz.Last.AddRange(raiz.Izquierdo.Last);
                        raiz.Last.AddRange(raiz.Derecho.Last);
                    }
                    else
                    {
                        raiz.Last.AddRange(raiz.Izquierdo.Last);
                        raiz.Last.AddRange(raiz.Derecho.Last);
                    }
                }
                catch
                {

                }

            }
        }
    }
}
