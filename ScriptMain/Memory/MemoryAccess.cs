using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using GTA;
using GTA.Native;

namespace TornadoScript.ScriptMain.Memory
{
    public static unsafe class MemoryAccess
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr FwGetAssetIndexFn(IntPtr assetStore, out int index, StringBuilder name);

        public delegate IntPtr GetPooledPtfxAddressFn(int handle);

        private static IntPtr PtfxAssetStorePtr;

        private static FwGetAssetIndexFn FwGetAssetIndex;

        public static void Initialize()
        {
            #region SetupPTFXAssetStore

            var pattern = new Pattern("\x0F\xBF\x04\x9F\xB9", "xxxxx");

            var result = pattern.Get(0x19);

            if (result != IntPtr.Zero)
            {
                var rip = result.ToInt64() + 7;
                var value = Marshal.ReadInt32(IntPtr.Add(result, 3));
                PtfxAssetStorePtr = new IntPtr(rip + value);
            }

            #endregion

            #region SetupfwGetAssetIndex

            pattern = new Pattern("\x41\x8B\xDE\x4C\x63\x00", "xxxxx?");

            result = pattern.Get();

            if (result != IntPtr.Zero)
            {
                var rip = result.ToInt64();
                var value = Marshal.ReadInt32(result - 4);
                FwGetAssetIndex = Marshal.GetDelegateForFunctionPointer<FwGetAssetIndexFn>(new IntPtr(rip + value));
            }

            #endregion
        }
        
        private static PgDictionary* GetPtfxRuleDictionary(string ptxAssetName)
        {
            var assetStore = Marshal.PtrToStructure<PtfxAssetStore>(PtfxAssetStorePtr);

            FwGetAssetIndex(PtfxAssetStorePtr, out var index, new StringBuilder(ptxAssetName));

            var ptxFxListPtr = Marshal.ReadIntPtr(assetStore.Items + assetStore.ItemSize * index);

            return (PgDictionary*)Marshal.ReadIntPtr(ptxFxListPtr + 0x48);
        }

        public static bool FindPtxEffectRule(PgDictionary* ptxRulesDict, string fxName, out IntPtr result)
        {
            for (var i = 0; i < ptxRulesDict->ItemsCount; i++)
            {
                var itAddress = Marshal.ReadIntPtr(ptxRulesDict->Items + i * 8);

                if (itAddress == IntPtr.Zero) continue;

                var szName = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(itAddress + 0x20));

                if (szName != fxName) continue;

                result = itAddress;

                return true;
            }

            result = IntPtr.Zero;

            return false;
        }

        private static PtxEventEmitter* GetPtfxEventEmitterByName(IntPtr ptxAssetRulePtr, string particleName)
        {
            var ptxRule = Marshal.PtrToStructure<PtxEffectRule>(ptxAssetRulePtr);

            for (var i = 0; i < ptxRule.EmittersCount; i++)
            {
                var emitter = ptxRule.Emitters[i];

                var szName = Marshal.PtrToStringAnsi(emitter->SzEmitterName);

                if (szName == particleName)
                {
                    return emitter;
                }
            }

            return null;
        }

        public static void PatchPtfx()
        {
            Function.Call(Hash.REQUEST_NAMED_PTFX_ASSET, "core");

            if (!FindPtxEffectRule(GetPtfxRuleDictionary("core"), "ent_amb_smoke_foundry", out var result)) return;

            SetEmitterColour(result, "ent_amb_smoke_foundry_core2", Color.Black);

            SetEmitterColour(result, "ent_amb_smoke_foundry_core", Color.Black);
        }

        private static void SetEmitterColour(IntPtr ptfxRule, string particleName, Color colour)
        {
            SetEmitterColour(ptfxRule, particleName, colour.R, colour.G, colour.B, colour.A);
        }

        private static void SetEmitterColour(IntPtr ptfxRule, string particleName, byte red, byte green, byte blue, byte alpha)
        {
            var behaviourHash = Game.GenerateHash("ptxu_Colour");

            PtxEventEmitter* emitter = GetPtfxEventEmitterByName(ptfxRule, particleName);

            var r = 1.0f / 255 * red;
            var g = 1.0f / 255 * green;
            var b = 1.0f / 255 * blue;
            var a = 1.0f / 255 * alpha;

            for (var i = 0; i < emitter->ParticleRule->BehavioursCount; i++)
            {
                Ptxu_Colour* behaviour = emitter->ParticleRule->Behaviours[i];

                if (behaviour->HashName != (uint)behaviourHash) continue;

                for (var x = 0; x < behaviour->NumFrames; x++)
                {
                    PtxKeyframeProp* keyframe = behaviour->KeyframeProps[x];

                    if (keyframe->Current.Items == IntPtr.Zero) continue;

                    var items = (PtxVarVector*)keyframe->Current.Items;

                    for (var y = 0; y < keyframe->Current.Count; y++)
                    {
                        if (items == null) continue;

                        items[y].Min.R = r;
                        items[y].Min.G = g;
                        items[y].Min.B = b;
                        items[y].Min.A = a;

                        items[y].Max.R = r;
                        items[y].Max.G = g;
                        items[y].Max.B = b;
                        items[y].Max.A = a;
                    }
                }
            }
        }
    }
}

