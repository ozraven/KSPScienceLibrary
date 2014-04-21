using Toolbar;
using UnityEngine;

[KSPAddon(KSPAddon.Startup.Flight, false)]
public class KSPScienceMonitorButton : MonoBehaviour
{
    private readonly IButton toolbarKSPScienceMonitorButton;
    private string imgEnabledPath = "KSPScienceLibrary/img1m";
    private string imgPressedPath = "KSPScienceLibrary/img2m";

    internal KSPScienceMonitorButton()
    {
        toolbarKSPScienceMonitorButton = ToolbarManager.Instance.add("ScienceLibrary", "toolbarKSPScienceMonitorButton");
        toolbarKSPScienceMonitorButton.TexturePath = imgEnabledPath;
        toolbarKSPScienceMonitorButton.ToolTip = "ScienceLibrary Monitor";
        toolbarKSPScienceMonitorButton.Visible = true;
        toolbarKSPScienceMonitorButton.OnClick += KSPScienceMonitorButton_OnClick;
        KSPScienceMonitor.toolbarButton = this;
        toolbarKSPScienceMonitorButton.Visibility = new GameScenesVisibility(GameScenes.FLIGHT);
    }

    private void KSPScienceMonitorButton_OnClick(ClickEvent e)
    {
        KSPScienceMonitor.drawWindow = !KSPScienceMonitor.drawWindow;
    }

    public void UpdateIcon(bool drawWindow)
    {
        toolbarKSPScienceMonitorButton.TexturePath = drawWindow ? imgPressedPath : imgEnabledPath;
    }

    internal void OnDestroy()
    {
        print("Destroy Science Monitor Button");
        KSPScienceMonitor.toolbarButton = null;
        toolbarKSPScienceMonitorButton.Destroy();
    }
}