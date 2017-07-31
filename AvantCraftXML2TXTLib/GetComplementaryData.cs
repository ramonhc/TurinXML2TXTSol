using dataaccessXML2TXT;
using Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvantCraftXML2TXTLib
{
    public class GetComplementaryData
    {
        public static void Load(bool chkCargaSubcontratacion, bool chkFijos, string aPeriodo)
        {
            string LayOutsFolder = ConfigurationManager.AppSettings["LayOutsFolder"].ToString();
            bool exists = System.IO.Directory.Exists(LayOutsFolder);
            if (!exists) System.IO.Directory.CreateDirectory(LayOutsFolder);

            FileStream stream = File.Open(LayOutsFolder + "LayOutComplementarios" + aPeriodo + ".xlsx", FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            excelReader.IsFirstRowAsColumnNames = true;
            DataSet result = excelReader.AsDataSet();
            excelReader.Close();

            AvantCraft_nomina2017Entities db = new AvantCraft_nomina2017Entities();

            //--->>> fijosXempleado -- Subontratación
            foreach (DataRow r in result.Tables["fijosXempleado"].Rows)
            {
                string rfcEmpleado = r["rfcEmpleado"].ToString().Trim();
                string RfcLabora = r["RfcLabora"].ToString().Trim();
                if (rfcEmpleado != null && rfcEmpleado.Trim() != string.Empty)
                {

                    //-- get numEmpleado
                    string numempleado = (from a in db.TC_RFC where a.txyRfc == rfcEmpleado select a.txtNumEmp).FirstOrDefault();
                    if (numempleado == null)
                    {
                        numempleado = (from a in db.TC_RFC where a.txyRfc.Substring(0, 7) == rfcEmpleado.Substring(0, 7) select a.txtNumEmp).FirstOrDefault();
                        if (numempleado == null)
                        {
                            numempleado = (from a in db.TC_RFC where a.txyRfc.Substring(0, 5) == rfcEmpleado.Substring(0, 5) select a.txtNumEmp).FirstOrDefault();
                            if (numempleado == null)
                            {
                                numempleado = Utils.getNumEmpleadoFromXML(rfcEmpleado, aPeriodo);
                            }
                        }
                    }
                    //---------->>>>>>>>>>>>>> Subcontratacion
                    if (chkCargaSubcontratacion)
                    {
                        //VALIDATE (if exists then remove)
                        TC_Subcontratacion isthere = (from a in db.TC_Subcontratacion where a.RfcEmpleado == rfcEmpleado && a.RfcLabora == RfcLabora && a.txtPeriodo == aPeriodo select a).FirstOrDefault();
                        if (isthere != null)
                        {
                            db.TC_Subcontratacion.Remove(isthere);
                            db.SaveChanges();
                        }

                        decimal porcentajeTiempo = decimal.Parse(r["PorcentajeTiempo"].ToString());
                        if (porcentajeTiempo > 0)
                        {
                            //ADD NEW
                            TC_Subcontratacion s = new TC_Subcontratacion();
                            s.RfcEmpleado = rfcEmpleado.Trim();
                            s.RfcLabora = RfcLabora.Trim();
                            s.txtPeriodo = aPeriodo.Trim();
                            s.txtNumEmpleado = numempleado.Trim();

                            if (porcentajeTiempo < 1) porcentajeTiempo = porcentajeTiempo * 100;
                            s.PorcentajeTiempo = porcentajeTiempo;

                            s.txtPeriodo = aPeriodo.Trim();
                            db.TC_Subcontratacion.Add(s);
                            db.SaveChanges();
                        }
                    }

                    //---------->>>>>>>>>>>>>> fijosXempleado
                    if (chkFijos)
                    {
                        //VALIDATE (if exists then remove)
                        TC_DatosFijosPorEmpleado isthere = (from a in db.TC_DatosFijosPorEmpleado where a.rfcEmpleado == rfcEmpleado && a.txtPeriodo == aPeriodo select a).FirstOrDefault();
                        if (isthere != null)
                        {
                            db.TC_DatosFijosPorEmpleado.Remove(isthere);
                            db.SaveChanges();
                        }

                        //ADD NEW
                        TC_DatosFijosPorEmpleado s = new TC_DatosFijosPorEmpleado();
                        s.rfcEmpleado = rfcEmpleado.Trim();
                        s.Sindicalizado = r["Sindicalizado(SI/NO)"].ToString().Trim();
                        s.c_TipoJornada = r["c_TipoJornada"].ToString().Trim();
                        s.Departamento = r["Departamento"].ToString().Trim();
                        s.c_RiesgoPuesto = r["c_RiesgoPuesto"].ToString().Replace('.', ' ').Replace('(', ' ').Replace(')', ' ').Replace('&', 'N').Replace('&', 'N').Trim();
                        s.c_Banco = r["c_Banco"].ToString().Trim();
                        s.CuentaBancaria = r["CuentaBancaria"].ToString().Trim();
                        s.c_Estado = r["c_Estado"].ToString().Trim();
                        s.txtPeriodo = aPeriodo.Trim();
                        s.txtNumEmpleado = numempleado.Trim();
                        db.TC_DatosFijosPorEmpleado.Add(s);
                        db.SaveChanges();
                    }
                }
            }

            //--->>>
            /*
            foreach (DataRow r in result.Tables["camposNuevos"].Rows)
            {
              //Get Record and validate
              string periodo = r["nominaId"].ToString();  // Periodo ej: S012017
              string Num_Emp = r["Num Emp."].ToString();
              TE_Nomina n = (from a in db.TE_Nomina where a.periodo == periodo && a.Receptor_NumEmpleado == Num_Emp select a).FirstOrDefault();
              if (n != null)
              {
                n.c_TipoNomina = r["c_TipoNomina"].ToString();                                                           // IN AppConfig: <add key = "c_TipoNomina" value="O" />
                n.TotalPercepciones = Utils.s2d(r["TotalPercepciones"].ToString());
                n.TotalDeducciones = Utils.s2d(r["TotalDeducciones"].ToString());
                n.TotalOtrosPagos = Utils.s2d(r["TotalOtrosPagos"].ToString());
                n.Emisor_EntidadSNCF_c_OrigenRecurso = r["c_OrigenRecurso"].ToString();                                  // IN AppConfig: <add key = "c_OrigenRecurso" value="IP" />
                n.Emisor_EntidadSNCF_MontoRecursoPropio = Utils.s2d(r["MontoRecursoPropio"].ToString());
                n.Receptor_Antiguedad = r["Antigüedad"].ToString();                                                      // Calculated in code
                n.Receptor_c_TipoContrato = double.Parse(r["c_TipoContrato"].ToString());                                // IN AppConfig: <add key = "c_TipoContrato" value="1" />
                n.Percepciones_TotalSueldos = Utils.s2d(r["TotalSueldos"].ToString());
                n.Percepciones_TotalSeparacionIndemnizacion = Utils.s2d(r["TotalSeparacionIndemnizacion"].ToString());
                n.Percepciones_TotalJubilacionPensionRetiro = Utils.s2d(r["TotalJubilacionPensionRetiro"].ToString());
              }
            }  
            */
        }
    }
}


