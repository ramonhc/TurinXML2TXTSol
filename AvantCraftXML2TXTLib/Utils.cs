using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvantCraftXML2TXTLib
{
  public class Utils
  {

    public static string GetFinalDestination(string rfc)
    {
      string outFolder = ConfigurationManager.AppSettings["outFolder"].ToString();  //@"C:\Nomina\";
      string confValue = outFolder;

      switch (rfc)
      {
        case "default": //default
          confValue = outFolder;
          break;
        case "GTU870812BQ6": //GRUPO TURIN S.A. DE C.V.
          confValue = outFolder + @"GrupoTurin\IN\";
          break;
        case "SAI091203MU3": //SERVICIOS ADMINISTRATIVOS PARA LA INDUSTRIA DEL CHOCOLATE S DE RL DE CV
          confValue = outFolder + @"SAIC\IN\";
          break;
        case "TAR080214S12": //TAR
          confValue = outFolder + @"TAR\IN\";
          break;
        case "TSP1008164C9": //TURIN SERVICIOS PROFESIONALES S DE RL DE CV
          confValue = outFolder + @"TSP\IN\";
          break;
        case "CTU830715D15": //CHOCOLATES TURIN S.A. DE C.V.
          confValue = outFolder + @"Turin\IN\";
          break;
        default:
          confValue = outFolder;
          break;
          //case "": //Holdings
          //  confValue = confValue + @"C:\Nomina\Holdings\IN\";
          //  break;
      }

      bool exists2 = System.IO.Directory.Exists(confValue);
      if (!exists2) System.IO.Directory.CreateDirectory(confValue);

      return confValue;
    }

    //----------------------------------------------------------------------
    public static string GetConfigurationValues(string configKey)
    {
      //string confValue = @"C:\Users\rhernandez\Desktop\LEVICOM\TSP_FiniquitoMarzo15\";
      string confValue = @"C:\NominaInicio\";

      if (configKey == "BackupFolder")
        confValue = ConfigurationManager.AppSettings["BackupFolder"].ToString();

      if (configKey == "ErrorFolder")
        confValue = ConfigurationManager.AppSettings["ErrorFolder"].ToString();

      bool exists2 = System.IO.Directory.Exists(confValue);
      if (!exists2) System.IO.Directory.CreateDirectory(confValue);

      return confValue;
    }
    //----------------------------------------------------------------------

    public static decimal s2d(string value) //GetStringAsDecimal
    {
      decimal theResult;

      if (Decimal.TryParse(value, out theResult))
        return theResult;
      else
        return 0.0M;
    }

    //-----------------------------------------------------------------------
    public static string d2s(decimal value) //GetDecimalAsStringWith4Decimals
    {
      return value.ToString("F4");
    }
    //----------------------------------------------------------------------
  }
}
