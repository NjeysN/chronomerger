using UnityEngine;
using System.Collections.Generic;

public class Buffer : MonoBehaviour
{
    private Building _myBuilding;
    private GridManager _gridManager;
    private List<TurretAI> _buffedNeighbors = new List<TurretAI>();
    
    // How often to check for neighbors (Optimization)
    private float _checkTimer; 

    void Start()
    {
        _myBuilding = GetComponent<Building>();
        _gridManager = FindFirstObjectByType<GridManager>();
    }

    void Update()
    {
        // Only work if placed on the grid and IS a buffer
        if (_myBuilding == null || !_myBuilding.isPlaced) return;
        if (!_myBuilding.data.isBuffer) return;

        _checkTimer += Time.deltaTime;
        
        // Check neighbors every 0.5 seconds (Checking every frame is bad for performance)
        if (_checkTimer > 0.5f)
        {
            ApplyBuffsToNeighbors();
            _checkTimer = 0;
        }
    }

    void ApplyBuffsToNeighbors()
    {
        // 1. First, find out where I am
        Tile myTile = _gridManager.GetTileAtPosition(transform.position);
        if (myTile == null) return;

        // 2. Clear old buffs (In case neighbors moved/died)
        // Note: In a complex game, we'd subtract the buff. 
        // For a Jam, we just tell neighbors to "Reset" then re-apply.
        foreach (TurretAI neighbor in _buffedNeighbors)
        {
            if (neighbor != null) neighbor.ResetStats();
        }
        _buffedNeighbors.Clear();

        // 3. Get my Stats from Data
        float dmgMult = _myBuilding.data.damageMultiplier;
        float spdMult = _myBuilding.data.speedMultiplier;

        // 4. Check all 4 directions (Up, Down, Left, Right)
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        foreach (Vector2 dir in directions)
        {
            // Math to find neighbor tile
            Vector2 neighborPos = (Vector2)transform.position + (dir * _gridManager.cellSize);
            Tile neighborTile = _gridManager.GetTileAtPosition(neighborPos);

            if (neighborTile != null && neighborTile.occupiedBy != null)
            {
                // Found a building! Is it a Turret?
                TurretAI turret = neighborTile.occupiedBy.GetComponent<TurretAI>();
                if (turret != null)
                {
                    // BUFF IT!
                    turret.ApplyBuff(dmgMult, spdMult);
                    _buffedNeighbors.Add(turret); // Remember we buffed this guy
                }
            }
        }
    }
}