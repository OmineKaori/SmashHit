using UnityEngine;

[CreateAssetMenu(fileName="New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public new string name = "New Item";
    
    public Canvas itemCanvas;
    
    public Material usedItemMaterial;
    
    public int point = 3;
    
    public AudioClip itemCrackingSound;
}