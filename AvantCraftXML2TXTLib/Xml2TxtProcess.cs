using dataaccessXML2TXT;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Data.Linq;
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
      AvantCraft_nomina2017Entities db = new AvantCraft_nomina2017Entities();
      //----------------------------------------------------------------------------------------------------
      //H1-DATOS GENERALES DE LOS CFDI´S

      XNamespace cfdi = "http://www.sat.gob.mx/cfd/3";
      XNamespace nomina = "http://www.sat.gob.mx/nomina";
      XElement root = XElement.Load(file.FullName);

      //#################### VALIDATION PREVIOUS PROCESSED DOCUMENT ##########################
      var val_complemento_nomina = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina") select c).FirstOrDefault();

      string val_NumEmpleado = val_complemento_nomina.Attribute("NumEmpleado").Value;
      string val_CURP = val_complemento_nomina.Attribute("CURP").Value;
      string val_FechaPago = val_complemento_nomina.Attribute("FechaPago").Value;
      string val_FechaInicialPago = val_complemento_nomina.Attribute("FechaInicialPago").Value;
      string val_FechaFinalPago = val_complemento_nomina.Attribute("FechaFinalPago").Value;

      var found = (from a in db.TE_Nomina
                   where a.Receptor_NumEmpleado == val_NumEmpleado
                         && a.Receptor_CURP == val_CURP
                         && a.FechaPago == val_FechaPago
                         && a.FechaInicialPago == val_FechaInicialPago
                         && a.FechaFinalPago == val_FechaFinalPago
                   select a).FirstOrDefault();

      if (found == null)
      {
        //#################### END VALIDATION PREVIOUS PROCESSED DOCUMENT ##########################

        string c_TipoNomina_confValue = ConfigurationManager.AppSettings["c_TipoNomina"].ToString();
        string version_confValue = ConfigurationManager.AppSettings["version"].ToString();
        string c_OrigenRecurso_confValue = ConfigurationManager.AppSettings["c_OrigenRecurso"].ToString();
        string c_TipoContrato_confValue = ConfigurationManager.AppSettings["c_TipoContrato"].ToString();

        //----------------------
        TE_TXT_HEADER dbHead = new TE_TXT_HEADER();

        dbHead.filename = file.Name;
        //********************************************************
        //***** SOLO PARA EFECTOS DE NOMINAS DE OTROS PERIODOS ***
        //var complemento_nomina2 = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina") select c).FirstOrDefault();
        //string H1_5 = DateTime.Parse(complemento_nomina2.Attribute("FechaPago").Value).ToString("yyyyddMM");
        //**** Se comenta la linea siguiente para fozar otra fecha
        string H1_5 = DateTime.Now.ToString("yyyyddMM");//0
        dbHead.H1_05 = H1_5;
        //********************************************************

        string H1_8 = root.Attribute("formaDePago").Value;//1
        dbHead.H1_08 = H1_8;
        string H1_11 = "RECIBO_NOMINA";//2
        dbHead.H1_11 = H1_11;

        string H1_14 = (root.Attribute("descuento") != null) ? d2s(s2d(root.Attribute("descuento").Value)) : "0.00"; //3
        dbHead.H1_14 = H1_14;

        string H1_30 = root.Attribute("LugarExpedicion").Value;//4
        dbHead.H1_30 = H1_30;
        string H1_31 = root.Attribute("metodoDePago").Value;//5
        dbHead.H1_31 = H1_31;
        string H1_32 = "MXN";//6
        dbHead.H1_32 = H1_32;
        string H1_46 = "P";//7
        dbHead.H1_46 = H1_46;
        string H1_50 = "NOM";//8
        dbHead.H1_50 = H1_50;

        //----------------------------------------------------------------------------------------------------
        //H2- DATOS DEL EMISOR

        var emisor = (from c in root.Elements(cfdi + "Emisor") select c).FirstOrDefault();

        string H2_2 = emisor.Attribute("nombre").Value; //0
        dbHead.H2_02 = H2_2;
        string H2_3 = emisor.Attribute("rfc").Value; //1
        dbHead.H2_03 = H2_3;

        var domicilioFiscal = (from c in root.Elements(cfdi + "Emisor").Elements(cfdi + "DomicilioFiscal") select c).FirstOrDefault();
        string H2_5 = domicilioFiscal.Attribute("calle").Value; //2
        dbHead.H2_05 = H2_5;
        string H2_6 = domicilioFiscal.Attribute("noInterior").Value; //3
        dbHead.H2_06 = H2_6;
        string H2_8 = domicilioFiscal.Attribute("colonia").Value; //4
        dbHead.H2_08 = H2_8;
        string H2_9 = domicilioFiscal.Attribute("localidad").Value; //5
        dbHead.H2_09 = H2_9;
        string H2_11 = domicilioFiscal.Attribute("municipio").Value; //6
        dbHead.H2_11 = H2_11;
        string H2_12 = domicilioFiscal.Attribute("estado").Value; //7
        dbHead.H2_12 = H2_12;
        string H2_13 = domicilioFiscal.Attribute("pais").Value;  //8
        dbHead.H2_13 = H2_13;
        string H2_14 = domicilioFiscal.Attribute("codigoPostal").Value; //9
        dbHead.H2_14 = H2_14;

        //----------------------------------------------------------------------------------------------------
        //H4- DATOS DEL RECEPTOR DEL CFDI

        var receptor = (from c in root.Elements(cfdi + "Receptor") select c).FirstOrDefault();

        string H4_2 = receptor.Attribute("nombre").Value; //0
        dbHead.H4_02 = H4_2;
        string H4_3 = receptor.Attribute("rfc").Value; //1
        dbHead.H4_03 = H4_3;

        var receptorDomicilio = (from c in root.Elements(cfdi + "Receptor").Elements(cfdi + "Domicilio") select c).FirstOrDefault();

        string H4_13 = receptorDomicilio.Attribute("pais").Value; //2
        dbHead.H4_13 = H4_13;

        //----------------------------------------------------------------------------------------------------
        //D- DATOS POR PARTIDA (PRODUCTOS) DEL CFDI

        var concepto = (from c in root.Elements(cfdi + "Conceptos").Elements(cfdi + "Concepto") select c).FirstOrDefault();

        string D_4 = concepto.Attribute("descripcion").Value; //0
        dbHead.D_04 = D_4;
        string D_6 = d2s(s2d(concepto.Attribute("cantidad").Value)); //1
        dbHead.D_06 = D_6;
        string D_7 = concepto.Attribute("unidad").Value; //2
        dbHead.D_07 = D_7;
        string D_9 = d2s(s2d(concepto.Attribute("importe").Value)); //3
        dbHead.D_09 = D_9;
        string D_25 = d2s(s2d(concepto.Attribute("valorUnitario").Value)); //4
        dbHead.D_25 = D_25;

        string D_37 = d2s(s2d(root.Attribute("subTotal").Value));//5  Total de percepciones = campo subTotal de la parte de comprobante (root)
        dbHead.D_37 = D_37;

        string D_38 = (root.Attribute("descuento") != null) ? d2s(s2d(root.Attribute("descuento").Value)) : "0.00"; //6
        dbHead.D_38 = D_38;

        var impuestos = (from c in root.Elements(cfdi + "Impuestos") select c).FirstOrDefault();

        string D_42 = (impuestos.Attribute("totalImpuestosRetenidos") != null) ? d2s(s2d(impuestos.Attribute("totalImpuestosRetenidos").Value)) : "0.0000"; //7
        dbHead.D_42 = D_42;

        //----------------------------------------------------------------------------------------------------
        //S- DATOS TOTALES DEL CFDI

        string S_10 = d2s(s2d(root.Attribute("total").Value)); //3
        dbHead.S_10 = S_10;

        var impuestos2 = (from c in root.Elements(cfdi + "Impuestos") select c).FirstOrDefault();

        string S_16 = (impuestos2.Attribute("totalImpuestosRetenidos") != null) ? d2s(s2d(impuestos2.Attribute("totalImpuestosRetenidos").Value)) : "0.0000"; //0
        dbHead.S_16 = S_16;

        var impuestos3 = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina").Elements(nomina + "Percepciones") select c).FirstOrDefault();

        string S_36 = d2s(s2d(impuestos3.Attribute("TotalExento").Value) + s2d(impuestos3.Attribute("TotalGravado").Value)); //1
        dbHead.S_36 = S_36;

        var impuestos4 = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina").Elements(nomina + "Deducciones") select c).FirstOrDefault();

        decimal totalExcento = 0.0M;
        decimal totalGravado = 0.0M;
        if (impuestos4 != null)
        {
          totalExcento = (impuestos4.Attribute("TotalExento") != null) ? s2d(impuestos4.Attribute("TotalExento").Value) : 0.0M;
          totalGravado = (impuestos4.Attribute("TotalGravado") != null) ? s2d(impuestos4.Attribute("TotalGravado").Value) : 0.0M;
        }


        string S_37 = d2s(totalExcento + totalGravado); //2
        dbHead.S_37 = S_37;

        //----------------------------------------------------------------------------------------------------

        db.TE_TXT_HEADER.Add(dbHead);
        db.SaveChanges();
        //====================================================================================================
        //============================= .NOM =================================================================
        //====================================================================================================

        TE_Nomina dbNO = new TE_Nomina();

        dbNO.c_TipoNomina = c_TipoNomina_confValue;
        dbNO.version = version_confValue;

        dbNO.Emisor_RfcPatronOrigen = H2_3;
        dbNO.Emisor_EntidadSNCF_c_OrigenRecurso = c_OrigenRecurso_confValue;
        dbNO.Receptor_c_TipoContrato = double.Parse(c_TipoContrato_confValue);

        //----------------------------------------------------------------------------------------------------

        var complemento_nomina = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina") select c).FirstOrDefault();

        string NO_00 = "NO";
        string NO_01 = complemento_nomina.Attribute("RegistroPatronal").Value;
        dbNO.Emisor_RegistroPatronal = NO_01;
        string NO_02 = complemento_nomina.Attribute("NumEmpleado").Value;
        dbNO.Receptor_NumEmpleado = NO_02;
        string NO_03 = complemento_nomina.Attribute("CURP").Value;
        dbNO.Receptor_CURP = NO_03;

        string NO_04 = complemento_nomina.Attribute("TipoRegimen").Value;
        dbNO.Receptor_TipoRegimen = NO_04;
        dbNO.Receptor_c_TipoRegimen = double.Parse(NO_04);

        string NO_05 = complemento_nomina.Attribute("NumSeguridadSocial").Value;
        dbNO.Receptor_NumSeguridadSocial = NO_05;
        string NO_06 = complemento_nomina.Attribute("FechaPago").Value;
        dbNO.FechaPago = NO_06;
        string NO_07 = complemento_nomina.Attribute("FechaInicialPago").Value;
        dbNO.FechaInicialPago = NO_07;
        string NO_08 = complemento_nomina.Attribute("FechaFinalPago").Value;
        dbNO.FechaFinalPago = NO_08;
        //string NO_09 = d2s(s2d(complemento_nomina.Attribute("NumDiasPagados").Value));
        dbNO.NumDiasPagados = s2d(complemento_nomina.Attribute("NumDiasPagados").Value);
        string NO_10 = "";
        string NO_11 = "";
        string NO_12 = "";
        string NO_13 = complemento_nomina.Attribute("FechaInicioRelLaboral").Value;
        dbNO.Receptor_FechaInicioRelLaboral = NO_13;
        string NO_14 = "";
        string NO_15 = (complemento_nomina.Attribute("Puesto") != null) ? complemento_nomina.Attribute("Puesto").Value : "EMPLEADO";
        dbNO.Receptor_Puesto = NO_15;
        string NO_16 = "";
        string NO_17 = "";
        string NO_18 = complemento_nomina.Attribute("PeriodicidadPago").Value;
        dbNO.Receptor_PeriodicidadPago = NO_18;
        dbNO.Receptor_c_PeriodicidadPago = getClavePeriodidicadPago(NO_18);

        //string NO_19 = (complemento_nomina.Attribute("SalarioBaseCotApor") != null) ? d2s(s2d(complemento_nomina.Attribute("SalarioBaseCotApor").Value)) : "0.00"; //3
        dbNO.Receptor_SalarioBaseCotApor = (complemento_nomina.Attribute("SalarioBaseCotApor") != null) ? s2d(complemento_nomina.Attribute("SalarioBaseCotApor").Value) : s2d("0.00"); //3

        string NO_20 = "";
        string NO_21 = (complemento_nomina.Attribute("SalarioDiarioIntegrado") != null) ? d2s(s2d(complemento_nomina.Attribute("SalarioDiarioIntegrado").Value)) : "0.00"; //3
        dbNO.Receptor_SalarioDiarioIntegrado = decimal.Parse(NO_21);

        //----------------------------------------------------------------------------------------------------

        var complemento_nomina_percepciones = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina").Elements(nomina + "Percepciones") select c).FirstOrDefault();

        string PES_0 = "PES";

        string PES_1 = d2s(s2d(complemento_nomina_percepciones.Attribute("TotalGravado").Value));
        dbNO.Percepciones_TotalGravado = s2d(complemento_nomina_percepciones.Attribute("TotalGravado").Value);
        string PES_2 = d2s(s2d(complemento_nomina_percepciones.Attribute("TotalExento").Value));
        dbNO.Percepciones_TotalExento = s2d(complemento_nomina_percepciones.Attribute("TotalExento").Value);

        dbNO.TotalPercepciones = s2d(complemento_nomina_percepciones.Attribute("TotalGravado").Value) + s2d(complemento_nomina_percepciones.Attribute("TotalExento").Value);
        dbNO.Emisor_EntidadSNCF_MontoRecursoPropio = s2d(complemento_nomina_percepciones.Attribute("TotalGravado").Value) + s2d(complemento_nomina_percepciones.Attribute("TotalExento").Value);

        db.TE_Nomina.Add(dbNO);
        db.SaveChanges();

        dbHead.nominaId = dbNO.nominaId;
        db.SaveChanges();
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
            TE_Percepcion dbPE = new TE_Percepcion();
            dbPE.nominaId = dbNO.nominaId;

            //p = percepcion.Element(nomina + "Percepcion");
            PE_1 = percepcion.Attribute("TipoPercepcion").Value;
            dbPE.TipoPercepcion = PE_1;
            PE_2 = percepcion.Attribute("Clave").Value;
            dbPE.Clave = PE_2;
            PE_3 = percepcion.Attribute("Concepto").Value;
            dbPE.Concepto = PE_3;
            PE_4 = d2s(s2d(percepcion.Attribute("ImporteGravado").Value));
            dbPE.ImporteGravado = s2d(percepcion.Attribute("ImporteGravado").Value);
            PE_5 = d2s(s2d(percepcion.Attribute("ImporteExento").Value));
            dbPE.ImporteExcento = s2d(percepcion.Attribute("ImporteExento").Value);

            db.TE_Percepcion.Add(dbPE);
            db.SaveChanges();
          }

        }
        //----------------------------------------------------------------------------------------------------

        var complemento_nomina_deducciones = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina").Elements(nomina + "Deducciones") select c).FirstOrDefault();
        if (complemento_nomina_deducciones != null)
        {
          string DES_0 = "DES";
          string DES_1 = d2s(s2d(complemento_nomina_deducciones.Attribute("TotalGravado").Value));
          string DES_2 = d2s(s2d(complemento_nomina_deducciones.Attribute("TotalExento").Value));

          decimal totalDeducciones = s2d(complemento_nomina_deducciones.Attribute("TotalGravado").Value) + s2d(complemento_nomina_deducciones.Attribute("TotalExento").Value);
          dbNO.TotalDeducciones = totalDeducciones;
          db.SaveChanges();
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
            TE_Deduccion dbDE = new TE_Deduccion();
            dbDE.nominaId = dbNO.nominaId;

            //p = deduccion.Element(nomina + "Deduccion");
            DE_1 = deduccion.Attribute("TipoDeduccion").Value;
            dbDE.TipoDeduccion = DE_1;
            DE_2 = deduccion.Attribute("Clave").Value;
            dbDE.Clave = DE_2;
            DE_3 = deduccion.Attribute("Concepto").Value;
            dbDE.Concepto = DE_3;
            DE_4 = d2s(s2d(deduccion.Attribute("ImporteGravado").Value));
            DE_5 = d2s(s2d(deduccion.Attribute("ImporteExento").Value));

            //++++
            decimal importe = s2d(deduccion.Attribute("ImporteGravado").Value) + s2d(deduccion.Attribute("ImporteExento").Value);
            dbDE.Importe = importe;
            db.TE_Deduccion.Add(dbDE);
            db.SaveChanges();

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
            TE_Incapacidad dbIN = new TE_Incapacidad();
            dbIN.nominaId = dbNO.nominaId;

            //p = incapacidad.Element(nomina + "Incapacidad");
            IN_1 = d2s(s2d(incapacidad.Attribute("DiasIncapacidad").Value));
            dbIN.DiasIncapacidad = Decimal.ToInt32(s2d(incapacidad.Attribute("DiasIncapacidad").Value));//Int32.Parse(d2s(s2d(incapacidad.Attribute("DiasIncapacidad").Value)));
            IN_2 = incapacidad.Attribute("TipoIncapacidad").Value;
            dbIN.TipoIncapacidad = IN_2;
            IN_3 = d2s(s2d(incapacidad.Attribute("Descuento").Value));
            dbIN.ImporteMonetario = s2d(incapacidad.Attribute("Descuento").Value);

            db.TE_Incapacidad.Add(dbIN);
            db.SaveChanges();

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

            //p = percepcion.Element(nomina + "Percepcion");
            TE_Percepcion dbPE = new TE_Percepcion();
            dbPE.nominaId = dbNO.nominaId;
            dbPE.TipoPercepcion = "Horas Extra";
            dbPE.Clave = "";
            dbPE.Concepto = "Horas Extra";
            dbPE.ImporteGravado = s2d(he.Attribute("ImportePagado").Value);
            dbPE.ImporteExcento = s2d("0.00");
            db.TE_Percepcion.Add(dbPE);
            db.SaveChanges();

            TE_Percepcion_HorasExtra dbPEHE = new TE_Percepcion_HorasExtra();
            dbPEHE.percepcionId = dbPE.percepcionId;

            dbPEHE.Dias = Int32.Parse(HE_1);
            dbPEHE.TipoHoras = HE_2;
            dbPEHE.HorasExtra = Int32.Parse(HE_3);
            dbPEHE.ImportePagado = s2d(he.Attribute("ImportePagado").Value);

            db.TE_Percepcion_HorasExtra.Add(dbPEHE);
            db.SaveChanges();
          }
        }

        //DATOS FIJOS
        TC_DatosFijosPorEmpleado dfxe = (from c in db.TC_DatosFijosPorEmpleado where c.rfcEmpleado == dbHead.H4_03 select c).FirstOrDefault();
        dbNO.Receptor_c_Banco = double.Parse(dfxe.c_Banco);
        dbNO.Receptor_CuentaBancaria = dfxe.CuentaBancaria;
        db.SaveChanges();
        //SUBCONTRATACION
        IQueryable<TC_Subcontratacion> sc = (from s in db.TC_Subcontratacion where s.RfcEmpleado == dbHead.H4_03 select s).DefaultIfEmpty();
        if (!(sc.Count() == 1 && sc.First() == null))
        {
          foreach (TC_Subcontratacion s in sc)
          {
            TE_Receptor_Subcontratacion rsc = new TE_Receptor_Subcontratacion();
            rsc.nominaId = dbNO.nominaId;
            rsc.RfcLabora = s.RfcLabora;
            rsc.PorcentajeTiempo = s.PorcentajeTiempo;
            db.SaveChanges();
          }
        }
      }
    }

    //---------------------------------------------------------------------------+
    private static double getClavePeriodidicadPago(string txtPeriodicidadPago)
    {
      double retPErPag = 0;

      AvantCraft_nomina2017Entities db = new AvantCraft_nomina2017Entities();

      c_PeriodicidadPago perPag = (from a in db.c_PeriodicidadPago where a.Descripcion == txtPeriodicidadPago select a).FirstOrDefault();


      if (perPag != null)
        retPErPag = perPag.c_PeriodicidadPago1;

      return retPErPag;
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
      return Utils.s2d(value);
    }

    //---------------------------------------------------------------------------+
    private static string d2s(decimal value) //GetDecimalAsStringWith4Decimals
    {
      return Utils.d2s(value);
    }

    //---------------------------------------------------------------------------+
    public static string GetConfigurationValues(string configKey)
    {
      return Utils.GetConfigurationValues(configKey);
    }

    //---------------------------------------------------------------------------+

    public static string GetFinalDestination(string rfc)
    {
      return Utils.GetFinalDestination(rfc);
    }

    //----------------------------------------------------------------------

    public void GetLAyoutsInExcel()
    {
      GetLayoutsInExcel.GetLayouts();
    }

    //----------------------------------------------------------------------
    public void GetComplementaryDataFromExcel(bool chkCargaSubcontratacion, bool chkFijos)
    {
      GetComplementaryData.Load(chkCargaSubcontratacion, chkFijos);
    }

    //----------------------------------------------------------------------
    public void PayRoll2TXT()
    {
      AvantCraft_nomina2017Entities db = new AvantCraft_nomina2017Entities();
      //---------------------- H1
      IQueryable<TE_Nomina> nominas = (from a in db.TE_Nomina select a).DefaultIfEmpty();

      foreach (TE_Nomina n in nominas)
      {
        TE_TXT_HEADER h = (from b in db.TE_TXT_HEADER where b.nominaId == n.nominaId select b).FirstOrDefault();
        string H1 = string.Format("[H1]||||{0}|||{1}|||{2}|||{3}||||||||||||||||{4}|{5}|{6}||||||||||||||{7}||||{8}|||||||||", h.H1_05, h.H1_08, h.H1_11, h.H1_14, h.H1_30, h.H1_31, h.H1_32, h.H1_46, h.H1_50);
        string H2 = string.Format("[H2]|{0}|{1}||{2}|{3}||{4}|{5}||{6}|{7}|{8}|{9}||||||||||||", h.H2_02, h.H2_03, h.H2_05, h.H2_06, h.H2_08, h.H2_09, h.H2_11, h.H2_12, h.H2_13, h.H2_14);
        string H3 = "[H3]|||||||||||||||||||||||||";
        string H4 = string.Format("[H4]|{0}|{1}||||||||||{2}|||||||||||||", h.H4_02, h.H4_03, h.H4_13);
        string H5 = "[H5]|||||||||||||||||||||||||";
        string D = string.Format("[D]|||{0}||{1}|{2}||{3}||||||||||||||||{4}||||||||||||{5}|{6}||||{7}||||||||||||||||||||||||||||||||", h.D_04, h.D_06, h.D_07, h.D_09, h.D_25, h.D_37, h.D_38, h.D_42);
        string S = string.Format("[S]|||||||||{3}||||||{0}||||||||||||||||||||{1}|{2}||||", h.S_16, h.S_36, h.S_37, h.S_10);
        
        //====================================================================================================
        //============================= .NOM =================================================================
        //====================================================================================================

        StringBuilder sb = new StringBuilder();

        sb.Append("[Nomina]" + "|");   //--- 0 fixed
        sb.Append("1.2" + "|");        //--- 1 fixed
        sb.Append("O" + "|");          //--- 2 fixed
        sb.Append(n.FechaPago + "|");
        sb.Append(n.FechaInicialPago + "|");
        sb.Append(n.FechaFinalPago + "|");
        sb.Append(n.NumDiasPagados + "|");
        sb.Append(n.TotalPercepciones + "|");
        sb.Append(n.TotalDeducciones + "|");
        sb.Append(n.TotalOtrosPagos + "|");
        sb.Append(Environment.NewLine);

        sb.Append("[EMISOR]" + "|");
        sb.Append(n.Emisor_CURP + "|");
        sb.Append(n.Emisor_RegistroPatronal + "|");
        sb.Append(n.Emisor_RfcPatronOrigen + "|");
        sb.Append(Environment.NewLine);

        sb.Append("[EntSNCF]" + "|");
        sb.Append(n.Emisor_EntidadSNCF_c_OrigenRecurso + "|");
        sb.Append(n.Emisor_EntidadSNCF_MontoRecursoPropio + "|");
        sb.Append(Environment.NewLine);

        sb.Append("[Receptor]" + " | ");
        sb.Append(n.Receptor_CURP + "|");
        sb.Append(n.Receptor_NumSeguridadSocial + "|");
        sb.Append(n.Receptor_FechaInicioRelLaboral + "|");
        sb.Append(n.Receptor_Antiguedad + "|");
        sb.Append(n.c_TipoContrato.c_TipoContrato1 + "|");
        sb.Append(n.Receptor_Sindicalizado + "|");
        sb.Append(n.c_TipoJornada.c_TipoJornada1 + "|");
        sb.Append(n.c_TipoRegimen.c_TipoRegimen1 + "|");
        sb.Append(n.Receptor_NumEmpleado + "|");
        sb.Append(n.Receptor_Departamento + "|");
        sb.Append(n.Receptor_Puesto + "|");
        sb.Append(n.c_RiesgoPuesto.c_RiesgoPuesto1 + "|");
        sb.Append(n.c_PeriodicidadPago.c_PeriodicidadPago1 + "|");
        sb.Append(n.c_Banco.c_Banco1 + "|");
        sb.Append(n.Receptor_CuentaBancaria + "|");
        sb.Append(n.Receptor_SalarioBaseCotApor + "|");
        sb.Append(n.Receptor_SalarioDiarioIntegrado + "|");
        sb.Append(n.Receptor_c_ClaveEntFed + "|");
        sb.Append(Environment.NewLine);

        var subc = (from suc in db.TE_Receptor_Subcontratacion where suc.nominaId == n.nominaId select suc).DefaultIfEmpty();
        foreach (TE_Receptor_Subcontratacion sub in subc)
        {
          sb.Append("[Subcontratacion]" + " | ");
          sb.Append(sub.RfcLabora + "|");
          sb.Append(sub.PorcentajeTiempo + "|");
          sb.Append(Environment.NewLine);
        }

        sb.Append("[Percepciones]" + " | ");
        sb.Append(n.Percepciones_TotalSueldos + "|");
        sb.Append(n.Percepciones_TotalSeparacionIndemnizacion + "|");
        sb.Append(n.Percepciones_TotalJubilacionPensionRetiro + "|");
        sb.Append(n.Percepciones_TotalGravado + "|");
        sb.Append(n.Percepciones_TotalExento + "|");
        sb.Append(Environment.NewLine);

        var ps = (from per in db.TE_Percepcion where per.nominaId == n.nominaId select per).DefaultIfEmpty();
        foreach (TE_Percepcion p in ps)
        {
          sb.Append("[Percepcion]" + "|");
          sb.Append(p.c_TipoPercepcion1.c_TipoPercepcion1 + "|");
          sb.Append(p.Clave + "|");
          sb.Append(p.Concepto + "|");
          sb.Append(p.ImporteGravado + "|");
          sb.Append(p.ImporteExcento + "|");
          sb.Append(Environment.NewLine);
        }

        //[AccionesOTitulos] contained in TE_Percepcion, not implemented by now

        IQueryable<TE_Percepcion_HorasExtra> hes = (from horexts in db.TE_Percepcion_HorasExtra where horexts.TE_Percepcion.nominaId == n.nominaId select horexts).DefaultIfEmpty();
        if (!(hes.Count() == 1 && hes.First() == null))
        {
          foreach (TE_Percepcion_HorasExtra he in hes)
          {
            sb.Append("[HorasExtra]" + "|");
            sb.Append(he.Dias + "|");
            sb.Append(he.c_TipoHoras1.c_TipoHoras1 + "|");
            sb.Append(he.HorasExtra + "|");
            sb.Append(he.ImportePagado + "|");
            sb.Append(Environment.NewLine);
          }
        }

        //[JubilacionPensionRetiro]  To implement
        //[SeparacionIndemnizacion]  To implement

        sb.Append("[Deducciones]" + " | ");
        sb.Append(n.Deducciones_TotalOtrasDeducciones + "|");
        sb.Append(n.Deducciones_TotalImpuestosRetenidos + "|");
        sb.Append(Environment.NewLine);

        var ds = (from ded in db.TE_Deduccion where ded.nominaId == n.nominaId select ded).DefaultIfEmpty();
        foreach (TE_Deduccion d in ds)
        {
          sb.Append("[Deduccion]" + "|");
          sb.Append(d.c_TipoDeduccion1.c_TipoDeduccion1 + "|");
          sb.Append(d.Clave + "|");
          sb.Append(d.Concepto + "|");
          sb.Append(d.Importe + "|");
          sb.Append(Environment.NewLine);
        }

        sb.Append("[OtrosPagos]" + " | ");
        sb.Append(Environment.NewLine);

        var os = (from ops in db.TE_OtroPago where ops.nominaId == n.nominaId select ops).DefaultIfEmpty();
        foreach (TE_OtroPago o in os)
        {
          sb.Append("[OtroPago]" + "|");
          sb.Append(o.c_TipoOtroPago1.c_TipoOtroPago1 + "|");
          sb.Append(o.Clave + "|");
          sb.Append(o.Concepto + "|");
          sb.Append(o.Importe + "|");
          sb.Append(Environment.NewLine);
        }

        //[SubsidioAlEmpleo]  To implement
        //[CompensacionSaldosAFavor]  To implement

        sb.Append("[Incapacidades]" + " | ");
        sb.Append(Environment.NewLine);

        var ins = (from incs in db.TE_Incapacidad where incs.nominaId == n.nominaId select incs).DefaultIfEmpty();
        foreach (TE_Incapacidad i in ins)
        {
          sb.Append("[Incapacidad]" + "|");
          sb.Append(i.DiasIncapacidad + "|");
          sb.Append(i.c_TipoIncapacidad1.c_TipoIncapacidad1 + "|");
          sb.Append(i.ImporteMonetario + "|");
          sb.Append(Environment.NewLine);
        }

        
        try
        {
          string textToPrintHead = H1 + Environment.NewLine + H2 + Environment.NewLine + H3 + Environment.NewLine + H4 + Environment.NewLine + H5 + Environment.NewLine + D + Environment.NewLine + S + Environment.NewLine;
          TextWriter sw = new StreamWriter(GetFinalDestination(h.H2_03) + h.H4_03 + "_" + h.H1_05 + ".txt", false, Encoding.GetEncoding(1252), 512);
          sw.Write(textToPrintHead + sb.ToString());
          sw.Close();
        }
        catch (Exception e)
        {
          StreamWriter sw = new StreamWriter(GetConfigurationValues("ErrorFolder") + "LogDeErrores.txt", true, Encoding.GetEncoding(1252), 512);
          sw.Write(h.H4_03 + "_" + h.H1_05 + " :: " + e.Message + " (" + e.InnerException + ")"  + Environment.NewLine + Environment.NewLine);
          sw.Close();
        }
      } // for each nomina

    }
  }
}

