using dataaccessXML2TXT;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvantCraftXML2TXTLib
{
  public class GetLayoutsInExcel
  {
    public static void GetLayouts()
    {
      string LayOutsFolder = ConfigurationManager.AppSettings["LayOutsFolder"].ToString();
      bool exists = System.IO.Directory.Exists(LayOutsFolder);
      if (!exists) System.IO.Directory.CreateDirectory(LayOutsFolder);

      StringBuilder HB = new StringBuilder();
      StringBuilder NB = new StringBuilder();

      AvantCraft_nomina2017Entities db = new AvantCraft_nomina2017Entities();

      var allhead = from a in db.TE_TXT_HEADER select a;
      var allNOM = from b in db.TE_Nomina select b;

      HB.Append("headerId,H1_05,H1_08,H1_11,H1_14,H1_30,H1_31,H1_32,H1_46,H1_50,H2_02,H2_03,H2_05,H2_06,H2_08,H2_09,H2_11,H2_12,H2_13,H2_14,H4_02,H4_03,H4_13,D_04,D_06,D_07,D_09,D_25,D_37,D_38,D_42,S_10,S_16,S_36,S_37,filename" + Environment.NewLine);
      foreach(TE_TXT_HEADER h in allhead)
      {
        HB.Append(h.headerId + "," + h.H1_05 + "," + h.H1_08 + "," + h.H1_11 + "," + h.H1_14 + "," + h.H1_30 + "," + h.H1_31.Replace(',',';') + "," + h.H1_32 + "," + h.H1_46 + "," + h.H1_50 + "," + h.H2_02 + "," + h.H2_03 + "," + h.H2_05 + "," + h.H2_06 + "," + h.H2_08 + "," + h.H2_09 + "," + h.H2_11 + "," + h.H2_12 + "," + h.H2_13 + "," + h.H2_14 + "," + h.H4_02 + "," + h.H4_03 + "," + h.H4_13 + "," + h.D_04 + "," + h.D_06 + "," + h.D_07 + "," + h.D_09 + "," + h.D_25 + "," + h.D_37 + "," + h.D_38 + "," + h.D_42 + "," + h.S_10 + "," + h.S_16 + "," + h.S_36 + "," + h.S_37 + "," + h.filename + Environment.NewLine);
      }

      NB.Append("nominaId,version,c_TipoNomina,FechaPago,FechaInicialPago,FechaFinalPago,NumDiasPagados,TotalPercepciones,TotalDeducciones,TotalOtrosPagos,Emisor_CURP,Emisor_RegistroPatronal,Emisor_RfcPatronOrigen,Emisor_EntidadSNCF_c_OrigenRecurso,Emisor_EntidadSNCF_MontoRecursoPropio,Receptor_CURP,Receptor_NumSeguridadSocial,Receptor_FechaInicioRelLaboral,Receptor_Antiguedad,Receptor_c_TipoContrato,Receptor_Sindicalizado,Receptor_c_TipoJornada,Receptor_TipoRegimen,Receptor_c_TipoRegimen,Receptor_NumEmpleado,Receptor_Departamento,Receptor_Puesto,Receptor_c_RiesgoPuesto,Receptor_PeriodicidadPago,Receptor_c_PeriodicidadPago,Receptor_c_Banco,Receptor_CuentaBancaria,Receptor_SalarioBaseCotApor,Receptor_SalarioDiarioIntegrado,Receptor_c_ClaveEntFed,Percepciones_TotalSueldos,Percepciones_TotalSeparacionIndemnizacion,Percepciones_TotalJubilacionPensionRetiro,Percepciones_TotalGravado,Percepciones_TotalExento,Deducciones_TotalOtrasDeducciones,Deducciones_TotalImpuestosRetenidos" + Environment.NewLine);
      foreach (TE_Nomina n in allNOM)
      {
        NB.Append(n.nominaId + "," + n.version + "," + n.c_TipoNomina + "," + n.FechaPago + "," + n.FechaInicialPago + "," + n.FechaFinalPago + "," + n.NumDiasPagados + "," + n.TotalPercepciones + "," + n.TotalDeducciones + "," + n.TotalOtrosPagos + "," + n.Emisor_CURP + "," + n.Emisor_RegistroPatronal + "," + n.Emisor_RfcPatronOrigen + "," + n.Emisor_EntidadSNCF_c_OrigenRecurso + "," + n.Emisor_EntidadSNCF_MontoRecursoPropio + "," + n.Receptor_CURP + "," + n.Receptor_NumSeguridadSocial + "," + n.Receptor_FechaInicioRelLaboral + "," + n.Receptor_Antiguedad + "," + n.Receptor_c_TipoContrato + "," + n.Receptor_Sindicalizado + "," + n.Receptor_c_TipoJornada + "," + n.Receptor_TipoRegimen + "," + n.Receptor_c_TipoRegimen + "," + n.Receptor_NumEmpleado + "," + n.Receptor_Departamento + "," + n.Receptor_Puesto + "," + n.Receptor_c_RiesgoPuesto + "," + n.Receptor_PeriodicidadPago + "," + n.Receptor_c_PeriodicidadPago + "," + n.Receptor_c_Banco + "," + n.Receptor_CuentaBancaria + "," + n.Receptor_SalarioBaseCotApor + "," + n.Receptor_SalarioDiarioIntegrado + "," + n.Receptor_c_ClaveEntFed + "," + n.Percepciones_TotalSueldos + "," + n.Percepciones_TotalSeparacionIndemnizacion + "," + n.Percepciones_TotalJubilacionPensionRetiro + "," + n.Percepciones_TotalGravado + "," + n.Percepciones_TotalExento + "," + n.Deducciones_TotalOtrasDeducciones + "," + n.Deducciones_TotalImpuestosRetenidos + Environment.NewLine);
      }
      
      string HBtextToPrint = HB.ToString();
      string NBtextToPrint = NB.ToString();

     TextWriter sw = new StreamWriter(Utils.GetFinalDestination("default") + "concentradoHEAD" + ".csv", false, Encoding.GetEncoding(1252), 512);
      sw.Write(HBtextToPrint);
      sw.Close();

      sw = new StreamWriter(Utils.GetFinalDestination("default") + "concentradoNOM" + ".csv", false, Encoding.GetEncoding(1252), 512);
      sw.Write(NBtextToPrint);
      sw.Close();
    }
  }
}
