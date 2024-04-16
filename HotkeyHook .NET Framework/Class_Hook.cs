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
        public delegate void HookedFunction(Dictionary<string, object> inArgs);

        public static List<Class_Hook> MyHooks { get; private set; } = new List<Class_Hook>();
        internal static Dictionary<Enum_SupportedKeys, HookedFunction> Dict_HotkeyAndFunction { get; set; } = new Dictionary<Enum_SupportedKeys, HookedFunction>();

        // Properties
        internal IntPtr HookId { get; set; }
        internal Enum_SupportedKeys Hotkey { get; set; }

        private Class_HookManager.HookProc HookProc { get; set; }     // Prevents disposal of callback that is still used
        internal HookedFunction FunctionToCallOnKeyPress { get; set; }
        public Dictionary<string, object> Args { get; set; } = new Dictionary<string, object>();    // Arguments of hooked function

        /// <summary>
        /// This variable is used to prevent the hook from being triggered multiple times when hotkey is being held down
        /// Hook will only trigger if hotkey is down AND this variable is true
        /// This variable is true upon key up, and false upon key down
        /// </summary>
        internal bool isHotkeyReleased { get; set; } = false;

        public static Class_Hook GetHookByHotkey(Enum_SupportedKeys inHotkey)
        {
            return MyHooks.FirstOrDefault(hook => hook.Hotkey == inHotkey);
        }

        internal Class_Hook(IntPtr inHookId, Enum_SupportedKeys inHotkey, Class_HookManager.HookProc inHookProc, HookedFunction inFunctionToCallOnKeyPress)
        {
            this.HookId = inHookId;
            this.Hotkey = inHotkey;

            this.HookProc = inHookProc;
            this.FunctionToCallOnKeyPress = inFunctionToCallOnKeyPress;

            Dict_HotkeyAndFunction.Add(this.Hotkey, inFunctionToCallOnKeyPress);
            MyHooks.Add(this);
        }

        internal void DeleteHook()
        {
            Dict_HotkeyAndFunction.Remove(this.Hotkey);
            MyHooks.Remove(this);
        }

        internal static bool isHotkeyExist(Key inHotkey)
        {
            foreach (Class_Hook hook in MyHooks)
            {
                if ((Key) hook.Hotkey == inHotkey)
                    return true;
            }
            return false;
        }
    }
}
