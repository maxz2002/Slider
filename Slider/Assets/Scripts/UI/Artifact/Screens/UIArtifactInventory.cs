using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIArtifactInventory : MonoBehaviour
{
    public List<ArtifactInventoryCollectible> collectibles;

    public ArtifactInventoryCollectible anchorCollectible; // the check is player.pickedupanchor
    public ArtifactInventoryCollectible scrollCollectible; // swaps between scrap and scroll
    // boots can just be done with the other collectibles

    public TextMeshProUGUI inventoryText;


    [Header("Special Collectible Counters")] // could be refactored
    public TextMeshProUGUI villagePagesCount;
    public TextMeshProUGUI flashlightCount;

    private void OnEnable() 
    {
        UpdateIcons();
        UpdateText("Inventory");

        UpdateCollectibleCounters(this, null);
        PlayerInventory.OnPlayerGetCollectible += UpdateCollectibleCounters;
    }

    private void OnDisable() 
    {
        PlayerInventory.OnPlayerGetCollectible -= UpdateCollectibleCounters;
    }

    public void UpdateIcons()
    {
        foreach (ArtifactInventoryCollectible c in collectibles)
        {
            c.SetVisible(PlayerInventory.Contains(c.collectibleName));
        }

        anchorCollectible.SetVisible(PlayerInventory.GetHasCollectedAnchor());
        
        scrollCollectible.SetVisible(PlayerInventory.Contains(scrollCollectible.collectibleName));
    }

    public void UpdateText(string text)
    {
        inventoryText.text = text;
    }


    private void UpdateCollectibleCounters(object sender, PlayerInventory.InventoryEvent e)
    {
        int numPages = 0;
        if (PlayerInventory.Contains("Page 1", Area.Village)) numPages += 1;
        if (PlayerInventory.Contains("Page 2", Area.Village)) numPages += 1;
        if (PlayerInventory.Contains("Page 3", Area.Village)) numPages += 1;
        if (PlayerInventory.Contains("Page 4", Area.Village)) numPages += 1;
        
        villagePagesCount.text = numPages.ToString();
        villagePagesCount.gameObject.SetActive(numPages > 1);

        
        int numFlashlight = 0;
        if (PlayerInventory.Contains("Flashlight", Area.Village)) numFlashlight += 1;
        if (PlayerInventory.Contains("Flashlight", Area.Caves))   numFlashlight += 1;
        
        flashlightCount.text = numFlashlight.ToString();
        flashlightCount.gameObject.SetActive(numFlashlight > 1);
    }
}