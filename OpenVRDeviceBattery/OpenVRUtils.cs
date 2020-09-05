using System.Text;
using Valve.VR;

namespace OpenVRDeviceBattery
{
    class OpenVRUtils
    {
		// https://github.com/ValveSoftware/openvr/blob/60eb187801956ad277f1cae6680e3a410ee0873b/samples/unity_keyboard_sample/Assets/SteamVR/Scripts/SteamVR.cs#L166-L177
		public static string GetTrackedDeviceProperty(uint deviceId, ETrackedDeviceProperty prop)
		{
			var error = ETrackedPropertyError.TrackedProp_Success;
			var capacity = Program.OpenVRHandle.GetStringTrackedDeviceProperty(deviceId, prop, null, 0, ref error);
			if (capacity > 1)
			{
				var result = new StringBuilder((int)capacity);
				Program.OpenVRHandle.GetStringTrackedDeviceProperty(deviceId, prop, result, capacity, ref error);
				return result.ToString();
			}
			return null;
		}

		public static bool DeviceIsSupported(uint deviceId)
		{
			var deviceClass = Program.OpenVRHandle.GetTrackedDeviceClass(deviceId);

			switch (deviceClass)
			{
				case ETrackedDeviceClass.Controller:
				case ETrackedDeviceClass.HMD:
				case ETrackedDeviceClass.GenericTracker:
				case ETrackedDeviceClass.TrackingReference:
				case ETrackedDeviceClass.DisplayRedirect:
					return true;
			}

			return false;
		}

		public static string DeviceTypeHumanName(uint deviceId)
		{
			var deviceClass = Program.OpenVRHandle.GetTrackedDeviceClass(deviceId);

			switch(deviceClass)
			{
				case ETrackedDeviceClass.Controller:
					var role = Program.OpenVRHandle.GetControllerRoleForTrackedDeviceIndex(deviceId);
					switch(role)
					{
						case ETrackedControllerRole.LeftHand:
							return "Left Controller";
						case ETrackedControllerRole.RightHand:
							return "Right Controller";
						case ETrackedControllerRole.Invalid:
							return "Controller";
						case ETrackedControllerRole.Stylus:
							return "Stylus";
						case ETrackedControllerRole.Treadmill:
							return "Treadmill";
					}
					break;
				case ETrackedDeviceClass.HMD:
					return $"HMD ({GetTrackedDeviceProperty(deviceId, ETrackedDeviceProperty.Prop_SerialNumber_String)})";
				case ETrackedDeviceClass.GenericTracker:
					return $"Tracker ({GetTrackedDeviceProperty(deviceId, ETrackedDeviceProperty.Prop_SerialNumber_String)})";
				case ETrackedDeviceClass.TrackingReference:
					return $"Base Station ({GetTrackedDeviceProperty(deviceId, ETrackedDeviceProperty.Prop_SerialNumber_String)})";
				case ETrackedDeviceClass.DisplayRedirect:
					return "Display Redirect";
				default:
					return "Unknown Device";
			}

			return "Unknown Device";
		}
	}
}
