using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MelonLoader;
using HarmonyLib;
using UnityEngine;


using MyBhapticsTactsuit;

[assembly: MelonInfo(typeof(AGAINST_bhaptics.AGAINST_bhaptics), "AGAINST_bhaptics", "2.0.0", "Florian Fahrenberger")]
[assembly: MelonGame("JoyWay", "AGAINST")]


namespace AGAINST_bhaptics
{
    public class AGAINST_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr;
        public static bool swordRightHand = true;

        public override void OnInitializeMelon()
        {
            //base.OnApplicationStart();
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }

        #region Weapon feedback

        [HarmonyPatch(typeof(Gun), "Shoot", new Type[] { typeof(bool) })]
        public class bhaptics_GunShoot
        {
            [HarmonyPostfix]
            public static void Postfix(Gun __instance, bool _state)
            {
                bool isRightHand = (__instance.handController.handType == HandType.Right);
                tactsuitVr.Recoil("Gun", isRightHand);
            }
        }

        [HarmonyPatch(typeof(Shotgun), "Shoot", new Type[] { typeof(bool) })]
        public class bhaptics_ShotgunShoot
        {
            [HarmonyPostfix]
            public static void Postfix(Shotgun __instance, bool _state)
            {
                bool isRightHand = (__instance.handController.handType == HandType.Right);
                tactsuitVr.Recoil("Shotgun", isRightHand);
            }
        }

        [HarmonyPatch(typeof(HandController), "StrongVibration", new Type[] {  })]
        public class bhaptics_StrongVibration
        {
            [HarmonyPostfix]
            public static void Postfix(HandController __instance)
            {
                string weaponName = "";
                bool isRightHand = (__instance.handType == HandType.Right);
                if (__instance.weaponSwitcher.WeaponType == HandWeapon.Blade)
                    { weaponName = "Blade"; }
                if (__instance.weaponSwitcher.WeaponType == HandWeapon.BoxGlove)
                { weaponName = "Knuckle"; }
                if (__instance.weaponSwitcher.WeaponType == HandWeapon.Whip)
                { weaponName = "Whip"; }

                // Not all vibration need feedback
                if (weaponName == "") { return; }
                tactsuitVr.Recoil(weaponName, isRightHand);
            }
        }


        #endregion

        #region Player damage

        [HarmonyPatch(typeof(Vignette), "Damage", new Type[] { typeof(int), typeof(int) } )]
        public class bhaptics_DamageVignette
        {
            [HarmonyPostfix]
            public static void Postfix(int lastLivesNumber, int livesNumber)
            {
                if (livesNumber > lastLivesNumber)
                { tactsuitVr.PlaybackHaptics("Healing"); }
                else { tactsuitVr.PlaybackHaptics("Impact"); }
            }
        }

        #endregion

        #region World interaction

        [HarmonyPatch(typeof(Vignette), "LivesChange", new Type[] { typeof(int), typeof(int) })]
        public class bhaptics_LivesVignette
        {
            [HarmonyPostfix]
            public static void Postfix(int lastLivesNumber, int livesNumber)
            {
                // tactsuitVr.LOG("Damage: " + livesNumber.ToString());
                if (livesNumber == 1) { tactsuitVr.StartHeartBeat(); }
                else { tactsuitVr.StopHeartBeat(); }
                tactsuitVr.PlaybackHaptics("Impact");
            }
        }

        [HarmonyPatch(typeof(AnalyticsManager), "LevelFailed", new Type[] { })]
        public class bhaptics_LevelFailed
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.StopThreads();
            }
        }

        [HarmonyPatch(typeof(AnalyticsManager), "LevelFinished", new Type[] { })]
        public class bhaptics_LevelFinished
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.StopThreads();
            }
        }

        [HarmonyPatch(typeof(AnalyticsManager), "LevelStarted", new Type[] { })]
        public class bhaptics_LevelStarted
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.StopThreads();
            }
        }

        [HarmonyPatch(typeof(Vignette), "ShowMoveVignette", new Type[] { })]
        public class bhaptics_MoveVignette
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("SwooshUp");
            }
        }


        [HarmonyPatch(typeof(Boss), "Exploding", new Type[] { })]
        public class bhaptics_BossExploding
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("ExplosionUp");
            }
        }

        #endregion


    }
}
