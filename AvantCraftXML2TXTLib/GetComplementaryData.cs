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
    public static void Load()
    {
      string LayOutsFolder = ConfigurationManager.AppSettings["LayOutsFolder"].ToString();
      bool exists = System.IO.Directory.Exists(LayOutsFolder);
      if (!exists) System.IO.Directory.CreateDirectory(LayOutsFolder);

      FileStream stream = File.Open(LayOutsFolder, FileMode.Open, FileAccess.Read);

      //Choose one of either 1 or 2
      //1. Reading from a binary Excel file ('97-2003 format; *.xls)
      ////IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

      //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
      IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

      //Choose one of either 3, 4, or 5
      //3. DataSet - The result of each spreadsheet will be created in the result.Tables
      ////DataSet result = excelReader.AsDataSet();

      //4. DataSet - Create column names from first row
      excelReader.IsFirstRowAsColumnNames = true;
      DataSet result = excelReader.AsDataSet();

      //5. Data Reader methods
      ////while (excelReader.Read())
      ////{
      ////  //excelReader.GetInt32(0);
      ////}

      //6. Free resources (IExcelDataReader is IDisposable)
      excelReader.Close();
    }
  }
}
