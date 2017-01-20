﻿using dataaccessXML2TXT;
using Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvantCraftXML2TXTLib
{
  public class GetComplementaryData
  {
    public static void Load()
    {
      string LayOutsFolder = ConfigurationManager.AppSettings["LayOutsFolder"].ToString();
      bool exists = System.IO.Directory.Exists(LayOutsFolder);
      if (!exists) System.IO.Directory.CreateDirectory(LayOutsFolder);

      FileStream stream = File.Open(LayOutsFolder + "LayOutComplementariosS012017.xlsx", FileMode.Open, FileAccess.Read);
      IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
      excelReader.IsFirstRowAsColumnNames = true;
      DataSet result = excelReader.AsDataSet();
      excelReader.Close();

      AvantCraft_nomina2017Entities db = new AvantCraft_nomina2017Entities();

      //--->>> fijosXempleado
      foreach (DataRow r in result.Tables["fijosXempleado"].Rows)
      {
        //VALIDATE (if exists then remove)
        TC_Subcontratacion isthere = (from a in db.TC_Subcontratacion where a.RfcEmpleado == r["rfcEmpleado"].ToString() && a.RfcLabora == r["RfcLabora"].ToString() select a).FirstOrDefault();
        if (isthere != null)
        {
          db.TC_Subcontratacion.Remove(isthere);
        }

        //ADD NEW
        TC_Subcontratacion s = new TC_Subcontratacion();
        s.RfcEmpleado = r["rfcEmpleado"].ToString();
        s.RfcLabora = r["RfcLabora"].ToString();
        s.PorcentajeTiempo = decimal.Parse(r["PorcentajeTiempo"].ToString());

        db.TC_Subcontratacion.Add(s);
        db.SaveChanges();
      }

      //--->>>
      foreach (DataRow r in result.Tables["camposNuevos"].Rows)
      {
      }
    }
  }
}