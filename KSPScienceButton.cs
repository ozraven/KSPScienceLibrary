using System;

using UnityEngine;

namespace KSPScienceLibrary
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class KSPScienceButton : MonoBehaviour
    {
        private readonly IButton toolbarKSPScienceButton;
        private string imgEnabledPath = "KSPScienceLibrary/img1l";
        private string imgPressedPath = "KSPScienceLibrary/img2l";

        internal KSPScienceButton()
        {
            if(ToolbarManager.ToolbarAvailable)
            {
                toolbarKSPScienceButton = ToolbarManager.Instance.add("ScienceLibrary", "toolbarKSPScienceButton");
                toolbarKSPScienceButton.TexturePath = imgEnabledPath;
                toolbarKSPScienceButton.ToolTip = "ScienceLibrary";
                toolbarKSPScienceButton.Visible = true;
                toolbarKSPScienceButton.OnClick += button1_OnClick;
                KSPScienceLibraryPlugin.toolbarButton = this;
                toolbarKSPScienceButton.Visibility = new GameScenesVisibility(GameScenes.SPACECENTER, GameScenes.FLIGHT, GameScenes.EDITOR, GameScenes.TRACKSTATION);
            }
        }

        private void button1_OnClick(ClickEvent e)
        {
            if(KSPScienceLibraryPlugin.drawWindow)
                KSPScienceLibraryPlugin.Hide();
            else
                KSPScienceLibraryPlugin.Show();
        }

        public void UpdateIcon(bool drawWindow)
        {
            if(toolbarKSPScienceButton != null)
                toolbarKSPScienceButton.TexturePath = drawWindow ? imgPressedPath : imgEnabledPath;
        }

        internal void OnDestroy()
        {
            if(toolbarKSPScienceButton != null)
            {
                //print("Destroy Science Window Button");
                KSPScienceLibraryPlugin.toolbarButton = null;
                toolbarKSPScienceButton.Destroy();
            }
        }
    }
}