using UnityEngine;
using UnityEngine.UI;

public class PlayerPortraitUI : MonoBehaviour
{
    [Header("角色头像 (0=战士 1=弓箭手 2=僧侣)")]
    public Sprite[] characterSprites;

    [Header("UI 引用")]
    public Image currentPortrait;
    public Image[] partyPortraits;

    [Header("样式")]
    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(1f, 1f, 1f, 0.5f);
    public float activeScale = 1.0f;
    public float inactiveScale = 0.7f;

    private int currentIndex = 0;

    void Start()
    {
        UpdatePortrait(0);
    }

    public void UpdatePortrait(int index)
    {
        if (index < 0 || index >= characterSprites.Length) return;

        currentIndex = index;

        // 更新主头像
        if (currentPortrait != null)
        {
            currentPortrait.sprite = characterSprites[index];
        }

        // 更新小队头像
        for (int i = 0; i < partyPortraits.Length; i++)
        {
            if (partyPortraits[i] == null) continue;

            if (i < characterSprites.Length)
            {
                partyPortraits[i].sprite = characterSprites[i];
            }

            bool isActive = (i == index);
            partyPortraits[i].color = isActive ? activeColor : inactiveColor;
            partyPortraits[i].transform.localScale = Vector3.one * (isActive ? activeScale : inactiveScale);
        }
    }
}
