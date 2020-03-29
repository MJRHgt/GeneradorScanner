namespace GeneradorScanner
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.TXT_Path = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BTM_Explore = new System.Windows.Forms.Button();
            this.txtArea1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtArea2 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDFA = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // TXT_Path
            // 
            this.TXT_Path.Location = new System.Drawing.Point(12, 24);
            this.TXT_Path.Name = "TXT_Path";
            this.TXT_Path.ReadOnly = true;
            this.TXT_Path.Size = new System.Drawing.Size(635, 20);
            this.TXT_Path.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Ubicacion del archivo:";
            // 
            // BTM_Explore
            // 
            this.BTM_Explore.Location = new System.Drawing.Point(660, 22);
            this.BTM_Explore.Name = "BTM_Explore";
            this.BTM_Explore.Size = new System.Drawing.Size(75, 23);
            this.BTM_Explore.TabIndex = 3;
            this.BTM_Explore.Text = "Explore";
            this.BTM_Explore.UseVisualStyleBackColor = true;
            this.BTM_Explore.Click += new System.EventHandler(this.BTM_Explore_Click);
            // 
            // txtArea1
            // 
            this.txtArea1.AcceptsReturn = true;
            this.txtArea1.AcceptsTab = true;
            this.txtArea1.AllowDrop = true;
            this.txtArea1.Location = new System.Drawing.Point(12, 69);
            this.txtArea1.Multiline = true;
            this.txtArea1.Name = "txtArea1";
            this.txtArea1.ReadOnly = true;
            this.txtArea1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtArea1.Size = new System.Drawing.Size(348, 242);
            this.txtArea1.TabIndex = 4;
            this.txtArea1.TextChanged += new System.EventHandler(this.txtArea1_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Set\'s y Token\'s";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 325);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Automata finito determinista";
            // 
            // txtArea2
            // 
            this.txtArea2.AcceptsReturn = true;
            this.txtArea2.AcceptsTab = true;
            this.txtArea2.AllowDrop = true;
            this.txtArea2.Location = new System.Drawing.Point(366, 69);
            this.txtArea2.Multiline = true;
            this.txtArea2.Name = "txtArea2";
            this.txtArea2.ReadOnly = true;
            this.txtArea2.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtArea2.Size = new System.Drawing.Size(369, 242);
            this.txtArea2.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(363, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Follows";
            // 
            // txtDFA
            // 
            this.txtDFA.Location = new System.Drawing.Point(12, 341);
            this.txtDFA.Multiline = true;
            this.txtDFA.Name = "txtDFA";
            this.txtDFA.ReadOnly = true;
            this.txtDFA.Size = new System.Drawing.Size(723, 197);
            this.txtDFA.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(747, 550);
            this.Controls.Add(this.txtDFA);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtArea2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtArea1);
            this.Controls.Add(this.BTM_Explore);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TXT_Path);
            this.Name = "Form1";
            this.Text = "Proyecto ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox TXT_Path;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BTM_Explore;
        private System.Windows.Forms.TextBox txtArea1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtArea2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDFA;
    }
}

