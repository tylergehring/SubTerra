using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    private PlayerController player;
    public Gradient gradient;
    public Image fill;

    void Start()
    {
        player = PlayerController.Instance;
        if (player != null)
        {
            SetMaxHealth(player.getHealth());
        }
    }

    void Update()
    {
        if (player != null)
        {
            SetHealth(player.getHealth());
        }
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(int health)
    {
        slider.value = health;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
