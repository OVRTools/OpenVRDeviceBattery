using System;
using System.Windows.Forms;
using Valve.VR;

namespace OpenVRDeviceBattery
{
    static class Program
    {
        public static CVRSystem OpenVRHandle { get; private set; } = null;

        [STAThread]
        static void Main()
        {
            var error = EVRInitError.None;
            var handle = OpenVR.Init(ref error, EVRApplicationType.VRApplication_Background);

            if (error != EVRInitError.None)
            {
                MessageBox.Show($"Could not connect to OpenVR server: {OpenVR.GetStringForHmdError(error)}\n\nPlease make sure an OpenVR server is running (e.g. SteamVR) and try again.", "Fatal Error");
                Application.Exit();
                return;
            }

            OpenVRHandle = handle;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            new BatteryStatus().Start();
            Application.Run();
        }
    }
}
