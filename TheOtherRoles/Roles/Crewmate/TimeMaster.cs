using System;
using System.Linq;
using Hazel;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate
{
    internal static class TimeMaster
    {
        public static PlayerControl timeMaster;
        public static Color color = new Color32(112, 142, 239, byte.MaxValue);

        public static bool reviveDuringRewind = false;
        public static float rewindTime = 3f;
        public static float shieldDuration = 3f;
        public static float cooldown = 30f;
        public static float rewindCooldown = 30f;
        public static bool canUseRewind = true;

        public static bool shieldActive = false;
        public static bool isRewinding = false;

        private static Sprite buttonSprite;
        private static Sprite rewindbuttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.TimeShieldButton.png", 115f);
            return buttonSprite;
        }

        public static Sprite getRewindButtonSprite()
        {
            if (rewindbuttonSprite) return rewindbuttonSprite;
            rewindbuttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.RewindButton.png", 115f);
            return rewindbuttonSprite;
        }

        public static void RewindTime()
        {
            shieldActive = false; // Shield is no longer active when rewinding
            SoundEffectsManager.stop("timemasterShield");  // Shield sound stopped when rewinding
            if (timeMaster != null && timeMaster == PlayerControl.LocalPlayer)
            {
                HudManagerStartPatch.resetTimeMasterButton();
            }
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.color = new Color(0f, 0.5f, 0.8f, 0.3f);
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.SetActive(true);
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(rewindTime / 2, new Action<float>((p) =>
            {
                if (p == 1f) FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = false;
            })));

            if (timeMaster == null || PlayerControl.LocalPlayer == timeMaster) return; // Time Master himself does not rewind

            isRewinding = true;

            if (MapBehaviour.Instance)
                MapBehaviour.Instance.Close();
            if (Minigame.Instance)
                Minigame.Instance.ForceClose();

            // Revive dead player
            if (reviveDuringRewind)
            {
                DeadPlayer deadPlayer = GameHistory.deadPlayers.FirstOrDefault();
                if ((DateTime.UtcNow - deadPlayer.timeOfDeath).Seconds <= rewindTime)
                {
                    // Revive Player
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.RevivePlayer, SendOption.Reliable);
                    writer.Write(deadPlayer.player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.stopStart(deadPlayer.player.PlayerId);
                    // Remove dead player
                    GameHistory.deadPlayers.Remove(deadPlayer);
                }
            }
        }

        public static void clearAndReload()
        {
            timeMaster = null;
            isRewinding = false;
            shieldActive = false;
            reviveDuringRewind = CustomOptionHolder.timeMasterReviveDuringRewind.getBool();
            rewindTime = CustomOptionHolder.timeMasterRewindTime.getFloat();
            shieldDuration = CustomOptionHolder.timeMasterShieldDuration.getFloat();
            cooldown = CustomOptionHolder.timeMasterCooldown.getFloat();
            rewindCooldown = CustomOptionHolder.timeMasterRewindCooldown.getFloat();
            canUseRewind = CustomOptionHolder.timeMasterCanRewind.getBool();
        }
    }
}
