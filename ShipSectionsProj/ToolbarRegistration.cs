using UnityEngine;
using ToolbarControl_NS;

namespace JKorTech.ShipSections
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(SectionNameUI.MODID, SectionNameUI.MODNAME);
        }
    }
}