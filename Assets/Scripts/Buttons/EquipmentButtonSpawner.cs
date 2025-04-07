using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentButtonSpawner : MonoBehaviour
{
    public GameObject Viewport;
    public Inventory PublicItems;
    public Button Prefab;
    void Start()
    {
        EventManager.OnItemSelect += SpawnButtons;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnButtons(ItemType Search)
    {
        List<Item> SearchResults = new List<Item>();

        foreach(Item x in PublicItems.InInventory)
        {
            if(x.Type == Search)
            {
                SearchResults.Add(x);
            }
        }

        for (int x = 0; x < SearchResults.Count; x++)
        {
            Button Generated = CreateButton();
            Generated.GetComponentInChildren<Text>().text = SearchResults[x].Name;
            Generated.gameObject.transform.position = new Vector3(x * 30, Generated.gameObject.transform.position.y, 0);
        }
    }


    public Button CreateButton()
    {
        var button = Object.Instantiate(Prefab, Vector3.zero, Quaternion.identity) as Button;
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        rectTransform.SetParent(Viewport.transform);
      //  rectTransform.anchorMax = cornerTopRight;
      //  rectTransform.anchorMin = cornerBottomLeft;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
        return button;
    }

    public void OnDestroy()
    {
        EventManager.OnItemSelect -= SpawnButtons;
    }

}
