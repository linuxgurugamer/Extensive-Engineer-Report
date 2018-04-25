using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using static JKorTech.Extensive_Engineer_Report.ConcernUtils;
using static JKorTech.Extensive_Engineer_Report.KSPExtensions;
using ToolbarControl_NS;

namespace JKorTech.Extensive_Engineer_Report
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class EERWindow : KSPPluginFramework.MonoBehaviourWindow
    {

        private const int WindowWidth = 400, WindowHeight = 600;
        //private ApplicationLauncherButton button;
        ToolbarControl toolbarControl;

        private Vector2 scrollPos;
        private static readonly string TestsPassingIconLocation = "ExtensiveEngineerReport/Textures/TestPass";
        private static readonly string TestsFailIconLocation = "ExtensiveEngineerReport/Textures/TestFail";
        private GUIStyle passStyle;
        private GUIStyle failStyle;
        private GUIStyle descriptionStyle;
        private bool stylesInit;

        internal override void Start()
        {
            DragEnabled = true;
            WindowRect.Set((Screen.width - WindowWidth) / 4, (Screen.height - WindowHeight) / 2, WindowWidth, WindowHeight);

            WindowCaption = "Extensive Engineer Report";

            OnAppLauncherReady();

        }

        internal override void Update()
        {
            base.Update();
            if (ConcernRunner.Instance == null)
                return;
            if (ConcernRunner.Instance.TestsPass)
            {
                Log.Info("Update TestsPass");
                toolbarControl.SetTexture(TestsPassingIconLocation + "-38", TestsPassingIconLocation + "-24");
                EditorLogic.fetch.launchBtn.image.color = Color.green;
            }
            else
            {
                Log.Info("Update TEstsFail");
                toolbarControl.SetTexture(TestsFailIconLocation + "-38", TestsFailIconLocation + "-24");
                EditorLogic.fetch.launchBtn.image.color = Color.red;
            }
        }

        internal const string MODID = "EER_NS";
        internal const string MODNAME = "Extensive Engineer Report";

        private void OnAppLauncherReady()
        {
            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(ToggleButton, ToggleButton,
                ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH,
                MODID,
                "eerButton",
                TestsPassingIconLocation + "-38",
                TestsPassingIconLocation + "-24",
                MODNAME
            );

        }
        void ToggleButton()
        {
            Visible = !Visible;
        }
        internal override void OnDestroy()
        {
            toolbarControl.OnDestroy();
            Destroy(toolbarControl);
            Visible = false;
        }

        internal override void DrawWindow(int id)
        {

            if (!stylesInit)
            {
                InitStyles();
                stylesInit = true;
            }
            using (new GuiLayout(GuiLayout.Method.Horizontal))
            {
                var settings = GetScenarioModules<GeneralSettings>().FirstOrDefault();
                if (settings == null)
                    return;
                GUILayout.Label("Enabled Test Severity");
                var old = settings.critical;
                settings.critical = GUILayout.Toggle(settings.critical, "Critical", KSPPluginFramework.SkinsLibrary.CurrentSkin.button);
                if (old != settings.critical) ConcernRunner.Instance.RunTests();
                old = settings.warning;
                settings.warning = GUILayout.Toggle(settings.warning, "Warning", KSPPluginFramework.SkinsLibrary.CurrentSkin.button);
                if (old != settings.warning) ConcernRunner.Instance.RunTests();
                old = settings.notice;
                settings.notice = GUILayout.Toggle(settings.notice, "Notice", KSPPluginFramework.SkinsLibrary.CurrentSkin.button);
                if (old != settings.notice) ConcernRunner.Instance.RunTests();
            }

            using (new GuiLayout(GuiLayout.Method.ScrollView, ref scrollPos))
            {
                GUILayout.Label("Ship-Wide Tests");

                // Needed to add check for the count since apparently if a dictinary is empty, accessing it will cause an error
                if (ConcernRunner.Instance.ShipConcerns.Count > 0)
                    foreach (var test in ConcernLoader.ShipDesignConcerns.Where(test => InCorrectFacility(test) && test.IsApplicable()))
                    {
                        var passed = ConcernRunner.Instance.ShipConcerns[test];
                        GUILayout.Toggle(true, test.GetConcernTitle(), passed ? passStyle : failStyle);
                        if (!passed)
                        {
                            GUILayout.Label(test.GetConcernDescription(), descriptionStyle);
                        }
                    }
                GUILayout.Label("Section-Specific Tests");
                foreach (var section in ShipSections.API.PartsBySection)
                {
                    var sectionData = ConcernRunner.Instance.SectionConcerns[section.Key];
                    GUILayout.Label(section.Key);
                    foreach (var test in ConcernLoader.SectionDesignConcerns.Where(test => InCorrectFacility(test) && test.IsApplicable(section)))
                    {
                        var run = sectionData.ContainsKey(test);
                        bool runNext;
                        if (run)
                        {
                            runNext = GUILayout.Toggle(run, test.GetConcernTitle(), sectionData[test] ? passStyle : failStyle);
                            if (!sectionData[test])
                            {
                                GUILayout.Label(test.GetConcernDescription(), descriptionStyle);
                            }
                        }
                        else
                            runNext = GUILayout.Toggle(run, test.GetConcernTitle(), passStyle);
                        if (!runNext)
                            ConcernRunner.Instance.DisableTest(section.Key, test);
                        if (!run && runNext)
                            ConcernRunner.Instance.EnableTest(section.Key, test);
                    }
                }
            }
        }

        private void InitStyles()
        {
            passStyle = new GUIStyle(KSPPluginFramework.SkinsLibrary.CurrentSkin.toggle);
            passStyle.onNormal.textColor = XKCDColors.AlgaeGreen;
            passStyle.hover = passStyle.onHover;
            LogFormatted_DebugOnly("Created pass style");
            failStyle = new GUIStyle(KSPPluginFramework.SkinsLibrary.CurrentSkin.toggle);
            failStyle.onNormal = failStyle.hover;
            failStyle.onNormal.textColor = Color.red;
            LogFormatted_DebugOnly("Created fail style");
            descriptionStyle = new GUIStyle(KSPPluginFramework.SkinsLibrary.CurrentSkin.label)
            {
                wordWrap = true
            };
            descriptionStyle.normal.textColor = Color.red;
            LogFormatted_DebugOnly("Created description style");
        }
    }
}
