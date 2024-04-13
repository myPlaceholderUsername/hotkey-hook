using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace HotkeyHook
{
    public class Class_HotkeyHookManager
    {
        delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

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

            IntPtr hookId = SetWindowsHookExA(WH_KEYBOARD,
                                              HookCallback,
                                              moduleHandle,
                                              inThreadId);

            //IntPtr hookId = SetWindowsHookExA(WH_KEYBOARD,
            //                                  HookCallback,
            //                                  IntPtr.Zero,
            //                                  inThreadId);

            Console.WriteLine("hookId: " + hookId.ToString());
            if (hookId == IntPtr.Zero)
            {
                Console.WriteLine("Hook Error Code: " + Marshal.GetLastWin32Error().ToString());
                return IntPtr.Zero;
            }

            new Class_Hook(hookId, inHotKey, inFunctionToRunOnKeyUp);

            Console.WriteLine("Hotkey \"" + inHotKey + "\" is successfully set");
            return hookId;
        }

        public static void UnsetHotkey(IntPtr inHookId)
        {
            Class_Hook removedHook = Class_Hook.MyHooks.First(hook => hook.HookId == inHookId);
            if (removedHook == null)
            {
                Console.WriteLine("Trying to remove hook that doesn't exist");
                return;
            }

            removedHook.DeleteHook();
            UnhookWindowsHookEx(inHookId);
        }

        // wParam = The key that triggers the event
        // lParam = Key event
        static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode <= 0)
                return IntPtr.Zero;

            int vkCode = Marshal.ReadInt32(wParam); // Read vkCode (int) at wParam (IntPtr)

            if (!Class_Hook.isHotkeyExist(vkCode))
                return IntPtr.Zero;

            Class_Hook connectedHook = Class_Hook.MyHooks.First(hook => hook.HotkeyVkCode == vkCode);
            if (isKeyUp(lParam))
                connectedHook.FunctionToCallOnKeyPress.Invoke(connectedHook.Args);

            //foreach (Class_Hook hook in Class_Hook.MyHooks)
            //{
            //    if (Keyboard.IsKeyUp(Key.))
            //    {

            //    }
            //}

            return IntPtr.Zero;
        }

        const int WM_KEYUP = 0x0101;
        const int WM_SYSKEYUP = 0x0105;
        static bool isKeyUp(IntPtr inMessage)
        {
            return (inMessage == (IntPtr)WM_KEYUP ||
                    inMessage == (IntPtr)WM_SYSKEYUP);
        }

        // idHook = Type of message/hook
        // lpfn = Function to run when message is received. In this case, function to run when key is pressed
        // hmod = Handle to module of this DLL
        // dwThreadId = Thread ID to the project that calls this DLL
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookExA(int idHook, HookProc lpfn, IntPtr hmod, uint dwThreadId);

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