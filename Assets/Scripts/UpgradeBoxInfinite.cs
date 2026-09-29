using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradeBoxInfinite : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] Upgrade myUpgrade;
    [SerializeField] Image myArrow;
    [SerializeField] Image nextUpgradeArrow;
    [SerializeField] Image prevArrow;
    Image myIcon;
    Image myImage;
    TextMeshProUGUI myText;
    [SerializeField] float defaultOpacity;
    [SerializeField] float unselectedOpacity;
    [SerializeField] float disabledOpacity;
    InfiniteManager infiniteManager;
    bool selected = false;
    bool unlocked = false;
    bool prevSelected = false;
    UpgradeBoxInfinite matchedBox;
    List<UpgradeBoxInfinite> dependentBoxes = new List<UpgradeBoxInfinite>();
    int myIndex;
    Transform upgradeHolder;
    UpgradeShower shower;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        shower.Show(myUpgrade);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        shower.Hide();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (unlocked && prevSelected)
        {
            if (!selected)
            {
                SetOpacity(defaultOpacity);
                SetUpgradeArrowOpacity(defaultOpacity);
                selected = true;
                infiniteManager.AddUpgrade(myUpgrade);
                matchedBox.Deselect(true);
                foreach (UpgradeBoxInfinite box in dependentBoxes)
                {
                    box.Deselect(false);
                }
            }
            else
            {
                Deselect(false);
            }
        }
    }

    void Awake()
    {
        myIcon = transform.GetChild(0).GetComponent<Image>();
        myText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        myImage = GetComponent<Image>();
        infiniteManager = FindFirstObjectByType<InfiniteManager>();
        upgradeHolder = transform.parent;
        shower = FindFirstObjectByType<UpgradeShower>();
    }

    void Start()
    {
        SetBoxes();
        SetVisuals();
    }

    void Update()
    {
        UpdatePrevSelected();
    }

    void UpdatePrevSelected()
    {
        if (prevArrow == null)
        {
            prevSelected = true;
        }
        else
        {
            if (prevArrow.color.a == defaultOpacity)
            {
                prevSelected = true;
            }
            else
            {
                prevSelected = false;
            }
        }
    }

    void SetBoxes()
    {
        myIndex = transform.GetSiblingIndex();
        int searchStartIndex;
        if (myIndex % 2 == 0)
        {
            matchedBox = upgradeHolder.GetChild(myIndex + 1).GetComponent<UpgradeBoxInfinite>();
            searchStartIndex = myIndex + 2;
        }
        else
        {
            matchedBox = upgradeHolder.GetChild(myIndex - 1).GetComponent<UpgradeBoxInfinite>();
            searchStartIndex = myIndex + 1;
        }
        for (int i = searchStartIndex; i < upgradeHolder.childCount; i++)
        {
            dependentBoxes.Add(upgradeHolder.GetChild(i).GetComponent<UpgradeBoxInfinite>());
        }
    }

    void SetVisuals()
    {
        if (PersData.unlockedUpgrades.Contains(myUpgrade.tag))
        {
            unlocked = true;
        }
        myIcon.sprite = myUpgrade.icon;
        myText.text = myUpgrade.upgradeName;
        if (!unlocked)
        {
            SetOpacity(disabledOpacity);
            if (!PersData.unlockedUpgrades.Contains(matchedBox.GetUpgrade().tag))
            {
                SetUpgradeArrowOpacity(disabledOpacity);
            }
            else
            {
                SetUpgradeArrowOpacity(unselectedOpacity);
            }
        }
        else
        {
            SetOpacity(unselectedOpacity);
            SetUpgradeArrowOpacity(unselectedOpacity);
        }
    }

    void SetUpgradeArrowOpacity(float opacity)
    {
        if (nextUpgradeArrow != null)
        {
            nextUpgradeArrow.color = new Color(nextUpgradeArrow.color.r, nextUpgradeArrow.color.g, nextUpgradeArrow.color.b, opacity);
        }
    }

    void SetOpacity(float opacity)
    {
        myIcon.color = new Color(myIcon.color.r, myIcon.color.g, myIcon.color.b, opacity);
        myImage.color = new Color(myImage.color.r, myImage.color.g, myImage.color.b, opacity);
        myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, opacity);
        myArrow.color = new Color(myArrow.color.r, myArrow.color.g, myArrow.color.b, opacity);
    }

    public void Deselect(bool replaced)
    {
        if (selected)
        {
            SetOpacity(unselectedOpacity);
            selected = false;
            if (!replaced)
            {
                infiniteManager.RemoveUpgrade(myUpgrade);
                SetUpgradeArrowOpacity(unselectedOpacity);
            }
        }
    }

    public Upgrade GetUpgrade()
    {
        return myUpgrade;
    }
}
