using dataaccessXML2TXT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AvantCraftXML2TXTLib
{
  public class GetUUIDFromXML
  {
    //----------------------------------------------------------------------------------------------------
    public void GetUUID(string txt_FolderUUID)
    {
      StringBuilder sb = new StringBuilder();
      DirectoryInfo dir = new DirectoryInfo(txt_FolderUUID);
      FileInfo[] files;


      files = dir.GetFiles("*.xml");

      foreach (FileInfo file in files)
      {
        //sb.Append(ParseXml(file) + Environment.NewLine);
        GetXmlInfoIntoDB(file);
      }

      //TextWriter sw = new StreamWriter(txt_FolderUUID +  "\\_UUIDList.csv", false, Encoding.GetEncoding(1252), 512);
      //sw.Write(sb.ToString());
      //sw.Close();
    }

    //----------------------------------------------------------------------------------------------------

    private static string ParseXml(FileInfo file)
    {
      //----------------------------------------------------------------------------------------------------
      //H1-DATOS GENERALES DE LOS CFDI´S

      XNamespace cfdi = "http://www.sat.gob.mx/cfd/3";
      XNamespace nomina12 = "http://www.sat.gob.mx/nomina12";
      XNamespace tfd = "http://www.sat.gob.mx/TimbreFiscalDigital";

      XElement root = XElement.Load(file.FullName);
      //var comprobante = (from c in root.Elements(cfdi + "Comprobante") select c).FirstOrDefault();

      var Receptor = (from c in root.Elements(cfdi + "Receptor") select c).FirstOrDefault();
      string rfc = Receptor.Attribute("rfc").Value;
      string nombre = Receptor.Attribute("nombre").Value;

      var Nomina = (from c in root.Elements(cfdi + "Complemento").Elements(nomina12 + "Nomina") select c).FirstOrDefault();
      string fechaFinalPago = Nomina.Attribute("FechaFinalPago").Value;

      var TimbreFiscalDigital = (from c in root.Elements(cfdi + "Complemento").Elements(tfd + "TimbreFiscalDigital") select c).FirstOrDefault();
      string UUID = TimbreFiscalDigital.Attribute("UUID").Value; //0
      return rfc + "," + nombre + "," + fechaFinalPago + "," + UUID;
    }


    //---------------------------------------------------------------------------+

    private static void GetXmlInfoIntoDB(FileInfo file)
    {
      string aPeriodo = string.Empty;
      AvantCraft_nomina2017Entities db = new AvantCraft_nomina2017Entities();

      XNamespace cfdi = "http://www.sat.gob.mx/cfd/3";
      XNamespace nomina12 = "http://www.sat.gob.mx/nomina12";
      XNamespace tfd = "http://www.sat.gob.mx/TimbreFiscalDigital";

      XElement root = XElement.Load(file.FullName);

      var Receptor = (from c in root.Elements(cfdi + "Receptor") select c).FirstOrDefault();
      string rfc = Receptor.Attribute("rfc").Value;
      string nombre = Receptor.Attribute("nombre").Value;

      string curp = "";
      string frecuencia = "";

      //var complemento_nomina_percepciones_coll = (from c in root.Elements(cfdi + "Complemento").Elements(nomina12 + "Nomina").Elements(nomina12 + "Receptor") select c).FirstOrDefault();
      //if (complemento_nomina_percepciones_coll != null)
      //{
      //  double dFrecuencia = Utils.s2double(complemento_nomina_percepciones_coll.Attribute("PeriodicidadPago").Value);
      //  frecuencia = (from t in db.c_PeriodicidadPago where t.c_PeriodicidadPago1 == dFrecuencia select t.Descripcion).FirstOrDefault();
      //}

      var Nomina = (from c in root.Elements(cfdi + "Complemento").Elements(nomina12 + "Nomina") select c).FirstOrDefault();
      string fechaFinalPago = Nomina.Attribute("FechaFinalPago").Value;
      string NumDiasPagados = Nomina.Attribute("NumDiasPagados").Value;

      var TimbreFiscalDigital = (from c in root.Elements(cfdi + "Complemento").Elements(tfd + "TimbreFiscalDigital") select c).FirstOrDefault();
      string UUID = TimbreFiscalDigital.Attribute("UUID").Value; //0

      //if (frecuencia == "Semanal")
      //{
      //  switch (fechaFinalPago)
      //  {
      //    case "2017-01-08":
      //      aPeriodo = "S012017";
      //      break;
      //    case "2017-01-06":
      //      aPeriodo = "S012017";
      //      break;
      //    case "2017-01-15":
      //      aPeriodo = "S022017";
      //      break;
      //    case "2017-01-22":
      //      aPeriodo = "S032017";
      //      break;
      //    case "2017-01-29":
      //      aPeriodo = "S042017";
      //      break;
      //    case "2017-01-27":
      //      aPeriodo = "S042017";
      //      break;
      //    default:
      //      aPeriodo = string.Empty;
      //      break;
      //  }
      //}
      //else if (frecuencia == "Quincenal")
      //{
      //  switch (fechaFinalPago)
      //  {
      //    case "2017-01-08":
      //      aPeriodo = "Q";
      //      break;
      //    case "2017-01-15":
      //      aPeriodo = "Q";
      //      break;
      //    case "2017-01-22":
      //      aPeriodo = "Q";
      //      break;
      //    case "2017-01-29":
      //      aPeriodo = "Q";
      //      break;
      //    default:
      //      aPeriodo = string.Empty;
      //      break;
      //  }
      //}


      var complemento_nomina_percepciones_coll = (from c in root.Elements(cfdi + "Complemento").Elements(nomina12 + "Nomina").Elements(nomina12 + "Receptor") select c).FirstOrDefault();
      if (complemento_nomina_percepciones_coll != null)
      {
        curp = complemento_nomina_percepciones_coll.Attribute("Curp").Value;
      }

      aPeriodo = (from r in db.TE_Nomina where r.Receptor_CURP == curp && r.FechaFinalPago == fechaFinalPago select r.periodo).FirstOrDefault();
      //frecuencia = (from r in db.TE_Nomina where r.Receptor_CURP == curp select r.c_PeriodicidadPago.Descripcion).FirstOrDefault();


      var found = (from a in db.TE_RfcTimbrado
                   where a.txtRFC == rfc
                         && a.txtPeriodo == aPeriodo
                         && a.txtFecha == fechaFinalPago
                         && a.txtFileName == file.Name
                         && a.txtUUID == UUID
                   select a).FirstOrDefault();

      if (found == null)
      {
        TE_RfcTimbrado t = new TE_RfcTimbrado();
        t.txtFileName = file.Name;
        t.txtFecha = fechaFinalPago;
        t.txtPeriodo = aPeriodo;
        t.txtRFC = rfc;
        t.txtUUID = UUID;
        db.TE_RfcTimbrado.Add(t);
        db.SaveChanges();
      }
    }
  }
}
