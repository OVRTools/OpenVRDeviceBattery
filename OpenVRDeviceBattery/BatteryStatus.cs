using System;
using System.Windows.Forms;
using Valve.VR;

namespace OpenVRDeviceBattery
{
    class BatteryStatus
    {
        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenuStrip;
        private Timer timer;

        public BatteryStatus()
        {
            notifyIcon = new NotifyIcon();
            contextMenuStrip = new ContextMenuStrip();
            timer = new Timer();
        }

        public void Start()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));

            notifyIcon.ContextMenuStrip = contextMenuStrip;
            notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            notifyIcon.Text = "notifyIcon";
            notifyIcon.Visible = true;

            contextMenuStrip.Name = "contextMenuStrip";
            contextMenuStrip.Size = new System.Drawing.Size(61, 4);

            timer.Enabled = true;
            timer.Interval = 1000;
            timer.Tick += new EventHandler(timer_Tick);
        }

        private void UpdateContextMenu()
        {
            contextMenuStrip.Items.Clear();

            for (uint i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
            {
                if (!OpenVRUtils.DeviceIsSupported(i)) continue;

                var lastErr = ETrackedPropertyError.TrackedProp_Success;

                var providesBattery = Program.openVRHandle.GetBoolTrackedDeviceProperty(i, ETrackedDeviceProperty.Prop_DeviceProvidesBatteryStatus_Bool, ref lastErr);

                var battPct = Program.openVRHandle.GetFloatTrackedDeviceProperty(i, ETrackedDeviceProperty.Prop_DeviceBatteryPercentage_Float, ref lastErr);
                var isCharging = Program.openVRHandle.GetBoolTrackedDeviceProperty(i, ETrackedDeviceProperty.Prop_DeviceIsCharging_Bool, ref lastErr);

                var text = $"{OpenVRUtils.DeviceTypeHumanName(i)}";
                if (providesBattery) text += $" - {battPct * 100}%";
                if (isCharging) text += " ⚡️";

                var item = new ToolStripMenuItem
                {
                    Enabled = true,
                    Text = text
                };

                var index = i;
                item.Click += (object sender, EventArgs e) =>
                {
                    var infoForm = new DeviceInfo(index);
                    infoForm.Show();
                };

                contextMenuStrip.Items.Add(item);
            }

            contextMenuStrip.Items.Add(new ToolStripSeparator());

            var quit = new ToolStripMenuItem
            {
                Text = "Quit"
            };

            quit.Click += Quit_Click;
            contextMenuStrip.Items.Add(quit);
        }

        private void Quit_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Application.Exit();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!contextMenuStrip.Visible) UpdateContextMenu();
        }
    }
}
