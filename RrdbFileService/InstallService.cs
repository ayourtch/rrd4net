using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Text;

namespace RrdbFileService
{
   [RunInstaller(true)]
   public class InstallService : Installer
   {
      private ServiceInstaller serviceInstaller;
      private ServiceProcessInstaller processInstaller;

      public InstallService()
      {
         // Instantiate installers for process and services.
         processInstaller = new ServiceProcessInstaller();
         serviceInstaller = new ServiceInstaller();

         // The services run under the system account.
         processInstaller.Account = ServiceAccount.LocalSystem;

         // The services are started manually.
         serviceInstaller.StartType = ServiceStartMode.Automatic;

         // ServiceName must equal those on ServiceBase derived classes.            
         serviceInstaller.ServiceName = "RrdbFileService";
         serviceInstaller.DisplayName = "RrdDb remote file server";
         serviceInstaller.Description = "Server hosting the rrdb file server access object";
      
         // Add installers to collection. Order is not important.
         Installers.Add(serviceInstaller);
         Installers.Add(processInstaller);
      }
   }
}
