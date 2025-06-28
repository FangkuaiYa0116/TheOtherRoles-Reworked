using AmongUs.Data;
using Assets.InnerNet;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.UI.Button;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Modules
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public class MainMenuPatch
    {
        private static AnnouncementPopUp popUp;

        private static void Prefix(MainMenuManager __instance)
        {
            SoundEffectsManager.Load();

            GameObject NewsB = GameObject.Find("NewsButton");
            GameObject AccountB = GameObject.Find("AcountButton");
            GameObject SettingsB = GameObject.Find("SettingsButton");
            List<GameObject> objects = new() { NewsB, AccountB, SettingsB };
            foreach (GameObject obj in objects)
            {
                obj.transform.localScale = new Vector3(0.41f, 0.84f, 1);
                var pos = obj.transform.localPosition;
                pos.x = -0.87f;
                obj.transform.localPosition = pos;

                var FontPlacer = obj.transform.FindChild("FontPlacer").gameObject;
                FontPlacer.transform.localScale = new Vector3(2, 1, 1);
                FontPlacer.transform.localPosition = new Vector3(-1.6159f, -0.0818f, 0);

                var Icon = obj.transform.FindChild("Inactive").FindChild("Icon").gameObject;
                Icon.transform.localScale += new Vector3(0.4f, 0, 0);

                var Icon2 = obj.transform.FindChild("Highlight").FindChild("Icon").gameObject;
                Icon2.transform.localScale += new Vector3(0.4f, 0, 0);
            }



            GameObject buttonDiscord = Object.Instantiate(AccountB, AccountB.transform.parent);
            buttonDiscord.name = "DiscordButton";
            buttonDiscord.transform.localPosition = new Vector3(0.87f, -0.387f, 0);
            buttonDiscord.transform.FindChild("Inactive").FindChild("Icon").GetComponent<SpriteRenderer>().sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.discord.png", 240f);
            buttonDiscord.transform.FindChild("Highlight").FindChild("Icon").GetComponent<SpriteRenderer>().sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.discord.png", 240f);

            TMPro.TMP_Text textDiscord = buttonDiscord.transform.GetComponentInChildren<TMPro.TMP_Text>();
            __instance.StartCoroutine(Effects.Lerp(0.5f, new Action<float>((p) =>
            {
                textDiscord.SetText("Mod Discord");
            })));
            PassiveButton passiveButtonDiscord = buttonDiscord.GetComponent<PassiveButton>();
            passiveButtonDiscord.OnClick = new ButtonClickedEvent();
            passiveButtonDiscord.OnClick.AddListener((Action)(() => Application.OpenURL("https://discord.gg/TUceswKfRg")));

            GameObject creditsButton = Object.Instantiate(AccountB, AccountB.transform.parent);
            creditsButton.name = "CreditsButton";
            creditsButton.transform.localPosition = new Vector3(0.87f, -0.912f, 0);
            creditsButton.transform.FindChild("Inactive").FindChild("Icon").GetComponent<SpriteRenderer>().sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CreditsButton.png", 200f);
            creditsButton.transform.FindChild("Highlight").FindChild("Icon").GetComponent<SpriteRenderer>().sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CreditsButton.png", 200f);


            TMPro.TMP_Text textCreditsButton = creditsButton.transform.GetComponentInChildren<TMPro.TMP_Text>();
            __instance.StartCoroutine(Effects.Lerp(0.5f, new Action<float>((p) =>
            {
                textCreditsButton.SetText("Mod Credits");
            })));

            PassiveButton passiveCreditsButton = creditsButton.GetComponent<PassiveButton>();
            passiveCreditsButton.OnClick = new ButtonClickedEvent();
            passiveCreditsButton.OnClick.AddListener((Action)delegate
            {
                // do stuff
                if (popUp != null) Object.Destroy(popUp);
                AnnouncementPopUp popUpTemplate = Object.FindObjectOfType<AnnouncementPopUp>(true);
                if (popUpTemplate == null)
                {
                    return;
                }
                popUp = Object.Instantiate(popUpTemplate);
                popUp.gameObject.SetActive(true);
                string creditsString = @$"<align=""center""><b>Team:</b>
Xtremw Wave

<b>TORR Develop Members:</b>
FangkuaiYa   ELinmei   Duye

<b>Additional Devs:</b>
Imp11

<b>Github Contributors:</b>
None

<b>[https://discord.gg/TUceswKfRg]Discord[] Moderators:</b>
None

Thanks to miniduikboot & GD for hosting modded servers (and so much more)

";
                creditsString += $@"<size=60%> <b>Other Credits & Resources:</b>
OxygenFilter - For the versions v2.3.0 to v2.6.1, we were using the OxygenFilter for automatic deobfuscation
Reactor - The framework used for all versions before v2.0.0, and again since 4.2.0
BepInEx - Used to hook game functions
Essentials - Custom game options by DorCoMaNdO:
Before v1.6: We used the default Essentials release
v1.6-v1.8: We slightly changed the default Essentials.
v2.0.0 and later: As we were not using Reactor anymore, we are using our own implementation, inspired by the one from DorCoMaNdO
Jackal and Sidekick - Original idea for the Jackal and Sidekick came from Dhalucard
Among-Us-Love-Couple-Mod - Idea for the Lovers modifier comes from Woodi-dev
Jester - Idea for the Jester role came from Maartii
ExtraRolesAmongUs - Idea for the Engineer and Medic role came from NotHunter101. Also some code snippets from their implementation were used.
Among-Us-Sheriff-Mod - Idea for the Sheriff role came from Woodi-dev
TooManyRolesMods - Idea for the Detective and Time Master roles comes from Hardel-DW. Also some code snippets from their implementation were used.
TownOfUs - Idea for the Swapper, Shifter, Arsonist and a similar Mayor role came from Slushiegoose
Ottomated - Idea for the Morphling, Snitch and Camouflager role came from Ottomated
Crowded-Mod - Our implementation for 10+ player lobbies was inspired by the one from the Crowded Mod Team
Goose-Goose-Duck - Idea for the Vulture role came from Slushiegoose
TheEpicRoles - Idea for the first kill shield (partly) and the (old) tabbed option menu (fully + some code), by LaicosVK DasMonschta Nova
StellarRole - Main menu code and custom option some code also some art resources
四个憨批汉化组 - Some button resources
ugackMiner53 - Idea and core code for the Prop Hunt game mode
Role Draft Music: [https://www.youtube.com/watch?v=9STiQ8cCIo0]Unreal Superhero 3 by Kenët & Rez[]

License: TheOtherRoles is licensed under the [https://github.com/FangkuaiYa0116/TheOtherRoles?tab=GPL-3.0-1-ov-file#readme]GPLv3[]
</size>";
                creditsString += "</align>";

                Announcement creditsAnnouncement = new()
                {
                    Id = "ModCredits",
                    Language = 0,
                    Number = 502,
                    Title = "TheOtherRoles-Reworked\nCredits & Resources",
                    ShortTitle = "TheOtherRoles-Reworked\nCredits",
                    SubTitle = "Credits & Resources",
                    PinState = false,
                    Date = "06.07.2025",
                    Text = creditsString,
                };

                __instance.StartCoroutine(Effects.Lerp(0.01f, new Action<float>((p) =>
                {
                    if (p == 1)
                    {
                        Il2CppSystem.Collections.Generic.List<Announcement> backup = DataManager.Player.Announcements.allAnnouncements;
                        DataManager.Player.Announcements.allAnnouncements = new();
                        popUp.Init(false);
                        DataManager.Player.Announcements.SetAnnouncements(new Announcement[] { creditsAnnouncement });
                        popUp.CreateAnnouncementList();
                        popUp.UpdateAnnouncementText(creditsAnnouncement.Number);
                        popUp.visibleAnnouncements[0].PassiveButton.OnClick.RemoveAllListeners();
                        DataManager.Player.Announcements.allAnnouncements = backup;
                    }
                })));
            });

            GameObject buttonQQ = Object.Instantiate(AccountB, AccountB.transform.parent);
            buttonQQ.name = "QQButton";
            buttonQQ.transform.localPosition = new Vector3(0.87f, -1.444f, 0);
            buttonQQ.transform.FindChild("Inactive").FindChild("Icon").GetComponent<SpriteRenderer>().sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.QQGroup.png", 240f);
            buttonQQ.transform.FindChild("Highlight").FindChild("Icon").GetComponent<SpriteRenderer>().sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.QQGroup.png", 240f);

            TMPro.TMP_Text textRegion = buttonQQ.transform.GetComponentInChildren<TMPro.TMP_Text>();
            __instance.StartCoroutine(Effects.Lerp(0.5f, new Action<float>((p) =>
            {
                textRegion.SetText("QQ Group");
            })));
            PassiveButton passiveButtonQQGroup = buttonQQ.GetComponent<PassiveButton>();
            passiveButtonQQGroup.OnClick = new ButtonClickedEvent();
            passiveButtonQQGroup.OnClick.AddListener((Action)(() => Application.OpenURL("https://qm.qq.com/q/PYnVTb9IQK")));
        }

        public static void addSceneChangeCallbacks()
        {
            SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)((scene, _) =>
            {
                if (!scene.name.Equals("MatchMaking", StringComparison.Ordinal)) return;
                TORMapOptions.gameMode = CustomGamemodes.Classic;
                // Add buttons For Guesser Mode, Hide N Seek in this scene.
                // find "HostLocalGameButton"
                var template = GameObject.FindObjectOfType<HostLocalGameButton>();
                var gameButton = template.transform.FindChild("CreateGameButton");
                var gameButtonPassiveButton = gameButton.GetComponentInChildren<PassiveButton>();

                var guesserButton = GameObject.Instantiate<Transform>(gameButton, gameButton.parent);
                guesserButton.transform.localPosition += new Vector3(0f, -0.5f);
                var guesserButtonText = guesserButton.GetComponentInChildren<TMPro.TextMeshPro>();
                var guesserButtonPassiveButton = guesserButton.GetComponentInChildren<PassiveButton>();

                guesserButtonPassiveButton.OnClick = new Button.ButtonClickedEvent();
                guesserButtonPassiveButton.OnClick.AddListener((System.Action)(() =>
                {
                    TORMapOptions.gameMode = CustomGamemodes.Guesser;
                    template.OnClick();
                }));

                var HideNSeekButton = GameObject.Instantiate<Transform>(gameButton, gameButton.parent);
                HideNSeekButton.transform.localPosition += new Vector3(1.7f, -0.5f);
                var HideNSeekButtonText = HideNSeekButton.GetComponentInChildren<TMPro.TextMeshPro>();
                var HideNSeekButtonPassiveButton = HideNSeekButton.GetComponentInChildren<PassiveButton>();

                HideNSeekButtonPassiveButton.OnClick = new Button.ButtonClickedEvent();
                HideNSeekButtonPassiveButton.OnClick.AddListener((System.Action)(() =>
                {
                    TORMapOptions.gameMode = CustomGamemodes.HideNSeek;
                    template.OnClick();
                }));

                var PropHuntButton = GameObject.Instantiate<Transform>(gameButton, gameButton.parent);
                PropHuntButton.transform.localPosition += new Vector3(3.4f, -0.5f);
                var PropHuntButtonText = PropHuntButton.GetComponentInChildren<TMPro.TextMeshPro>();
                var PropHuntButtonPassiveButton = PropHuntButton.GetComponentInChildren<PassiveButton>();

                PropHuntButtonPassiveButton.OnClick = new Button.ButtonClickedEvent();
                PropHuntButtonPassiveButton.OnClick.AddListener((System.Action)(() =>
                {
                    TORMapOptions.gameMode = CustomGamemodes.PropHunt;
                    template.OnClick();
                }));

                template.StartCoroutine(Effects.Lerp(0.1f, new System.Action<float>((p) =>
                {
                    guesserButtonText.SetText("TOR Guesser");
                    HideNSeekButtonText.SetText("TOR Hide N Seek");
                    PropHuntButtonText.SetText("TOR Prop Hunt");
                })));
            }));
        }
    }
}