using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Valve.VR;

namespace OpenVRDeviceBattery
{
    class BatteryStatus
    {
        private readonly NotifyIcon notifyIcon;
        private readonly ContextMenuStrip contextMenuStrip;
        private readonly Timer timer;

        public BatteryStatus()
        {
            notifyIcon = new NotifyIcon();
            contextMenuStrip = new ContextMenuStrip();
            timer = new Timer();
        }

        public void Start()
        {
            notifyIcon.ContextMenuStrip = contextMenuStrip;
            notifyIcon.Icon = ((System.Drawing.Icon)(Properties.Resources.ResourceManager.GetObject("logo")));
            notifyIcon.Text = "OpenVRBatteryStatus";
            notifyIcon.MouseUp += NotifyIcon_MouseUp;
            notifyIcon.Visible = true;

            contextMenuStrip.Name = "contextMenuStrip";
            contextMenuStrip.Size = new System.Drawing.Size(61, 4);

            timer.Enabled = true;
            timer.Interval = 1000;
            timer.Tick += new EventHandler(timer_Tick);

            UpdateContextMenu();
        }

        // https://social.msdn.microsoft.com/Forums/windows/en-US/8de03b21-e144-4614-96cd-d382c2a2fbe9/open-contextmenustrip-on-left-mouse-click-on-notifyicon?forum=winforms
        private void NotifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(notifyIcon, null);
            }
        }

        private void UpdateContextMenu()
        {
            contextMenuStrip.Items.Clear();

            var atLeastOneDevice = false;

            var devClasses = new Dictionary<ETrackedDeviceClass, string> {
                { ETrackedDeviceClass.Controller, "Controllers" },
                { ETrackedDeviceClass.GenericTracker, "Trackers" },
                { ETrackedDeviceClass.HMD, "HMDs" },
                { ETrackedDeviceClass.TrackingReference, "Base Stations" },
                { ETrackedDeviceClass.DisplayRedirect, "Display Redirects" }
            };

            foreach(var devClass in devClasses)
            {
                var devices = new uint[OpenVR.k_unMaxTrackedDeviceCount];
                var len = Program.OpenVRHandle.GetSortedTrackedDeviceIndicesOfClass(devClass.Key, devices, 0);
                if (len == 0) continue;

                contextMenuStrip.Items.Add(new ToolStripMenuItem
                {
                    Enabled = false,
                    Text = devClass.Value
                });

                for (uint i = 0; i < len; i++)
                {
                    var index = devices[i];

                    if (!OpenVRUtils.DeviceIsSupported(index)) continue;
                    atLeastOneDevice = true;

                    var lastErr = ETrackedPropertyError.TrackedProp_Success;

                    var providesBattery = Program.OpenVRHandle.GetBoolTrackedDeviceProperty(index, ETrackedDeviceProperty.Prop_DeviceProvidesBatteryStatus_Bool, ref lastErr);

                    var battPct = Program.OpenVRHandle.GetFloatTrackedDeviceProperty(index, ETrackedDeviceProperty.Prop_DeviceBatteryPercentage_Float, ref lastErr);
                    var isCharging = Program.OpenVRHandle.GetBoolTrackedDeviceProperty(index, ETrackedDeviceProperty.Prop_DeviceIsCharging_Bool, ref lastErr);

                    var text = $"{OpenVRUtils.DeviceTypeHumanName(index)}";
                    if (providesBattery) text += $" - {battPct * 100}%";
                    if (isCharging) text += " ⚡️";

                    var item = new ToolStripMenuItem
                    {
                        Enabled = true,
                        Text = text
                    };

                    item.Click += (object sender, EventArgs e) =>
                    {
                        var infoForm = new DeviceInfo(index);
                        infoForm.Show();
                    };

                    contextMenuStrip.Items.Add(item);
                }

                contextMenuStrip.Items.Add(new ToolStripSeparator());
            }

            if (!atLeastOneDevice)
            {
                contextMenuStrip.Items.Add(new ToolStripMenuItem
                {
                    Enabled = false,
                    Text = "No OpenVR devices found"
                });

                contextMenuStrip.Items.Add(new ToolStripSeparator());
            }

            var notShowingHelper = new ToolStripMenuItem
            {
                Text = "Missing device?"
            };

            notShowingHelper.Click += NotShowingHelper_Click;
            contextMenuStrip.Items.Add(notShowingHelper);

            var quit = new ToolStripMenuItem
            {
                Text = "Quit"
            };

            quit.Click += Quit_Click;
            contextMenuStrip.Items.Add(quit);
        }

        private void NotShowingHelper_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Make sure the device you are looking for is tracking, otherwise it will not show in the list.", "Missing device?");
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
