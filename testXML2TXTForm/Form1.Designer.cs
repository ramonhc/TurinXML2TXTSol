namespace testXML2TXTForm
{
  partial class Form1
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
      this.button1 = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.button2 = new System.Windows.Forms.Button();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.button3 = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.button4 = new System.Windows.Forms.Button();
      this.boxSeleccion = new System.Windows.Forms.GroupBox();
      this.chkFijos = new System.Windows.Forms.CheckBox();
      this.chkCargaSubcontratacion = new System.Windows.Forms.CheckBox();
      this.button5 = new System.Windows.Forms.Button();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.txtPeriodo = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.boxSeleccion.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(12, 272);
      this.button1.Margin = new System.Windows.Forms.Padding(4);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(391, 28);
      this.button1.TabIndex = 0;
      this.button1.Text = "Load Payroll from XML";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
      this.label1.Location = new System.Drawing.Point(9, 21);
      this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(107, 29);
      this.label1.TabIndex = 1;
      this.label1.Text = "waiting...";
      this.label1.Click += new System.EventHandler(this.label1_Click);
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(12, 415);
      this.button2.Margin = new System.Windows.Forms.Padding(4);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(391, 28);
      this.button2.TabIndex = 2;
      this.button2.Text = "Get UUID List";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
      this.pictureBox1.Location = new System.Drawing.Point(24, 11);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(364, 226);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pictureBox1.TabIndex = 3;
      this.pictureBox1.TabStop = false;
      // 
      // button3
      // 
      this.button3.Location = new System.Drawing.Point(12, 366);
      this.button3.Name = "button3";
      this.button3.Size = new System.Drawing.Size(390, 31);
      this.button3.TabIndex = 4;
      this.button3.Text = "Make Layouts (Excel)";
      this.button3.UseVisualStyleBackColor = true;
      this.button3.Click += new System.EventHandler(this.button3_Click);
      // 
      // groupBox1
      // 
      this.groupBox1.BackColor = System.Drawing.SystemColors.HotTrack;
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Location = new System.Drawing.Point(13, 470);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(680, 62);
      this.groupBox1.TabIndex = 5;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Status";
      this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
      // 
      // button4
      // 
      this.button4.Location = new System.Drawing.Point(12, 317);
      this.button4.Name = "button4";
      this.button4.Size = new System.Drawing.Size(390, 31);
      this.button4.TabIndex = 6;
      this.button4.Text = "Load Complementary Data";
      this.button4.UseVisualStyleBackColor = true;
      this.button4.Click += new System.EventHandler(this.button4_Click);
      // 
      // boxSeleccion
      // 
      this.boxSeleccion.Controls.Add(this.chkFijos);
      this.boxSeleccion.Controls.Add(this.chkCargaSubcontratacion);
      this.boxSeleccion.Location = new System.Drawing.Point(427, 27);
      this.boxSeleccion.Name = "boxSeleccion";
      this.boxSeleccion.Size = new System.Drawing.Size(266, 108);
      this.boxSeleccion.TabIndex = 7;
      this.boxSeleccion.TabStop = false;
      this.boxSeleccion.Text = "Carga datos";
      // 
      // chkFijos
      // 
      this.chkFijos.AutoSize = true;
      this.chkFijos.Checked = true;
      this.chkFijos.CheckState = System.Windows.Forms.CheckState.Checked;
      this.chkFijos.Location = new System.Drawing.Point(16, 62);
      this.chkFijos.Name = "chkFijos";
      this.chkFijos.Size = new System.Drawing.Size(100, 21);
      this.chkFijos.TabIndex = 1;
      this.chkFijos.Text = "Datos Fijos";
      this.chkFijos.UseVisualStyleBackColor = true;
      // 
      // chkCargaSubcontratacion
      // 
      this.chkCargaSubcontratacion.AutoSize = true;
      this.chkCargaSubcontratacion.Checked = true;
      this.chkCargaSubcontratacion.CheckState = System.Windows.Forms.CheckState.Checked;
      this.chkCargaSubcontratacion.Location = new System.Drawing.Point(16, 35);
      this.chkCargaSubcontratacion.Name = "chkCargaSubcontratacion";
      this.chkCargaSubcontratacion.Size = new System.Drawing.Size(135, 21);
      this.chkCargaSubcontratacion.TabIndex = 0;
      this.chkCargaSubcontratacion.Text = "SubContratacion";
      this.chkCargaSubcontratacion.UseVisualStyleBackColor = true;
      // 
      // button5
      // 
      this.button5.BackColor = System.Drawing.Color.Red;
      this.button5.ForeColor = System.Drawing.Color.Yellow;
      this.button5.Location = new System.Drawing.Point(427, 366);
      this.button5.Margin = new System.Windows.Forms.Padding(4);
      this.button5.Name = "button5";
      this.button5.Size = new System.Drawing.Size(266, 76);
      this.button5.TabIndex = 8;
      this.button5.Text = "Payroll 2 TXT";
      this.button5.UseVisualStyleBackColor = false;
      this.button5.Click += new System.EventHandler(this.button5_Click);
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.label2);
      this.groupBox2.Controls.Add(this.txtPeriodo);
      this.groupBox2.Location = new System.Drawing.Point(427, 182);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(265, 165);
      this.groupBox2.TabIndex = 9;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "params";
      // 
      // txtPeriodo
      // 
      this.txtPeriodo.Location = new System.Drawing.Point(18, 37);
      this.txtPeriodo.Name = "txtPeriodo";
      this.txtPeriodo.Size = new System.Drawing.Size(132, 22);
      this.txtPeriodo.TabIndex = 0;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(174, 40);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(57, 17);
      this.label2.TabIndex = 1;
      this.label2.Text = "Periodo";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(708, 544);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.button5);
      this.Controls.Add(this.boxSeleccion);
      this.Controls.Add(this.button4);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.button3);
      this.Controls.Add(this.pictureBox1);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.button1);
      this.Margin = new System.Windows.Forms.Padding(4);
      this.Name = "Form1";
      this.Text = "Form1";
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.boxSeleccion.ResumeLayout(false);
      this.boxSeleccion.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button button3;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Button button4;
    private System.Windows.Forms.GroupBox boxSeleccion;
    private System.Windows.Forms.CheckBox chkCargaSubcontratacion;
    private System.Windows.Forms.CheckBox chkFijos;
    private System.Windows.Forms.Button button5;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txtPeriodo;
  }
}

