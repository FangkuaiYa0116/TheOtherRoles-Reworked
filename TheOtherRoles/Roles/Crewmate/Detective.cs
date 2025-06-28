using TheOtherRoles.Objects;
using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate
{
    internal class Detective : RoleBase<Detective>
    {
        public static Color color = new Color32(45, 106, 165, byte.MaxValue);

        public static float footprintIntervall = 1f;
        public static float footprintDuration = 1f;
        public static bool anonymousFootprints = false;
        public static float reportNameDuration = 0f;
        public static float reportColorDuration = 20f;
        public float timer = 6.2f;

        public Detective()
        {
            RoleId = roleId = RoleId.Detective;
            timer = 6.2f;
        }

        public override void FixedUpdate()
        {
            timer -= Time.fixedDeltaTime;
            if (timer <= 0f)
            {
                timer = footprintIntervall;
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                {
                    if (player != null && player != PlayerControl.LocalPlayer && !player.Data.IsDead && !player.inVent)
                    {
                        FootprintHolder.Instance.MakeFootprint(player);
                    }
                }
            }
        }

        public static void clearAndReload()
        {
            anonymousFootprints = CustomOptionHolder.detectiveAnonymousFootprints.getBool();
            footprintIntervall = CustomOptionHolder.detectiveFootprintIntervall.getFloat();
            footprintDuration = CustomOptionHolder.detectiveFootprintDuration.getFloat();
            reportNameDuration = CustomOptionHolder.detectiveReportNameDuration.getFloat();
            reportColorDuration = CustomOptionHolder.detectiveReportColorDuration.getFloat();
            players = new();
        }
    }
}
