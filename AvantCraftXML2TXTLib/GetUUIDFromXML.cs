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
    public void GetUUID()
    {
      StringBuilder sb = new StringBuilder();
      DirectoryInfo dir = new DirectoryInfo(GetConfigurationValues("dirXML"));
      FileInfo[] files;

      files = dir.GetFiles("*.xml");

      bool exists = System.IO.Directory.Exists(GetConfigurationValues("BackupFolder"));
      if (!exists) System.IO.Directory.CreateDirectory(GetConfigurationValues("BackupFolder"));

      foreach (FileInfo file in files)
      {
        file.CopyTo(GetConfigurationValues("BackupFolder") + file.Name, true);
        sb.Append(ParseXml(file) + "\r\n");
        file.Delete();
      }

      TextWriter sw = new StreamWriter(GetConfigurationValues("dirXML") +  "_UUIDList.txt", false, Encoding.GetEncoding(1252), 512);
      sw.Write(sb.ToString());
      sw.Close();
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

      var TimbreFiscalDigital = (from c in root.Elements(cfdi + "Complemento").Elements(tfd + "TimbreFiscalDigital") select c).FirstOrDefault();

      string UUID = TimbreFiscalDigital.Attribute("UUID").Value; //0

      return UUID;
    }


    //---------------------------------------------------------------------------+

    public static string GetConfigurationValues(string configKey)
    {
      string confValue = @"C:\Users\rhernandez\Desktop\LEVICOM\check\SAIC\";

      if (configKey == "BackupFolder")
        confValue = confValue + @"BACKUP\";

      if (configKey == "ErrorFolder")
        confValue = confValue + @"ERROR\";

      return confValue;
    }
  }
}
