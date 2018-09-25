using System;
<<<<<<< HEAD
=======
using System.Linq;
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
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
<<<<<<< HEAD
            MODULEINFO module;

            Win32Native.GetModuleInformation(
                Win32Native.GetCurrentProcess(), 
                Win32Native.GetModuleHandle(null), out module, (uint)sizeof(MODULEINFO));
=======
            Win32Native.MODULEINFO module;

            Win32Native.GetModuleInformation(
                Win32Native.GetCurrentProcess(), 
                Win32Native.GetModuleHandle(null), out module, (uint)sizeof(Win32Native.MODULEINFO));
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48

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
<<<<<<< HEAD
            for (int i = 0; i < bMask.Length; i++)
            {
                if (szMask[i] != '?' && pData[i] != bMask[i])
                {
                    break;
                }
                else if (i + 1 == bMask.Length)
                {
                    return true;
                }
            }

            return false;
=======
            return !bMask.Where((t, i) => szMask[i] == 'x' && pData[i] != t).Any();
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
        }
    }
}
