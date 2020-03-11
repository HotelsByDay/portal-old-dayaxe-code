namespace Dayaxe.Schedule
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ServiceAutoSendEmailProductionInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.AutoSendEmailScheduleProduction = new System.ServiceProcess.ServiceInstaller();
            // 
            // ServiceAutoSendEmailProductionInstaller
            // 
            this.ServiceAutoSendEmailProductionInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.ServiceAutoSendEmailProductionInstaller.Password = null;
            this.ServiceAutoSendEmailProductionInstaller.Username = null;
            // 
            // AutoSendEmailScheduleProduction
            // 
            this.AutoSendEmailScheduleProduction.Description = "Auto Send Email Production of DayAxe Every 1 minutes";
            this.AutoSendEmailScheduleProduction.DisplayName = "Dayaxe Scheduler of Production send email every 1 minute";
            this.AutoSendEmailScheduleProduction.ServiceName = "AutoSendEmailServiceProduction";
            this.AutoSendEmailScheduleProduction.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.ServiceAutoSendEmailProductionInstaller,
            this.AutoSendEmailScheduleProduction});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller ServiceAutoSendEmailProductionInstaller;
        private System.ServiceProcess.ServiceInstaller AutoSendEmailScheduleProduction;
    }
}