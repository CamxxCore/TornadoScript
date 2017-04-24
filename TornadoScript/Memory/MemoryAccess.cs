using System;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using GTA;
using GTA.Math;

namespace TornadoScript.Memory
{
    public unsafe static class MemoryAccess
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr FwGetAssetIndexFn(IntPtr assetStore, out int index, StringBuilder name);

        public delegate IntPtr GetPooledPtfxAddressFn(int handle);

        private static IntPtr _ptfxAssetStorePtr;

        //private static getPooledPtfxAddressFn _getPooledPtfxAddress;

        private static FwGetAssetIndexFn _fwGetAssetIndex;

        static MemoryAccess()
        {
            #region SetupPTFXAssetStore

            var pattern = new Pattern("\x48\x8D\x0D\x00\x00\x00\x00\xFF\x50\x10\x8B\x45\x38", "xxx????xxxxxx");

            IntPtr result = pattern.Get();

            if (result != null)
            {
                long rip = result.ToInt64() + 7;
                int value = Marshal.ReadInt32(IntPtr.Add(result, 3));
                _ptfxAssetStorePtr = new IntPtr(rip + value);
            }

            #endregion

            #region SetupfwGetAssetIndex

            pattern = new Pattern("\x48\x8D\x54\x24\x00\x8B\xCF\x0D\x00\x00\x00\x00", "xxxx?xxx????");

            result = pattern.Get(0x4F);

            if (result != null)
            {
                long rip = result.ToInt64() + 4;
                int value = Marshal.ReadInt32(result);
                _fwGetAssetIndex = Marshal.GetDelegateForFunctionPointer<FwGetAssetIndexFn>(new IntPtr(rip + value));
            }

            #endregion
/*
           #region SetupGetPooledPtfxAddress

            pattern = new Pattern("\x48\x8B\x40\x20\x0F\x28\x0B", "xxxxxxx");

            result = pattern.Get(-0xA);

            if (result != null)
            {
                long rip = result.ToInt64() + 5;
                int value = Marshal.ReadInt32(result + 0x1);
                _getPooledPtfxAddress = Marshal.GetDelegateForFunctionPointer<getPooledPtfxAddressFn>(new IntPtr(rip + value));
            }
            
            #endregion*/
        }

     /*   public static IntPtr GetPtfxEntity(int ptfxHandle)
        {
            return Marshal.ReadIntPtr(_getPooledPtfxAddress(ptfxHandle));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void SetPtfxPosition(int ptfxHandle, Vector3 position)
        {
            IntPtr pooledFx = _getPooledPtfxAddress(ptfxHandle);

            if (pooledFx != IntPtr.Zero)
            {

                IntPtr entity = Marshal.ReadIntPtr(pooledFx + 0x20);

                if (entity != IntPtr.Zero)
                {

                    *(float*)(entity + 0x90) = position.X;
                    *(float*)(entity + 0x94) = position.Y;
                    *(float*)(entity + 0x98) = position.Z;
                }
            }

            //  Marshal.StructureToPtr(position, (entity + 0x90), true);
        }
        //   Marshal.Copy(new float[] { position.X, position.Y, position.Z }, 0, entity + 0x90, 3);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetPtfxPosition(int ptfxHandle)
        {
            IntPtr pooledFx = _getPooledPtfxAddress(ptfxHandle);
            IntPtr entity = Marshal.ReadIntPtr(pooledFx + 0x20);
            return (Vector3)Marshal.PtrToStructure(entity + 0x90, typeof(Vector3));
        }
        */

        private static PgDictionary* GetPtfxRuleDictionary(string ptxAssetName)
        {
            PtfxAssetStore assetStore = Marshal.PtrToStructure<PtfxAssetStore>(_ptfxAssetStorePtr);

            int index;

            _fwGetAssetIndex(_ptfxAssetStorePtr, out index, new StringBuilder(ptxAssetName));

            //Logger.Log(string.Format("GetPtfxRuleDictionaryItems() - fwGetAssetIndex returned \'{0}\' for asset \"{1}\".", index, ptxAssetName));

            IntPtr ptxFxListPtr = Marshal.ReadIntPtr(assetStore.Items + assetStore.ItemSize * index);

            return (PgDictionary*)Marshal.ReadIntPtr(ptxFxListPtr + 0x48);
        }

        public static bool FindPtxEffectRule(PgDictionary* ptxRulesDict, string fxName, out IntPtr result)
        {
            IntPtr itAddress;

            for (int i = 0; i < ptxRulesDict->ItemsCount; i++)
            {
                itAddress = Marshal.ReadIntPtr(ptxRulesDict->Items + i * 8);

                if (itAddress != null)
                {
                    string szName = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(itAddress + 0x20));

                    if (szName == fxName)
                    {
                        //Logger.Log(string.Format("GetPTFXAssetRule() - Returning asset at 0x{0:X}.", itAddress));

                        result = itAddress;

                        return true;
                    }
                }
            }
            result = IntPtr.Zero;
            return false;
        }

        private static PtxEventEmitter* GetPtfxEventEmitterByName(IntPtr ptxAssetRulePtr, string particleName)
        {
            PtxEffectRule ptxRule = Marshal.PtrToStructure<PtxEffectRule>(ptxAssetRulePtr);

            for (int i = 0; i < ptxRule.EmittersCount; i++)
            {
                PtxEventEmitter* emitter = ptxRule.Emitters[i];

                string szName = Marshal.PtrToStringAnsi(emitter->SzEmitterName);

                if (szName == particleName)
                {
                    return emitter;
                }
            }

            return null;
        }

        public static void PatchPtfx()
        {
            //Logger.Log("PatchPTFX() - Patching PTFX...");

            PgDictionary* ptxRulesDict = GetPtfxRuleDictionary("core");

            IntPtr result;

            if (FindPtxEffectRule(ptxRulesDict, "ent_amb_smoke_foundry", out result))
            {
                //Logger.Log("PatchPTFX() - Found particle asset rule...");

                Color color = Color.FromArgb(230, 47, 46, 51);
                
                //  SetPtxParticleEmitterColour(result, "ent_amb_smoke_foundry_end", Color.Black);

                //  SetPtxParticleEmitterColour(result, "ent_amb_smoke_foundry_core", Color.Black);

                SetPtxParticleEmitterColour(result, "ent_amb_smoke_foundry_core2", Color.Black);

                //Logger.Log("PatchPTFX() - Success!");
            }
        }


        private static void SetPtxParticleEmitterColour(IntPtr ptfxRule, string particleName, Color colour)
        {
            //Logger.Log(string.Format("SetPtxParticleEmitterColour() - Setting colour for emitter \"{0}\" to ({1}, {2}, {3})", particleName, colour.R, colour.G, colour.B));

            PtxEventEmitter* emitter = GetPtfxEventEmitterByName(ptfxRule, particleName);

            int behaviourHash = Game.GenerateHash("ptxu_Colour");

            for (int i = 0; i < emitter->ParticleRule->BehavioursCount; i++)
            {
                PtxBehaviour* behaviour = emitter->ParticleRule->Behaviours[i];

                if (behaviour->HashName == (uint)behaviourHash)
                {
                    for (int x = 0; x < behaviour->NumFrames - 1; x++)
                    {
                        behaviour->KeyframeProps[x]->Defaults.Vectors[0].Value[0] = (1.0f / 255) * colour.R; // r
                        behaviour->KeyframeProps[x]->Defaults.Vectors[0].Value[1] = (1.0f / 255) * colour.G; // g
                        behaviour->KeyframeProps[x]->Defaults.Vectors[0].Value[2] = (1.0f / 255) * colour.B; // b
                        behaviour->KeyframeProps[x]->Defaults.Vectors[0].Value[3] = (1.0f / 255) * colour.A; // a

                        behaviour->KeyframeProps[x]->Defaults.Vectors[1].Value[0] = (1.0f / 255) * colour.R; // r
                        behaviour->KeyframeProps[x]->Defaults.Vectors[1].Value[1] = (1.0f / 255) * colour.G; // g
                        behaviour->KeyframeProps[x]->Defaults.Vectors[1].Value[2] = (1.0f / 255) * colour.B; // b
                        behaviour->KeyframeProps[x]->Defaults.Vectors[1].Value[3] = (1.0f / 255) * colour.A; // a*/

                        if (behaviour->KeyframeProps[x]->F1 != null)
                        {
                            for (int y = 0; y < behaviour->KeyframeProps[x]->F1->Vars->UnkCount; y++)
                            {
                                behaviour->KeyframeProps[x]->F1->Vars->Vectors[y].Value[0] = (1.0f / 255) * colour.R; // r
                                behaviour->KeyframeProps[x]->F1->Vars->Vectors[y].Value[1] = (1.0f / 255) * colour.G; // g
                                behaviour->KeyframeProps[x]->F1->Vars->Vectors[y].Value[2] = (1.0f / 255) * colour.B; // b
                                behaviour->KeyframeProps[x]->F1->Vars->Vectors[y].Value[3] = (1.0f / 255) * colour.A; // a
                            }
                        }

                        if (behaviour->KeyframeProps[x]->F2 != null)
                        {
                            for (int y = 0; y < behaviour->KeyframeProps[x]->F2->Vars->UnkCount; y++)
                            {
                                behaviour->KeyframeProps[x]->F2->Vars->Vectors[y].Value[0] = (1.0f / 255) * colour.R; // r
                                behaviour->KeyframeProps[x]->F2->Vars->Vectors[y].Value[1] = (1.0f / 255) * colour.G; // g
                                behaviour->KeyframeProps[x]->F2->Vars->Vectors[y].Value[2] = (1.0f / 255) * colour.B; // b
                                behaviour->KeyframeProps[x]->F2->Vars->Vectors[y].Value[3] = (1.0f / 255) * colour.A; // a
                            }
                        }

                        if (behaviour->KeyframeProps[x]->F3 != null)
                        {
                            for (int y = 0; y < behaviour->KeyframeProps[x]->F3->Vars->UnkCount; y++)
                            {
                                behaviour->KeyframeProps[x]->F3->Vars->Vectors[y].Value[0] = (1.0f / 255) * colour.R; // r
                                behaviour->KeyframeProps[x]->F3->Vars->Vectors[y].Value[1] = (1.0f / 255) * colour.G; // g
                                behaviour->KeyframeProps[x]->F3->Vars->Vectors[y].Value[2] = (1.0f / 255) * colour.B; // b
                                behaviour->KeyframeProps[x]->F3->Vars->Vectors[y].Value[3] = (1.0f / 255) * colour.A; // a

                            }
                        }

                        if (behaviour->KeyframeProps[x]->F4 != null)
                        {
                            for (int y = 0; y < behaviour->KeyframeProps[x]->F4->Vars->UnkCount; y++)
                            {
                                behaviour->KeyframeProps[x]->F4->Vars->Vectors[y].Value[0] = (1.0f / 255) * colour.R; // r
                                behaviour->KeyframeProps[x]->F4->Vars->Vectors[y].Value[1] = (1.0f / 255) * colour.G; // g
                                behaviour->KeyframeProps[x]->F4->Vars->Vectors[y].Value[2] = (1.0f / 255) * colour.B; // b
                                behaviour->KeyframeProps[x]->F4->Vars->Vectors[y].Value[3] = (1.0f / 255) * colour.A; // a
                            }
                        }
                    }
                }
            }
        }
    }
}

