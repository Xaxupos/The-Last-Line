using System.Text;
using TMPro;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class UpgradeTooltip : UIPanel
{
    public TMP_Text upgradeName;
    public TMP_Text upgradeDescription;
    public TMP_Text costValue;
    public TMP_Text stackValue;
    public RectTransform rectTransform;
    public Canvas rootCanvas;
    public bool followMouse = true;
    public Vector2 cursorOffset = new Vector2(16f, -16f);

    private bool _visible;

    private void Awake()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        if (rootCanvas == null)
            rootCanvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        if (!_visible || !followMouse || rectTransform == null)
            return;

        UpdatePosition(GetPointerPosition());
    }

    public void Show(UpgradeDefinition definition, int currentStacks)
    {
        if (definition == null)
        {
            HidePanel();
            return;
        }

        if (upgradeName != null)
            upgradeName.text = string.IsNullOrWhiteSpace(definition.displayName) ? definition.name : definition.displayName;

        if (upgradeDescription != null)
            upgradeDescription.text = definition.description ?? string.Empty;

        if (costValue != null)
            costValue.text = BuildCostText(definition.costs);

        if (stackValue != null)
            stackValue.text = BuildStackText(definition.maxStacks, currentStacks);

        _visible = true;
        if (followMouse)
            UpdatePosition(GetPointerPosition());

        ShowPanel();
    }

    public override void HidePanel()
    {
        _visible = false;
        base.HidePanel();
    }

    private static string BuildCostText(CurrencyAmount[] costs)
    {
        if (costs == null || costs.Length == 0)
            return "Free";

        var sb = new StringBuilder();
        for (int i = 0; i < costs.Length; i++)
        {
            if (i > 0)
                sb.Append(", ");

            sb.Append(costs[i].amount);
        }

        return sb.ToString();
    }

    private static string BuildStackText(int maxStacks, int currentStacks)
    {
        if (maxStacks <= 0)
            return $"{currentStacks}/∞";

        return $"{currentStacks}/{maxStacks}";
    }

    private void UpdatePosition(Vector3 screenPosition)
    {
        if (rootCanvas == null)
        {
            rectTransform.position = screenPosition + (Vector3)cursorOffset;
            return;
        }

        if (rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            rectTransform.position = screenPosition + (Vector3)cursorOffset;
            return;
        }

        var canvasRect = rootCanvas.transform as RectTransform;
        if (canvasRect == null)
        {
            rectTransform.position = screenPosition + (Vector3)cursorOffset;
            return;
        }

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, rootCanvas.worldCamera, out var localPoint))
            rectTransform.localPosition = localPoint + cursorOffset;
    }

    private static Vector2 GetPointerPosition()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null)
            return Mouse.current.position.ReadValue();
        return Vector2.zero;
#elif ENABLE_LEGACY_INPUT_MANAGER
        return Input.mousePosition;
#else
        return Vector2.zero;
#endif
    }
}
