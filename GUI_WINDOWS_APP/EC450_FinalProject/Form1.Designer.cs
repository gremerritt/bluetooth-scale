namespace EC450_FinalProject
{
    partial class WeightSensor
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox_ports = new System.Windows.Forms.ComboBox();
            this.button_scan = new System.Windows.Forms.Button();
            this.button_connect = new System.Windows.Forms.Button();
            this.button_disconnect = new System.Windows.Forms.Button();
            this.label_weight = new System.Windows.Forms.Label();
            this.label_data = new System.Windows.Forms.Label();
            this.label_adc = new System.Windows.Forms.Label();
            this.label_adc_val = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 20);
            this.label1.TabIndex = 0;
            this.label1.UseWaitCursor = true;
            // 
            // comboBox_ports
            // 
            this.comboBox_ports.Enabled = false;
            this.comboBox_ports.FormattingEnabled = true;
            this.comboBox_ports.Location = new System.Drawing.Point(163, 23);
            this.comboBox_ports.Name = "comboBox_ports";
            this.comboBox_ports.Size = new System.Drawing.Size(308, 28);
            this.comboBox_ports.TabIndex = 1;
            // 
            // button_scan
            // 
            this.button_scan.Location = new System.Drawing.Point(12, 23);
            this.button_scan.Name = "button_scan";
            this.button_scan.Size = new System.Drawing.Size(145, 41);
            this.button_scan.TabIndex = 2;
            this.button_scan.Text = "Scan for Ports";
            this.button_scan.UseVisualStyleBackColor = true;
            this.button_scan.Click += new System.EventHandler(this.button_scan_Click);
            // 
            // button_connect
            // 
            this.button_connect.Location = new System.Drawing.Point(12, 81);
            this.button_connect.Name = "button_connect";
            this.button_connect.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.button_connect.Size = new System.Drawing.Size(145, 42);
            this.button_connect.TabIndex = 3;
            this.button_connect.Text = "Connect";
            this.button_connect.UseVisualStyleBackColor = true;
            this.button_connect.Click += new System.EventHandler(this.button_connect_Click);
            // 
            // button_disconnect
            // 
            this.button_disconnect.Enabled = false;
            this.button_disconnect.Location = new System.Drawing.Point(163, 81);
            this.button_disconnect.Name = "button_disconnect";
            this.button_disconnect.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.button_disconnect.Size = new System.Drawing.Size(145, 42);
            this.button_disconnect.TabIndex = 4;
            this.button_disconnect.Text = "Disconnect";
            this.button_disconnect.UseVisualStyleBackColor = true;
            this.button_disconnect.Click += new System.EventHandler(this.button_disconnect_Click);
            // 
            // label_weight
            // 
            this.label_weight.AutoSize = true;
            this.label_weight.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_weight.Location = new System.Drawing.Point(13, 164);
            this.label_weight.Name = "label_weight";
            this.label_weight.Size = new System.Drawing.Size(370, 108);
            this.label_weight.TabIndex = 5;
            this.label_weight.Text = "Weight:";
            // 
            // label_data
            // 
            this.label_data.AutoSize = true;
            this.label_data.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_data.Location = new System.Drawing.Point(403, 164);
            this.label_data.Name = "label_data";
            this.label_data.Size = new System.Drawing.Size(0, 108);
            this.label_data.TabIndex = 6;
            // 
            // label_adc
            // 
            this.label_adc.AutoSize = true;
            this.label_adc.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_adc.Location = new System.Drawing.Point(238, 297);
            this.label_adc.Name = "label_adc";
            this.label_adc.Size = new System.Drawing.Size(117, 25);
            this.label_adc.TabIndex = 7;
            this.label_adc.Text = "ADC Value:";
            // 
            // label_adc_val
            // 
            this.label_adc_val.AutoSize = true;
            this.label_adc_val.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_adc_val.Location = new System.Drawing.Point(361, 297);
            this.label_adc_val.Name = "label_adc_val";
            this.label_adc_val.Size = new System.Drawing.Size(0, 25);
            this.label_adc_val.TabIndex = 8;
            // 
            // WeightSensor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 356);
            this.Controls.Add(this.label_adc_val);
            this.Controls.Add(this.label_adc);
            this.Controls.Add(this.label_data);
            this.Controls.Add(this.label_weight);
            this.Controls.Add(this.button_disconnect);
            this.Controls.Add(this.button_connect);
            this.Controls.Add(this.button_scan);
            this.Controls.Add(this.comboBox_ports);
            this.Controls.Add(this.label1);
            this.Name = "WeightSensor";
            this.Text = "WeightSesnor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox_ports;
        private System.Windows.Forms.Button button_scan;
        private System.Windows.Forms.Button button_connect;
        private System.Windows.Forms.Button button_disconnect;
        private System.Windows.Forms.Label label_weight;
        private System.Windows.Forms.Label label_data;
        private System.Windows.Forms.Label label_adc;
        private System.Windows.Forms.Label label_adc_val;
    }
}

