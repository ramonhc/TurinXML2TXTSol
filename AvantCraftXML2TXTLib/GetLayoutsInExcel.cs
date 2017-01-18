using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvantCraftXML2TXTLib
{
  class GetLayoutsInExcel
  {
    public void GetLayouts()
    {
      string LayOutsFolder = ConfigurationManager.AppSettings["LayOutsFolder"].ToString();
      bool exists = System.IO.Directory.Exists(LayOutsFolder);
      if (!exists) System.IO.Directory.CreateDirectory(LayOutsFolder);
    }
  }
}
