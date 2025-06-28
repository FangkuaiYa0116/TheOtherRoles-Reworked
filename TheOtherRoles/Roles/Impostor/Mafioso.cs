using UnityEngine;

namespace TheOtherRoles.Roles.Impostor
{
    internal class Mafioso : RoleBase<Mafioso>
    {
        public static Color color = Palette.ImpostorRed;

        public Mafioso()
        {
            RoleId = roleId = RoleId.Mafioso;
        }

        public static void clearAndReload()
        {
            players = new();
        }
    }
}
