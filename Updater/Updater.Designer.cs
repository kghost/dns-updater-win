namespace Updater
{
    partial class updater
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
            this.updateTimer = new System.Timers.Timer();
            this.logger = new System.Diagnostics.EventLog();
            ((System.ComponentModel.ISupportInitialize)(this.updateTimer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.logger)).BeginInit();
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            this.updateTimer.Interval = 120000D;
            this.updateTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.updateTimer_Elapsed);
            // 
            // logger
            // 
            this.logger.Log = "Application";
            this.logger.Source = "DNS Updater";
            // 
            // updater
            // 
            this.ServiceName = "localDnsUpdater";
            ((System.ComponentModel.ISupportInitialize)(this.updateTimer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.logger)).EndInit();

        }

        #endregion

        private System.Timers.Timer updateTimer;
        private System.Diagnostics.EventLog logger;
    }
}
