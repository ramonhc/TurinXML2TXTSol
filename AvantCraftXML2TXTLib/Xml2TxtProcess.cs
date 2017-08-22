using dataaccessXML2TXT;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AvantCraftXML2TXTLib
{
    public class Xml2TxtProcess
    {

        private static XNamespace cfdi = "http://www.sat.gob.mx/cfd/3";
        private static XNamespace nomina = "http://www.sat.gob.mx/nomina";


        //----------------------------------------------------------------------------------------------------
        public void Processfiles(string txtPeriodo)
        {
            DirectoryInfo dir = new DirectoryInfo(GetConfigurationValues("dirXML") + txtPeriodo + "\\");
            FileInfo[] files;

            files = dir.GetFiles("*.xml");

            foreach (FileInfo file in files)
            {
                string bkpDtmFolder = GetConfigurationValues("BackupFolder") + txtPeriodo + "\\";
                bool exists3 = System.IO.Directory.Exists(bkpDtmFolder);
                if (!exists3) System.IO.Directory.CreateDirectory(bkpDtmFolder);

                file.CopyTo(bkpDtmFolder + file.Name, true);
                ParseXml(file, txtPeriodo);
                file.Delete();
            }
            dir.Delete();
        }

        //----------------------------------------------------------------------------------------------------

        private static void ParseXml(FileInfo file, string txtPeriodo)
        {
            AvantCraft_nomina2017Entities db = new AvantCraft_nomina2017Entities();
            //----------------------------------------------------------------------------------------------------
            //H1-DATOS GENERALES DE LOS CFDI´S


            XElement root = XElement.Load(file.FullName);

            //#################### VALIDATION PREVIOUS PROCESSED DOCUMENT ##########################
            var val_complemento_nomina = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina") select c).FirstOrDefault();

            string val_NumEmpleado = val_complemento_nomina.Attribute("NumEmpleado").Value;
            string val_CURP = val_complemento_nomina.Attribute("CURP").Value;

            // get real CURP n RFC 
            TC_RFC realRfcNCurp = (from a in db.TC_RFC where a.txtNumEmp == val_NumEmpleado select a).FirstOrDefault();

            // --------------------use real curp but if none... then create'em with available info
            if (realRfcNCurp == null)
            {
                realRfcNCurp = new TC_RFC();
                realRfcNCurp.txtCURP = val_CURP;
                realRfcNCurp.txtNumEmp = val_NumEmpleado;

                var receptor = (from c in root.Elements(cfdi + "Receptor") select c).FirstOrDefault();
                realRfcNCurp.txtNombre = receptor.Attribute("nombre").Value;
                realRfcNCurp.txyRfc = receptor.Attribute("rfc").Value;
                db.TC_RFC.Add(realRfcNCurp);
                db.SaveChanges();

                TC_DatosFijosPorEmpleado aDfxe = (from a in db.TC_DatosFijosPorEmpleado where a.txtPeriodo == txtPeriodo && a.rfcEmpleado == realRfcNCurp.txyRfc select a).FirstOrDefault();
                if (aDfxe != null)
                {
                    aDfxe.txtNumEmpleado = realRfcNCurp.txtNumEmp;
                    db.SaveChanges();
                }
            }
            val_CURP = realRfcNCurp.txtCURP;
            //----


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

                //string H1_14 = (root.Attribute("descuento") != null) ? d2s(s2d(root.Attribute("descuento").Value)) : "0.00"; //3
                //dbHead.H1_14 = H1_14;

                dbHead.H1_14 = getRealDescuento(root).ToString();

                string H1_30 = root.Attribute("LugarExpedicion").Value;//4
                dbHead.H1_30 = H1_30;  //- - - - - - - - - - - - - - - - - - - > changed to C.P. a few lines below H2_14
                string H1_31 = root.Attribute("metodoDePago").Value;//5
                dbHead.H1_31 = H1_31;  // - - - - > should be "NA" so we change it next
                dbHead.H1_31 = "NA";
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

                //- - - - - -  < here we change this var
                dbHead.H1_30 = H2_14;

                //***** NOTA.- No se debe registrar el domicilio fiscal, borramos aquí los campos:
                dbHead.H2_05 = string.Empty;
                dbHead.H2_06 = string.Empty;
                dbHead.H2_08 = string.Empty;
                dbHead.H2_09 = string.Empty;
                dbHead.H2_11 = string.Empty;
                dbHead.H2_12 = string.Empty;
                dbHead.H2_13 = string.Empty;
                dbHead.H2_14 = string.Empty;


                //----------------------------------------------------------------------------------------------------
                //H4- DATOS DEL RECEPTOR DEL CFDI

                var receptor = (from c in root.Elements(cfdi + "Receptor") select c).FirstOrDefault();

                string H4_2 = receptor.Attribute("nombre").Value; //0
                dbHead.H4_02 = H4_2;
                string H4_3 = receptor.Attribute("rfc").Value; //1

                // use real RFC
                if (realRfcNCurp != null)
                {
                    H4_3 = realRfcNCurp.txyRfc;
                }
                //----

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
                dbHead.D_07 = "ACT"; //Para este atributo se debe registrar el valor “ACT”
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
                dbHead.D_42 = string.Empty; //No debe registrarse

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
                dbNO.Receptor_Puesto = NO_15.Replace('.', ' ').Replace('(', ' ').Replace(')', ' ').Replace('(', ' ').Replace(')', ' ').Replace('&', 'N');
                string NO_16 = "";
                string NO_17 = "";
                string NO_18 = complemento_nomina.Attribute("PeriodicidadPago").Value;
                dbNO.Receptor_PeriodicidadPago = NO_18;
                dbNO.Receptor_c_PeriodicidadPago = getClavePeriodidicadPago(NO_18);

                //dbNO.periodo = setPeriodo(NO_08, NO_18);  // NO_08 = FechaFinalPago    NO_18 = quincenal/Semanal
                dbNO.periodo = txtPeriodo;


                //string NO_19 = (complemento_nomina.Attribute("SalarioBaseCotApor") != null) ? d2s(s2d(complemento_nomina.Attribute("SalarioBaseCotApor").Value)) : "0.00"; //3
                dbNO.Receptor_SalarioBaseCotApor = (complemento_nomina.Attribute("SalarioBaseCotApor") != null) ? s2d(complemento_nomina.Attribute("SalarioBaseCotApor").Value) : s2d("0.00"); //3

                string NO_20 = "";
                string NO_21 = (complemento_nomina.Attribute("SalarioDiarioIntegrado") != null) ? d2s(s2d(complemento_nomina.Attribute("SalarioDiarioIntegrado").Value)) : "0.00"; //3
                dbNO.Receptor_SalarioDiarioIntegrado = decimal.Parse(NO_21);

                //----------------------------------------------------------------------------------------------------

                var complemento_nomina_percepciones = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina").Elements(nomina + "Percepciones") select c).FirstOrDefault();

                string PES_0 = "PES";

                //--- PERCEPCION total percepcion
                dbNO.Percepciones_TotalSueldos = decimal.Parse(dbHead.D_37);
                //dbNO.Percepciones_TotalSeparacionIndemnizacion]
                //dbNO.Percepciones_TotalJubilacionPensionRetiro]

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
                        PE_1 = percepcion.Attribute("TipoPercepcion").Value;
                        PE_2 = percepcion.Attribute("Clave").Value;
                        PE_3 = percepcion.Attribute("Concepto").Value.Replace(".", string.Empty);
                        PE_4 = d2s(s2d(percepcion.Attribute("ImporteGravado").Value));
                        PE_5 = d2s(s2d(percepcion.Attribute("ImporteExento").Value));

                        //--- find negatives
                        decimal PerImpG = s2d(percepcion.Attribute("ImporteGravado").Value);
                        decimal PerImpX = s2d(percepcion.Attribute("ImporteExento").Value);

                        if (PerImpG < 0 || PerImpX < 0)  //--- en caso de percepcion negativa se registra en Deducción 'Otros'
                        {
                            TE_Deduccion dbDE = new TE_Deduccion();
                            dbDE.nominaId = dbNO.nominaId;
                            dbDE.TipoDeduccion = "004";
                            dbDE.Clave = PE_2.Replace("/", "");                                 //Remove dashes ("/")
                            dbDE.c_TipoDeduccion = 4;
                            dbDE.Concepto = PE_3.Replace(".", string.Empty);
                            dbDE.Importe = Math.Abs(PerImpG + PerImpX);
                            db.TE_Deduccion.Add(dbDE);
                            db.SaveChanges();
                        }
                        else
                        {
                            if (PE_1 != "017" && PE_1 != "016")   // en caso de ser = 17, es un Subsidio,  016 = otro .. debe considerarse en: TE_OtroPago
                            {
                                TE_Percepcion dbPE = new TE_Percepcion();
                                dbPE.nominaId = dbNO.nominaId;

                                //p = percepcion.Element(nomina + "Percepcion");
                                //find c_TipoPercepcion ID
                                c_TipoPercepcion ctp = (from p in db.c_TipoPercepcion where p.Value == PE_1 select p).FirstOrDefault();
                                if (ctp != null) dbPE.c_TipoPercepcion = ctp.c_TipoPercepcion1;
                                else dbPE.c_TipoPercepcion = 1;

                                dbPE.TipoPercepcion = PE_1;
                                dbPE.Clave = PE_2.Replace("/", "");                                 //Remove dashes ("/")
                                dbPE.Concepto = PE_3.Replace(".", string.Empty);
                                dbPE.ImporteGravado = PerImpG;
                                dbPE.ImporteExcento = PerImpX;

                                db.TE_Percepcion.Add(dbPE);
                            }
                            else
                            {
                                TE_OtroPago dbOP = new TE_OtroPago();
                                dbOP.nominaId = dbNO.nominaId;
                                dbOP.c_TipoOtroPago = (PE_1 == "017") ? 2 : 999;  //subsidio, debe ser 002
                                dbOP.Clave = PE_2.Replace("/", "");                                 //Remove dashes ("/")
                                dbOP.Concepto = PE_3.Replace(".", string.Empty);
                                dbOP.Importe = s2d(percepcion.Attribute("ImporteExento").Value) + s2d(percepcion.Attribute("ImporteGravado").Value);
                                db.TE_OtroPago.Add(dbOP);
                            }

                            db.SaveChanges();
                        }
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

                    decimal Deducciones_TotalOtrasDeducciones = 0;
                    decimal Deducciones_TotalImpuestosRetenidos = 0;

                    string DE_0 = "DE";
                    string DE_1 = "TipoDeduccion";
                    string DE_2 = "Clave";
                    string DE_3 = "Concepto";
                    string DE_4 = "ImporteGravado";
                    string DE_5 = "ImporteExento";

                    XElement p;

                    foreach (var deduccion in complemento_nomina_deducciones_coll)
                    {
                        DE_1 = deduccion.Attribute("TipoDeduccion").Value;
                        DE_2 = deduccion.Attribute("Clave").Value;
                        DE_3 = deduccion.Attribute("Concepto").Value.Replace(".", string.Empty);
                        DE_4 = d2s(s2d(deduccion.Attribute("ImporteGravado").Value));
                        DE_5 = d2s(s2d(deduccion.Attribute("ImporteExento").Value));

                        //--- find negatives
                        decimal DedImpG = s2d(deduccion.Attribute("ImporteGravado").Value);
                        decimal DedImpX = s2d(deduccion.Attribute("ImporteExento").Value);

                        if (DedImpG < 0 || DedImpX < 0)  //--- en caso de deducción negativa se registra en OtroPago 
                        {
                            TE_OtroPago dbOPd = new TE_OtroPago();
                            dbOPd.nominaId = dbNO.nominaId;
                            dbOPd.c_TipoOtroPago = 999;
                            dbOPd.Clave = DE_2.Replace("/", "");                                 //Remove dashes ("/")
                            dbOPd.Concepto = DE_3.Replace(".", string.Empty);
                            dbOPd.Importe = Math.Abs(DedImpG + DedImpX);
                            db.TE_OtroPago.Add(dbOPd);
                        }
                        else
                        {
                            TE_Deduccion dbDE = new TE_Deduccion();
                            dbDE.nominaId = dbNO.nominaId;
                            c_TipoDeduccion ctd = (from p2 in db.c_TipoDeduccion where p2.Value == DE_1 select p2).FirstOrDefault();
                            if (ctd != null) dbDE.c_TipoDeduccion = ctd.c_TipoDeduccion1;
                            else dbDE.c_TipoDeduccion = 1;
                            dbDE.TipoDeduccion = DE_1;
                            dbDE.Clave = DE_2;
                            dbDE.Concepto = DE_3.Replace(".", string.Empty);
                            //++++
                            decimal importe = s2d(deduccion.Attribute("ImporteGravado").Value) + s2d(deduccion.Attribute("ImporteExento").Value);
                            dbDE.Importe = importe;
                            db.TE_Deduccion.Add(dbDE);
                            db.SaveChanges();

                            if (DE_1.Trim() == "002")
                            {
                                Deducciones_TotalImpuestosRetenidos += importe;
                            }
                            else
                            {
                                Deducciones_TotalOtrasDeducciones += importe;
                            }
                        }

                        dbNO.Deducciones_TotalOtrasDeducciones = Deducciones_TotalOtrasDeducciones;
                        dbNO.Deducciones_TotalImpuestosRetenidos = Deducciones_TotalImpuestosRetenidos;
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
                        dbIN.c_TipoIncapacidad = double.Parse(IN_2);
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


                        //* ***** SE USARA EN EL TXT FINAL EN LUGAR DEL RESTO DE LAS CLAVES 19 PORQUE SE DUPLICA CON NODOS PERCEPCION:TIEMPO EXTRA DOBLE, ETC.
                        //--- no debe incluirse en la lista de percepciones **** se usa solo para ligar el detalle con los headers
                        //p = percepcion.Element(nomina + "Percepcion");
                        TE_Percepcion dbPE = new TE_Percepcion();
                        dbPE.nominaId = dbNO.nominaId;
                        dbPE.TipoPercepcion = "Horas Extra";
                        //dbPE.Clave = "";
                        dbPE.Clave = "REMPLAZO";                         //FIXED
                        dbPE.Concepto = "Horas Extra";
                        dbPE.ImporteGravado = s2d(he.Attribute("ImportePagado").Value);
                        dbPE.ImporteExcento = s2d("0.00");
                        db.TE_Percepcion.Add(dbPE);
                        db.SaveChanges();



                        TE_Percepcion_HorasExtra dbPEHE = new TE_Percepcion_HorasExtra();
                        dbPEHE.percepcionId = dbPE.percepcionId;

                        //dbPEHE.Dias = Int32.Parse(HE_1);
                        dbPEHE.Dias = 1;                           //FIXED

                        dbPEHE.TipoHoras = HE_2;
                        switch (HE_2)
                        {
                            case "Dobles":
                                dbPEHE.c_TipoHoras = 1;
                                break;
                            case "Triples":
                                dbPEHE.c_TipoHoras = 2;
                                break;
                            case "Simples":
                                dbPEHE.c_TipoHoras = 3;
                                break;
                            default:
                                dbPEHE.c_TipoHoras = 1;
                                break;
                        }
                        dbPEHE.HorasExtra = Int32.Parse(HE_3);
                        dbPEHE.ImportePagado = s2d(he.Attribute("ImportePagado").Value);

                        db.TE_Percepcion_HorasExtra.Add(dbPEHE);
                        db.SaveChanges();
                    }
                }

                //----------- DATOS FIJOS  ------------- 
                TC_DatosFijosPorEmpleado dfxe = (from c in db.TC_DatosFijosPorEmpleado where c.txtNumEmpleado == dbNO.Receptor_NumEmpleado && c.txtPeriodo == txtPeriodo select c).FirstOrDefault();
                dbNO.Receptor_c_Banco = double.Parse(dfxe.c_Banco);
                dbNO.Receptor_CuentaBancaria = dfxe.CuentaBancaria;
                dbNO.Receptor_c_ClaveEntFed = dfxe.c_Estado;
                db.SaveChanges();
                //SUBCONTRATACION
                IQueryable<TC_Subcontratacion> sc = (from s in db.TC_Subcontratacion where s.txtNumEmpleado == dbNO.Receptor_NumEmpleado && s.txtPeriodo == txtPeriodo select s).DefaultIfEmpty();
                if (!(sc.Count() == 1 && sc.First() == null))
                {
                    foreach (TC_Subcontratacion s in sc)
                    {
                        if (s.PorcentajeTiempo > 0)
                        {
                            TE_Receptor_Subcontratacion rsc = new TE_Receptor_Subcontratacion();
                            rsc.nominaId = dbNO.nominaId;
                            rsc.RfcLabora = s.RfcLabora;
                            rsc.PorcentajeTiempo = s.PorcentajeTiempo;
                            db.TE_Receptor_Subcontratacion.Add(rsc);
                        }
                    }
                    db.SaveChanges();
                }
                //FIX CATALOG REFERENCES
                //-- FIX c_TipoPercepcion
                IQueryable<TE_Percepcion> fixPer = (from per in db.TE_Percepcion where per.nominaId == dbNO.nominaId && per.Clave != "REMPLAZO" select per).DefaultIfEmpty();
                if (!(fixPer.Count() == 1 && fixPer.First() == null))
                {
                    foreach (TE_Percepcion p in fixPer)
                    {
                        try
                        {
                            double parsedRef = 1;
                            if (double.TryParse(p.TipoPercepcion, out parsedRef))
                            {
                                p.c_TipoPercepcion = parsedRef;
                            }
                        }
                        catch
                        {
                            switch (p.TipoPercepcion)
                            {
                                case "Horas Extra":
                                    p.c_TipoPercepcion = 19;
                                    break;
                                default:
                                    p.c_TipoPercepcion = 1;
                                    break;
                            }
                        }

                    }
                    db.SaveChanges();
                }
                //-- FIX c_TipoHoras
                IQueryable<TE_Percepcion_HorasExtra> hexs = (from hexss in db.TE_Percepcion_HorasExtra where hexss.TE_Percepcion.nominaId == dbNO.nominaId select hexss).DefaultIfEmpty();
                if (!(hexs.Count() == 1 && hexs.First() == null))
                {
                    foreach (TE_Percepcion_HorasExtra p in hexs)
                    {
                        switch (p.TipoHoras)
                        {
                            case "Dobles":
                                p.c_TipoHoras = 1;
                                break;
                            case "Triples":
                                p.c_TipoHoras = 2;
                                break;
                            default:
                                p.c_TipoHoras = 3;
                                break;
                        }

                    }
                    db.SaveChanges();
                }

                //-- FIX c_TipoDeduccion
                IQueryable<TE_Deduccion> dexs = (from d in db.TE_Deduccion where d.nominaId == dbNO.nominaId select d).DefaultIfEmpty();
                if (!(dexs.Count() == 1 && dexs.First() == null))
                {
                    foreach (TE_Deduccion p in dexs)
                    {
                        try
                        {
                            double parsedRef = 4;
                            if (double.TryParse(p.TipoDeduccion, out parsedRef))
                            {
                                p.c_TipoDeduccion = parsedRef;
                            }
                        }
                        catch
                        {
                            p.c_TipoDeduccion = 4;
                        }

                    }
                    db.SaveChanges();
                }

                //-- FIX c_TipoIncapacidad
                //--- Also fix: Error: NOM215: El atributo Deduccion:Importe no es igual a la suma de los nodos Incapacidad:ImporteMonetario. Ya que la clave expresada en Nomina.Deducciones.Deduccion.TipoDeduccion es 006

                IQueryable<TE_Incapacidad> incs = (from d in db.TE_Incapacidad where d.nominaId == dbNO.nominaId select d).DefaultIfEmpty();
                if (!(incs.Count() == 1 && incs.First() == null))
                {
                    foreach (TE_Incapacidad p in incs)
                    {
                        //--- clave
                        try
                        {
                            double parsedRef = 4;
                            if (double.TryParse(p.TipoIncapacidad, out parsedRef))
                            {
                                p.c_TipoIncapacidad = parsedRef;
                            }
                        }
                        catch
                        {
                            p.c_TipoIncapacidad = 2;
                        }

                        //--- Importe
                        TE_Deduccion dedIncs = (from d in db.TE_Deduccion where d.nominaId == dbNO.nominaId && d.c_TipoDeduccion == 6 select d).FirstOrDefault();
                        if (dedIncs != null)
                        {
                            p.ImporteMonetario = dedIncs.Importe;
                        }


                    }


                    db.SaveChanges();
                }
                //------------- MARK Deducciones and/or Percepciones as Exclusions if needed
                //------------- Find VALES concepts to exclude
                /*
                 List<Tuple<int,int>> excludePercecpcionDeductionList = (from a in db.TE_Percepcion join b in db.TE_Deduccion on a.nominaId equals b.nominaId
                                                                              where a.Clave == "8R50" && a.ImporteExcento.Value == 2265.00m && b.TipoDeduccion == "004" && b.Clave == "9015" && a.ImporteGravado == b.Importe  select (new Tuple<int,int>(a.percepcionId, b.deduccionId))).ToList();
                */
                List<int> ExcludedPercepciones = (from a in db.TE_Percepcion
                                                  join b in db.TE_Deduccion on a.nominaId equals b.nominaId
                                                  where a.Clave == "8R50" && a.ImporteExcento.Value == 2265.00m && b.TipoDeduccion == "004" && b.Clave == "9015" && a.ImporteGravado == b.Importe && a.nominaId == dbNO.nominaId
                                                  select a.percepcionId).ToList();

                List<int> ExcludedDeducciones = (from a in db.TE_Percepcion
                                                 join b in db.TE_Deduccion on a.nominaId equals b.nominaId
                                                 where a.Clave == "8R50" && a.ImporteExcento.Value == 2265.00m && b.TipoDeduccion == "004" && b.Clave == "9015" && a.ImporteGravado == b.Importe && a.nominaId == dbNO.nominaId
                                                 select b.deduccionId).ToList();

                //----- exclude in DB
                (from up in db.TE_Percepcion where ExcludedPercepciones.Contains(up.percepcionId) && up.nominaId == dbNO.nominaId select up).ToList().ForEach(p => p.bolExclude = true);
                (from up in db.TE_Deduccion where ExcludedDeducciones.Contains(up.deduccionId) && up.nominaId == dbNO.nominaId select up).ToList().ForEach(p => p.bolExclude = true);
                db.SaveChanges();

                //------------- FIX TotalSueldos, TotalGravado, TotalExcento, TotalDeducciones, TotalOtrosPagos

                //----- FIX TotalGravado
                decimal fixTotalGravado = (from a in db.TE_Percepcion where a.nominaId == dbNO.nominaId && a.Clave != "REMPLAZO" && a.bolExclude != true select a.ImporteGravado).Sum().Value;
                dbNO.Percepciones_TotalGravado = fixTotalGravado;

                //----- FIX TotalExcento
                decimal fixTotalExcento = (from a in db.TE_Percepcion where a.nominaId == dbNO.nominaId && a.Clave != "REMPLAZO" && a.bolExclude != true select a.ImporteExcento).Sum().Value;
                dbNO.Percepciones_TotalExento = fixTotalExcento;

                //----- FIX TotalDeducciones
                //El valor de [Nomina/TotalDeducciones] debe ser igual a la suma de valores de los atributos [Deducciones/(TotalOtrasDeducciones+TotalImpuestosRetenidos)]

                decimal totOtrasDed = (dbNO.Deducciones_TotalOtrasDeducciones.HasValue) ? dbNO.Deducciones_TotalOtrasDeducciones.Value : 0;
                decimal totImpReten = (dbNO.Deducciones_TotalImpuestosRetenidos.HasValue) ? dbNO.Deducciones_TotalImpuestosRetenidos.Value : 0;
                decimal fixTotalDeducciones = totOtrasDed + totImpReten; // (from a in dbNO where a.nominaId == dbNO.nominaId && a.Clave != "REMPLAZO" select a.ImporteExcento).Sum().Value;
                dbNO.TotalDeducciones = fixTotalDeducciones;
                dbHead.H1_14 = fixTotalDeducciones.ToString(); //-- also for "NOM107: El valor de descuento no es igual a Nomina12:TotalDeducciones."

                //----- FIX TotalOtrosPagos
                decimal fixTotalOtrosPagos = 0;
                var checkTotalOtrosPagos = (from a in db.TE_OtroPago where a.nominaId == dbNO.nominaId select a).DefaultIfEmpty();
                if (!(checkTotalOtrosPagos.Count() == 1 && checkTotalOtrosPagos.First() == null))
                {
                    fixTotalOtrosPagos = (from a in db.TE_OtroPago where a.nominaId == dbNO.nominaId select a.Importe).Sum().Value;
                    dbNO.TotalOtrosPagos = fixTotalOtrosPagos;
                }

                //----- FIX TotalSueldos
                decimal fixTotalSueldos = fixTotalGravado + fixTotalExcento;
                dbNO.Percepciones_TotalSueldos = fixTotalSueldos;

                //----- FIX TotalPercepciones
                // El valor de [Nomina/TotalPercepciones] debe ser igual a la suma de valores de los atributos [Percepciones/(TotalSueldos+TotalJubilacionPensionRetiro+TotalSeparacionIndemnizacion)]
                decimal fixTotalPercepciones = fixTotalSueldos;
                dbNO.TotalPercepciones = fixTotalPercepciones;

                //----- Segmento: [Nomina], Campo: [NumDiasPagados], Valor: [0]. Error: Es menor al límite del mínimo incluyente [0.001].
                if (dbNO.NumDiasPagados < 0.001m) dbNO.NumDiasPagados = 0.001m;



                //----- FIX HEAD


                //----- FIX HEAD SUBtotal
                dbHead.D_09 = (fixTotalPercepciones + fixTotalOtrosPagos).ToString();
                //NOM109: El valor del atributo total :: suma Nomina12:TotalPercepciones más Nomina12:TotalOtrosPagos menos Nomina12:TotalDeducciones
                dbHead.D_25 = (fixTotalPercepciones + fixTotalOtrosPagos).ToString();
                dbHead.D_37 = (fixTotalPercepciones + fixTotalOtrosPagos).ToString();
                dbHead.S_36 = (fixTotalPercepciones + fixTotalOtrosPagos).ToString();

                //----- FIX HEAD total
                //NOM109: El valor del atributo total :: suma Nomina12:TotalPercepciones más Nomina12:TotalOtrosPagos menos Nomina12:TotalDeducciones
                dbHead.S_10 = (fixTotalPercepciones + fixTotalOtrosPagos - fixTotalDeducciones).ToString();

                db.SaveChanges();


            } // end if found .. line 66
        }


        //---------------------------------------------------------------------------+
        private static decimal getRealDescuento(XElement root)
        {
            decimal totalDeducciones = 0;
            var complemento_nomina_deducciones = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina").Elements(nomina + "Deducciones") select c).FirstOrDefault();
            if (complemento_nomina_deducciones != null)
            {
                totalDeducciones = s2d(complemento_nomina_deducciones.Attribute("TotalGravado").Value) + s2d(complemento_nomina_deducciones.Attribute("TotalExento").Value);
            }
            return totalDeducciones;
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
        public void GetComplementaryDataFromExcel(bool chkCargaSubcontratacion, bool chkFijos, string aPeriodo)
        {
            GetComplementaryData.Load(chkCargaSubcontratacion, chkFijos, aPeriodo);
        }

        //----------------------------------------------------------------------
        public void PayRoll2TXT(string txt_Periodo, bool useRfcExclusionList, bool useRfcIncludeList)
        {
            AvantCraft_nomina2017Entities db = new AvantCraft_nomina2017Entities();

            //----- get a list of valid RFCs
            List<string> validRFCs = (from a in db.TC_RFC where a.bitValido == true select a.txyRfc).ToList();


            IQueryable<TE_Nomina> nominas = (from a in db.TE_Nomina where a.periodo == txt_Periodo select a).DefaultIfEmpty();
            foreach (TE_Nomina n in nominas)
            {
                //------- just get theRFC
                string theRFC = (from a in db.TE_TXT_HEADER where a.nominaId == n.nominaId select a.H4_03).FirstOrDefault();

                //------- check if not already publiched with UUID :: if so, then ignore and continue to next record
                string alreadyProcessed = (from a in db.TE_RfcTimbrado where a.txtRFC == theRFC && a.txtPeriodo == txt_Periodo select a.txtRFC).FirstOrDefault();
                if (alreadyProcessed != null) continue;

                //------- check if should use tbl TC_RfcExclusionList :: if so, then check to exclude
                if (useRfcExclusionList)
                {
                    string excludeRFC = (from a in db.TC_RfcExclusionList where a.txtRfc == theRFC select a.txtRfc).FirstOrDefault();
                    if (excludeRFC != null) continue;
                }

                //------- check if should use tbl TC_RfcIncludeList :: if so, then check to include
                if (useRfcIncludeList)
                {
                    string includeRFC = (from a in db.TC_RfcIncludeList where a.txtRfc == theRFC select a.txtRfc).FirstOrDefault();
                    if (includeRFC == null) continue;
                }

                //------- if is a valid RFC go on, else "continue" with next value
                TE_TXT_HEADER testNotValidRFC = (from a in db.TE_TXT_HEADER where validRFCs.Contains(a.H4_03) && a.nominaId == n.nominaId select a).FirstOrDefault();
                if (testNotValidRFC == null) continue;

                //------- validate no negative perceciones
                IQueryable<TE_Percepcion> negPer = (from a in db.TE_Percepcion where (a.ImporteExcento < 0 || a.ImporteGravado < 0) && a.nominaId == n.nominaId select a).DefaultIfEmpty();
                if (!(negPer.Count() == 1 && negPer.First() == null)) continue;

                //------- validate no negative deducciones
                IQueryable<TE_Deduccion> negDed = (from a in db.TE_Deduccion where a.Importe < 0 && a.nominaId == n.nominaId select a).DefaultIfEmpty();
                if (!(negDed.Count() == 1 && negDed.First() == null)) continue;


                TE_TXT_HEADER h = (from b in db.TE_TXT_HEADER where b.nominaId == n.nominaId select b).FirstOrDefault();
                string H1 = string.Format("[H1]||||{0}|||{1}|||{2}|||{3}||||||||||||||||{4}|{5}|{6}||||||||||||||{7}||||{8}|||||||||", h.H1_05, h.H1_08, h.H1_11, h.H1_14, h.H1_30, h.H1_31, h.H1_32, h.H1_46, h.H1_50);
                string H2 = string.Format("[H2]|{0}|{1}||{2}|{3}||{4}|{5}||{6}|{7}|{8}|{9}||||||||||||", h.H2_02, h.H2_03, h.H2_05, h.H2_06, h.H2_08, h.H2_09, h.H2_11, h.H2_12, h.H2_13, h.H2_14);
                string H3 = "[H3]|||||||||||||||||||||||||";

                //string H4 = string.Format("[H4]|{0}|{1}||||||||||{2}|||||||||||||", h.H4_02, h.H4_03, h.H4_13);
                string H4 = string.Format("[H4]|{0}|{1}|||||||||||||||||||||||", h.H4_02, h.H4_03);
                string H5 = "[H5]|||||||||||||||||||||||||";

                //string D = string.Format("[D]|||{0}||{1}|{2}||{3}||||||||||||||||{4}||||||||||||{5}|{6}||||{7}||||||||||||||||||||||||||||||||", h.D_04, h.D_06, h.D_07, h.D_09, h.D_25, h.D_37, h.D_38, h.D_42);

                //-- D_04 must be "Pago de nómina" in every case ... so we use D_04 stead of h.D_04
                string D_04 = "Pago de nómina";

                string D = string.Format("[D]|||{0}||{1}|{2}||{3}||||||||||||||||{4}||||||||||||{5}|{6}||||{7}||||||||||||||||||||||||||||||||", D_04, Decimal.ToInt32(Decimal.Parse(h.D_06)).ToString(), h.D_07, h.D_09, h.D_25, h.D_37, h.D_38, h.D_42);

                //string S = string.Format("[S]|||||||||{3}||||||{0}|||||||||||||||||||||{1}|{2}|||", h.S_16, h.S_36, h.S_37, h.S_10);
                string S = string.Format("[S]|||||||||{2}|||||||||||||||||||||||||||{0}|{1}|||", h.S_36, h.S_37, h.S_10);

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

                decimal forcedDiasPagados = (n.NumDiasPagados.Value < 0.001m) ? 0.001m : n.NumDiasPagados.Value;

                sb.Append(forcedDiasPagados + "|");
                sb.Append(n.TotalPercepciones + "|");
                sb.Append(n.TotalDeducciones + "|");
                sb.Append(n.TotalOtrosPagos);
                sb.Append(Environment.NewLine);

                sb.Append("[Emisor]" + "|");
                sb.Append(n.Emisor_CURP + "|");
                sb.Append(n.Emisor_RegistroPatronal + "|");
                sb.Append(n.Emisor_RfcPatronOrigen);
                sb.Append(Environment.NewLine);
                /*
                sb.Append("[EntSNCF]" + "|");
                sb.Append(n.Emisor_EntidadSNCF_c_OrigenRecurso + "|");
                sb.Append(n.Emisor_EntidadSNCF_MontoRecursoPropio);
                sb.Append(Environment.NewLine);
                */
                sb.Append("[Receptor]" + "|");
                sb.Append(n.Receptor_CURP + "|");
                sb.Append(n.Receptor_NumSeguridadSocial + "|");
                sb.Append(n.Receptor_FechaInicioRelLaboral + "|");

                //------------------------------
                //sb.Append("P" + n.Receptor_Antiguedad + "W|");    // calculate ANTIGUEDAD    --------------------

                DateTime fechFin = DateTime.MinValue;
                DateTime.TryParseExact(n.FechaFinalPago, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out fechFin);

                DateTime FechIni = DateTime.MinValue;
                DateTime.TryParseExact(n.Receptor_FechaInicioRelLaboral, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out FechIni);


                double weeksDbl = Math.Floor((fechFin - FechIni).TotalDays / 7);
                sb.Append("P" + Convert.ToInt32(weeksDbl) + "W|");

                //---------------------------------

                sb.Append(n.c_TipoContrato.Value + "|");
                sb.Append(n.Receptor_Sindicalizado + "|");
                try
                {
                    sb.Append(n.c_TipoJornada.c_TipoJornada1 + "|");
                }
                catch
                {
                    sb.Append("|");
                }
                sb.Append(n.c_TipoRegimen.Value + "|");
                sb.Append(n.Receptor_NumEmpleado + "|");
                sb.Append(n.Receptor_Departamento + "|");
                sb.Append(n.Receptor_Puesto.Replace('.', ' ').Replace('(', ' ').Replace(')', ' ').Replace('&', 'N') + "|");
                try
                {
                    //sb.Append(n.c_RiesgoPuesto.c_RiesgoPuesto1 + "|");
                    sb.Append(((n.Emisor_RfcPatronOrigen == "SAI091203MU3") ? "3" : "1") + "|");   // condicionado al pagador  SAI = 3, TSP = 1
                }
                catch
                {
                    sb.Append("|");
                }
                sb.Append(n.c_PeriodicidadPago.Value + "|");

                //sb.Append(n.c_Banco.Value + "|");
                //condicional la clave bancaria a la longitud de la CuentaBancaria, en caso de ser 18 dígitos (CLABE) no debe colocarse la clave de banco:
                if (n.Receptor_CuentaBancaria != null)
                {
                    sb.Append(((n.Receptor_CuentaBancaria.Length == 18) ? "" : n.c_Banco.Value) + "|");
                    sb.Append(n.Receptor_CuentaBancaria + "|");
                }
                else
                {
                    sb.Append("|");
                    sb.Append("|");
                }
                sb.Append(n.Receptor_SalarioBaseCotApor + "|");
                sb.Append(n.Receptor_SalarioDiarioIntegrado + "|");
                sb.Append(n.Receptor_c_ClaveEntFed);
                sb.Append(Environment.NewLine);

                var subc = (from suc in db.TE_Receptor_Subcontratacion where suc.nominaId == n.nominaId select suc).DefaultIfEmpty();
                foreach (TE_Receptor_Subcontratacion sub in subc)
                {
                    sb.Append("[Subcontratacion]" + "|");
                    sb.Append(sub.RfcLabora + "|");
                    sb.Append(sub.PorcentajeTiempo);
                    sb.Append(Environment.NewLine);
                }

                sb.Append("[Percepciones]" + "|");
                sb.Append(n.Percepciones_TotalSueldos + "|");
                sb.Append(n.Percepciones_TotalSeparacionIndemnizacion + "|");
                sb.Append(n.Percepciones_TotalJubilacionPensionRetiro + "|");
                sb.Append(n.Percepciones_TotalGravado + "|");
                sb.Append(n.Percepciones_TotalExento);
                sb.Append(Environment.NewLine);

                var ps = (from per in db.TE_Percepcion where per.nominaId == n.nominaId && per.c_TipoPercepcion != 19 && per.Clave != "REMPLAZO" && per.bolExclude != true select per).DefaultIfEmpty();
                foreach (TE_Percepcion p in ps)
                {
                    string tipoPercep = string.Empty;
                    try { tipoPercep = p.c_TipoPercepcion1.Value; }
                    catch { tipoPercep = string.Empty; }

                    if (tipoPercep.Trim() != "019")   // En el siguiente nodo incluiremos Horas Extra, lo excluimos de éste para evitar duplicidad
                    {
                        sb.Append("[Percepcion]" + "|");

                        sb.Append(tipoPercep + "|");

                        sb.Append(p.Clave + "|");
                        sb.Append(p.Concepto.Replace(".", string.Empty).Replace('(', ' ').Replace(')', ' ').Replace('&', 'N') + "|");
                        sb.Append(p.ImporteGravado + "|");
                        sb.Append(p.ImporteExcento);
                        sb.Append(Environment.NewLine);
                    }
                }
                //[AccionesOTitulos] contained in TE_Percepcion, not implemented by now

                //--- HORAS EXTRA
                //------- Nodo Percepciones
                IQueryable<TE_Percepcion> psHorasExtra = (from perHEX in db.TE_Percepcion where perHEX.nominaId == n.nominaId && perHEX.c_TipoPercepcion == 19 && perHEX.Clave != "REMPLAZO" select perHEX).DefaultIfEmpty();
                if (!(psHorasExtra.Count() == 1 && psHorasExtra.First() == null))
                {
                    decimal heGravado = 0;
                    decimal heExcento = 0;
                    foreach (TE_Percepcion phe in psHorasExtra)
                    {
                        heGravado += phe.ImporteGravado.Value;
                        heExcento += phe.ImporteExcento.Value;
                    }

                    sb.Append("[Percepcion]" + "|");

                    sb.Append("019|");

                    sb.Append("1020|");
                    sb.Append("Horas Extra|");
                    sb.Append(heGravado + "|");
                    sb.Append(heExcento);
                    sb.Append(Environment.NewLine);



                    IQueryable<TE_Percepcion_HorasExtra> hes = (from horexts in db.TE_Percepcion_HorasExtra where horexts.TE_Percepcion.nominaId == n.nominaId select horexts).DefaultIfEmpty();
                    if (!(hes.Count() == 1 && hes.First() == null))
                    {
                        foreach (TE_Percepcion_HorasExtra he in hes)
                        {
                            sb.Append("[HorasExtra]" + "|");
                            sb.Append(he.Dias + "|");
                            sb.Append("0" + he.c_TipoHoras1.c_TipoHoras1 + "|");  // FIXED 1st "0"
                            sb.Append(he.HorasExtra + "|");
                            sb.Append(he.ImportePagado);
                            sb.Append(Environment.NewLine);
                        }
                    }
                }
                //[JubilacionPensionRetiro]  To implement
                //[SeparacionIndemnizacion]  To implement

                sb.Append("[Deducciones]" + "|");
                sb.Append(n.Deducciones_TotalOtrasDeducciones + "|");

                if (n.Deducciones_TotalImpuestosRetenidos != 0) sb.Append(n.Deducciones_TotalImpuestosRetenidos); //validate is not 0.00, either case should be empty

                sb.Append(Environment.NewLine);

                var ds = (from ded in db.TE_Deduccion where ded.nominaId == n.nominaId && ded.bolExclude != true select ded).DefaultIfEmpty();
                if (!(ds.Count() == 1 && ds.First() == null))
                {
                    foreach (TE_Deduccion d in ds)
                    {
                        sb.Append("[Deduccion]" + "|");
                        sb.Append(d.c_TipoDeduccion1.Value + "|");
                        sb.Append(d.Clave.Replace("/", string.Empty) + "|");
                        sb.Append(d.Concepto.Replace(".", string.Empty).Replace('(', ' ').Replace(')', ' ').Replace('&', 'N') + "|");
                        sb.Append(d.Importe);
                        sb.Append(Environment.NewLine);
                    }
                }

                var os = (from ops in db.TE_OtroPago where ops.nominaId == n.nominaId select ops).DefaultIfEmpty();
                if (!(os.Count() == 1 && os.First() == null))
                {
                    sb.Append("[OtrosPagos]");
                    sb.Append(Environment.NewLine);

                    foreach (TE_OtroPago o in os)
                    {
                        sb.Append("[OtroPago]" + "|");
                        //sb.Append("00" + o.c_TipoOtroPago1.c_TipoOtroPago1 + "|");
                        sb.Append(Utils.FillWithCerosToTheLeft(o.c_TipoOtroPago1.c_TipoOtroPago1, 3) + "|");
                        sb.Append(o.Clave + "|");
                        sb.Append(o.Concepto.Replace(".", string.Empty).Replace('(', ' ').Replace(')', ' ').Replace('&', 'N') + "|");
                        sb.Append(o.Importe);
                        sb.Append(Environment.NewLine);

                        if (o.c_TipoOtroPago == 2)
                        {
                            sb.Append("[SubsidioAlEmpleo]" + "|");
                            sb.Append(o.Importe);
                            sb.Append(Environment.NewLine);
                        }
                    }
                }

                ////[SubsidioAlEmpleo]  
                //var op = (from ops in db.TE_OtroPago where ops.nominaId == n.nominaId && ops.c_TipoOtroPago == 2 select ops).DefaultIfEmpty();
                //if (!(op.Count() == 1 && op.First() == null))
                //{
                //  foreach (TE_OtroPago o in op)
                //  {
                //    sb.Append("[SubsidioAlEmpleo]" + "|");
                //    sb.Append(o.Importe);
                //    sb.Append(Environment.NewLine);
                //  }
                //}


                //[CompensacionSaldosAFavor]  To implement



                var ins = (from incs in db.TE_Incapacidad where incs.nominaId == n.nominaId select incs).DefaultIfEmpty();
                if (!(ins.Count() == 1 && ins.First() == null))
                {
                    sb.Append("[Incapacidades]");
                    sb.Append(Environment.NewLine);

                    foreach (TE_Incapacidad i in ins)
                    {
                        sb.Append("[Incapacidad]" + "|");
                        sb.Append(i.DiasIncapacidad + "|");
                        sb.Append("0" + i.c_TipoIncapacidad1.c_TipoIncapacidad1 + "|");
                        sb.Append(i.ImporteMonetario);
                        sb.Append(Environment.NewLine);
                    }
                }

                // MAKE FILE ----
                try
                {
                    bool exists4 = System.IO.Directory.Exists(GetFinalDestination(h.H2_03) + "\\");
                    if (!exists4) System.IO.Directory.CreateDirectory(GetFinalDestination(h.H2_03) + "\\");

                    string textToPrintHead = H1 + Environment.NewLine + H2 + Environment.NewLine + H3 + Environment.NewLine + H4 + Environment.NewLine + H5 + Environment.NewLine + D + Environment.NewLine + S + Environment.NewLine;
                    TextWriter sw = new StreamWriter(GetFinalDestination(h.H2_03) + "\\" + h.H4_03 + "_" + n.periodo + ".txt", false, Encoding.GetEncoding(1252), 512);
                    sw.Write(textToPrintHead + sb.ToString());
                    sw.Close();
                }
                catch (Exception e)
                {
                    StreamWriter sw = new StreamWriter(GetConfigurationValues("ErrorFolder") + "LogDeErrores.txt", true, Encoding.GetEncoding(1252), 512);
                    sw.Write(h.H4_03 + "_" + n.periodo + " :: " + e.Message + " (" + e.InnerException + ")" + Environment.NewLine + Environment.NewLine);
                    sw.Close();
                }
            } // for each nomina

        }
    }
}

