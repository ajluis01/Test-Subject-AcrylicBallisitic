using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] SlotsDisplay hitPointsDisplay;
    [SerializeField] SlotsDisplay ammoDisplay;
    [SerializeField] Slider netWorthSlider;
    [SerializeField] Slider netWorthDamageSlider;
    [SerializeField] TMP_Text netWorthText;
    [SerializeField] Image reticle;

    public void UpdatePlayerHitPoints(int hitPoints)
    {
        //Will probably be a number
        hitPointsDisplay.SetHealth(hitPoints);
    }

    public void UpdatePlayerAmmo(Ammo[] ammo)
    {
        ammoDisplay.SetSlots(ammo);
    }

    public void UpdateNetWorth(float netWorth, float previousNetWorth, float damage)
    {
        float maxNetWorth = GameManager.GetManager().GetMaxNetWorth();

        RectTransform netWorthSliderRect = netWorthSlider.transform as RectTransform;
        float right = netWorthSliderRect.anchoredPosition.x + netWorthSliderRect.rect.width / 2;
        float left = netWorthSliderRect.anchoredPosition.x - netWorthSliderRect.rect.width / 2;

        float end = left + previousNetWorth / maxNetWorth * (right - left);

        netWorthSlider.value = netWorth / maxNetWorth;
        float start = left + netWorthSlider.value * (right - left);

        RectTransform netWorthDamageSliderRect = netWorthDamageSlider.transform as RectTransform;
        netWorthDamageSliderRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, end - start);
        netWorthDamageSliderRect.anchoredPosition = new Vector2((end + start) / 2, netWorthDamageSliderRect.anchoredPosition.y);
        netWorthDamageSlider.value = 1.0f;
        
        float netWorthValue = previousNetWorth * 10000f;
        netWorthText.text = Mathf.RoundToInt(netWorthValue).ToString("C2");
    }

    public void UpdateReticle(float size)
    {
        reticle.transform.position = Input.mousePosition;
        reticle.transform.localScale = new Vector3(size, size, size);
    }

    void Start()
    {
        netWorthSlider.value = 1.0f;
        netWorthDamageSlider.value = 0.0f;
    }

    void Update()
    {
    }
}