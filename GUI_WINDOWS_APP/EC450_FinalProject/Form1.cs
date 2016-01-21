using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace EC450_FinalProject
{
    public partial class WeightSensor : Form
    {
        SerialPort port = new SerialPort();
        private delegate void SetDataCallback(string text);
        private delegate void SetADCCallback(string text);
        public WeightSensor()
        {
            InitializeComponent();
            port.BaudRate = 9600;
            port.Parity = Parity.None;
            port.DataBits = 8;
            port.StopBits = StopBits.One;
            port.Handshake = Handshake.None;
            port.RtsEnable = true;
            port.DataReceived += new SerialDataReceivedEventHandler(this.data_received);
        }

        private void button_scan_Click(object sender, EventArgs e)
        {
            comboBox_ports.Items.Clear();
            string[] port_names = SerialPort.GetPortNames();

            if (port_names.Length == 0) {
                comboBox_ports.Enabled = false;
                System.Windows.Forms.MessageBox.Show("No ports found.");
            }
            else
            {
                foreach (string p in port_names) { comboBox_ports.Items.Add(p); }
                comboBox_ports.Enabled = true;
            }
        }

        private void button_connect_Click(object sender, EventArgs e)
        {
            if (comboBox_ports.Text != "")
            {
                port.PortName = comboBox_ports.Text;
                try { 
                    port.Open();
                    button_connect.Enabled = false;
                    button_disconnect.Enabled = true;
                }
                catch { System.Windows.Forms.MessageBox.Show("Cannot connect."); }
            }
            else { System.Windows.Forms.MessageBox.Show("Please select a port."); }
        }

        private void button_disconnect_Click(object sender, EventArgs e)
        {
            port.Close();
            label_data.Text = "";
            label_adc_val.Text = "";
            button_connect.Enabled = true;
            button_disconnect.Enabled = false;
        }

        private void data_received(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadLine();
            int int_data = Int32.Parse(data);
            float weight;
            if (int_data < 250) { weight = 0; }
            else
            {
                weight = (((float) 8.3)/200)*(int_data - 250);
            }
            SetData(weight.ToString("n2") + " lbs.");
            SetADC(data);
        }

        private void SetData(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.label_data.InvokeRequired)
            {
                SetDataCallback d = new SetDataCallback(SetData);
                try { this.Invoke(d, new object[] { text }); }
                catch {
                    // do nothing... it was an issue disposing the obj
                }
            }
            else
            {
                this.label_data.Text = text;
            }
        }

        private void SetADC(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.label_data.InvokeRequired)
            {
                SetADCCallback d = new SetADCCallback(SetADC);
                try { this.Invoke(d, new object[] { text }); }
                catch
                {
                    // do nothing... it was an issue disposing the obj
                }
            }
            else
            {
                this.label_adc_val.Text = text;
            }
        }
    }
}
