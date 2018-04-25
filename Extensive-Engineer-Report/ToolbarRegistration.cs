
using UnityEngine;
using ToolbarControl_NS;

namespace JKorTech.Extensive_Engineer_Report
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(EERWindow.MODID, EERWindow.MODNAME);
        }
    }
}