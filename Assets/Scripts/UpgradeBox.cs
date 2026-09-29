using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeBox : MonoBehaviour
{
    Upgrade myUpgrade;
    [SerializeField] Image myImage;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] Button buyButton;
    UpgradeManager manager;

    void Awake()
    {
        manager = FindFirstObjectByType<UpgradeManager>();
    }

    void UpdateVisuals()
    {
        nameText.text = myUpgrade.upgradeName;
        myImage.sprite = myUpgrade.icon;
        descriptionText.text = myUpgrade.description;
        buyButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = myUpgrade.price + " Pings";
    }

    public void SetUpgrade(Upgrade newUpgrade)
    {
        myUpgrade = newUpgrade;
        UpdateVisuals();
    }

    public void BuyUpgrade()
    {
        manager.BuyUpgrade(myUpgrade);
    }
}
