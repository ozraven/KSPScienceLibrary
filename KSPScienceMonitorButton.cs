using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Toolbar;

[KSPAddon(KSPAddon.Startup.Flight, false)]
class KSPScienceMonitorButton : MonoBehaviour
{
    private IButton toolbarKSPScienceMonitorButton;

    internal KSPScienceMonitorButton()
    {
        toolbarKSPScienceMonitorButton = ToolbarManager.Instance.add("ScienceLibrary", "toolbarKSPScienceMonitorButton");
        toolbarKSPScienceMonitorButton.TexturePath = "ScienceLibrary/img1m";
        toolbarKSPScienceMonitorButton.ToolTip = "ScienceLibrary Monitor";
        toolbarKSPScienceMonitorButton.Visible = true;
        toolbarKSPScienceMonitorButton.OnClick += KSPScienceMonitorButton_OnClick;
        KSPScienceMonitor.toolbarButton = toolbarKSPScienceMonitorButton;
    }

    void KSPScienceMonitorButton_OnClick(ClickEvent e)
    {
        KSPScienceMonitor.drawWindow = !KSPScienceMonitor.drawWindow;
    }

    internal void OnDestroy()
    {
        print("Destroy Science Monitor Button");
        KSPScienceMonitor.toolbarButton = null;
        toolbarKSPScienceMonitorButton.Destroy();
    }
}

