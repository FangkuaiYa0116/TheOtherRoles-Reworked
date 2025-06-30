using System.Collections.Generic;
using TheOtherRoles.Roles.Neutral;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate
{
    internal class Engineer : RoleBase<Engineer>
    {
        public static Color color = new Color32(0, 40, 245, byte.MaxValue);
        private static Sprite buttonSprite;
        private static Sprite doorButtonSprite;

        public int remainingFixes = 1;
        public static bool highlightForImpostors = true;
        public static bool highlightForTeamJackal = true;

        public static float doorOpenCooldown = 30f;
        public static float doorOpenDuration = 3f;
        public static float doorOpenTimer = 0f;
        public static int remainingUsesDoorOpen = -1;

        private static Il2CppArrayBase<PlainDoor> doors = null;
        private static List<bool> enableDoors = null;

        public Engineer()
        {
            RoleId = roleId = RoleId.Engineer;
            remainingFixes = Mathf.RoundToInt(CustomOptionHolder.engineerNumberOfFixes.getFloat());
        }

        public override void FixedUpdate()
        {
            bool jackalHighlight = highlightForTeamJackal && (PlayerControl.LocalPlayer == Jackal.jackal || PlayerControl.LocalPlayer == Sidekick.sidekick);
            bool impostorHighlight = highlightForImpostors && PlayerControl.LocalPlayer.Data.Role.IsImpostor;
            if ((jackalHighlight || impostorHighlight) && MapUtilities.CachedShipStatus?.AllVents != null)
            {
                foreach (Vent vent in MapUtilities.CachedShipStatus.AllVents)
                {
                    try
                    {
                        if (vent?.myRend?.material != null)
                        {
                            if (local.player.inVent)
                            {
                                vent.myRend.material.SetFloat("_Outline", 1f);
                                vent.myRend.material.SetColor("_OutlineColor", Engineer.color);
                            }
                            else if (vent.myRend.material.GetColor("_AddColor") != Color.red)
                            {
                                vent.myRend.material.SetFloat("_Outline", 0);
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.RepairButton.png", 115f);
            return buttonSprite;
        }

        public static Sprite getDoorButtonSprite()
        {
            if (doorButtonSprite) return doorButtonSprite;
            doorButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.EngineerOpenDoorButton.png.png", 115f);
            return doorButtonSprite;
        }

        public static void DisableDoors(int playerId)
        {
            if (!exists || !Helpers.playerById((byte)playerId).isRole(RoleId.Engineer) || remainingUsesDoorOpen == 0) return;

            if (playerId == PlayerControl.LocalPlayer.PlayerId)
            {
                doors = GameObject.FindObjectsOfType<PlainDoor>();
                if (doors != null && doors.Count > 0)
                {
                    doorOpenTimer = doorOpenDuration;
                    enableDoors = new List<bool>();
                    for (int i = 0; i < doors.Count; ++i)
                    {
                        enableDoors.Add(doors[i].myCollider.enabled);
                        doors[i].myCollider.enabled = false;
                        enableDoors.Add(doors[i].shadowCollider.enabled);
                        doors[i].shadowCollider.enabled = false;
                    }
                }
            }
            else
            {
                Helpers.playerById((byte)playerId).Collider.isTrigger = true;
            }
        }

        public static void ResetDoors(bool consumeRemain = false)
        {
            if (!exists) return;
            if (consumeRemain && remainingUsesDoorOpen != -1)
                --remainingUsesDoorOpen;
            doorOpenTimer = 0f;
            local.player.Collider.isTrigger = false;
            if (doors == null) return;
            for (int i = 0; i < doors.Count; ++i)
            {
                doors[i].myCollider.enabled = enableDoors[i];
                doors[i].shadowCollider.enabled = enableDoors[i];
            }
            enableDoors.Clear();
            doors = null;
        }

        public static void clearAndReload()
        {
            doorOpenCooldown = CustomOptionHolder.doorOpenCooldown.getFloat();
            doorOpenDuration = CustomOptionHolder.doorOpenDuration.getFloat();
            int num = Mathf.RoundToInt(CustomOptionHolder.doorOpenNumberOfUses.getFloat());
            remainingUsesDoorOpen = num == 0 ? -1 : num;
            highlightForImpostors = CustomOptionHolder.engineerHighlightForImpostors.getBool();
            highlightForTeamJackal = CustomOptionHolder.engineerHighlightForTeamJackal.getBool();

            ResetDoors();

            players = new();
        }
    }
}
