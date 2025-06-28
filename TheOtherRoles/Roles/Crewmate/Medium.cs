using System;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Modules;
using TheOtherRoles.Roles.Impostor;
using TheOtherRoles.Roles.Modifier;
using TheOtherRoles.Roles.Neutral;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate
{
    internal class Medium : RoleBase<Medium>
    {
        public DeadPlayer target;
        public DeadPlayer soulTarget;
        public static Color color = new Color32(98, 120, 115, byte.MaxValue);
        public List<Tuple<DeadPlayer, Vector3>> deadBodies = new List<Tuple<DeadPlayer, Vector3>>();
        public List<Tuple<DeadPlayer, Vector3>> futureDeadBodies = new List<Tuple<DeadPlayer, Vector3>>();
        public List<SpriteRenderer> souls = new List<SpriteRenderer>();
        public DateTime meetingStartTime = DateTime.UtcNow;

        public static float cooldown = 30f;
        public static float duration = 3f;
        public static bool oneTimeUse = false;
        public static float chanceAdditionalInfo = 0f;

        public Medium()
        {
            RoleId = roleId = RoleId.Medium;

            target = null;
            soulTarget = null;
            deadBodies = new List<Tuple<DeadPlayer, Vector3>>();
            futureDeadBodies = new List<Tuple<DeadPlayer, Vector3>>();
            souls = new List<SpriteRenderer>();
            meetingStartTime = DateTime.UtcNow;
        }

        public override void FixedUpdate()
        {
            if (!PlayerControl.LocalPlayer.isRole(RoleId.Medium) || player.Data.IsDead || deadBodies == null || MapUtilities.CachedShipStatus?.AllVents == null) return;

            DeadPlayer target = null;
            Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            float closestDistance = float.MaxValue;
            float usableDistance = MapUtilities.CachedShipStatus.AllVents.FirstOrDefault().UsableDistance;
            foreach ((DeadPlayer dp, Vector3 ps) in deadBodies)
            {
                float distance = Vector2.Distance(ps, truePosition);
                if (distance <= usableDistance && distance < closestDistance)
                {
                    closestDistance = distance;
                    target = dp;
                }
            }
            this.target = target;
        }

        public override void OnMeetingStart()
        {
            // Medium meeting start time
            meetingStartTime = DateTime.UtcNow;
        }

        public override void OnMeetingEnd(PlayerControl exiled = null)
        {
            // Medium spawn souls
            if (PlayerControl.LocalPlayer.isRole(RoleId.Medium))
            {
                if (souls != null)
                {
                    foreach (SpriteRenderer sr in souls) UnityEngine.Object.Destroy(sr.gameObject);
                    souls = new List<SpriteRenderer>();
                }

                if (futureDeadBodies != null)
                {
                    foreach ((DeadPlayer db, Vector3 ps) in futureDeadBodies)
                    {
                        GameObject s = new GameObject();
                        //s.transform.position = ps;
                        s.transform.position = new Vector3(ps.x, ps.y, ps.y / 1000 - 1f);
                        s.layer = 5;
                        var rend = s.AddComponent<SpriteRenderer>();
                        s.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
                        rend.sprite = Medium.getSoulSprite();
                        souls.Add(rend);
                    }
                    deadBodies = futureDeadBodies;
                    futureDeadBodies = new List<Tuple<DeadPlayer, Vector3>>();
                }
            }
        }

        private static Sprite soulSprite;

        enum SpecialMediumInfo
        {
            SheriffSuicide,
            ThiefSuicide,
            ActiveLoverDies,
            PassiveLoverSuicide,
            LawyerKilledByClient,
            JackalKillsSidekick,
            ImpostorTeamkill,

            SubmergedO2,
            WarlockSuicide,
            BodyCleaned,
        }
        public static Sprite getSoulSprite()
        {
            if (soulSprite) return soulSprite;
            soulSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Soul.png", 500f);
            return soulSprite;
        }

        private static Sprite question;
        public static Sprite getQuestionSprite()
        {
            if (question) return question;
            question = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.MediumButton.png", 115f);
            return question;
        }

        public static void clearAndReload()
        {
            cooldown = CustomOptionHolder.mediumCooldown.getFloat();
            duration = CustomOptionHolder.mediumDuration.getFloat();
            oneTimeUse = CustomOptionHolder.mediumOneTimeUse.getBool();
            chanceAdditionalInfo = CustomOptionHolder.mediumChanceAdditionalInfo.getSelection() / 10f;
        }

        public static string getInfo(PlayerControl target, PlayerControl killer, DeadPlayer.CustomDeathReason deathReason)
        {
            string msg = "";

            List<SpecialMediumInfo> infos = new List<SpecialMediumInfo>();
            // collect fitting death info types.
            // suicides:
            foreach (var medium in Medium.players)
            {
                if (killer == target)
                {
                    if (target.isRole(RoleId.Sheriff) && deathReason != DeadPlayer.CustomDeathReason.LoverSuicide) infos.Add(SpecialMediumInfo.SheriffSuicide);
                    if (target == Lovers.lover1 || target == Lovers.lover2) infos.Add(SpecialMediumInfo.PassiveLoverSuicide);
                    if (target == Thief.thief && deathReason != DeadPlayer.CustomDeathReason.LoverSuicide) infos.Add(SpecialMediumInfo.ThiefSuicide);
                    if (target == Warlock.warlock && deathReason != DeadPlayer.CustomDeathReason.LoverSuicide) infos.Add(SpecialMediumInfo.WarlockSuicide);
                }
                else
                {
                    if (target == Lovers.lover1 || target == Lovers.lover2) infos.Add(SpecialMediumInfo.ActiveLoverDies);
                    if (target.Data.Role.IsImpostor && killer.Data.Role.IsImpostor && Thief.formerThief != killer) infos.Add(SpecialMediumInfo.ImpostorTeamkill);
                }
                if (target == Sidekick.sidekick && (killer == Jackal.jackal || Jackal.formerJackals.Any(x => x.PlayerId == killer.PlayerId))) infos.Add(SpecialMediumInfo.JackalKillsSidekick);
                if (target == Lawyer.lawyer && killer == Lawyer.target) infos.Add(SpecialMediumInfo.LawyerKilledByClient);
                if (medium.target.wasCleaned) infos.Add(SpecialMediumInfo.BodyCleaned);

                if (infos.Count > 0)
                {
                    var selectedInfo = infos[TheOtherRoles.rnd.Next(infos.Count)];
                    switch (selectedInfo)
                    {
                        case SpecialMediumInfo.SheriffSuicide:
                            msg = ModTranslation.getString("mediumSheriffSuicide");
                            break;
                        case SpecialMediumInfo.WarlockSuicide:
                            msg = ModTranslation.getString("mediumWarlockSuicide");
                            break;
                        case SpecialMediumInfo.ThiefSuicide:
                            msg = ModTranslation.getString("mediumThiefSuicide");
                            break;
                        case SpecialMediumInfo.ActiveLoverDies:
                            msg = ModTranslation.getString("mediumActiveLoverDies");
                            break;
                        case SpecialMediumInfo.PassiveLoverSuicide:
                            msg = ModTranslation.getString("mediumPassiveLoverSuicide");
                            break;
                        case SpecialMediumInfo.LawyerKilledByClient:
                            msg = ModTranslation.getString("mediumLawyerKilledByClient");
                            break;
                        case SpecialMediumInfo.JackalKillsSidekick:
                            msg = ModTranslation.getString("mediumJackalKillsSidekick");
                            break;
                        case SpecialMediumInfo.ImpostorTeamkill:
                            msg = ModTranslation.getString("mediumImpostorTeamkill");
                            break;
                        case SpecialMediumInfo.BodyCleaned:
                            msg = ModTranslation.getString("mediumBodyCleaned");
                            break;
                    }
                }
                else
                {
                    var randomNumber = TheOtherRoles.rnd.Next(4);
                    var typeOfColor = Helpers.isLighterColor(medium.target.killerIfExisting) ? "colorLight".Translate() : "colorDark".Translate();
                    var timeSinceDeath = (float)(medium.meetingStartTime - medium.target.timeOfDeath).TotalMilliseconds;
                    var roleString = RoleInfo.GetRolesString(medium.target.player, false);
                    var roleInfo = RoleInfo.getRoleInfoForPlayer(medium.target.player);
                    if (randomNumber == 0)
                    {
                        if (!roleInfo.Contains(RoleInfo.impostor) && !roleInfo.Contains(RoleInfo.crewmate)) msg = string.Format(ModTranslation.getString("mediumQuestion1"), RoleInfo.GetRolesString(medium.target.player, false));
                        else msg = string.Format(ModTranslation.getString("mediumQuestion5"), roleString);
                    }
                    else if (randomNumber == 1) msg = string.Format(ModTranslation.getString("mediumQuestion2"), typeOfColor);
                    else if (randomNumber == 2) msg = string.Format(ModTranslation.getString("mediumQuestion3"), Math.Round(timeSinceDeath / 1000));
                    else msg = string.Format(ModTranslation.getString("mediumQuestion4"), RoleInfo.GetRolesString(medium.target.killerIfExisting, false, false, true)) + ".";
                }

                if (TheOtherRoles.rnd.NextDouble() < chanceAdditionalInfo)
                {
                    int count = 0;
                    string condition = "";
                    var alivePlayersList = PlayerControl.AllPlayerControls.ToArray().Where(pc => !pc.Data.IsDead);
                    switch (TheOtherRoles.rnd.Next(3))
                    {
                        case 0:
                            count = alivePlayersList.Where(pc => pc.Data.Role.IsImpostor || new List<RoleInfo>() { RoleInfo.jackal, RoleInfo.sidekick, RoleInfo.sheriff, RoleInfo.thief }.Contains(RoleInfo.getRoleInfoForPlayer(pc, false).FirstOrDefault())).Count();
                            condition = ModTranslation.getString($"mediumKiller{(count == 1 ? "" : "Plural")}");
                            break;
                        case 1:
                            count = alivePlayersList.Where(Helpers.roleCanUseVents).Count();
                            condition = ModTranslation.getString($"mediumPlayerUseVents{(count == 1 ? "" : "Plural")}");
                            break;
                        case 2:
                            count = alivePlayersList.Where(pc => Helpers.isNeutral(pc) && pc != Jackal.jackal && pc != Sidekick.sidekick && pc != Thief.thief).Count();
                            condition = ModTranslation.getString($"mediumPlayerNeutral{(count == 1 ? "" : "Plural")}");
                            break;
                        case 3:
                            //count = alivePlayersList.Where(pc =>
                            break;
                    }
                    msg += $"\n" + string.Format(ModTranslation.getString("mediumAskPrefix"), string.Format(ModTranslation.getString($"mediumStillAlive{(count == 1 ? "" : "Plural")}"), string.Format(condition, count)));
                }
            }

            return string.Format(ModTranslation.getString("mediumSoulPlayerPrefix"), Medium.local.player.Data.PlayerName) + msg;
        }
    }
}
