using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HotkeyHook
{
    public enum Enum_SupportedKeys
    {
        None = 0,
        F1 = Key.F1,
        F2 = Key.F2,
        F3 = Key.F3,
        F4 = Key.F4,
        F5 = Key.F5,
        F6 = Key.F6,
        F7 = Key.F7,
        F8 = Key.F8,
        F9 = Key.F9,
        F10 = Key.F10,
        F11 = Key.F11,
        F12 = Key.F12,
    }

    public static class Class_SupportedKeys
    {
        public static Enum_SupportedKeys GetEnumNameFromString(string inStr)
        {
            try
            {
                return (Enum_SupportedKeys)Enum.Parse(typeof(Enum_SupportedKeys), inStr);
            }
            catch
            {
                Console.WriteLine("Hotkey is not found in Enum, therefore not supported");
                return Enum_SupportedKeys.None;
            }
        }

        internal static bool IsHotkeySupported(Enum_SupportedKeys inHotkey)
        {
            if (inHotkey == Enum_SupportedKeys.None)
                return false;
            return true;
        }
    }
}
