using System;
using System.Windows.Forms;
using Valve.VR;

namespace OpenVRDeviceBattery
{
    static class Program
    {
        public static CVRSystem openVRHandle { get; private set; } = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var error = EVRInitError.None;
            var handle = OpenVR.Init(ref error, EVRApplicationType.VRApplication_Background);

            if (error != EVRInitError.None)
            {
                Application.Exit();
                return;
            }

            openVRHandle = handle;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            new BatteryStatus().Start();
            Application.Run();
        }
    }
}
