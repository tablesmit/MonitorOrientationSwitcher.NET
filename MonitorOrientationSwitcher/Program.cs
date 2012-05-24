using System;

namespace MonitorOrientationSwitcher
{
    class Program
    {
        private int[] orientationValues = new int[4]{NativeMethods.DMDO_DEFAULT,
														NativeMethods.DMDO_90,
														NativeMethods.DMDO_180,
														NativeMethods.DMDO_270};
        public static void Main(string[] args)
        {
            if (args.Length == 3)
            {
                AntiClockWise(int.Parse(args[0]), int.Parse(args[1]), int.Parse(args[2]));
            } else
            {
                Console.Write(@"MonitorOrientationSwitcher
Usage: MonitorOrientationSwitcher.exe <rotation-angle> <width> <height>");
            }
        }
        private static void AntiClockWise(int angle, int width, int height)
        {
            // obtain current settings
            DEVMODE dm = NativeMethods.CreateDevmode();
            GetSettings(ref dm);

            // swap height and width
            int temp = dm.dmPelsHeight;
            dm.dmPelsHeight = dm.dmPelsWidth;
            dm.dmPelsWidth = temp;

            // set the orientation value to what's next anti-clockwise
            dm.dmPelsWidth = width;
            dm.dmPelsHeight = height;
            switch (angle)
            {
                case 0:
                    dm.dmDisplayOrientation = NativeMethods.DMDO_DEFAULT;
                    break;
                case 90:
                    dm.dmDisplayOrientation = NativeMethods.DMDO_90;
                    break;
                case 180:
                    dm.dmDisplayOrientation = NativeMethods.DMDO_180;
                    break;
                case 270:
                    dm.dmDisplayOrientation = NativeMethods.DMDO_270;
                    break;
            }

            // switch to new settings
            ChangeSettings(dm);
        }
        private static int GetSettings(ref DEVMODE dm)
        {
            // helper to obtain current settings
            return GetSettings(ref dm, NativeMethods.ENUM_CURRENT_SETTINGS);
        }

        private static int GetSettings(ref DEVMODE dm, int iModeNum)
        {
            // helper to wrap EnumDisplaySettings Win32 API
            return NativeMethods.EnumDisplaySettings(null, iModeNum, ref dm);
        }

        private static void ChangeSettings(DEVMODE dm)
        {
            // helper to wrap ChangeDisplaySettings Win32 API

            var iRet = NativeMethods.ChangeDisplaySettings(ref dm, 0);
            switch (iRet)
            {
                case NativeMethods.DISP_CHANGE_SUCCESSFUL:
                    break;
                case NativeMethods.DISP_CHANGE_RESTART:
                    Console.WriteLine("Please restart your system");
                    break;
                case NativeMethods.DISP_CHANGE_FAILED:
                    Console.WriteLine("ChangeDisplaySettigns API failed");
                    break;
                case NativeMethods.DISP_CHANGE_BADDUALVIEW:
                    Console.WriteLine("The settings change was unsuccessful because system is DualView capable.");
                    break;
                case NativeMethods.DISP_CHANGE_BADFLAGS:
                    Console.WriteLine("An invalid set of flags was passed in.");
                    break;
                case NativeMethods.DISP_CHANGE_BADPARAM:
                    Console.WriteLine("An invalid parameter was passed in. This can include an invalid flag or combination of flags.");
                    break;
                case NativeMethods.DISP_CHANGE_NOTUPDATED:
                    Console.WriteLine("Unable to write settings to the registry.");
                    break;
                default:
                    Console.WriteLine("Unknown return value from ChangeDisplaySettings API");
                    break;
            }

        }
    }
}
