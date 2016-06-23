using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AvantCraftXML2TXTLib
{
  public class Xml2TxtProcess
  {
    //----------------------------------------------------------------------------------------------------
    public void Processfiles()
    {
      DirectoryInfo dir = new DirectoryInfo(GetConfigurationValues("dirXML"));
      FileInfo[] files;

      files = dir.GetFiles("*.xml");

      //bool exists = System.IO.Directory.Exists(GetConfigurationValues("BackupFolder"));
      //if (!exists) System.IO.Directory.CreateDirectory(GetConfigurationValues("BackupFolder"));

      //bool exists2 = System.IO.Directory.Exists(GetConfigurationValues("ErrorFolder"));
      //if (!exists2) System.IO.Directory.CreateDirectory(GetConfigurationValues("ErrorFolder"));

      foreach (FileInfo file in files)
      {
        string bkpDtmFolder = GetConfigurationValues("BackupFolder") + DateTime.Now.ToString("yyyyMMdd") + "\\";
        bool exists3 = System.IO.Directory.Exists(bkpDtmFolder);
        if (!exists3) System.IO.Directory.CreateDirectory(bkpDtmFolder);

        file.CopyTo(bkpDtmFolder + file.Name, true);
        ParseXml(file);
        file.Delete();
      }
    }

    //----------------------------------------------------------------------------------------------------

    private static void ParseXml(FileInfo file)
    {
      //try
      //{

      //----------------------------------------------------------------------------------------------------
      //H1-DATOS GENERALES DE LOS CFDI´S

      XNamespace cfdi = "http://www.sat.gob.mx/cfd/3";
      XNamespace nomina = "http://www.sat.gob.mx/nomina";

      XElement root = XElement.Load(file.FullName);
      //var comprobante = (from c in root.Elements(cfdi + "Comprobante") select c).FirstOrDefault();

      //********************************************************
      //***** SOLO PARA EFECTOS DE NOMINAS DE OTROS PERIODOS ***
      //var complemento_nomina2 = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina") select c).FirstOrDefault();
      //string H1_5 = DateTime.Parse(complemento_nomina2.Attribute("FechaPago").Value).ToString("yyyyddMM");
      //**** Se comenta la linea siguiente para fozar otra fecha
      string H1_5 = DateTime.Now.ToString("yyyyddMM");//0
      //********************************************************

      string H1_8 = root.Attribute("formaDePago").Value;//1
      string H1_11 = "RECIBO_NOMINA";//2

      string H1_14 = (root.Attribute("descuento") != null) ? d2s(s2d(root.Attribute("descuento").Value)) : "0.00"; //3

      string H1_30 = root.Attribute("LugarExpedicion").Value;//4
      string H1_31 = root.Attribute("metodoDePago").Value;//5
      string H1_32 = "MXN";//6
      string H1_46 = "P";//7
      string H1_50 = "NOM";//8

      string H1 = string.Format("[H1]||||{0}|||{1}|||{2}|||{3}||||||||||||||||{4}|{5}|{6}||||||||||||||{7}||||{8}|||||||||", H1_5, H1_8, H1_11, H1_14, H1_30, H1_31, H1_32, H1_46, H1_50);

      //----------------------------------------------------------------------------------------------------
      //H2- DATOS DEL EMISOR

      var emisor = (from c in root.Elements(cfdi + "Emisor") select c).FirstOrDefault();

      string H2_2 = emisor.Attribute("nombre").Value; //0
      string H2_3 = emisor.Attribute("rfc").Value; //1

      var domicilioFiscal = (from c in root.Elements(cfdi + "Emisor").Elements(cfdi + "DomicilioFiscal") select c).FirstOrDefault();
      string H2_5 = domicilioFiscal.Attribute("calle").Value; //2
      string H2_6 = domicilioFiscal.Attribute("noInterior").Value; //3
      string H2_8 = domicilioFiscal.Attribute("colonia").Value; //4
      string H2_9 = domicilioFiscal.Attribute("localidad").Value; //5
      string H2_11 = domicilioFiscal.Attribute("municipio").Value; //6
      string H2_12 = domicilioFiscal.Attribute("estado").Value; //7
      string H2_13 = domicilioFiscal.Attribute("pais").Value;  //8
      string H2_14 = domicilioFiscal.Attribute("codigoPostal").Value; //9

      string H2 = string.Format("[H2]|{0}|{1}||{2}|{3}||{4}|{5}||{6}|{7}|{8}|{9}||||||||||||", H2_2, H2_3, H2_5, H2_6, H2_8, H2_9, H2_11, H2_12, H2_13, H2_14);

      //----------------------------------------------------------------------------------------------------
      //H3-**SECCION OPCIONAL PARA EL LUGAR DE EXPEDICIÓN ** "EXPEDIDO EN" - (DOMICILIO DE SUCURSAL)

      string H3 = "[H3]|||||||||||||||||||||||||";

      //----------------------------------------------------------------------------------------------------
      //H4- DATOS DEL RECEPTOR DEL CFDI

      var receptor = (from c in root.Elements(cfdi + "Receptor") select c).FirstOrDefault();

      string H4_2 = receptor.Attribute("nombre").Value; //0
      string H4_3 = receptor.Attribute("rfc").Value; //1

      var receptorDomicilio = (from c in root.Elements(cfdi + "Receptor").Elements(cfdi + "Domicilio") select c).FirstOrDefault();

      string H4_13 = receptorDomicilio.Attribute("pais").Value; //2

      string H4 = string.Format("[H4]|{0}|{1}||||||||||{2}|||||||||||||", H4_2, H4_3, H4_13);

      //----------------------------------------------------------------------------------------------------
      //H5- DATOS DEL LUGAR DE ENTREGA  DE LA MERCANCIA

      string H5 = "[H5]|||||||||||||||||||||||||";

      //----------------------------------------------------------------------------------------------------
      //D- DATOS POR PARTIDA (PRODUCTOS) DEL CFDI

      var concepto = (from c in root.Elements(cfdi + "Conceptos").Elements(cfdi + "Concepto") select c).FirstOrDefault();

      string D_4 = concepto.Attribute("descripcion").Value; //0
      string D_6 = d2s(s2d(concepto.Attribute("cantidad").Value)); //1
      string D_7 = concepto.Attribute("unidad").Value; //2
      string D_9 = d2s(s2d(concepto.Attribute("importe").Value)); //3
      string D_25 = d2s(s2d(concepto.Attribute("valorUnitario").Value)); //4

      string D_37 = d2s(s2d(root.Attribute("subTotal").Value));//5  Total de percepciones = campo subTotal de la parte de comprobante (root)

      string D_38 = (root.Attribute("descuento") != null) ? d2s(s2d(root.Attribute("descuento").Value)) : "0.00"; //6

      var impuestos = (from c in root.Elements(cfdi + "Impuestos") select c).FirstOrDefault();

      string D_42 = (impuestos.Attribute("totalImpuestosRetenidos") != null) ? d2s(s2d(impuestos.Attribute("totalImpuestosRetenidos").Value)) : "0.0000"; //7

      string D = string.Format("[D]|||{0}||{1}|{2}||{3}||||||||||||||||{4}||||||||||||{5}|{6}||||{7}||||||||||||||||||||||||||||||||", D_4, D_6, D_7, D_9, D_25, D_37, D_38, D_42);

      //----------------------------------------------------------------------------------------------------
      //S- DATOS TOTALES DEL CFDI

      string S_10 = d2s(s2d(root.Attribute("total").Value)); //3

      var impuestos2 = (from c in root.Elements(cfdi + "Impuestos") select c).FirstOrDefault();

      string S_16 = (impuestos2.Attribute("totalImpuestosRetenidos") != null) ? d2s(s2d(impuestos2.Attribute("totalImpuestosRetenidos").Value)) : "0.0000"; //0

      var impuestos3 = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina").Elements(nomina + "Percepciones") select c).FirstOrDefault();

      string S_36 = d2s(s2d(impuestos3.Attribute("TotalExento").Value) + s2d(impuestos3.Attribute("TotalGravado").Value)); //1

      var impuestos4 = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina").Elements(nomina + "Deducciones") select c).FirstOrDefault();

      //string xxxx = (s2d(impuestos4.Attribute("TotalExento").Value) + s2d(impuestos4.Attribute("TotalGravado").Value)); //2


      decimal totalExcento = 0.0M;
      decimal totalGravado = 0.0M;
      if (impuestos4 != null)
      {
        totalExcento = (impuestos4.Attribute("TotalExento") != null) ? s2d(impuestos4.Attribute("TotalExento").Value) : 0.0M;
        totalGravado = (impuestos4.Attribute("TotalGravado") != null) ? s2d(impuestos4.Attribute("TotalGravado").Value) : 0.0M;
      }


      string S_37 = d2s(totalExcento + totalGravado); //2

      string S = string.Format("[S]|||||||||{3}||||||{0}||||||||||||||||||||{1}|{2}||||", S_16, S_36, S_37, S_10);

      //----------------------------------------------------------------------------------------------------


      string textToPrint = H1 + "\r\n" + H2 + "\r\n" + H3 + "\r\n" + H4 + "\r\n" + H5 + "\r\n" + D + "\r\n" + S + "\r\n";

      //TextWriter sw = new StreamWriter(GetConfigurationValues("dirXML") + H4_3 + "_" + H1_5 + ".txt", false, Encoding.GetEncoding(1252), 512);
      TextWriter sw = new StreamWriter(GetFinalDestination(H2_3) + H4_3 + "_" + H1_5 + ".txt", false, Encoding.GetEncoding(1252), 512);
      sw.Write(textToPrint);
      sw.Close();

      //string H2_3 = emisor.Attribute("rfc").Value; //1
      //====================================================================================================
      //============================= .NOM =================================================================
      //====================================================================================================

      StringBuilder sb = new StringBuilder();

      //----------------------------------------------------------------------------------------------------

      var complemento_nomina = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina") select c).FirstOrDefault();

      string NO_00 = "NO";
      string NO_01 = complemento_nomina.Attribute("RegistroPatronal").Value;
      string NO_02 = complemento_nomina.Attribute("NumEmpleado").Value;
      string NO_03 = complemento_nomina.Attribute("CURP").Value;
      string NO_04 = complemento_nomina.Attribute("TipoRegimen").Value;
      string NO_05 = complemento_nomina.Attribute("NumSeguridadSocial").Value;
      string NO_06 = complemento_nomina.Attribute("FechaPago").Value;
      string NO_07 = complemento_nomina.Attribute("FechaInicialPago").Value;
      string NO_08 = complemento_nomina.Attribute("FechaFinalPago").Value;
      string NO_09 = d2s(s2d(complemento_nomina.Attribute("NumDiasPagados").Value));
      string NO_10 = "";
      string NO_11 = "";
      string NO_12 = "";
      string NO_13 = complemento_nomina.Attribute("FechaInicioRelLaboral").Value;
      string NO_14 = "";
      string NO_15 = (complemento_nomina.Attribute("Puesto") != null) ? complemento_nomina.Attribute("Puesto").Value : "EMPLEADO";
      string NO_16 = "";
      string NO_17 = "";
      string NO_18 = complemento_nomina.Attribute("PeriodicidadPago").Value;

      string NO_19 = (complemento_nomina.Attribute("SalarioBaseCotApor") != null) ? d2s(s2d(complemento_nomina.Attribute("SalarioBaseCotApor").Value)) : "0.00"; //3

      //string NO_19 = "";
      //if (complemento_nomina.Attribute("SalarioBaseCotApor") != null)
      //{
      //  NO_19 = d2s(s2d(complemento_nomina.Attribute("SalarioBaseCotApor").Value));
      //}
      //else
      //{
      //  file.CopyTo(GetConfigurationValues("ErrorFolder") + file.Name, true);
      //  return;
      //}


      string NO_20 = "";
      string NO_21 = (complemento_nomina.Attribute("SalarioDiarioIntegrado") != null) ? d2s(s2d(complemento_nomina.Attribute("SalarioDiarioIntegrado").Value)) : "0.00"; //3

      sb.Append(NO_00 + "|");
      sb.Append(NO_01 + "|");
      sb.Append(NO_02 + "|");
      sb.Append(NO_03 + "|");
      sb.Append(NO_04 + "|");
      sb.Append(NO_05 + "|");
      sb.Append(NO_06 + "|");
      sb.Append(NO_07 + "|");
      sb.Append(NO_08 + "|");
      sb.Append(NO_09 + "|");
      sb.Append(NO_10 + "|");
      sb.Append(NO_11 + "|");
      sb.Append(NO_12 + "|");
      sb.Append(NO_13 + "|");
      sb.Append(NO_14 + "|");
      sb.Append(NO_15 + "|");
      sb.Append(NO_16 + "|");
      sb.Append(NO_17 + "|");
      sb.Append(NO_18 + "|");
      sb.Append(NO_19 + "|");
      sb.Append(NO_20 + "|");
      sb.Append(NO_21 + "\r\n");

      //----------------------------------------------------------------------------------------------------

      var complemento_nomina_percepciones = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina").Elements(nomina + "Percepciones") select c).FirstOrDefault();

      string PES_0 = "PES";
      string PES_1 = d2s(s2d(complemento_nomina_percepciones.Attribute("TotalGravado").Value));
      string PES_2 = d2s(s2d(complemento_nomina_percepciones.Attribute("TotalExento").Value));

      sb.Append(PES_0 + "|");
      sb.Append(PES_1 + "|");
      sb.Append(PES_2 + "\r\n");

      //----------------------------------------------------------------------------------------------------

      var complemento_nomina_percepciones_coll = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina").Elements(nomina + "Percepciones").Elements(nomina + "Percepcion") select c).DefaultIfEmpty();
      if (!(complemento_nomina_percepciones_coll.Count() == 1 && complemento_nomina_percepciones_coll.First() == null))
      {
        string PE_0 = "PE";
        string PE_1 = "TipoPercepcion";
        string PE_2 = "Clave";
        string PE_3 = "Concepto";
        string PE_4 = "ImporteGravado";
        string PE_5 = "ImporteExento";

        //XElement p;

        foreach (var percepcion in complemento_nomina_percepciones_coll)
        {
          //p = percepcion.Element(nomina + "Percepcion");
          PE_1 = percepcion.Attribute("TipoPercepcion").Value;
          PE_2 = percepcion.Attribute("Clave").Value;
          PE_3 = percepcion.Attribute("Concepto").Value;
          PE_4 = d2s(s2d(percepcion.Attribute("ImporteGravado").Value));
          PE_5 = d2s(s2d(percepcion.Attribute("ImporteExento").Value));

          sb.Append(PE_0 + "|");
          sb.Append(PE_1 + "|");
          sb.Append(PE_2 + "|");
          sb.Append(PE_3 + "|");
          sb.Append(PE_4 + "|");
          sb.Append(PE_5 + "\r\n");
        }
      }

      //----------------------------------------------------------------------------------------------------

      var complemento_nomina_deducciones = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina").Elements(nomina + "Deducciones") select c).FirstOrDefault();

      string DES_0 = "DES";

      string DES_1 = d2s(0.0M);
      string DES_2 = d2s(0.0M);
      if (complemento_nomina_deducciones != null)
      {
        DES_1 = d2s(s2d(complemento_nomina_deducciones.Attribute("TotalGravado").Value));
        DES_2 = d2s(s2d(complemento_nomina_deducciones.Attribute("TotalExento").Value));
      }

      if (DES_1 != "0.0000" && DES_2 != "0.0000")
      {
        sb.Append(DES_0 + "|");
        sb.Append(DES_1 + "|");
        sb.Append(DES_2 + "\r\n");
      }
      //----------------------------------------------------------------------------------------------------

      var complemento_nomina_deducciones_coll = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina").Elements(nomina + "Deducciones").Elements(nomina + "Deduccion") select c).DefaultIfEmpty();
      if (!(complemento_nomina_deducciones_coll.Count() == 1 && complemento_nomina_deducciones_coll.First() == null))
      {
        string DE_0 = "DE";
        string DE_1 = "TipoDeduccion";
        string DE_2 = "Clave";
        string DE_3 = "Concepto";
        string DE_4 = "ImporteGravado";
        string DE_5 = "ImporteExento";

        XElement p;

        foreach (var deduccion in complemento_nomina_deducciones_coll)
        {
          //p = deduccion.Element(nomina + "Deduccion");
          DE_1 = deduccion.Attribute("TipoDeduccion").Value;
          DE_2 = deduccion.Attribute("Clave").Value;
          DE_3 = deduccion.Attribute("Concepto").Value;
          DE_4 = d2s(s2d(deduccion.Attribute("ImporteGravado").Value));
          DE_5 = d2s(s2d(deduccion.Attribute("ImporteExento").Value));

          sb.Append(DE_0 + "|");
          sb.Append(DE_1 + "|");
          sb.Append(DE_2 + "|");
          sb.Append(DE_3 + "|");
          sb.Append(DE_4 + "|");
          sb.Append(DE_5 + "\r\n");
        }
      }

      //----------------------------------------------------------------------------------------------------

      var complemento_nomina_incapacidades_coll = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina").Elements(nomina + "Incapacidades").Elements(nomina + "Incapacidad") select c).DefaultIfEmpty();
      if (!(complemento_nomina_incapacidades_coll.Count() == 1 && complemento_nomina_incapacidades_coll.First() == null))
      {
        string IN_0 = "IN";
        string IN_1 = "DiasIncapacidad";
        string IN_2 = "TipoIncapacidad";
        string IN_3 = "descuento";

        XElement p;

        foreach (var incapacidad in complemento_nomina_incapacidades_coll)
        {
          //p = incapacidad.Element(nomina + "Incapacidad");
          IN_1 = d2s(s2d(incapacidad.Attribute("DiasIncapacidad").Value));
          IN_2 = incapacidad.Attribute("TipoIncapacidad").Value;
          IN_3 = d2s(s2d(incapacidad.Attribute("Descuento").Value));

          sb.Append(IN_0 + "|");
          sb.Append(IN_1 + "|");
          sb.Append(IN_2 + "|");
          sb.Append(IN_3 + "\r\n");
        }
      }

      //----------------------------------------------------------------------------------------------------

      var complemento_nomina_horasextras_coll = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina").Elements(nomina + "HorasExtras").Elements(nomina + "HorasExtra") select c).DefaultIfEmpty();
      if (!(complemento_nomina_horasextras_coll.Count() == 1 && complemento_nomina_horasextras_coll.First() == null))
      {
        string HE_0 = "HE";
        string HE_1 = "Dias";
        string HE_2 = "TipoHoras";
        string HE_3 = "HorasExtra";
        string HE_4 = "ImportePagado";

        XElement p;

        foreach (var he in complemento_nomina_horasextras_coll)
        {
          //p = he.Element(nomina + "HorasExtra");
          HE_1 = he.Attribute("Dias").Value;
          HE_2 = he.Attribute("TipoHoras").Value;
          HE_3 = he.Attribute("HorasExtra").Value;
          HE_4 = d2s(s2d(he.Attribute("ImportePagado").Value));

          sb.Append(HE_0 + "|");
          sb.Append(HE_1 + "|");
          sb.Append(HE_2 + "|");
          sb.Append(HE_3 + "|");
          sb.Append(HE_4 + "\r\n");
        }
      }

      //sw = new StreamWriter(GetConfigurationValues("dirXML") + H4_3 + "_" + H1_5 + ".NOM", false, Encoding.GetEncoding(1252), 512);
      sw = new StreamWriter(GetFinalDestination(H2_3) + H4_3 + "_" + H1_5 + ".NOM", false, Encoding.GetEncoding(1252), 512);
      sw.Write(sb.ToString());
      sw.Close();

      //}
      //catch (Exception e)
      //{
      //  file.CopyTo(GetConfigurationValues("ErrorFolder") + file.Name, true);

      //  StreamWriter sw = new StreamWriter(GetConfigurationValues("ErrorFolder") + "LogDeErrores.txt", true, Encoding.GetEncoding(1252), 512);
      //  sw.Write(file.Name + " - " + e.Message + " (" + e.InnerException + ") \n\n");
      //  sw.Close();

      //}
    }

    //---------------------------------------------------------------------------+
    private static void LogError(string fileName, string message)
    {
      //TeXmlError xmlError = new TeXmlError();
      //xmlError.TxtFileName = fileName;
      //xmlError.TxtErrorDetail = message;
      //xmlError.DtmRecord = DateTime.Now;
      //db.TeXmlErrors.InsertOnSubmit(xmlError);
      //db.SubmitChanges();
    }

    //---------------------------------------------------------------------------+
    private static decimal s2d(string value) //GetStringAsDecimal
    {
      decimal theResult;

      if (Decimal.TryParse(value, out theResult))
        return theResult;
      else
        return 0.0M;
    }

    //---------------------------------------------------------------------------+
    private static string d2s(decimal value) //GetDecimalAsStringWith4Decimals
    {
      return value.ToString("F4");
    }

    //---------------------------------------------------------------------------+
    //private static string GetNumberPart(string cell)
    //{
    //  //string cell = "ABCD4321";
    //  string numberpart = "0";

    //  int a = GetIndexofNumber(cell);

    //  if (a != -1)
    //  {
    //    numberpart = cell.Substring(a, cell.Length - a);
    //  }
    //  return numberpart;
    //  //return Convert.ToDecimal(numberpart);
    //  //string stringpart = cell.Substring(0, a);
    //}

    //---------------------------------------------------------------------------+
    //private static int GetIndexofNumber(string cell)
    //{
    //  int indexofNum = -1;
    //  foreach (char c in cell)
    //  {
    //    indexofNum++;
    //    if (Char.IsDigit(c))
    //    {
    //      return indexofNum;
    //    }
    //  }
    //  return -1;
    //}

    //

    //---------------------------------------------------------------------------+

    public static string GetConfigurationValues(string configKey)
    {
      //string confValue = @"C:\Users\rhernandez\Desktop\LEVICOM\TSP_FiniquitoMarzo15\";
      string confValue = @"C:\NominaInicio\";

      if (configKey == "BackupFolder")
        confValue = confValue + @"BACKUP\";

      if (configKey == "ErrorFolder")
        confValue = confValue + @"ERROR\";

      bool exists2 = System.IO.Directory.Exists(confValue);
      if (!exists2) System.IO.Directory.CreateDirectory(confValue);

      return confValue;
    }

    //---------------------------------------------------------------------------+

    public static string GetFinalDestination(string rfc)
    {
      string confValue = @"C:\Nomina\";

      switch(rfc)
      {
        case "GTU870812BQ6": //GRUPO TURIN S.A. DE C.V.
          confValue = @"C:\Nomina\GrupoTurin\IN\";
          break;
        case "SAI091203MU3": //SERVICIOS ADMINISTRATIVOS PARA LA INDUSTRIA DEL CHOCOLATE S DE RL DE CV
          confValue = @"C:\Nomina\SAIC\IN\";
          break;
        case "TAR080214S12": //TAR
          confValue = @"C:\Nomina\TAR\IN\";
          break;
        case "TSP1008164C9": //TURIN SERVICIOS PROFESIONALES S DE RL DE CV
          confValue = @"C:\Nomina\TSP\IN\";
          break;
        case "CTU830715D15": //CHOCOLATES TURIN S.A. DE C.V.
          confValue = @"C:\Nomina\Turin\IN\";
          break;
        default:
          confValue = @"C:\Nomina\";
          break;
        //case "": //Holdings
        //  confValue = confValue + @"C:\Nomina\Holdings\IN\";
        //  break;
      }

      bool exists2 = System.IO.Directory.Exists(confValue);
      if (!exists2) System.IO.Directory.CreateDirectory(confValue);

      return confValue;
    }
  }
}
