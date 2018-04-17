using System;
using System.Linq;
using TornadoScript.ScriptMain.Utility;

namespace TornadoScript.ScriptMain.Memory
{
    public sealed unsafe class Pattern
    {
        private readonly string _bytes, _mask;

        public Pattern(string bytes, string mask)
        {
            _bytes = bytes;
            _mask = mask;
        }

        public IntPtr Get(int offset = 0)
        {
            Win32Native.MODULEINFO module;

            Win32Native.GetModuleInformation(
                Win32Native.GetCurrentProcess(), 
                Win32Native.GetModuleHandle(null), out module, (uint)sizeof(Win32Native.MODULEINFO));

            for (var address = module.LpBaseOfDll.ToInt64(); 
                address < address + module.SizeOfImage; address++)
            {
                if (ByteCompare((byte*)address, _bytes.ToCharArray(), _mask.ToCharArray()))
                {
                    return new IntPtr(address + offset);
                }
            }

            return IntPtr.Zero;
        }

        private static bool ByteCompare(byte* pData, char[] bMask, char[] szMask)
        {
            return !bMask.Where((t, i) => szMask[i] == 'x' && pData[i] != t).Any();
        }
    }
}
