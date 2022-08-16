using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModelSpawnButton : MonoBehaviour
{
    private int index;

    public void SetUp(int _index, string itemName)
    {
        index = _index;

        GetComponent<Button>().onClick.AddListener(Click);
        GetComponentInChildren<TextMeshProUGUI>().text = itemName;
    }

    private void Click()
    {
        FindObjectOfType<BonePuzzleManager>().Spawn(index);
        VRPointer.Instance.ToggleOffVRUI();
    }
}
