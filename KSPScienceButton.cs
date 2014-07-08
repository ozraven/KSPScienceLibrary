using Toolbar;
using UnityEngine;

[KSPAddon(KSPAddon.Startup.EveryScene, false)]
public class KSPScienceButton : MonoBehaviour
{
    private readonly IButton toolbarKSPScienceButton;
    private string imgEnabledPath = "KSPScienceLibrary/img1l";
    private string imgPressedPath = "KSPScienceLibrary/img2l";

    internal KSPScienceButton()
    {
        toolbarKSPScienceButton = ToolbarManager.Instance.add("ScienceLibrary", "toolbarKSPScienceButton");
        toolbarKSPScienceButton.TexturePath = imgEnabledPath;
        toolbarKSPScienceButton.ToolTip = "ScienceLibrary";
        toolbarKSPScienceButton.Visible = true;
        toolbarKSPScienceButton.OnClick += button1_OnClick;
        KSPScienceLibrary.toolbarButton = this;
        toolbarKSPScienceButton.Visibility = new GameScenesVisibility(GameScenes.SPACECENTER, GameScenes.FLIGHT, GameScenes.EDITOR);
    }

    private void button1_OnClick(ClickEvent e)
    {
        if (KSPScienceLibrary.DrawWindow)
            KSPScienceLibrary.Hide();
        else
            KSPScienceLibrary.Show();
    }

    public void UpdateIcon(bool drawWindow)
    {
        toolbarKSPScienceButton.TexturePath = drawWindow ? imgPressedPath : imgEnabledPath;
    }

    internal void OnDestroy()
    {
        //print("Destroy Science Window Button");
        KSPScienceLibrary.toolbarButton = null;
        toolbarKSPScienceButton.Destroy();
    }
}