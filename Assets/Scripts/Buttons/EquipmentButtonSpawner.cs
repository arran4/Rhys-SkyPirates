using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentButtonSpawner : MonoBehaviour
{
    public RectTransform ScrollSpace;
    public GameObject Viewport;
    public Inventory PublicItems;
    public Button Prefab;
    private List<Button> Inventorylist;
    void Start()
    {
        EventManager.OnItemSelect += SpawnButtons;
        Inventorylist = new List<Button>();
    }

    // Update is called once per frame
    public void SpawnButtons(Item Search)
    {
        for (int x = 0; x < Inventorylist.Count; x++)
        {
            Destroy(Inventorylist[x].gameObject);
        }
        List<Item> SearchResults = new List<Item>();
        Inventorylist = new List<Button>();
        foreach(Item x in PublicItems.InInventory)
        {
            if(x.Type == Search.Type)
            {
                SearchResults.Add(x);
            }
        }

        for (int x = 0; x < SearchResults.Count; x++)
        {
            Button Generated = CreateButton(SearchResults[x], Search);
            Generated.GetComponentInChildren<Text>().text = SearchResults[x].Name;
            Generated.gameObject.transform.position = new Vector3(Generated.gameObject.transform.position.x, ScrollSpace.rect.height - (x *(ScrollSpace.rect.height /6)), 0);
            Inventorylist.Add(Generated);
        }
    }


    public Button CreateButton(Item item, Item Equipment)
    {
        var button = Object.Instantiate(Prefab, Vector3.zero, Quaternion.identity) as Button;
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        rectTransform.SetParent(Viewport.transform);
        rectTransform.anchorMax = this.GetComponent<RectTransform>().anchorMax;
        rectTransform.anchorMin = this.GetComponent<RectTransform>().anchorMin;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
        InventroyItemButton newScript = button.gameObject.AddComponent<InventroyItemButton>();
        newScript.Equip = item;
        button.onClick.AddListener(newScript.onClick);

        rectTransform.sizeDelta = new Vector2((float)ScrollSpace.rect.width, (float)ScrollSpace.rect.height / 6);
        return button;
    }

    public void OnDestroy()
    {
        EventManager.OnItemSelect -= SpawnButtons;
    }

}
