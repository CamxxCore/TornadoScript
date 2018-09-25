using System;
<<<<<<< HEAD
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
=======
using System.Runtime.InteropServices;
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
using System.Text;
using System.Windows.Input;

namespace TornadoScript.ScriptMain.Utility
{
<<<<<<< HEAD
    [Flags]
    public enum ThreadAccess : int
    {
        TERMINATE = (0x0001),
        SUSPEND_RESUME = (0x0002),
        GET_CONTEXT = (0x0008),
        SET_CONTEXT = (0x0010),
        SET_INFORMATION = (0x0020),
        QUERY_INFORMATION = (0x0040),
        SET_THREAD_TOKEN = (0x0080),
        IMPERSONATE = (0x0100),
        DIRECT_IMPERSONATION = (0x0200)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CLIENT_ID
    {
        public uint UniqueProcess; // original: PVOID
        public uint UniqueThread; // original: PVOID
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x30)]
    public struct THREAD_BASIC_INFORMATION
    {
        [FieldOffset(0x0000)] public int ExitStatus;
        [FieldOffset(0x0008)] public IntPtr TebBaseAddress;
    }

    // http://msdn.moonsols.com/win7rtm_x64/TEB.html
    [StructLayout(LayoutKind.Explicit, Size = 0x1818)]
    public struct TEB
    {
        [FieldOffset(0x0058)] public IntPtr ThreadLocalStoragePointer;
    }

    public enum PlaySoundFlags : uint
    {
        SND_SYNC = 0x0,            // play synchronously (default)
        SND_ASYNC = 0x1,            // play asynchronously
        SND_NODEFAULT = 0x2,        // silence (!default) if sound not found
        SND_MEMORY = 0x4,            // pszSound points to a memory file
        SND_LOOP = 0x8,            // loop the sound until next sndPlaySound
        SND_NOSTOP = 0x10,            // don't stop any currently playing sound
        SND_NOWAIT = 0x2000,        // don't wait if the driver is busy
        SND_ALIAS = 0x10000,        // name is a registry alias
        SND_ALIAS_ID = 0x110000,        // alias is a predefined ID
        SND_FILENAME = 0x20000,        // name is file name
        SND_RESOURCE = 0x40004,        // name is resource name or atom
    };

    public struct MODULEINFO
    {
        public IntPtr LpBaseOfDll;
        public uint SizeOfImage;
        public IntPtr EntryPoint;
    }

    public enum ThreadInfoClass : int
    {
        ThreadQuerySetWin32StartAddress = 9
    }

    public sealed unsafe class Win32Native
    {
        public delegate int NtQueryInformationThreadDelegate(IntPtr threadHandle, uint threadInformationClass, THREAD_BASIC_INFORMATION* outThreadInformation, ulong threadInformationLength, ulong* returnLength);

        public static NtQueryInformationThreadDelegate NtQueryInformationThread { get; }

        static Win32Native()
        {
            IntPtr ntdllHandle = GetModuleHandle("ntdll.dll");
            NtQueryInformationThread = Marshal.GetDelegateForFunctionPointer<NtQueryInformationThreadDelegate>(GetProcAddress(ntdllHandle, "NtQueryInformationThread"));
        }

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr moduleHandle, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenThread(ThreadAccess desiredAccess, bool inheritHandle, int threadId);

        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

=======
    public sealed class Win32Native
    {
        public struct MODULEINFO
        {
            public IntPtr LpBaseOfDll;
            public uint SizeOfImage;
            public IntPtr EntryPoint;
        }

>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool GetModuleInformation(IntPtr hProcess, IntPtr hModule, out MODULEINFO lpmodinfo, uint cb);

        [DllImport("user32.dll", EntryPoint = "BlockInput")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BlockInput([MarshalAs(UnmanagedType.Bool)] bool fBlockIt);

        public enum MapType : uint
        {
            MapvkVkToVsc = 0x0,
            MapvkVscToVk = 0x1,
            MapvkVkToChar = 0x2,
            MapvkVscToVkEx = 0x3,
        }

        [DllImport("user32.dll")]
        public static extern int ToUnicode(
            uint wVirtKey,
            uint wScanCode,
            byte[] lpKeyState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)]
            StringBuilder pwszBuff,
            int cchBuff,
            uint wFlags);

        [DllImport("user32.dll")]
        public static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, MapType uMapType);

        public static char GetCharFromKey(Key key, bool shift)
        {
            char ch = ' ';

            int virtualKey = KeyInterop.VirtualKeyFromKey(key);
            byte[] keyboardState = new byte[256];

            if (shift)
                keyboardState[0x10] = 0x80;
            GetKeyboardState(keyboardState);

            uint scanCode = MapVirtualKey((uint)virtualKey, MapType.MapvkVkToVsc);
            StringBuilder stringBuilder = new StringBuilder(2);

            int result = ToUnicode((uint)virtualKey, scanCode, keyboardState, stringBuilder, stringBuilder.Capacity, 0);
            switch (result)
            {
                case -1:
                    break;
                case 0:
                    break;
                case 1:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
                default:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
            }
            return ch;
        }
<<<<<<< HEAD

       [DllImport("winmm.dll", SetLastError = true)]
       public static extern int PlaySound(
       string szSound,
       IntPtr hModule,
       int flags);
=======
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
    }
}


