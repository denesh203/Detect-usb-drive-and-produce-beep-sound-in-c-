using System;
using System.Windows.Forms;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Threading;
using System.Xml.Linq;

namespace beep
{
    public partial class Form1 : Form
    {
        private ManagementEventWatcher watcher;
        private bool usbInserted;
        private Thread beepingThread;
        public Form1()
        {
            InitializeComponent();
            usbInserted = false;
            beepingThread = new Thread(ContinuousBeep);
            this.Visible = false;
            this.Opacity = 0;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");

            // Initialize the watcher and subscribe to the event
            watcher = new ManagementEventWatcher(query);
            watcher.EventArrived += new EventArrivedEventHandler(DeviceInsertedEvent);
            watcher.Start();

            
        }
        private void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
        {
            // Console.Beep();
            if (!usbInserted)
            {
                usbInserted = true;
                this.Invoke(new MethodInvoker(delegate
                {
                    this.Visible = true; // Show the form when beep occurs
                    this.Opacity = 1;
                }));
                if (!beepingThread.IsAlive)
                {
                    beepingThread = new Thread(ContinuousBeep);
                    beepingThread.Start();
                }
            }
        }
        private void ContinuousBeep()
        {
            // Continuously beep until the USB device is removed
            while (usbInserted)
            {
                Console.Beep();
                // Adjust the beep duration or frequency if needed
                Thread.Sleep(1000); // Beep every second
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (watcher != null)
            {
                watcher.Stop();
                watcher.Dispose();
            }

            // Signal the beeping thread to stop
            usbInserted = false;

            // Wait for the beeping thread to complete
            if (beepingThread.IsAlive)
            {
                beepingThread.Join();
            }
        }
        private void Check_Click(object sender, EventArgs e)
        {
            if (Comment.Text == "12345")
            {
                // Stop the continuous beep
                usbInserted = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
