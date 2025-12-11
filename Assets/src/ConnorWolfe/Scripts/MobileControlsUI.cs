using UnityEngine;
using UnityEngine.UIElements;

public class MobileControlsUI : MonoBehaviour
{
    [SerializeField] private UIDocument uiDoc;
    [SerializeField] private PlayerController player;

    private Button leftButton;
    private Button rightButton;
    private Button jumpButton;
    private Button useButton;

    private float horizontal = 0f;
    private bool jumpPressed = false;

    private void Awake()
    {
        // ✅ Hide UI if not running on mobile
        if (Application.platform != RuntimePlatform.Android &&
            Application.platform != RuntimePlatform.IPhonePlayer)
        {
            if (uiDoc != null)
                uiDoc.rootVisualElement.style.display = DisplayStyle.None;
            return;
        }

        // Auto‑assign if not set in Inspector
        if (uiDoc == null)
            uiDoc = GetComponent<UIDocument>();

        if (player == null)
            player = FindObjectOfType<PlayerController>();

        if (uiDoc == null || player == null)
        {
            Debug.LogError("MobileControlsUI: Missing UIDocument or PlayerController reference.");
            return;
        }

        var root = uiDoc.rootVisualElement;

        leftButton = root.Q<Button>("LeftButton");
        rightButton = root.Q<Button>("RightButton");
        jumpButton = root.Q<Button>("JumpButton");
        useButton = root.Q<Button>("UseButton");

        if (leftButton == null || rightButton == null || jumpButton == null || useButton == null)
        {
            Debug.LogError("MobileControlsUI: One or more buttons not found. Check names in UI Builder.");
            return;
        }

        // LEFT
        leftButton.RegisterCallback<PointerDownEvent>(evt => horizontal = -1f);
        leftButton.RegisterCallback<PointerUpEvent>(evt =>
        {
            if (horizontal < 0f)
                horizontal = 0f;
        });

        // RIGHT
        rightButton.RegisterCallback<PointerDownEvent>(evt => horizontal = 1f);
        rightButton.RegisterCallback<PointerUpEvent>(evt =>
        {
            if (horizontal > 0f)
                horizontal = 0f;
        });

        // JUMP
        jumpButton.clicked += () =>
        {
            jumpPressed = true;
        };

        // USE
        useButton.clicked += () =>
        {
            player.MobileUseCurrentItem();
        };
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            player.SetMobileInput(horizontal, jumpPressed);
            jumpPressed = false;
        }
    }
}
