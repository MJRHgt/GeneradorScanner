using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Collections;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Globalization;

namespace GeneradorScanner
{
    public class Procesos
    {
        /// <summary>
        /// variales privadas
        /// </summary>
        //private static Procesos _singleton;

        /// <summary>
        /// variales publicas
        /// </summary>
        public Dictionary<string, Set> sets;
        public Stack<Token> Tokens;
        public Dictionary<string, int> Actions;
        public Dictionary<string, int> Errors;
        public Dictionary<char, List<transitions>> transition ;//tabla del automata  
        public Dictionary<char, List<int>> groups ;//tabla de grupos 
        public StreamReader reader;
        public ExpressionTree Automat;
        public Dictionary<int, List<int>> Follow;
        public string SetTokenTXT = "";
        public Procesos()
        {
            sets = new Dictionary<string, Set>();
            Tokens = new Stack<Token>();
            Actions = new Dictionary<string, int>();
            Errors = new Dictionary<string, int>();
            transition = new Dictionary<char, List<transitions>>();
            groups = new Dictionary<char, List<int>>();         
        }
        /// <summary>
        /// constructores
        /// </summary>
        // private Procesos() {}
        /// <summary>
        /// singleton
        /// </summary>
        /// <returns></returns>
        //public static Procesos CreateInstance()
        //{
        //    if (_singleton == null)
        //    {
        //          _singleton = new Procesos();
        //    }
        //    return _singleton;
        //}
        /// <summary>
        /// empieza la lectura del archivo y construye un diccionario de sets 
        /// </summary>
        /// <param name="path"></param>
        public void ReadFile(string path)
        {
            reader = new StreamReader(path);
            string line="";
            try
            {
                line = reader.ReadLine();
                while (line!=null && line !=""||reader.Peek()>-1)
                {
                    if (line.Contains("SETS"))
                    {
                        line = reader.ReadLine();
                        do
                        {
                            Set settemp = new Set();
                            string pattern = @"[\s]+=[\s]+";
                            Regex rg = new Regex(pattern);
                            string[] parts = rg.Split(line);
                            string[] temp = parts[1].Split('+');
                            foreach (var item in temp)
                            {                                                               
                                if (parts[1].Contains("CHR")==true)
                                {
                                    char[] separator1 = { '.', '.' };
                                    string[] part1 = item.Split(separator1, StringSplitOptions.RemoveEmptyEntries);
                                    List<string> part2 = new List<string>();
                                    foreach (var item2 in part1)
                                    {
                                        char[] separator2 = { '(', ')' };
                                        part2.AddRange(item2.Split(separator2, StringSplitOptions.RemoveEmptyEntries).ToList<string>());
                                    }
                                    for (int i = Convert.ToInt32(part2[1]); i < Convert.ToInt32(part2[3]); i++)
                                    {
                                        settemp.set.Add(new Symbol { symbol= (char)i+"",isNonTerminal=false });
                                    }
                                }
                                else
                                {
                                    string[] tempo = item.Split('.', '.');
                                    if (tempo.Length != 1)
                                    {
                                        for (char letter = Convert.ToChar(tempo[0].Substring(1, 1)); letter <= Convert.ToChar(tempo[2].Substring(1, 1)); letter++)
                                        {
                                            settemp.set.Add(new Symbol { symbol= letter+"" ,isNonTerminal= false});
                                        }
                                    }
                                    else
                                    {
                                        settemp.set.Add(new Symbol { symbol= tempo[0].Substring(1, 1) , isNonTerminal= false});
                                    }
                                }
                            }
                            sets.Add(parts[0].Trim(), settemp);
                            line = reader.ReadLine();
                        } while (!(line.Contains("TOKENS")));
                    }
                    else if (line.Contains("TOKENS"))
                    {
                        ReadTokens(ref line);
                    } else if (line.Contains("ACTIONS"))
                    {
                        ReadActions(ref line);
                    }   else if (line.Contains("ERROR"))
                    {
                        ReadError(ref line);
                    }                                  
                }
                reader.Close();
                //SetTokenTXT = texttoshow();
                CreatAutomat();                
            }
            catch 
            {
                MessageBox.Show(line, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public string texttoshow()
        {
            foreach (var item in sets)
            {
                SetTokenTXT += item.Key.ToString() + ": ";
                List<Symbol> keyList = new List<Symbol>(item.Value.set);
                foreach (var item2 in keyList)
                {
                    SetTokenTXT += item2.symbol + ", ";
                }
                SetTokenTXT += Environment.NewLine;
                SetTokenTXT += Environment.NewLine;
            }
            SetTokenTXT += Environment.NewLine;
            SetTokenTXT += "Tokens";
            SetTokenTXT += Environment.NewLine;
            foreach (var item in Tokens)
            {
                SetTokenTXT += item.Number + ": ";
                List<Symbol> keyList = new List<Symbol>(item.ListSymbols);
                foreach (var item2 in keyList)
                {
                    SetTokenTXT += item2.symbol + ", ";
                }
                SetTokenTXT += Environment.NewLine;
                SetTokenTXT += Environment.NewLine;
            }
            return SetTokenTXT;
        } 
        /// <summary>
        /// lee cada token y lo clasifica 
        /// </summary>
        public void ReadTokens(ref string line)
        {
            line = reader.ReadLine();
            while (line != null && line.ToUpper() != "ACTIONS" && line != "")
            {
                line = line.TrimEnd().TrimStart();               
                if (line.ToUpper().Contains("TOKEN"))
                {
                    if (line.Contains("="))
                    {                        
                        char[] charLinea = line.ToCharArray();//linea a arreglo de chars
                        line = line.Remove(0, ReturnEqual(charLinea) + 1);//elimina del principio hasta despues del igual
                        int num = ReturnNum(charLinea);// retorna el numero de token
                        line = line.TrimEnd().TrimStart();//quita espacion en blanco
                        charLinea = line.ToCharArray();
                        int CountQuotes = 0;
                        string Symbol = "";
                        Token newToken = new Token();
                        newToken.Number = num;
                        Symbol SymbolToken;
                        foreach (char c in charLinea)
                        {
                            if (c == '\'')
                            {
                                if (CountQuotes == 0)
                                {
                                    CountQuotes++;
                                }
                                else
                                {
                                    SymbolToken = new Symbol();
                                    if (Symbol == "")
                                    {
                                        Symbol = "\'";
                                    }
                                    SymbolToken.symbol = Symbol;
                                    newToken.ListSymbols.Add(SymbolToken);
                                    CountQuotes = 0;
                                    Symbol = "";
                                }
                            }
                            else if (c == '*' || c == '|' || c == '(' || c == ')' || c == '?')
                            {
                                if (CountQuotes == 1)
                                {
                                    Symbol += c;
                                }
                                else
                                {
                                    SymbolToken = new Symbol();
                                    SymbolToken.symbol = c.ToString();
                                    SymbolToken.isNonTerminal = true;
                                    newToken.ListSymbols.Add(SymbolToken);
                                }
                            }
                            else if (c == ' ')
                            {
                                if (CountQuotes == 1)
                                {
                                    Symbol += c;
                                }
                                else if (Symbol.Length > 0)
                                {
                                    SymbolToken = new Symbol();
                                    SymbolToken.symbol = Symbol;
                                    newToken.ListSymbols.Add(SymbolToken);
                                    Symbol = "";
                                }
                            }
                            else
                            {
                                Symbol += c;
                            }
                        }
                        Tokens.Push(newToken);
                    }
                }
                line = reader.ReadLine();
             }
        }
        /// <summary>
        /// retorna el numero de token 
        /// </summary>
        /// <param name="linea"></param>
        /// <returns></returns>
        private int ReturnNum(char[] line)
        {
            string pattern = @"[0123456789]{1,}[\s]*=";
            Regex rg = new Regex(pattern);
            string lines = new string(line);
            System.Text.RegularExpressions.Match num = rg.Match(lines);
            string text = num.ToString();
            string[] toNum = text.Split('=');
            int number=0;
            if (toNum.Length > 0)
            {
                number = Convert.ToInt32(toNum[0]);
            }
            return number;
        }
        /// <summary>
        /// returna la cantidad de caracteres desde el inico hasta el signo =
        /// </summary>
        /// <param name="linea"></param>
        /// <returns></returns>
        private int ReturnEqual(char[] linea)
        {            
            List<char> tempo = linea.ToList<char>();
            return tempo.IndexOf('='); ;
        }        
        /// <summary>
        /// lee cada accion y la agraga al diccionario de actions 
        /// </summary>
        /// <param name="line"></param>
        public void ReadActions(ref string line)
        {
            try
            {
                line = reader.ReadLine();
                if (line.Contains("RESERVADAS()"))
                {
                    line = reader.ReadLine();
                    if (!line.Contains("{"))
                    {
                        Exception e = new Exception("Error en la linea: " + Environment.NewLine + line);
                        throw e;
                    }
                    else
                    {
                        line = reader.ReadLine();
                        do
                        {                            
                            line = line.TrimEnd().TrimStart();
                            string pattern = @"[\s]+=[\s]+";
                            Regex rg = new Regex(pattern);
                            string[] parts = rg.Split(line);
                            parts[1] = parts[1].Substring(1, parts[1].Length - 2);                           
                            Actions.Add(parts[1], Convert.ToInt32(parts[0]));
                            line = reader.ReadLine();
                        } while (!line.Contains("}"));
                        do
                        {
                            line = reader.ReadLine();
                        } while (!line.Contains("ERROR") || reader.Peek()>-1);
                    }
                }
                else
                {
                    Exception e = new Exception("Error en la linea: "+Environment.NewLine+line);
                    throw e;
                }
            }
            catch
            {
                MessageBox.Show(line, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        /// <summary>
        /// Lee el error y lo agrega a un diccionario llamado Errors
        /// </summary>
        /// <param name="line"></param>
        public void ReadError(ref string line)
        {
            do
            {
                line = line.TrimEnd().TrimStart();
                string pattern = @"[\s]+=[\s]+";
                Regex rg = new Regex(pattern);
                string[] parts = rg.Split(line);
                Errors.Add(parts[0], Convert.ToInt32(parts[1]));
                line = reader.ReadLine();
            } while (line != null);
        }
        /// <summary>
        /// metodo principal del automata finito determinista
        /// </summary>
        public void CreatAutomat() 
        {
            Automat = new ExpressionTree(Tokens,sets);
            Automat.Concatenate();            
            Automat.SetNumber(Automat.getRaiz());            
            Automat.Nullability(Automat.getRaiz());            
            Automat.AddFirst(Automat.getRaiz());
            Automat.AddLast(Automat.getRaiz());
            Follow = new Dictionary<int, List<int>>();
            for (int i = 1; i <= Automat.ContNodes(); i++)
            {
                Follow.Add(i, new List<int>());
            }
            Follows(Automat.getRaiz());
            CreateTransitions();
            save();
        }
        private void save()
        {
            DirectoryInfo directory = new DirectoryInfo(@"C:\Users\DISTELSA\Desktop\DataScanner\");
            foreach (var file in directory.GetFiles())
            {
                file.Delete();
            }
            string s = JsonConvert.SerializeObject(sets);//valida el como se hizo
            File.WriteAllText(@"C:\Users\DISTELSA\Desktop\DataScanner\JsonSets.txt", JsonConvert.SerializeObject(sets));
            File.WriteAllText(@"C:\Users\DISTELSA\Desktop\DataScanner\JsonTokens.txt", JsonConvert.SerializeObject(Tokens));
            File.WriteAllText(@"C:\Users\DISTELSA\Desktop\DataScanner\JsonActions.txt", JsonConvert.SerializeObject(Actions));
            File.WriteAllText(@"C:\Users\DISTELSA\Desktop\DataScanner\JsonSetErrors.txt", JsonConvert.SerializeObject(Errors));
            File.WriteAllText(@"C:\Users\DISTELSA\Desktop\DataScanner\JsonSetTransition.txt", JsonConvert.SerializeObject(transition));
            File.WriteAllText(@"C:\Users\DISTELSA\Desktop\DataScanner\JsonSetGroups.txt", JsonConvert.SerializeObject(groups));
        }
        /// <summary>
        /// crea el la tabla de tansiciones
        /// </summary>
        public void CreateTransitions()
        {                        
            char Character = 'A';//caracter del ultimo grupo agregado                        
            List<Node> nodes = Automat.ListNode(Automat.getRaiz());//lista de hojas
            bool flag = false;
            groups.Add(Character, Automat.getRaiz().First);
            List<Elemen> elements = new List<Elemen>();//posible elementos de la table
            foreach (var item in nodes)
            {
                bool exist = false;
                foreach (var item2 in elements)
                {
                    if (item.Simbolo.symbol.Equals(item2.symbol))
                    {
                        item2.numbers.Add(item.Num);
                        exist = true;
                        break;
                    }
                }
                if (exist==false)
                {
                    Elemen temp = new Elemen();
                    temp.symbol = item.Simbolo.symbol;
                    temp.numbers.Add(item.Num);
                    elements.Add(temp);
                }
            }
            elements.RemoveAt(elements.Count-1);
            transition.Add(Character, new List<transitions>());
            Character++;
            do
            {
                List<int> numtemp = new List<int>();//lista de numero que se hacen en cada grupo temporal
                List<transitions> temptran = new List<transitions>();//lista de transiciones que voy a agregar a la table
                foreach (var tran in transition)//por cada elemento en la table de transiciones
                {
                    if (tran.Value.Count==0)//que este vacia
                    {
                        foreach (var symbol in elements)//recorere mi tabla de simbolos 
                        {
                            foreach (var nums in groups[tran.Key])//y si un numero pertenece a este simbolo 
                            {
                                if (symbol.numbers.Contains(nums))
                                {
                                    numtemp.AddRange(Follow[nums]);
                                }
                            }
                            bool existe = false;
                            if (numtemp.Count!=0)
                            {
                                foreach (var item in groups.Keys)
                                {
                                    if (groups[item].SequenceEqual(numtemp))
                                    {
                                        temptran.Add(new transitions { symbol = symbol.symbol, destiny = item });
                                        existe = true;
                                    }
                                }
                                if (existe == false)
                                {
                                    groups.Add(Character, numtemp);
                                    temptran.Add(new transitions { symbol = symbol.symbol, destiny = Character });
                                    Character++;
                                }
                            }
                            else
                            {
                                temptran.Add(new transitions { symbol = symbol.symbol, destiny = '-' });
                            }
                            numtemp = new List<int>();
                        }
                    }
                    transition[tran.Key].AddRange(temptran);
                    temptran = new List<transitions>();
                }
                bool agrego = false;
                foreach (var group in groups)
                {
                    if (transition.ContainsKey(group.Key)==false)
                    {
                        transition.Add(group.Key, new List<transitions>());
                        agrego = true;
                    }
                }
                if (agrego == false)
                {
                    flag = true;
                }
            } while (!flag);

        }
        /// <summary>
        /// crea el diccionario de follows
        /// </summary>
        /// <param name="raiz"></param>
        private void Follows(Node raiz)
        {
            if (raiz == null)
            {
                return;
            }

            Follows(raiz.Izquierdo);
            Follows(raiz.Derecho);

            if (raiz.Simbolo.symbol == ".")
            {
                int[] lastpos_c1 = raiz.Izquierdo.Last.ToArray();
                List<int> firstpos_c2 = raiz.Derecho.First;
                for (int i = 0; i < lastpos_c1.Length; i++)
                {
                    Follow[lastpos_c1[i]].AddRange(firstpos_c2);
                }
            }
            else if (raiz.Simbolo.symbol == "*")
            {
                int[] lastpos_n = raiz.Last.ToArray();
                List<int> firstpos_n = raiz.First;
                for (int i = 0; i < lastpos_n.Length; i++)
                {
                    Follow[lastpos_n[i]].AddRange(firstpos_n);
                }
            }
        }
        /// <summary>
        /// muestra los follows
        /// </summary>
        /// <returns></returns>
        public string getFL()
        {
            string escritura = "";
            List<Node> lista = Automat.ListInter(Automat.getRaiz());
            escritura += "First";
            escritura += Environment.NewLine;
            foreach (var item1 in lista)
            {
                escritura += "Node("+item1.Simbolo.symbol+"): "+item1.Num+"--> ";
                foreach (var item2 in item1.First)
                {
                    escritura += item2+", ";
                }
                escritura += Environment.NewLine;
            }
            escritura += Environment.NewLine;
            escritura += "Last";
            escritura += Environment.NewLine;
            foreach (var item1 in lista)
            {
                escritura += "Node(" + item1.Simbolo.symbol + "): " + item1.Num + "--> ";
                foreach (var item2 in item1.Last)
                {
                    escritura += item2 + ", ";
                }
                escritura += Environment.NewLine;
            }
            return escritura;
        }
        /// <summary>
        /// follows a mostrat
        /// </summary>
        /// <returns></returns>
        public  string getFollows()
        {
            string escritura = "";
            escritura += getFL();
            escritura += Environment.NewLine;
            escritura += "Follows";
            escritura += Environment.NewLine;
            for (int i = 1; i <= Follow.Keys.Count-1; i++)
            {                
                escritura += "NODO: " + i+" --> ";                                
                for (int j = 0; j < Follow[i].Count; j++)
                {
                    escritura += Follow[i][j] + ", ";
                }
                escritura += Environment.NewLine;
            }
            return escritura;
        }       
        public string showAutomat()
        {
            string text = "Tabla de transiciones";
            text += Environment.NewLine;
            foreach (var simbol in transition)
            {
                text += simbol.Key + " \r|\t ";
                foreach (var destins in transition[simbol.Key])
                {
                    text += "-"+destins.destiny + "-\t|\t";
                }
                text += Environment.NewLine;
            }
            text += Environment.NewLine;
            text += Environment.NewLine;
            text += "grupos";
            text += Environment.NewLine;
            foreach (var charac in groups)
            {
                text += charac.Key + "->\r";
                foreach (var nums in groups[charac.Key])
                {
                    text +=  nums+ ",";
                }
                text += Environment.NewLine;
            }
            return text;
        }
        public void Compiler(string name)
        {
            string path = @"C:\Users\DISTELSA\Desktop\Compilado\";
            string filename = name+".cs";
            string exe = path + name + ".exe";
            //CompileCode(path, filename);            
            //Process.Start(exe);
            CompileExecutable(path+filename);
            
        }
        private bool CompileCode(string CompilableClassFolderPath, string ClassName)
        {
            string sourceName = Path.Combine(CompilableClassFolderPath, ClassName);
            using (CSharpCodeProvider provider = new CSharpCodeProvider(new Dictionary<String, String> { { "CompilerVersion", "v4.0" } }))
            {
                String exeName = String.Format(@"{0}\{1}", CompilableClassFolderPath, ClassName.Replace(".cs", ".exe"));
                CompilerParameters compi = new CompilerParameters { GenerateExecutable = true, OutputAssembly = exeName, GenerateInMemory = false, TreatWarningsAsErrors = false };
                compi.ReferencedAssemblies.Add("Newtonsoft.Json.dll");
                compi.ReferencedAssemblies.Add("System.dll");
                compi.ReferencedAssemblies.Add("System.Core.dll");               
                compi.ReferencedAssemblies.Add("System.Data.DataSetExtensions.dll");
                compi.ReferencedAssemblies.Add("System.IO.dll");
                compi.ReferencedAssemblies.Add("System.Linq.dll");
                compi.ReferencedAssemblies.Add("System.Threading.Tasks.dll");
                compi.ReferencedAssemblies.Add("System.Windows.Forms.dll");                
                CompilerResults cr = provider.CompileAssemblyFromFile(compi, sourceName);
                if (cr.Errors.Count > 0)
                {
                    foreach (CompilerError ce in cr.Errors)
                    {                        
                        MessageBox.Show(string.Format("{0}", ce.ToString()),"Error");
                    }
                }
                else
                {
                    Console.WriteLine("Source {0} built into {1} successfully.",
                        ClassName, ClassName.Replace(".cs", ".exe"));
                    Console.WriteLine("Executing .exe file...");
                }
                if (cr.Errors.Count > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public static bool CompileExecutable(String sourceName)
        {
            FileInfo sourceFile = new FileInfo(sourceName);
            CodeDomProvider provider = null;
            bool compileOk = false;

            // Select the code provider based on the input file extension.
            if (sourceFile.Extension.ToUpper(CultureInfo.InvariantCulture) == ".CS")
            {
                provider = CodeDomProvider.CreateProvider("CSharp");
            }
            else if (sourceFile.Extension.ToUpper(CultureInfo.InvariantCulture) == ".VB")
            {
                provider = CodeDomProvider.CreateProvider("VisualBasic");
            }
            else
            {
                Console.WriteLine("Source file must have a .cs or .vb extension");
            }

            if (provider != null)
            {

                // Format the executable file name.
                // Build the output assembly path using the current directory
                // and <source>_cs.exe or <source>_vb.exe.

                String exeName = String.Format(@"{0}\{1}.exe",
                    System.Environment.CurrentDirectory,
                    sourceFile.Name.Replace(".", "_"));

                CompilerParameters cp = new CompilerParameters();

                // Generate an executable instead of
                // a class library.
                cp.GenerateExecutable = true;

                // Specify the assembly file name to generate.
                cp.OutputAssembly = exeName;

                // Save the assembly as a physical file.
                cp.GenerateInMemory = false;

                // Set whether to treat all warnings as errors.
                cp.TreatWarningsAsErrors = false;
                //
                cp.ReferencedAssemblies.Add("Newtonsoft.Json.dll");
                cp.ReferencedAssemblies.Add("System.dll");
                cp.ReferencedAssemblies.Add("System.Core.dll");
                cp.ReferencedAssemblies.Add("System.Data.DataSetExtensions.dll");
                cp.ReferencedAssemblies.Add("System.IO.dll");
                cp.ReferencedAssemblies.Add("System.Linq.dll");
                cp.ReferencedAssemblies.Add("System.Threading.Tasks.dll");
                cp.ReferencedAssemblies.Add("System.Windows.Forms.dll");
                // Invoke compilation of the source file.
                CompilerResults cr = provider.CompileAssemblyFromFile(cp,
                    sourceName);

                if (cr.Errors.Count > 0)
                {
                    // Display compilation errors.
                    Console.WriteLine("Errors building {0} into {1}",
                        sourceName, cr.PathToAssembly);
                    foreach (CompilerError ce in cr.Errors)
                    {
                        //Console.WriteLine("  {0}", ce.ToString());
                        MessageBox.Show(string.Format(" {0}",ce.ToString()),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        //Console.WriteLine();
                    }
                }
                else
                {
                    // Display a successful compilation message.                    
                    //MessageBox.Show(string.Format("Source {0} built into {1} successfully.", sourceName, cr.PathToAssembly),"Todo bien:", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);                                                           
                    Process.Start(cr.PathToAssembly);
                }

                // Return the results of the compilation.
                if (cr.Errors.Count > 0)
                {
                    compileOk = false;
                }
                else
                {
                    compileOk = true;
                }
            }
            return compileOk;
        }
    }    
}