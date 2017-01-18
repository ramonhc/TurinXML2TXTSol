using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AvantCraftXML2TXTLib;


namespace testXML2TXTForm
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      Xml2TxtProcess obj = new Xml2TxtProcess();
      label1.Text = "Procesing Payrol...!";
      obj.Processfiles();
      label1.Text = "Done!";
    }

    private void button2_Click(object sender, EventArgs e)
    {
      GetUUIDFromXML obj = new GetUUIDFromXML();
      label1.Text = "Getting UUIDs...!";
      obj.GetUUID();
      label1.Text = "Done!";
    }

    private void button3_Click(object sender, EventArgs e)
    {
      Xml2TxtProcess obj = new Xml2TxtProcess();
      label1.Text = "Getting Layouts...!";
      obj.GetLAyoutsInExcel();
      label1.Text = "Done!";
    }
  }
}
