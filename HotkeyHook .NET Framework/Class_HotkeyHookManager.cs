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
    public class Class_HotkeyHookManager
    {
        internal delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        const int WH_KEYBOARD = 2;        //Hook id for key stroke
        const int WH_KEYBOARD_LL = 13;
        public static IntPtr SetHotkey(uint inThreadId, string inHotKey, Class_Hook.HookedFunction inFunctionToRunOnKeyUp)
        {
            inHotKey = inHotKey.ToUpper();

            if (!Class_Hook.isHotkeySupported(inHotKey))
                throw new Exception("Hotkey \"" + inHotKey + "\" is not supported");

            Process currentProcess = Process.GetCurrentProcess();
            ProcessModule currentModule = currentProcess.MainModule;

            IntPtr moduleHandle = GetModuleHandle(currentModule.ModuleName);

            //IntPtr hookId = SetWindowsHookExA(WH_KEYBOARD,
            //                                  HookCallback,
            //                                  moduleHandle,
            //                                  inThreadId);

            HookProc newHookProc = new HookProc(HookCallback);
            IntPtr hookId = SetWindowsHookExW(WH_KEYBOARD_LL,
                                              newHookProc,
                                              IntPtr.Zero,
                                              0);

            Console.WriteLine("hookId: " + hookId.ToString());
            if (hookId == IntPtr.Zero)
            {
                Console.WriteLine("Hook Error Code: " + Marshal.GetLastWin32Error().ToString());
                return IntPtr.Zero;
            }

            new Class_Hook(hookId, inHotKey, newHookProc, inFunctionToRunOnKeyUp);

            Console.WriteLine("Hotkey \"" + inHotKey + "\" is successfully set");
            return hookId;
        }

        public static void UnsetHotkey(string inStrHotkey)
        {
            Class_Hook removedHook;
            try
            {
                removedHook = Class_Hook.MyHooks.First(hook => hook.StrHotkey == inStrHotkey);
            }
            catch
            {
                Console.WriteLine("Trying to remove hook that doesn't exist");
                return;
            }

            UnhookWindowsHookEx(removedHook.HookId);
            removedHook.DeleteHook();
        }

        public static void UnsetAllHotkeys()
        {
            while (Class_Hook.MyHooks.Count > 0)
            {
                UnsetHotkey(Class_Hook.MyHooks[0].StrHotkey);
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
                if (Keyboard.IsKeyDown(hook.Hotkey) &&
                    hook.isHotkeyReleased)
                {
                    hook.isHotkeyReleased = false;

                    hook.FunctionToCallOnKeyPress.Invoke(hook.Args);

                    return IntPtr.Zero;
                }

                if (Keyboard.IsKeyUp(hook.Hotkey))
                    hook.isHotkeyReleased = true;
            }

            return IntPtr.Zero;
        }

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
        public static extern uint GetCurrentThreadId();
    }
}