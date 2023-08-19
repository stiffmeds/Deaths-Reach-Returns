using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;


namespace Deaths_Reach_Returns
{
    [BepInPlugin("com.meds.deathsreachreturns", "Death's Reach Returns", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private readonly Harmony harmony = new("com.meds.deathsreachreturns");
        internal static ManualLogSource Log;
        private void Awake()
        {
            Log = Logger;
            // Plugin startup logic
            harmony.PatchAll();
        }
    }
    [HarmonyPatch]
    public class ComeBackPls
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Globals), "CreateGameContent")]
        public static void CreateGameContentPostfix()
        {
            string path = "file:///" + Path.Combine(BepInEx.Paths.PluginPath, "Fear1.wav").Replace("\\", "/");
            //Plugin.Log.LogDebug("PATH: " + path);
            try
            {
                using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
                {
                    www.SendWebRequest();
                    while (!www.isDone)
                    {

                    }

                    if (www.result == UnityWebRequest.Result.ConnectionError)
                    {
                        Plugin.Log.LogDebug(www.error);
                    }
                    else
                    {
                        Dictionary<string, CardData> medsCardsSource = Traverse.Create(Globals.Instance).Field("_CardsSource").GetValue<Dictionary<string, CardData>>();
                        Dictionary<string, CardData> medsCards = Traverse.Create(Globals.Instance).Field("_Cards").GetValue<Dictionary<string, CardData>>();
                        AudioClip ac = DownloadHandlerAudioClip.GetContent(www);
                        ac.name = "Fear1";
                        medsCardsSource["deathsreach"].Sound = ac;
                        medsCards["deathsreach"].Sound = ac;
                        medsCardsSource["deathsreacha"].Sound = ac;
                        medsCards["deathsreacha"].Sound = ac;
                        medsCardsSource["deathsreachb"].Sound = ac;
                        medsCards["deathsreachb"].Sound = ac;
                        medsCardsSource["deathsreachrare"].Sound = ac;
                        medsCards["deathsreachrare"].Sound = ac;
                        Traverse.Create(Globals.Instance).Field("_CardsSource").SetValue(medsCardsSource);
                        Traverse.Create(Globals.Instance).Field("_Cards").SetValue(medsCards);
                        Plugin.Log.LogInfo($"FTONG! The original Death's Reach audio has returned!");
                    }
                }
            }
            catch { }

        }
    }
}
