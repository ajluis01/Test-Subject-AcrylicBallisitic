using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotsDisplay : MonoBehaviour
{
    [SerializeField] GameObject slotPrefab;
    [SerializeField] Sprite fullSlotSprite;
    [SerializeField] Sprite emptySlotSprite;
    [SerializeField] Sprite hitSlotSprite;
    [SerializeField] Sprite powerUpSlotSprite;
    [SerializeField] int maxSlots = 6;

    GameObject[] slots;
    Image[] slotImages;

    public void SetSlots(Ammo[] slots)
    {
        for (int i = 0; i < maxSlots; i++)
        {
            if (slotImages == null || slotImages[i] == null || i >= slotImages.Length) continue;
            if (slots[i] == Ammo.Loaded)
            {
                slotImages[i].sprite = fullSlotSprite;
            }
            else if(slots[i] == Ammo.Miss)
            {
                slotImages[i].sprite = emptySlotSprite;
            }
            else if (slots[i] == Ammo.Hit)
            {
                slotImages[i].sprite = hitSlotSprite;
            }
            else if (slots[i] == Ammo.PowerUp)
            {
                slotImages[i].sprite = powerUpSlotSprite;
            }
        }
    }

    public void SetHealth(int hpCount)
    {
        for (int i = 0; i < maxSlots; i++)
        {
            if (slotImages == null || slotImages[i] == null || i >= slotImages.Length) continue;
            if (i < hpCount)
            {
                slotImages[i].sprite = fullSlotSprite;
            }
            else
            {
                slotImages[i].sprite = emptySlotSprite;
            }
        }
    }

    void Awake()
    {
        // Delete all children (in case there are any in the editor)
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    void Start()
    {
        slots = new GameObject[maxSlots];
        slotImages = new Image[maxSlots];
        for (int i = 0; i < maxSlots; i++)
        {
            slots[i] = Instantiate(slotPrefab, transform);
            slotImages[i] = slots[i].GetComponent<Image>();
        }
    }

    void OnDestroy()
    {
        slots = null;
        slotImages = null;   
    }
}