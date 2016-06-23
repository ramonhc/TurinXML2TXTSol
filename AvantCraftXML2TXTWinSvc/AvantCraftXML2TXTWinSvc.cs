using AvantCraftXML2TXTLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AvantCraftXML2TXTWinSvc
{
  public partial class AvantCraftXML2TXTWinSvc : ServiceBase
  {
    private System.Timers.Timer myTimer = null;
    //------------------------------------------------------+
    public AvantCraftXML2TXTWinSvc()
    {
      InitializeComponent();
      if (!System.Diagnostics.EventLog.SourceExists("AvantCraftXML2TXTWinSvcSource"))
      {
        System.Diagnostics.EventLog.CreateEventSource("AvantCraftXML2TXTWinSvcSource", "AvantCraftXML2TXTWinSvcLog");
      }
      eventLog1.Source = "AvantCraftXML2TXTWinSvcSource";
      eventLog1.Log = "AvantCraftXML2TXTWinSvcLog";
    }

    //------------------------------------------------------+
    protected override void OnStart(string[] args)
    {
      eventLog1.WriteEntry("AvantCraftXML2TXT Service Started");
      try
      {
        myTimer = new Timer(60000); //60000 miliseconds (1 min)
        myTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.EventAction);
        myTimer.Enabled = true;
      }
      catch (Exception ex)
      {
        eventLog1.WriteEntry(ex.Message);
      }
    }

    //------------------------------------------------------+
    public void EventAction(object sender, System.Timers.ElapsedEventArgs e)
    {
      myTimer.Enabled = false;
      try
      {
        Xml2TxtProcess obj = new Xml2TxtProcess();
        obj.Processfiles();
      }
      catch (Exception ex)
      {
        eventLog1.WriteEntry(ex.Message);
      }
      myTimer.Enabled = true;
    }

    //------------------------------------------------------+
    protected override void OnStop()
    {
      eventLog1.WriteEntry("AvantCraftXML2TXT Service Stopped");
    }
  }
}
