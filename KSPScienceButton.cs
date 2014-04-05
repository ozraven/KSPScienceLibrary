using UnityEngine;
using Toolbar;

[KSPAddon(KSPAddon.Startup.EveryScene, false)]
public class KSPScienceButton : MonoBehaviour
{
    private IButton toolbarKSPScienceButton;

    internal KSPScienceButton()
    {
        toolbarKSPScienceButton = ToolbarManager.Instance.add("ScienceLibrary", "toolbarKSPScienceButton");
        toolbarKSPScienceButton.TexturePath = "ScienceLibrary/img1l";
        toolbarKSPScienceButton.ToolTip = "ScienceLibrary";
        toolbarKSPScienceButton.Visible = true;
        toolbarKSPScienceButton.OnClick += button1_OnClick;
        KSPScienceLibrary.toolbarButton = (KSPScienceButton) toolbarKSPScienceButton;
        toolbarKSPScienceButton.Visibility = new GameScenesVisibility(GameScenes.SPACECENTER, GameScenes.FLIGHT);
    }

    private void button1_OnClick(ClickEvent e)
    {
        KSPScienceLibrary.drawWindow = !KSPScienceLibrary.drawWindow;
    }

    public void UpdateIcon(bool drawWindow)
    {
        toolbarKSPScienceButton.TexturePath = drawWindow ? "ScienceLibrary/img2l" : "ScienceLibrary/img1l";
    }

    internal void OnDestroy()
    {
        print("Destroy Science Window Button");
        KSPScienceLibrary.toolbarButton = null;
        toolbarKSPScienceButton.Destroy();
    }
}

