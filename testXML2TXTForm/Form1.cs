﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AvantCraftXML2TXTLib;
using dataaccessXML2TXT;

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
            label1.Refresh();

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
            label1.Refresh();

            obj.GetUUID(txtFolderUUID.Text);
            label1.Text = "Done!";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Xml2TxtProcess obj = new Xml2TxtProcess();
            label1.Text = "Getting Layouts...!";
            label1.Refresh();

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
            label1.Refresh();

            if (txtPeriodo.Text.Trim() == string.Empty)
            {
                label1.Text = "FALTA EL PERIODO!!!!!";
                label1.Refresh();
                return;
            }

            obj.GetComplementaryDataFromExcel(chkCargaSubcontratacion.Checked, chkFijos.Checked, txtPeriodo.Text);
            label1.Text = "Done!";
        }

        // PAYROL PROCESS
        private void button5_Click(object sender, EventArgs e)
        {
            Xml2TxtProcess obj = new Xml2TxtProcess();
            label1.Text = "Procesing Payrol 2 TXT...!";
            label1.Refresh();

            if (!chbAll.Checked)
            {
                obj.PayRoll2TXT(txtPeriodo.Text, chbUseRfcExclusionList.Checked, chbUseRfcIncludeList.Checked);
            }
            else
            {
                AvantCraft_nomina2017Entities db = new AvantCraft_nomina2017Entities();
                List<string> periodosList = (from a in db.TE_Nomina orderby a.periodo select a.periodo).Distinct().ToList();
                foreach(string p in periodosList)
                {
                    obj.PayRoll2TXT(p, chbUseRfcExclusionList.Checked, chbUseRfcIncludeList.Checked);
                }
            }
            label1.Text = "Done!";
        }

        // SOMETHING ELSE
        private void button6_Click(object sender, EventArgs e)
        {
            GetUUIDFromXML obj = new GetUUIDFromXML();
            label1.Text = "2 CANCEL: Getting UUIDs...!";
            label1.Refresh();

            obj.GetUUID2CANCEL(txtFolderUUID.Text);
            label1.Text = "Done!";
        }
    }
}
