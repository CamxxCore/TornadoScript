using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using TornadoScript.ScriptMain.Utility;
using System.Runtime.InteropServices;
namespace TornadoScript.ScriptMain
{
    /// <summary>
    /// thanks alex
    /// https://github.com/alexguirre/Spotlight/blob/master/Source/Core/Memory/WinFunctions.cs
    /// </summary>
    public static unsafe class WinHelper
    {
        private static int mainThreadId = -1;

        private static Dictionary<int, IntPtr> threadHandleDictionary = new Dictionary<int, IntPtr>();

        private static Dictionary<IntPtr, THREAD_BASIC_INFORMATION> threadInformationDictionary = new Dictionary<IntPtr, THREAD_BASIC_INFORMATION>();

        public static int GetProcessMainThreadId()
        {
            if (mainThreadId == -1)
            {
                long lowestStartTime = long.MaxValue;
                ProcessThread lowestStartTimeThread = null;
                foreach (ProcessThread thread in Process.GetCurrentProcess().Threads)
                {
                    long startTime = thread.StartTime.Ticks;
                    if (startTime < lowestStartTime)
                    {
                        lowestStartTime = startTime;
                        lowestStartTimeThread = thread;
                    }
                }

                mainThreadId = lowestStartTimeThread == null ? -1 : lowestStartTimeThread.Id;
            }

            return mainThreadId;
        }

        public static void CopyTlsValues(IntPtr sourceThreadHandle, IntPtr targetThreadHandle, params int[] valuesOffsets)
        {
            THREAD_BASIC_INFORMATION sourceThreadInfo, targetThreadInfo;

            if (!threadInformationDictionary.TryGetValue(sourceThreadHandle, out sourceThreadInfo))
            {
                sourceThreadInfo = new THREAD_BASIC_INFORMATION();
                int sourceStatus = Win32Native.NtQueryInformationThread(sourceThreadHandle, 0, &sourceThreadInfo, (ulong)sizeof(THREAD_BASIC_INFORMATION), null);
                if (sourceStatus != 0)
                {
                    ScriptCore.Logger.Log($"Source Thread Invalid Query Status: {sourceStatus}");
                    return;
                }

                threadInformationDictionary[sourceThreadHandle] = sourceThreadInfo;
            }

            if (!threadInformationDictionary.TryGetValue(targetThreadHandle, out targetThreadInfo))
            {
                targetThreadInfo = new THREAD_BASIC_INFORMATION();
                int sourceStatus = Win32Native.NtQueryInformationThread(targetThreadHandle, 0, &targetThreadInfo, (ulong)sizeof(THREAD_BASIC_INFORMATION), null);
                if (sourceStatus != 0)
                {
                    ScriptCore.Logger.Log($"Source Thread Invalid Query Status: {sourceStatus}");
                    return;
                }

                threadInformationDictionary[targetThreadHandle] = targetThreadInfo;
            }

            TEB* sourceTeb = (TEB*)sourceThreadInfo.TebBaseAddress;
            TEB* targetTeb = (TEB*)targetThreadInfo.TebBaseAddress;

            foreach (int offset in valuesOffsets)
            {
                *(long*)(*(byte**)(targetTeb->ThreadLocalStoragePointer) + offset) = *(long*)(*(byte**)(sourceTeb->ThreadLocalStoragePointer) + offset);
            }
        }

        public static void CopyTlsValues(int sourceThreadId, int targetThreadId, params int[] valuesOffsets)
        {
            IntPtr sourceThreadHandle = IntPtr.Zero, targetThreadHandle = IntPtr.Zero;

            if (!threadHandleDictionary.TryGetValue(sourceThreadId, out sourceThreadHandle))
            {
                try
                {
                    sourceThreadHandle = Win32Native.OpenThread(ThreadAccess.QUERY_INFORMATION, false, sourceThreadId);

                    threadHandleDictionary[sourceThreadId] = sourceThreadHandle;
                }
                catch { }
            }

            if (!threadHandleDictionary.TryGetValue(targetThreadId, out targetThreadHandle))
            {
                try
                {
                    targetThreadHandle = Win32Native.OpenThread(ThreadAccess.QUERY_INFORMATION, false, targetThreadId);

                    threadHandleDictionary[targetThreadId] = targetThreadHandle;
                }
                catch { }
            }

            CopyTlsValues(sourceThreadHandle, targetThreadHandle, valuesOffsets);
        }
    }
}
