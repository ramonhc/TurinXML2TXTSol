﻿using dataaccessXML2TXT;
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


            files = dir.GetFiles("*.xml", SearchOption.AllDirectories);

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
            //string frecuencia = "";

            var Nomina = (from c in root.Elements(cfdi + "Complemento").Elements(nomina12 + "Nomina") select c).FirstOrDefault();
            string fechaPago = Nomina.Attribute("FechaPago").Value;
            string fechaInicialPago = Nomina.Attribute("FechaInicialPago").Value;
            string fechaFinalPago = Nomina.Attribute("FechaFinalPago").Value;
            string NumDiasPagados = Nomina.Attribute("NumDiasPagados").Value;
            string totalPercepciones = Nomina.Attribute("TotalPercepciones").Value;
            string totalDeducciones = Nomina.Attribute("TotalDeducciones").Value;
            string totalOtrosPagos = (Nomina.Attribute("TotalOtrosPagos") != null) ? Nomina.Attribute("TotalOtrosPagos").Value : "";

            var TimbreFiscalDigital = (from c in root.Elements(cfdi + "Complemento").Elements(tfd + "TimbreFiscalDigital") select c).FirstOrDefault();
            string UUID = TimbreFiscalDigital.Attribute("UUID").Value; //0 FechaTimbrado
            string fechaTimbrado = TimbreFiscalDigital.Attribute("FechaTimbrado").Value;


            var complemento_nomina_percepciones_coll = (from c in root.Elements(cfdi + "Complemento").Elements(nomina12 + "Nomina").Elements(nomina12 + "Receptor") select c).FirstOrDefault();
            if (complemento_nomina_percepciones_coll != null)
            {
                curp = complemento_nomina_percepciones_coll.Attribute("Curp").Value;
            }

            aPeriodo = (from r in db.TE_Nomina where r.Receptor_CURP == curp && r.FechaFinalPago == fechaFinalPago select r.periodo).FirstOrDefault();
            //frecuencia = (from r in db.TE_Nomina where r.Receptor_CURP == curp select r.c_PeriodicidadPago.Descripcion).FirstOrDefault();


            var found = (from a in db.TE_RfcTimbrado
                         where a.txtRFC == rfc
                               && a.txtFechaFinalPago == fechaFinalPago
                               && a.txtFileName == file.Name
                               && a.txtUUID == UUID
                         select a).FirstOrDefault();

            if (found == null)
            {
                TE_RfcTimbrado t = new TE_RfcTimbrado();
                t.txtRFC = rfc;
                t.txtNombre = nombre;
                t.txtFechaPago = fechaPago;
                t.txtFechaInicialPago = fechaInicialPago;
                t.txtFechaFinalPago = fechaInicialPago;
                t.fechaTimbrado = fechaTimbrado;
                t.numDiasPagados = NumDiasPagados;
                t.txtTotalPercepciones = totalPercepciones;
                t.txtTotalDeducciones = totalDeducciones;
                t.totalOtrosPagos = totalOtrosPagos;
                t.txtPeriodo = aPeriodo;
                t.txtUUID = UUID;
                t.txtFileName = file.Name;

                db.TE_RfcTimbrado.Add(t);
                db.SaveChanges();
            }
        }
        //----------------------------------------------------------

        //---------------------------------------------------------------------------+

        private static void AddXmlInfoIntoExistingTableDB(FileInfo file)
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
            //string frecuencia = "";

            var Nomina = (from c in root.Elements(cfdi + "Complemento").Elements(nomina12 + "Nomina") select c).FirstOrDefault();
            string fechaFinalPago = Nomina.Attribute("FechaFinalPago").Value;
            string NumDiasPagados = Nomina.Attribute("NumDiasPagados").Value;

            var TimbreFiscalDigital = (from c in root.Elements(cfdi + "Complemento").Elements(tfd + "TimbreFiscalDigital") select c).FirstOrDefault();
            string UUID = TimbreFiscalDigital.Attribute("UUID").Value; //0



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
                               && a.txtFechaFinalPago == fechaFinalPago
                               && a.txtFileName == file.Name
                               && a.txtUUID == UUID
                         select a).FirstOrDefault();

            if (found == null)
            {
                TE_RfcTimbrado t = new TE_RfcTimbrado();
                t.txtFileName = file.Name;
                t.txtFechaFinalPago = fechaFinalPago;
                t.txtPeriodo = aPeriodo;
                t.txtRFC = rfc;
                t.txtUUID = UUID;
                db.TE_RfcTimbrado.Add(t);
                db.SaveChanges();
            }
        }

        //---------------------------------------------------------------------------------------------------
        public void GetUUID2CANCEL(string txt_FolderUUID)
        {
            StringBuilder sb = new StringBuilder();
            DirectoryInfo dir = new DirectoryInfo(txt_FolderUUID);
            FileInfo[] files;

            files = dir.GetFiles("*.xml");

            foreach (FileInfo file in files)
            {

                CancelPurposeGetXmlInfoIntoDB(file);
            }
        }

        //---------------------------------------------------------------------------+

        private static void CancelPurposeGetXmlInfoIntoDB(FileInfo file)
        {
            string aPeriodo = string.Empty;
            AvantCraft_nomina2017Entities db = new AvantCraft_nomina2017Entities();

            XNamespace cfdi = "http://www.sat.gob.mx/cfd/3";
            XNamespace nomina = "http://www.sat.gob.mx/nomina";
            XNamespace tfd = "http://www.sat.gob.mx/TimbreFiscalDigital";

            XElement root = XElement.Load(file.FullName);

            var Receptor = (from c in root.Elements(cfdi + "Receptor") select c).FirstOrDefault();
            string Receptor_rfc = Receptor.Attribute("rfc").Value;
            string Receptor_nombre = Receptor.Attribute("nombre").Value;

            var Emisor = (from c in root.Elements(cfdi + "Emisor") select c).FirstOrDefault();
            string Emisor_RFC = Emisor.Attribute("rfc").Value;

            var Nomina = (from c in root.Elements(cfdi + "Complemento").Elements(nomina + "Nomina") select c).FirstOrDefault();
            string PeriodicidadPago = (Nomina.Attribute("PeriodicidadPago") != null) ? Nomina.Attribute("PeriodicidadPago").Value : "";
            string FechaFinalPago = Nomina.Attribute("FechaFinalPago").Value;

            var TimbreFiscalDigital = (from c in root.Elements(cfdi + "Complemento").Elements(tfd + "TimbreFiscalDigital") select c).FirstOrDefault();
            string UUID = TimbreFiscalDigital.Attribute("UUID").Value;

            var found = (from a in db.TE_UUID2CANCEL
                         where a.Emisor_RFC == Emisor_RFC
                               && a.Receptor_rfc == Receptor_rfc
                               && a.PeriodicidadPago == PeriodicidadPago
                               && a.FechaFinalPago == FechaFinalPago
                               && a.UUID == UUID
                         select a).FirstOrDefault();

            if (found == null)
            {
                TE_UUID2CANCEL t = new TE_UUID2CANCEL();
                t.Emisor_RFC = Emisor_RFC;
                t.Receptor_rfc = Receptor_rfc;
                t.Receptor_nombre = Receptor_nombre;
                t.PeriodicidadPago = aPeriodo;
                t.FechaFinalPago = FechaFinalPago;
                t.UUID = UUID;
                db.TE_UUID2CANCEL.Add(t);
                db.SaveChanges();
            }
        }
    }
}
