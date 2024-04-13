using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HotkeyHook
{
    public class Class_Hook
    {
        public delegate void HookedFunction(List<object> inArgs);

        static List<Class_Hook> _myHooks = new List<Class_Hook>();
        static Dictionary<string, HookedFunction> _dict_HotkeyAndFunction = new Dictionary<string, HookedFunction>();

        // Properties
        IntPtr _hookId;
        string _strHotkey = String.Empty;
        int _hotkeyVkCode;

        HookedFunction _functionToCallOnKeyPress;
        List<object> _args = new List<object>();

        internal Class_Hook(IntPtr inHookId, string inHotKey, HookedFunction inFunctionToCallOnKeyPress)
        {
            this.HookId = inHookId;
            this.StrHotkey = inHotKey;
            this.HotkeyVkCode = dict_HotkeyAndVkCode[this.StrHotkey];

            this.FunctionToCallOnKeyPress = inFunctionToCallOnKeyPress;

            Dict_HotkeyAndFunction.Add(this.StrHotkey, inFunctionToCallOnKeyPress);
            MyHooks.Add(this);
        }

        internal void DeleteHook()
        {
            Dict_HotkeyAndFunction.Remove(this.StrHotkey);
            MyHooks.Remove(this);
        }

        internal static bool isHotkeyExist(int inHotkeyVkCode)
        {
            for (int i = 0; i < MyHooks.Count; i++)
            {
                if (MyHooks[i].HotkeyVkCode == inHotkeyVkCode)
                    return true;
            }
            return false;
        }

        internal static bool isHotkeySupported(string inHotkey)
        {
            return dict_HotkeyAndVkCode.ContainsKey(inHotkey);
        }

        internal static Dictionary<string, int> dict_HotkeyAndVkCode { get; private set; } = new Dictionary<string, int>()
        {
            { "F1", 0x70 },
            { "F2", 0x71 },
            { "F3", 0x72 },
            { "F4", 0x73 },
            { "F5", 0x74 },
            { "F6", 0x75 },
            { "F7", 0x76 },
            { "F8", 0x77 },
            { "F9", 0x78 },
            { "F10", 0x79 },
            { "F11", 0x7A },
            { "F12", 0x7B },
        };

        //Get Set
        public static List<Class_Hook> MyHooks { get => _myHooks; private set => _myHooks = value; }
        internal static Dictionary<string, HookedFunction> Dict_HotkeyAndFunction { get => _dict_HotkeyAndFunction; set => _dict_HotkeyAndFunction = value; }

        public IntPtr HookId { get => _hookId; internal set => _hookId = value; }
        internal string StrHotkey { get => _strHotkey; set => _strHotkey = value; }
        internal int HotkeyVkCode { get => _hotkeyVkCode; set => _hotkeyVkCode = value; }

        internal HookedFunction FunctionToCallOnKeyPress { get => _functionToCallOnKeyPress; set => _functionToCallOnKeyPress = value; }
        public List<object> Args { get => _args; set => _args = value; }
    }
}
