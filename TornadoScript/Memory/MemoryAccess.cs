using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using GTA;

namespace TornadoScript.Memory
{
    public unsafe static class MemoryAccess
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr fwGetAssetIndexFunc(IntPtr assetStore, out int index, StringBuilder name);

        public delegate int getScriptEntityIndexFn(IntPtr entityAddress);

        private static IntPtr ptfxAssetStorePtr,
            fwGetAssetIndexPtr;

        static MemoryAccess()
        {
            #region SetupPTFXAssetStore

            var pattern = new Pattern("\x48\x8D\x0D\x00\x00\x00\x00\xFF\x50\x10\x8B\x45\x38", "xxx????xxxxxx");

            IntPtr result = pattern.Get();

            if (result != null)
            {
                long rip = result.ToInt64() + 7;
                int value = Marshal.ReadInt32(IntPtr.Add(result, 3));
                ptfxAssetStorePtr = new IntPtr(rip + value);
            }

            #endregion

            #region SetupfwGetAssetIndex

            pattern = new Pattern("\x48\x8D\x54\x24\x00\x8B\xCF\x0D\x00\x00\x00\x00", "xxxx?xxx????");

            result = pattern.Get(0x4F);

            if (result != null)
            {
                long rip = result.ToInt64() + 4;
                int value = Marshal.ReadInt32(result);
                fwGetAssetIndexPtr = new IntPtr(rip + value);
            }

            #endregion
        }

        private static pgDictionary* GetPtfxRuleDictionary(string ptxAssetName)
        {
            var fn = Marshal.GetDelegateForFunctionPointer<fwGetAssetIndexFunc>(fwGetAssetIndexPtr);

            ptfxAssetStore assetStore = Marshal.PtrToStructure<ptfxAssetStore>(ptfxAssetStorePtr);

            int index;

            fn(ptfxAssetStorePtr, out index, new StringBuilder(ptxAssetName));

            //Logger.Log(string.Format("GetPtfxRuleDictionaryItems() - fwGetAssetIndex returned \'{0}\' for asset \"{1}\".", index, ptxAssetName));

            IntPtr ptxFXListPtr = Marshal.ReadIntPtr(assetStore.Items + assetStore.ItemSize * index);

            return (pgDictionary*)Marshal.ReadIntPtr(ptxFXListPtr + 0x48);
        }

        public static bool FindPtxEffectRule(pgDictionary* ptxRulesDict, string fxName, out IntPtr result)
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

        private static ptxEventEmitter* GetPtfxEventEmitterByName(IntPtr ptxAssetRulePtr, string particleName)
        {
            ptxEffectRule ptxRule = Marshal.PtrToStructure<ptxEffectRule>(ptxAssetRulePtr);

            for (int i = 0; i < ptxRule.EmittersCount; i++)
            {
                ptxEventEmitter* emitter = ptxRule.Emitters[i];

                string szName = Marshal.PtrToStringAnsi(emitter->SzEmitterName);

                if (szName == particleName)
                {
                    return emitter;
                }
            }

            return null;
        }

        public static void PatchPTFX()
        {
            //Logger.Log("PatchPTFX() - Patching PTFX...");

            pgDictionary* ptxRulesDict = GetPtfxRuleDictionary("core");

            IntPtr result;

            if (FindPtxEffectRule(ptxRulesDict, "ent_amb_smoke_foundry", out result))
            {
                //Logger.Log("PatchPTFX() - Found particle asset rule...");

                Color color = Color.FromArgb(230, Color.Black);

                //  SetPtxParticleEmitterColour(result, "ent_amb_smoke_foundry_end", Color.Black);

                //  SetPtxParticleEmitterColour(result, "ent_amb_smoke_foundry_core", Color.Black);

                SetPtxParticleEmitterColour(result, "ent_amb_smoke_foundry_core2", Color.Black);

                //Logger.Log("PatchPTFX() - Success!");
            }
        }


        private static void SetPtxParticleEmitterColour(IntPtr ptfxRule, string particleName, Color colour)
        {
            //Logger.Log(string.Format("SetPtxParticleEmitterColour() - Setting colour for emitter \"{0}\" to ({1}, {2}, {3})", particleName, colour.R, colour.G, colour.B));

            ptxEventEmitter* emitter = GetPtfxEventEmitterByName(ptfxRule, particleName);

            int behaviourHash = Game.GenerateHash("ptxu_Colour");

            for (int i = 0; i < emitter->ParticleRule->BehavioursCount; i++)
            {
                ptxBehaviour* behaviour = emitter->ParticleRule->Behaviours[i];

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

