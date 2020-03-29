using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using GeneradorScanner;
using Microsoft.Azure.Amqp.Framing;
using System.IO.Abstractions;

namespace GeneradorScanner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public TextWriter wpath;
        private void BTM_Explore_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.DefaultExt = ".txt";
            open.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (open.ShowDialog() == DialogResult.OK)
            {
                TXT_Path.Text = open.FileName;
                //Procesos obj = Procesos.CreateInstance();
                Procesos obj = new Procesos();
                //Inicio de lectura
                obj.ReadFile(open.FileName);                
                txtArea1.Text = obj.texttoshow();
                txtArea2.Text = obj.getFollows();
                txtDFA.Text = obj.showAutomat();

                DirectoryInfo directory = new DirectoryInfo(@"C:\Users\DISTELSA\Desktop\Compilado\");
                foreach (var file in directory.GetFiles())
                {
                    file.Delete();
                }
                string name = Path.GetFileName(open.FileName);
                name = name.Substring(0,name.Length-4);
                File.Copy(@"C:\Users\DISTELSA\Desktop\Original.cs", @"C:\Users\DISTELSA\Desktop\Compilado\"+name+".cs");                
                obj.Compiler(name);
            }
        }               
        public void txtArea1_TextChanged(object sender, EventArgs e)
        {            
        }
    }
}
