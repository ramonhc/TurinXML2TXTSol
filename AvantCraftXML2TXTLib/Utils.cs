using dataaccessXML2TXT;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AvantCraftXML2TXTLib
{
    public class Utils
    {
        private static XNamespace cfdi = "http://www.sat.gob.mx/cfd/3";
        private static XNamespace nomina = "http://www.sat.gob.mx/nomina";

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
                    confValue = outFolder + @"GrupoTurin\";
                    break;
                case "SAI091203MU3": //SERVICIOS ADMINISTRATIVOS PARA LA INDUSTRIA DEL CHOCOLATE S DE RL DE CV
                    confValue = outFolder + @"SAIC\";
                    break;
                case "TAR080214S12": //TAR
                    confValue = outFolder + @"TAR\";
                    break;
                case "TSP1008164C9": //TURIN SERVICIOS PROFESIONALES S DE RL DE CV
                    confValue = outFolder + @"TSP\";
                    break;
                case "CTU830715D15": //CHOCOLATES TURIN S.A. DE C.V.
                    confValue = outFolder + @"Turin\";
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
        public static string getNumEmpleadoFromXML(string rfcEmpleado, string aPeriodo)
        {
            string rfcFromXml = string.Empty;
            DirectoryInfo dir = new DirectoryInfo(GetConfigurationValues("dirXML") + aPeriodo + "\\");
            FileInfo[] files;

            files = dir.GetFiles("*.xml");

            foreach (FileInfo file in files)
            {
                XElement root = XElement.Load(file.FullName);
                var receptor = (from c in root.Elements(cfdi + "Receptor") select c).FirstOrDefault();
                var val_complemento_nomina = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina") select c).FirstOrDefault();

                rfcFromXml = receptor.Attribute("rfc").Value;

                if (rfcEmpleado == rfcFromXml)
                {
                    AvantCraft_nomina2017Entities db = new AvantCraft_nomina2017Entities();
                    TC_RFC realRfcNCurp = new TC_RFC();
                    realRfcNCurp.txtCURP = val_complemento_nomina.Attribute("CURP").Value; ;
                    realRfcNCurp.txtNumEmp = val_complemento_nomina.Attribute("NumEmpleado").Value;
                    realRfcNCurp.txtNombre = receptor.Attribute("nombre").Value;
                    realRfcNCurp.txyRfc = rfcFromXml;
                    realRfcNCurp.bitValido = true;
                    db.TC_RFC.Add(realRfcNCurp);
                    db.SaveChanges();
                    break;
                }               
            }
            return rfcFromXml;
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

        //----------------------------------------------------------------------

        public static double s2double(string value) //GetStringAsDecimal
        {
            double theResult;

            if (double.TryParse(value, out theResult))
                return theResult;
            else
                return 0.0D;
        }

        //-----------------------------------------------------------------------
        public static string d2s(decimal value) //GetDecimalAsStringWith4Decimals
        {
            return value.ToString("F4");
        }

        //---------------------------------------------------------------------------+
        private static string GetNumberPart(string cell)
        {
            //string cell = "ABCD4321";
            string numberpart = "0";

            int a = GetIndexofNumber(cell);

            if (a != -1)
            {
                numberpart = cell.Substring(a, cell.Length - a);
            }
            return numberpart;
            //return Convert.ToDecimal(numberpart);
            //string stringpart = cell.Substring(0, a);
        }

        //---------------------------------------------------------------------------+
        private static int GetIndexofNumber(string cell)
        {
            int indexofNum = -1;
            foreach (char c in cell)
            {
                indexofNum++;
                if (Char.IsDigit(c))
                {
                    return indexofNum;
                }
            }
            return -1;
        }

        internal static string FillWithCerosToTheLeft(double theNumber, int totalLenghtOfString)
        {
            string txtNumber = theNumber.ToString();
            switch (txtNumber.Length)
            {
                case 3:
                    break;
                case 2:
                    txtNumber = "0" + txtNumber;
                    break;
                case 1:
                    txtNumber = "00" + txtNumber;
                    break;
                default:
                    break;
            }
            return txtNumber;
        }
        //---------------------------------------------------------------------------+

    }
}
