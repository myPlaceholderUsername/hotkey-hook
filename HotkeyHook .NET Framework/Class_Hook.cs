using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HotkeyHook
{
    public class Class_Hook
    {
        public delegate void HookedFunction(List<object> inArgs);

        internal static List<Class_Hook> MyHooks { get; private set; } = new List<Class_Hook>();
        internal static Dictionary<string, HookedFunction> Dict_HotkeyAndFunction { get; set; } = new Dictionary<string, HookedFunction>();

        // Properties
        public IntPtr HookId { get; internal set; }
        internal string StrHotkey { get; set; }
        internal Key Hotkey { get; set; }

        private Class_HotkeyHookManager.HookProc HookProc { get; set; }     // Prevents disposal of callback that is still used
        internal HookedFunction FunctionToCallOnKeyPress { get; set; }
        public List<object> Args { get; set; }

        /// <summary>
        /// This variable is used to prevent the hook from being triggered multiple times when hotkey is being held down
        /// Hook will only trigger if hotkey is down AND this variable is true
        /// This variable is true upon key up, and false upon key down
        /// </summary>
        internal bool isHotkeyReleased { get; set; } = false;

        internal Class_Hook(IntPtr inHookId, string inHotKey, Class_HotkeyHookManager.HookProc inHookProc, HookedFunction inFunctionToCallOnKeyPress)
        {
            this.HookId = inHookId;
            this.StrHotkey = inHotKey;
            this.Hotkey = Dict_StrHotkeyAndKey[this.StrHotkey];

            this.HookProc = inHookProc;
            this.FunctionToCallOnKeyPress = inFunctionToCallOnKeyPress;

            Dict_HotkeyAndFunction.Add(this.StrHotkey, inFunctionToCallOnKeyPress);
            MyHooks.Add(this);
        }

        internal void DeleteHook()
        {
            Dict_HotkeyAndFunction.Remove(this.StrHotkey);
            MyHooks.Remove(this);
        }

        internal static bool isHotkeyExist(Key inHotkeyVkCode)
        {
            for (int i = 0; i < MyHooks.Count; i++)
            {
                if (MyHooks[i].Hotkey == inHotkeyVkCode)
                    return true;
            }
            return false;
        }

        internal static bool isHotkeySupported(string inHotkey)
        {
            return Dict_StrHotkeyAndKey.ContainsKey(inHotkey);
        }

        internal static Dictionary<string, Key> Dict_StrHotkeyAndKey { get; private set; } = new Dictionary<string, Key>()
        {
            { "F1", Key.F1 },
            { "F2", Key.F2 },
            { "F3", Key.F3 },
            { "F4", Key.F4 },
            { "F5", Key.F5 },
            { "F6", Key.F6 },
            { "F7", Key.F7 },
            { "F8", Key.F8 },
            { "F9", Key.F9 },
            { "F10", Key.F10 },
            { "F11", Key.F11 },
            { "F12", Key.F12 },
        };
    }
}
