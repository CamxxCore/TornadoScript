using System;
using System.Runtime.InteropServices;

namespace TornadoScript.Memory
{
    public unsafe sealed class Pattern
    {
        private string bytes, mask;

        public Pattern(string bytes, string mask)
        {
            this.bytes = bytes;
            this.mask = mask;
        }

        public IntPtr Get(int offset = 0)
        {
            MODULEINFO module;

            Interop.GetModuleInformation(Interop.GetCurrentProcess(), Interop.GetModuleHandle(null), out module, (uint)sizeof(MODULEINFO));

            var address = module.lpBaseOfDll.ToInt64();

            var end = address + module.SizeOfImage;

            for (; address < end; address++)
            {
                if (bCompare((byte*)(address), bytes.ToCharArray(), mask.ToCharArray()))
                {
                    return new IntPtr(address + offset);
                }
            }

            return IntPtr.Zero;
        }

        private bool bCompare(byte* pData, char[] bMask, char[] szMask)
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
