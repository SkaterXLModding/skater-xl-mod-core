using UnityEngine;
using UnityEngine.UI;

public static class UIExtensions
{
    private static MenuButton m_exitButton;

    public static void SetAnchor(this RectTransform rt, float xMin, float yMin, float xMax, float yMax)
    {
        rt.anchorMin = new Vector2(xMin, yMin);
        rt.anchorMax = new Vector2(xMax, yMax);
    }

    public static void SetOffset(this RectTransform rt, float left, float top, float right, float bottom)
    {
        rt.offsetMin = new Vector2(left, top);
        rt.offsetMax = new Vector2(-right, -bottom);
    }

    public static void CreateExitMenuButton()
    {
        if (m_exitButton != null) return;  // Exit Button Exists

        MenuButton[] pauseMenuComponents = GameManagement.GameStateMachine.Instance.PauseObject.GetComponentsInChildren<MenuButton>();
        if (pauseMenuComponents == null) return;

        MenuButton reference = pauseMenuComponents[0];
        m_exitButton = UnityEngine.Object.Instantiate<MenuButton>(reference, reference.transform.parent);

        if (m_exitButton == null) return;

        m_exitButton.Label.SetText("Quit");
        m_exitButton.gameObject.SetActive(true);
        m_exitButton.onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
        m_exitButton.onClick.AddListener(() => Application.Quit());
    }
}
