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
      label1.Text = "Loading Payrol from XML...!";

      if (txtPeriodo.Text.Trim() == string.Empty)
      {
        label1.Text = "FALTA EL PERIODO!!!!!";
        return;
      }

      obj.Processfiles(txtPeriodo.Text);
      label1.Text = "Done!";
    }

    private void button2_Click(object sender, EventArgs e)
    {
      GetUUIDFromXML obj = new GetUUIDFromXML();
      label1.Text = "Getting UUIDs...!";

      if (txtPeriodo.Text.Trim() == string.Empty)
      {
        label1.Text = "FALTA EL PERIODO!!!!!";
        return;
      }

      obj.GetUUID(txtFolderUUID.Text, txtPeriodo.Text);
      label1.Text = "Done!";
    }

    private void button3_Click(object sender, EventArgs e)
    {
      Xml2TxtProcess obj = new Xml2TxtProcess();
      label1.Text = "Getting Layouts...!";
      obj.GetLAyoutsInExcel();
      label1.Text = "Done!";
    }

    private void groupBox1_Enter(object sender, EventArgs e)
    {

    }

    private void label1_Click(object sender, EventArgs e)
    {

    }


    // LOAD COMPLEMENTARY DATA
    private void button4_Click(object sender, EventArgs e)
    {
      Xml2TxtProcess obj = new Xml2TxtProcess();
      label1.Text = "Getting Complementary Data...!";

      if (txtPeriodo.Text.Trim() == string.Empty)
      {
        label1.Text = "FALTA EL PERIODO!!!!!";
        return;
      }

      obj.GetComplementaryDataFromExcel(chkCargaSubcontratacion.Checked, chkFijos.Checked, txtPeriodo.Text);
      label1.Text = "Done!";
    }

    private void button5_Click(object sender, EventArgs e)
    {
      Xml2TxtProcess obj = new Xml2TxtProcess();
      label1.Text = "Procesing Payrol 2 TXT...!";
      obj.PayRoll2TXT(txtPeriodo.Text, chbUseRfcExclusionList.Checked, chbUseRfcIncludeList.Checked);
      label1.Text = "Done!";
    }
  }
}
