using UnityEngine;

namespace TheOtherRoles.Roles.Impostor
{
    internal class Godfather : RoleBase<Godfather>
    {
        public static Color color = Palette.ImpostorRed;

        public Godfather()
        {
            RoleId = roleId = RoleId.Godfather;
        }

        public static void clearAndReload()
        {
            players = new();
        }
    }
}
