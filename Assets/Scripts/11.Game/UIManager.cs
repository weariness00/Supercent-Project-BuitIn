using UnityEngine;
using UnityEngine.UI;
using Util;

public class UIManager : Singleton<UIManager>
{
    public Canvas mainCanvas;
    public Canvas tutorialCanvas;
    public Canvas customerCanvas;

    protected override void Initialize()
    {
        base.Initialize();

        void InstantiateCanvas(out Canvas canvas, string canvasName)
        {
            var obj = new GameObject(canvasName);
            obj.transform.SetParent(transform);
            canvas = obj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            obj.AddComponent<GraphicRaycaster>();
            obj.AddComponent<CanvasScaler>();
        }

        InstantiateCanvas(out mainCanvas, "Dont Destroy Canvas");
        InstantiateCanvas(out tutorialCanvas, "Tutorial Canvas");
        InstantiateCanvas(out customerCanvas, "Customer Canvas");

        mainCanvas.sortingOrder = 1000;
    }
}

