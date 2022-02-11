using UnityEngine;
using UnityEngine.UI;

public enum UpgradeType {
    UnlockFrenzy, UnlockShield, UnlockDash,
    FrenzyTime, ShieldTime, DashTime,
    SpeedUp, Vision, Lucky, RhythmMode
}


public class Upgrades : MonoBehaviour 
{
    public int cost = 0;
    public UpgradeType type;
    public Image toggledImage;
    public TMPro.TextMeshProUGUI costText;
    public string saveDataString;

    private bool isBought = false;
    private GameManager gameManager;
    public bool startToggleOn = false;
    private bool toggledOn = false;

    public GameObject[] unlockUpgrades;
    

    private void Start() {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        isBought = UpdateProgress();

        if (isBought) {
          
            costText.text = "On";
            toggledImage.enabled = true;
            toggledOn = true;
            //GameManager.Instance.BuyUpgrade(0, type);

            foreach (GameObject up in unlockUpgrades) {
                up.SetActive(true);
            }
        } else {
            costText.text = cost.ToString();
            toggledImage.enabled = false;
            toggledOn = false;

            foreach (GameObject up in unlockUpgrades) {
                up.SetActive(false);
            }
        }
    }

    public void Bought() {
        if (isBought) {
            Toggle();
            return;
        }

        //if (!GameManager.Instance.BuyUpgrade(cost, type)) return;

        foreach (GameObject up in unlockUpgrades) {
            up.SetActive(true);
        }

        Toggle();
        isBought = true;    
    }

    private void Toggle() {
 
        if (toggledOn) {
            costText.text = "Off";
            toggledImage.enabled = false;
            //GameManager.Instance.SetUpgrade(type, false);
            toggledOn = false;
        } else {
            costText.text = "On";
            toggledImage.enabled = true;
            //GameManager.Instance.SetUpgrade(type, true);
            toggledOn = true;
        }        
    }

    public bool UpdateProgress() {
        int intBool = PlayerPrefs.GetInt(saveDataString);
        if (intBool > 0) return true;

        return false;
    }
}
