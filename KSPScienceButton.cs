using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Toolbar;

[KSPAddon(KSPAddon.Startup.EveryScene, false)]
class KSPScienceButton : MonoBehaviour
{
    private IButton toolbarKSPScienceButton;

    internal KSPScienceButton()
    {
        toolbarKSPScienceButton = ToolbarManager.Instance.add("ScienceLibrary", "toolbarKSPScienceButton");
        toolbarKSPScienceButton.TexturePath = "ScienceLibrary/img1l";
        toolbarKSPScienceButton.ToolTip = "ScienceLibrary";
        toolbarKSPScienceButton.Visible = true;
        toolbarKSPScienceButton.OnClick += button1_OnClick;
        KSPScienceLibrary.toolbarButton = toolbarKSPScienceButton;
        toolbarKSPScienceButton.Visibility = new GameScenesVisibility(GameScenes.SPACECENTER, GameScenes.FLIGHT);
    }

    void button1_OnClick(ClickEvent e)
    {
        KSPScienceLibrary.drawWindow = !KSPScienceLibrary.drawWindow;
    }

    internal void OnDestroy()
    {
        print("Destroy Science Window Button");
        KSPScienceLibrary.toolbarButton = null;
        toolbarKSPScienceButton.Destroy();
    }
}

