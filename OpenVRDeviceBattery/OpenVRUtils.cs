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
			var capacity = Program.openVRHandle.GetStringTrackedDeviceProperty(deviceId, prop, null, 0, ref error);
			if (capacity > 1)
			{
				var result = new StringBuilder((int)capacity);
				Program.openVRHandle.GetStringTrackedDeviceProperty(deviceId, prop, result, capacity, ref error);
				return result.ToString();
			}
			return null;
		}

		public static bool DeviceIsSupported(uint deviceId)
		{
			var deviceClass = Program.openVRHandle.GetTrackedDeviceClass(deviceId);

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
			var deviceClass = Program.openVRHandle.GetTrackedDeviceClass(deviceId);

			switch(deviceClass)
			{
				case ETrackedDeviceClass.Controller:
					var role = Program.openVRHandle.GetControllerRoleForTrackedDeviceIndex(deviceId);
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
					return "HMD";
				case ETrackedDeviceClass.GenericTracker:
					var serial = GetTrackedDeviceProperty(deviceId, ETrackedDeviceProperty.Prop_SerialNumber_String);
					return $"Tracker ({serial})";
				case ETrackedDeviceClass.TrackingReference:
					return "Base Station";
				case ETrackedDeviceClass.DisplayRedirect:
					return "Display Redirect";
				default:
					return "Unknown Device";
			}

			return "Unknown Device";
		}
	}
}
