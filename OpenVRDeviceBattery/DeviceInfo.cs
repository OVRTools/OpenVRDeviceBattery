using System;
using System.Windows.Forms;
using Valve.VR;

namespace OpenVRDeviceBattery
{
    public partial class DeviceInfo : Form
    {
        private uint deviceIndex;

        public DeviceInfo(uint index)
        {
            deviceIndex = index;

            InitializeComponent();
            Text = OpenVRUtils.DeviceTypeHumanName(deviceIndex);
            PopulateTable();
        }

        private void PopulateTable()
        {
            foreach(string name in Enum.GetNames(typeof(ETrackedDeviceProperty)))
            {
                string value = "Unknown";

                ETrackedDeviceProperty prop = (ETrackedDeviceProperty) Enum.Parse(typeof(ETrackedDeviceProperty), name);

                var err = ETrackedPropertyError.TrackedProp_Success;

                if (name.EndsWith("_String"))
                    value = OpenVRUtils.GetTrackedDeviceProperty(deviceIndex, prop);
                else if (name.EndsWith("_Bool"))
                    value = Program.OpenVRHandle.GetBoolTrackedDeviceProperty(deviceIndex, prop, ref err).ToString();
                else if (name.EndsWith("_Float"))
                    value = Program.OpenVRHandle.GetFloatTrackedDeviceProperty(deviceIndex, prop, ref err).ToString();
                else if (name.EndsWith("_Uint64"))
                    value = Program.OpenVRHandle.GetUint64TrackedDeviceProperty(deviceIndex, prop, ref err).ToString();
                else if (name.EndsWith("_Int32"))
                    value = Program.OpenVRHandle.GetInt32TrackedDeviceProperty(deviceIndex, prop, ref err).ToString();
                else if (name.EndsWith("_Matrix34"))
                    value = "Unsupported Property Type";

                string[] row =
                {
                    name,
                    value
                };

                dataGridView.Rows.Add(row);
            }
        }
    }
}
