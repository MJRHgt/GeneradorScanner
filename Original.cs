using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using GeneradorScanner;
namespace GeneradorScanner
{
    class Program
    {
        static Dictionary<string, Set> sets;
        static Stack<Token> Tokens;
        static Dictionary<string, int> Actions;
        static Dictionary<string, int> Errors;
        static Dictionary<char, List<Transitions>> transition;
        static Dictionary<char, List<int>> groups;
        static Dictionary<string, int> words;
        [STAThread]
        static void Main(string[] args)
        {
            FileReader myReader = new FileReader();
            Rebuild();
            int option;
            do
            {
                Console.WriteLine("1. Cargar un archivo");
                Console.WriteLine("2. Mostrar Resultados");
                Console.WriteLine("3. Salir");
                bool result = int.TryParse(Console.ReadLine(), out option);
                if (result)
                {
                    switch (option)
                    {
                        case 1:
                            myReader.SelectFile();
                            break;
                        case 2:
                            OpenFile(myReader.FilePath);
                            break;
                        case 3:
                            Environment.Exit(0);
                            break;
                        default:
                            break;
                    }
                }
                Console.Clear();
            } while (option != 3);
        }
        static void OpenFile(string path)
        {
            if (File.Exists(path))
            {
                SetWords(path);
                Classify();
                Console.Clear();
                Console.WriteLine("Resultados:");
                foreach (var item in words)
                {
                    Console.WriteLine(item.Key + "\t-->\t" + item.Value);
                }
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("path does not exist");
            }
        }
        static void Rebuild()
        {
            sets = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Set>>(File.ReadAllText(@"C:\Users\DISTELSA\Desktop\DataScanner\JsonSets.txt"));
            Tokens = JsonConvert.DeserializeObject<Stack<Token>>(File.ReadAllText(@"C:\Users\DISTELSA\Desktop\DataScanner\JsonTokens.txt"));
            Actions = JsonConvert.DeserializeObject<Dictionary<string, int>>(File.ReadAllText(@"C:\Users\DISTELSA\Desktop\DataScanner\JsonActions.txt"));
            Errors = JsonConvert.DeserializeObject<Dictionary<string, int>>(File.ReadAllText(@"C:\Users\DISTELSA\Desktop\DataScanner\JsonSetErrors.txt"));
            transition = JsonConvert.DeserializeObject<Dictionary<char, List<Transitions>>>(File.ReadAllText(@"C:\Users\DISTELSA\Desktop\DataScanner\JsonSetTransition.txt"));
            groups = JsonConvert.DeserializeObject<Dictionary<char, List<int>>>(File.ReadAllText(@"C:\Users\DISTELSA\Desktop\DataScanner\JsonSetGroups.txt"));
        }
        static void SetWords(string path)
        {
            string file = File.ReadAllText(path);
            words = new Dictionary<string, int>();
            char[] spliters = { ' ', '\t' };
            string[] phrases = file.Split(spliters, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in phrases)
            {
                words.Add(item, 0);
            }
        }
        static void Classify()
        {
            //preparacion                       
            int last;
            List<char> aceptacion = new List<char>();
            List<int> temp = new List<int>();
            foreach (var nums in groups.Values)
            {
                foreach (var num in nums)
                {
                    if (temp.Contains(num) == false)
                    {
                        temp.Add(num);
                    }
                }
            }
            temp.Sort();
            last = temp[temp.Count - 1];
            foreach (var state in groups)
            {
                if (state.Value.Contains(last) == true)
                {
                    aceptacion.Add(state.Key);
                }
            }
            // clasificando            
            Dictionary<string, int> wordsTemp = new Dictionary<string, int>(words);
            foreach (var word in wordsTemp)
            {
                if (Actions.ContainsKey(word.Key))
                {
                    words[word.Key] = Actions[word.Key];
                }
                else
                {
                    bool modificado = false;
                    char actual = 'A';
                    List<string> recorido = new List<string>();
                    bool entro = false;
                    foreach (var wors in word.Key)
                    {
                        Symbol resul = null;
                        modificado = false;
                        foreach (var state in transition[actual])
                        {
                            if (sets.ContainsKey(state.symbol) && state.destiny != 45 && state.symbol.Length > 1 && !state.symbol.Equals(words))//entonces estoy validando dentro de un set
                            {
                                Set tem = sets[state.symbol];
                                resul = tem.set.Find(x => x.symbol == Convert.ToString(wors));
                                if (resul != null)
                                {
                                    actual = state.destiny;
                                    recorido.Add(state.symbol);
                                    modificado = true;
                                    entro = true;
                                    break;
                                }
                            }
                            if (state.symbol.Equals(Convert.ToString(wors)) && state.destiny != 45 && entro == false)
                            {
                                actual = state.destiny;
                                recorido.Add(state.symbol);
                                modificado = true;
                                break;
                            }
                        }
                        if (modificado == false)
                        {
                            words[word.Key] = Errors["ERROR"];
                            break;
                        }
                    }
                    if (aceptacion.Contains(actual) && words[word.Key] != Errors["ERROR"])//validar estado de aceptacion 
                    {
                        List<Symbol> temporal = new List<Symbol>();
                        foreach (var item in recorido)
                        {
                            temporal.Add(new Symbol { symbol = item, isNonTerminal = false });
                        }
                        foreach (var item in Tokens)
                        {
                            List<Symbol> t1 = new List<Symbol>();
                            foreach (var item2 in temporal)
                            {
                                List<Symbol> t2 = item.ListSymbols.FindAll(x => x.symbol.Equals(item2.symbol) && (x.isNonTerminal == item2.isNonTerminal));
                                if (t2.Count > 0)
                                {
                                    t1.Add(t2[0]);
                                }
                            }
                            //revisar si son iguales
                            if (temporal.Count == t1.Count)
                            {
                                bool equal = true;
                                for (int i = 0; i < temporal.Count; i++)
                                {
                                    if (!temporal[i].symbol.Equals(t1[i].symbol))
                                    {
                                        equal = false;
                                    }
                                }
                                if (equal == true)
                                {
                                    words[word.Key] = item.Number;
                                    break;
                                }
                                else
                                {
                                    words[word.Key] = Errors["ERROR"];
                                    break;
                                }
                            }
                        }
                    }
                    //else
                    //{
                    //    words[word.Key] = Errors["ERROR"];
                    //    break;
                    //}
                }
            }
        }
    }
    public class FileReader
    {
        public string FilePath { get; set; }

        public void SelectFile()
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Multiselect = false;
            OFD.Title = "Select file to process";
            //OFD.Filter = "txt files (.txt)|.txt";
            OFD.ShowDialog();
            FilePath = OFD.FileName;
        }
    }
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
    public class Token
    {
        public List<Symbol> ListSymbols { get; set; }
        public int Number { get; set; }
        public Token()
        {
            ListSymbols = new List<Symbol>();
        }
    }
    public class Transitions
    {
        public string symbol { get; set; }
        public char destiny { get; set; }

    }
}