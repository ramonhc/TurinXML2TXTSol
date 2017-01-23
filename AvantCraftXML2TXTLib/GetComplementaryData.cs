using dataaccessXML2TXT;
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
    public static void Load(bool chkCargaSubcontratacion, bool chkFijos)
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

      //--->>> fijosXempleado -- Subontratación
      foreach (DataRow r in result.Tables["fijosXempleado"].Rows)
      {
        string rfcEmpleado = r["rfcEmpleado"].ToString();
        string RfcLabora = r["RfcLabora"].ToString();
        //---------->>>>>>>>>>>>>> Subcontratacion
        if (chkCargaSubcontratacion)
        {
          //VALIDATE (if exists then remove)
          TC_Subcontratacion isthere = (from a in db.TC_Subcontratacion where a.RfcEmpleado == rfcEmpleado && a.RfcLabora == RfcLabora select a).FirstOrDefault();
          if (isthere != null)
          {
            db.TC_Subcontratacion.Remove(isthere);
          }

          //ADD NEW
          TC_Subcontratacion s = new TC_Subcontratacion();
          s.RfcEmpleado = rfcEmpleado;
          s.RfcLabora = RfcLabora;
          s.PorcentajeTiempo = decimal.Parse(r["PorcentajeTiempo"].ToString());

          db.TC_Subcontratacion.Add(s);
          db.SaveChanges();
        }

        //---------->>>>>>>>>>>>>> fijosXempleado
        if (chkFijos)
        {
          //VALIDATE (if exists then remove)
          TC_DatosFijosPorEmpleado isthere = (from a in db.TC_DatosFijosPorEmpleado where a.rfcEmpleado == rfcEmpleado select a).FirstOrDefault();
          if (isthere != null)
          {
            db.TC_DatosFijosPorEmpleado.Remove(isthere);
          }

          //ADD NEW
          TC_DatosFijosPorEmpleado s = new TC_DatosFijosPorEmpleado();
          s.rfcEmpleado = rfcEmpleado;
          s.Sindicalizado = r["Sindicalizado"].ToString();
          s.c_TipoJornada = r["c_TipoJornada"].ToString();
          s.Departamento = r["Departamento"].ToString();
          s.c_RiesgoPuesto = r["c_RiesgoPuesto"].ToString();
          s.c_Banco = r["c_Banco"].ToString();
          s.CuentaBancaria = r["CuentaBancaria"].ToString();
          s.c_Estado = r["c_Estado"].ToString();

          db.TC_DatosFijosPorEmpleado.Add(s);
          db.SaveChanges();
        }
      }

      //--->>>
      foreach (DataRow r in result.Tables["camposNuevos"].Rows)
      {
        //Get Record and validate
        string periodo = r["periodo"].ToString();
        string Num_Emp = r["Num_Emp"].ToString();
        TE_Nomina n = (from a in db.TE_Nomina where a.periodo == periodo && a.Receptor_NumEmpleado == Num_Emp select a).FirstOrDefault();
        if (n != null)
        {
          n.c_TipoNomina = r["c_TipoNomina"].ToString();
          n.TotalPercepciones = Utils.s2d(r["TotalPercepciones"].ToString());
          n.TotalDeducciones = Utils.s2d(r["TotalDeducciones"].ToString());
          n.TotalOtrosPagos = Utils.s2d(r["TotalOtrosPagos"].ToString());
          n.Emisor_EntidadSNCF_c_OrigenRecurso = r["c_OrigenRecurso"].ToString();
          n.Emisor_EntidadSNCF_MontoRecursoPropio = Utils.s2d(r["MontoRecursoPropio"].ToString());
          n.Receptor_Antiguedad = r["Antiguedad"].ToString();
          n.Receptor_c_TipoContrato = double.Parse(r["c_TipoContrato"].ToString());
          n.Percepciones_TotalSueldos = Utils.s2d(r["TotalSueldos"].ToString());
          n.Percepciones_TotalSeparacionIndemnizacion = Utils.s2d(r["TotalSeparacionIndemnizacion"].ToString());
          n.Percepciones_TotalJubilacionPensionRetiro = Utils.s2d(r["TotalJubilacionPensionRetiro"].ToString());
        }
      }
    }
  }
}
