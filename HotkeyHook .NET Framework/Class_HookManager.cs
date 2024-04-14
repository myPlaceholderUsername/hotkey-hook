using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Input;

namespace HotkeyHook
{
    public class Class_HookManager
    {
        internal delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        const int WH_KEYBOARD = 2;          //Hook id for key stroke
        const int WH_KEYBOARD_LL = 13;
        public static bool SetHotkeyGlobal(Enum_SupportedKeys inHotKey, Class_Hook.HookedFunction inFunctionToRunOnKeyPress)
        {
            if (!Class_SupportedKeys.IsHotkeySupported(inHotKey))
                return false;

            HookProc newHookProc = new HookProc(HookCallback);
            IntPtr hookId = SetWindowsHookExW(WH_KEYBOARD_LL,
                                              newHookProc,
                                              IntPtr.Zero,          // Last 2 parameters are 0 for global hotkey
                                              0);
            if (IsSetHookError(hookId))
                return false;

            new Class_Hook(hookId, inHotKey, newHookProc, inFunctionToRunOnKeyPress);
            LogSetHookSuccess(inHotKey);
            return true;
        }

        public static bool SetHotkeyLocal(Enum_SupportedKeys inHotKey, Class_Hook.HookedFunction inFunctionToRunOnKeyPress)
        {
            if (!Class_SupportedKeys.IsHotkeySupported(inHotKey))
                return false;

            // Set module handle
            IntPtr moduleHandle;
            {
                using (Process currentProcess = Process.GetCurrentProcess())
                using (ProcessModule currentModule = currentProcess.MainModule)
                    moduleHandle = GetModuleHandle(currentModule.ModuleName);
            }
            uint threadId = GetCurrentThreadId();

            HookProc newHookProc = new HookProc(HookCallback);
            IntPtr hookId = SetWindowsHookExW(WH_KEYBOARD,
                                              newHookProc,
                                              moduleHandle,
                                              threadId);

            if (IsSetHookError(hookId))
                return false;

            new Class_Hook(hookId, inHotKey, newHookProc, inFunctionToRunOnKeyPress);
            LogSetHookSuccess(inHotKey);
            return true;
        }

        public static void UnsetHotkey(Enum_SupportedKeys inHotkey)
        {
            foreach (Class_Hook hook in Class_Hook.MyHooks)
            {
                if (hook.Hotkey == inHotkey)
                {
                    UnhookWindowsHookEx(hook.HookId);
                    hook.DeleteHook();
                    return;
                }
            }
        }

        public static void UnsetAllHotkeys()
        {
            while (Class_Hook.MyHooks.Count > 0)
            {
                Class_Hook hook = Class_Hook.MyHooks[0];

                UnhookWindowsHookEx(hook.HookId);
                hook.DeleteHook();
            }
        }

        // wParam = The key that triggers the event
        // lParam = Key event
        static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
                return IntPtr.Zero;

            foreach (Class_Hook hook in Class_Hook.MyHooks)
            {
                if (Keyboard.IsKeyDown((Key) hook.Hotkey) &&
                    hook.isHotkeyReleased)
                {
                    hook.isHotkeyReleased = false;
                    hook.FunctionToCallOnKeyPress.Invoke(hook.Args);

                    return IntPtr.Zero;
                }

                if (Keyboard.IsKeyUp((Key) hook.Hotkey))
                    hook.isHotkeyReleased = true;
            }

            return IntPtr.Zero;
        }

        static bool IsSetHookError(IntPtr inHookId)
        {
            if (inHookId == IntPtr.Zero)
            {
                Console.WriteLine("Hook Error Code: " + Marshal.GetLastWin32Error().ToString());
                return true;
            }

            return false;
        }

        static void LogSetHookSuccess(Enum_SupportedKeys inHotkey)
        {
            Console.WriteLine("Hotkey \"" + inHotkey.ToString() + "\" is successfully set");
        }


        // Here be imported functions from C/C++

        // idHook = Type of message/hook
        // lpfn = Function to run when message is received. In this case, function to run when key is pressed
        // hmod = Handle to module of this DLL
        // dwThreadId = Thread ID to the project that calls this DLL
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookExW(int idHook, HookProc lpfn, IntPtr hmod, uint dwThreadId);
        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(IntPtr lpModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern uint GetCurrentThreadId();
    }
}