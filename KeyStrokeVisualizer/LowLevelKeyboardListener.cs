﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace DesktopWPFAppLowLevelKeyboardHook
{
    public class LowLevelKeyboardListener
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public event EventHandler<KeyDownArgs>? OnKeyDown;
        public event EventHandler<KeyUpArgs>? OnKeyUp;

        private readonly LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        public LowLevelKeyboardListener() => _proc = HookCallback;

        public void HookKeyboard() => _hookID = SetHook(_proc);

        public void UnHookKeyboard() => UnhookWindowsHookEx(_hookID);

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using Process currentProcess = Process.GetCurrentProcess();
            using ProcessModule currentModule = currentProcess.MainModule!;
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(currentModule.ModuleName!), 0);
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                OnKeyUp?.Invoke(this, new KeyUpArgs(KeyInterop.KeyFromVirtualKey(vkCode)));
                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }

            else if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                OnKeyDown?.Invoke(this, new KeyDownArgs(KeyInterop.KeyFromVirtualKey(vkCode)));
                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }
            else return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }

    public class KeyDownArgs : EventArgs
    {
        public Key KeyPressed { get; private set; }

        public KeyDownArgs(Key key)
        {
            KeyPressed = key;
        }
    }

    public class KeyUpArgs : EventArgs
    {
        public Key KeyPressed { get; private set; }

        public KeyUpArgs(Key key)
        {
            KeyPressed = key;
        }
    }
}