using UnityEngine;

/// <summary>
/// 面板标识组件 —— 挂载到每个 UI 面板的根 GameObject 上，
/// 用于 UIManager 在场景重载后自动重新绑定 CanvasGroup 引用。
/// </summary>
public class UIPanelIdentifier : MonoBehaviour
{
    public UIPanelType panelType;
}
