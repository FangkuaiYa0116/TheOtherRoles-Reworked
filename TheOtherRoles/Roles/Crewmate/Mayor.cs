using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate
{
    internal class Mayor : RoleBase<Mayor>
    {
        public static Color color = new Color32(32, 77, 66, byte.MaxValue);

        public static bool canSeeVoteColors = false;
        public static int tasksNeededToSeeVoteColors;
        public static bool meetingButton = true;
        public static int mayorChooseSingleVote;
        public static bool voteTwice = true;

        public Minigame emergency = null;
        public int remoteMeetingsLeft = 1;


        public Mayor()
        {
            RoleId = roleId = RoleId.Mayor;
            emergency = null;
            remoteMeetingsLeft = Mathf.RoundToInt(CustomOptionHolder.mayorMaxRemoteMeetings.getFloat());
        }

        public static Sprite emergencySprite = null;

        public static Sprite getMeetingSprite()
        {
            if (emergencySprite) return emergencySprite;
            emergencySprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.EmergencyButton.png", 550f);
            return emergencySprite;
        }

        public static void clearAndReload()
        {
            canSeeVoteColors = CustomOptionHolder.mayorCanSeeVoteColors.getBool();
            tasksNeededToSeeVoteColors = (int)CustomOptionHolder.mayorTasksNeededToSeeVoteColors.getFloat();
            meetingButton = CustomOptionHolder.mayorMeetingButton.getBool();
            mayorChooseSingleVote = CustomOptionHolder.mayorChooseSingleVote.getSelection();
            voteTwice = true;
            players = new();
        }
    }
}
