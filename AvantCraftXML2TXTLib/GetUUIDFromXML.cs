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
    public void GetUUID(string txt_FolderUUID, string aPeriodo)
    {
      StringBuilder sb = new StringBuilder();
      DirectoryInfo dir = new DirectoryInfo(txt_FolderUUID);
      FileInfo[] files;


      files = dir.GetFiles("*.xml");

      foreach (FileInfo file in files)
      {
        //sb.Append(ParseXml(file) + Environment.NewLine);
        GetXmlInfoIntoDB(file, aPeriodo);
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
      XNamespace nomina = "http://www.sat.gob.mx/nomina";
      XNamespace tfd = "http://www.sat.gob.mx/TimbreFiscalDigital";

      XElement root = XElement.Load(file.FullName);
      //var comprobante = (from c in root.Elements(cfdi + "Comprobante") select c).FirstOrDefault();

      var Receptor = (from c in root.Elements(cfdi + "Receptor") select c).FirstOrDefault();

      string rfc = Receptor.Attribute("rfc").Value;
      string nombre = Receptor.Attribute("nombre").Value;

      var Nomina = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina") select c).FirstOrDefault();

      string fechaFinalPago = Nomina.Attribute("FechaFinalPago").Value;

      var TimbreFiscalDigital = (from c in root.Elements(cfdi + "Complemento").Elements(tfd + "TimbreFiscalDigital") select c).FirstOrDefault();

      string UUID = TimbreFiscalDigital.Attribute("UUID").Value; //0

      return rfc + "," + nombre + "," + fechaFinalPago + "," + UUID;
    }


    //---------------------------------------------------------------------------+

    private static void GetXmlInfoIntoDB(FileInfo file, string aPeriodo)
    {
      AvantCraft_nomina2017Entities db = new AvantCraft_nomina2017Entities();

      XNamespace cfdi = "http://www.sat.gob.mx/cfd/3";
      XNamespace nomina = "http://www.sat.gob.mx/nomina";
      XNamespace tfd = "http://www.sat.gob.mx/TimbreFiscalDigital";

      XElement root = XElement.Load(file.FullName);

      var Receptor = (from c in root.Elements(cfdi + "Receptor") select c).FirstOrDefault();

      string rfc = Receptor.Attribute("rfc").Value;
      string nombre = Receptor.Attribute("nombre").Value;

      var Nomina = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina") select c).FirstOrDefault();

      var TimbreFiscalDigital = (from c in root.Elements(cfdi + "Complemento").Elements(tfd + "TimbreFiscalDigital") select c).FirstOrDefault();

      string UUID = TimbreFiscalDigital.Attribute("UUID").Value; //0

      var found = (from a in db.TE_RfcTimbrado
                   where a.txtRFC == rfc
                         && a.txtPeriodo == aPeriodo
                         && a.txtFileName == file.Name
                         && a.txtUUID == UUID
                   select a).FirstOrDefault();

      if (found == null)
      {
        TE_RfcTimbrado t = new TE_RfcTimbrado();
        t.txtFileName = file.Name;
        t.txtPeriodo = aPeriodo;
        t.txtRFC = rfc;
        t.txtUUID = UUID;
        db.TE_RfcTimbrado.Add(t);
        db.SaveChanges();
      }
    }
  }
}
