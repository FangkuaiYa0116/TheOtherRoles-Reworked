using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Patches;
using TheOtherRoles.Roles.Impostor;
using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate
{
    internal class Medic : RoleBase<Medic>
    {
        public PlayerControl shielded;
        public PlayerControl futureShielded;
        public bool usedShield;

        public static Color color = new Color32(126, 251, 194, byte.MaxValue);

        public static int showShielded = 0;
        public static bool showAttemptToShielded = false;
        public static bool showAttemptToMedic = false;
        public static bool setShieldAfterMeeting = false;
        public static bool showShieldAfterMeeting = false;
        public static bool meetingAfterShielding = false;

        public static Color shieldedColor = new Color32(0, 221, 255, byte.MaxValue);
        public PlayerControl currentTarget;

        public Medic()
        {
            RoleId = roleId = RoleId.Medic;
            shielded = null;
            futureShielded = null;
            currentTarget = null;
            usedShield = false;
        }

        public override void FixedUpdate()
        {
            if (player != PlayerControl.LocalPlayer) return;
            currentTarget = PlayerControlFixedUpdatePatch.setTarget();
            if (!usedShield) PlayerControlFixedUpdatePatch.setPlayerOutline(currentTarget, shieldedColor);
        }

        public override void OnDeath(PlayerControl killer = null)
        {
            shielded = null;
        }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.ShieldButton.png", 115f);
            return buttonSprite;
        }
        public static bool IsShielded(PlayerControl player) => players.Any(x => x.player != null && !x.player.Data.Disconnected && !x.player.Data.IsDead && x.shielded == player && player?.Data.IsDead == false);
        public static List<Medic> GetMedic(PlayerControl shielded) => players.Where(x => x.shielded == shielded || x.futureShielded == shielded).ToList();

        public static bool shieldVisible(PlayerControl target)
        {
            bool hasVisibleShield = false;

            bool isMorphedMorphling = target == Morphling.morphling && Morphling.morphTarget != null && Morphling.morphTimer > 0f;
            if ((IsShielded(target) && !isMorphedMorphling) || (isMorphedMorphling && IsShielded(Morphling.morphTarget)))
            {
                hasVisibleShield = showShielded == 0 || Helpers.shouldShowGhostInfo() // Everyone or Ghost info
                    || (showShielded == 1 && (IsShielded(PlayerControl.LocalPlayer) || PlayerControl.LocalPlayer.isRole(RoleId.Medic))) // Shielded + Medic
                    || (showShielded == 2 && PlayerControl.LocalPlayer.isRole(RoleId.Medic)); // Medic only
                // Make shield invisible till after the next meeting if the option is set (the medic can already see the shield)
                hasVisibleShield = hasVisibleShield && (meetingAfterShielding || !showShieldAfterMeeting || PlayerControl.LocalPlayer.isRole(RoleId.Medic) || Helpers.shouldShowGhostInfo());
            }
            return hasVisibleShield;
        }

        public static void clearAndReload()
        {
            showShielded = CustomOptionHolder.medicShowShielded.getSelection();
            showAttemptToShielded = CustomOptionHolder.medicShowAttemptToShielded.getBool();
            showAttemptToMedic = CustomOptionHolder.medicShowAttemptToMedic.getBool();
            setShieldAfterMeeting = CustomOptionHolder.medicSetOrShowShieldAfterMeeting.getSelection() == 2;
            showShieldAfterMeeting = CustomOptionHolder.medicSetOrShowShieldAfterMeeting.getSelection() == 1;
            meetingAfterShielding = false;
            players = new();
        }
    }
}
