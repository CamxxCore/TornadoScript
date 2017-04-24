using System;
using System.Runtime.InteropServices;

namespace TornadoScript.Memory
{
    public unsafe sealed class Pattern
    {
        private string _bytes, _mask;

        public Pattern(string bytes, string mask)
        {
            this._bytes = bytes;
            this._mask = mask;
        }

        public IntPtr Get(int offset = 0)
        {
            Win32Native.Moduleinfo module;

            Win32Native.GetModuleInformation(Win32Native.GetCurrentProcess(), Win32Native.GetModuleHandle(null), out module, (uint)sizeof(Win32Native.Moduleinfo));

            var address = module.LpBaseOfDll.ToInt64();

            var end = address + module.SizeOfImage;

            for (;address < end; address++)
            {
                if (BCompare((byte*)(address), _bytes.ToCharArray(), _mask.ToCharArray()))
                {
                    return new IntPtr(address + offset);
                }
            }

            return IntPtr.Zero;
        }

        private bool BCompare(byte* pData, char[] bMask, char[] szMask)
        {
            int i = 0;

            for (; i < bMask.Length; i++)
            {
                if (szMask[i] == 'x' && pData[i] != bMask[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
