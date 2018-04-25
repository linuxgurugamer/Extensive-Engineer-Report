#if true
using System.Linq;
using UnityEngine;
using KSP.UI.Screens;

using ToolbarControl_NS;

namespace JKorTech.ShipSections
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class SectionNameUI : KSPPluginFramework.MonoBehaviourWindow
    {
        private const int WindowWidth = 400, WindowHeight = 600;

        private readonly string AppLauncherIconLocation = "ExtensiveEngineerReport/Textures/ShipSections";

        private string sectionBeingRenamed;
        private string newNameInProgress = string.Empty;
        private string currentlyHighlightedSection;

        internal override void DrawWindow(int id)
        {
            if (API.AnyCurrentVesel)
            {
                DrawSectionWindow();
            }
            else
            {
                GUILayout.Label("Create a craft to use ShipSections.");
            }
        }

        const string UNKNOWN_SECTION = "Unknown Section";
        const string SECTION_PREFIX = "Stage Section ";
        const string UNSTAGED_SECTION = "Unstaged Section ";
        const string MERGED_SECTION = "Merged Section";
        void SectionByDecouplers()
        {
            foreach (var s in API.CurrentVesselParts)
            {
                var sectionInfo = s.FindModuleImplementing<SectionInfo>();
                string sectionName = UNKNOWN_SECTION;
                API.ChangeSectionName(sectionInfo.section, sectionName);
            }
            foreach (var s in API.CurrentVesselParts)
            {
                var sectionSplitter = s.FindModuleImplementing<SectionSplitter>();
                if (sectionSplitter != null)
                {
                    string sectionName = SECTION_PREFIX + s.inverseStage.ToString();
                    bool cancel = false;
                    if (sectionSplitter.section.Substring(0, SECTION_PREFIX.Length) == SECTION_PREFIX)
                    {
                        string number = sectionSplitter.section.Substring(SECTION_PREFIX.Length);
                        int result = -2;
                        int.TryParse(number, out result);
                        if (result > s.inverseStage)
                            cancel = true;
                    }
                    if (!cancel)
                    {
                        SectionSplitter.SetNewSection(s, sectionSplitter.section, sectionName);
                        sectionSplitter.InitializeAsNewSection();
                        sectionSplitter.isSectionRoot = true;
                        foreach (var symmetryPart in s.symmetryCounterparts)
                        {
                            SectionSplitter.SetNewSection(symmetryPart, symmetryPart.FindModuleImplementing<SectionInfo>().section, sectionSplitter.section);
                        }
                        API.NewSectionCreated.Fire(sectionSplitter.section);
                    }
                }
                else
                {
                    var sectionInfo = s.FindModuleImplementing<SectionInfo>();
                    if (sectionInfo.section == UNKNOWN_SECTION)
                    {
                        string sectionName = UNSTAGED_SECTION;
                        API.ChangeSectionName(sectionInfo.section, sectionName);
                    }
                }
            }
        }

        void MergeAllSections()
        {
            foreach (var s in API.CurrentVesselParts)
            {
                var sectionInfo = s.FindModuleImplementing<SectionInfo>();
                string sectionName = MERGED_SECTION;
                API.ChangeSectionName(sectionInfo.section, sectionName);
            }
        }

        private void DrawSectionWindow()
        {

            using (new GuiLayout(GuiLayout.Method.Vertical))
            using (new GuiLayout(GuiLayout.Method.Horizontal))
            {
                if (GUILayout.Button("ReSection by stages", GUILayout.ExpandWidth(true)))
                {
                    SectionByDecouplers();
                }
                if (GUILayout.Button("Merge All Sections", GUILayout.ExpandWidth(true)))
                {
                    MergeAllSections();
                }
            }
            using (new GuiLayout(GuiLayout.Method.Vertical))
            using (new GuiLayout(GuiLayout.Method.ScrollView, ref scrollPos))
            {
                string sectionNameToChange = null;
                foreach (var section in API.SectionNames)
                {
                    using (new GuiLayout(GuiLayout.Method.Horizontal))
                    {
                        GUILayout.Label(section, GUILayout.Width(200));
                        if (sectionBeingRenamed == null && GUILayout.Button("Rename", GUILayout.ExpandWidth(true)))
                        {
                            newNameInProgress = sectionBeingRenamed = section;
                        }
                        else if (sectionBeingRenamed == section)
                        {
                            newNameInProgress = GUILayout.TextField(newNameInProgress);
                            if (!string.IsNullOrEmpty(newNameInProgress) && GUILayout.Button("Save", GUILayout.ExpandWidth(false)))
                            {
                                sectionNameToChange = section;
                            }
                        }
                        bool currentlyHighlighted = currentlyHighlightedSection == section;
                        if (GUILayout.Button(currentlyHighlighted ? "Unhighlight" : "Highlight", GUILayout.ExpandWidth(false)))
                        {
                            if (currentlyHighlighted)
                            {
                                API.PartsBySection.First(group => group.Key == currentlyHighlightedSection).ToList().ForEach(part => part.SetHighlight(false, false));
                                currentlyHighlightedSection = null;
                            }
                            else
                            {
                                var sectionParts = API.PartsBySection.First(group => group.Key == section);
                                sectionParts.ToList().ForEach(part => part.SetHighlight(true, false));
                                if (!currentlyHighlighted && currentlyHighlightedSection != null)
                                    API.PartsBySection.First(group => group.Key == currentlyHighlightedSection).ToList().ForEach(part => part.SetHighlight(false, false));
                                currentlyHighlightedSection = section;
                            }
                        }
                    }
                }
                if (sectionNameToChange != null)
                {
                    API.ChangeSectionName(sectionNameToChange, newNameInProgress);
                    newNameInProgress = string.Empty;
                    sectionBeingRenamed = null;
                }
            }
        }

        internal override void Start()
        {
            DragEnabled = true;


            WindowRect.Set((Screen.width - WindowWidth) *0.75f, (Screen.height - WindowHeight) / 2, WindowWidth, WindowHeight);
            WindowCaption = nameof(ShipSections);
            if (ApplicationLauncher.Instance != null && ApplicationLauncher.Ready)
                OnAppLauncherReady();
            else
                GameEvents.onGUIApplicationLauncherReady.Add(OnAppLauncherReady);
        }

        //private ApplicationLauncherButton button;
        ToolbarControl toolbarControl;
        private Vector2 scrollPos;
        internal const string MODID = "ShipSections_NS";
        internal const string MODNAME = "Ship Sections";
        private void OnAppLauncherReady()
        {
            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(ToggleButton, ToggleButton,
                ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH,
                MODID,
                "shipSectionsButton",
                AppLauncherIconLocation + "-38",
                AppLauncherIconLocation + "-24",
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
            Destroy(toolbarControl﻿﻿);

        }
    }
}
#endif