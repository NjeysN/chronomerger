using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x, y;
    
    // CHANGED: Instead of "bool", we store the actual Building script here.
    // If this is null, the tile is empty.
    public Building occupiedBy; 
    
    private SpriteRenderer _renderer;
    
    private void Start() 
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void SetHighlight(bool highlight) 
    {
        if (_renderer == null) return;
        _renderer.color = highlight ? Color.green : Color.white; 
    }
}