using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("Unit Data")]
    public ItemData data;
    public bool isPlaced = false; 

    [Header("Health Stats")]
    public float currentHealth = 50f;
    public float maxHealth = 50f;

    private Collider2D _myCollider;
    private SpriteRenderer _myRenderer;
    private Vector3 _startDragPos;
    private bool _isDragging = false;

    void Start()
    {
        currentHealth = maxHealth;
        _myCollider = GetComponent<Collider2D>();
        _myRenderer = GetComponent<SpriteRenderer>();

        // 1. Load correct sprite immediately
        if (data != null && _myRenderer != null)
        {
            _myRenderer.sprite = data.icon; 
        }

        // 2. Disable collider if dragging from shop
        if (!isPlaced && _myCollider != null)
        {
            _myCollider.enabled = false;
        }
    }

    void Update()
    {
        // 1. Initial Placement Logic (Buying from Shop)
        if (!isPlaced)
        {
            HandleInitialPlacement();
        }
        // 2. Dragging Logic (Moving after placement)
        else if (_isDragging)
        {
            HandleDragging();
        }
    }

    // --- PLACEMENT & DRAGGING LOGIC ---

    void HandleInitialPlacement()
    {
        Vector3 mousePos = GetMouseWorldPos();
        mousePos.z = -1; // Keep visible

        // Visual snap to tile
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit.collider != null && hit.collider.CompareTag("Tile"))
        {
             transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y, -1);
             if (Input.GetMouseButtonDown(0)) PlaceUnit();
        }
        else
        {
             transform.position = mousePos; // Follow loosely
        }
    }

    void HandleDragging()
    {
        Vector3 mousePos = GetMouseWorldPos();
        mousePos.z = -1; 
        transform.position = mousePos;
    }

    // --- MOUSE EVENTS ---

    void OnMouseDown()
    {
        if (isPlaced)
        {
            _isDragging = true;
            _startDragPos = transform.position; // Remember start pos
            _myCollider.enabled = false; // Disable collider to see through self
        }
    }

    void OnMouseUp()
    {
        if (isPlaced && _isDragging)
        {
            _isDragging = false;
            _myCollider.enabled = true; // Re-enable collider
            CheckDropTarget();
        }
    }

    // --- MERGE & SWAP LOGIC ---

    void CheckDropTarget()
    {
        Vector3 mousePos = GetMouseWorldPos();
        // CircleCast to find what we dropped on
        RaycastHit2D[] hits = Physics2D.CircleCastAll(mousePos, 0.1f, Vector2.zero);

        Building targetUnit = null;
        Transform targetTile = null;

        foreach (var hit in hits)
        {
            if (hit.collider.gameObject == gameObject) continue; // Ignore self

            if (hit.collider.CompareTag("Tile"))
            {
                targetTile = hit.transform;
            }
            else if (hit.collider.GetComponent<Building>() != null)
            {
                targetUnit = hit.collider.GetComponent<Building>();
            }
        }

        // CASE 1: Dropped on another Unit
        if (targetUnit != null)
        {
            // A. Upgrade Check: Are we the "Material" for the target?
            if (targetUnit.data.validMergePartner == this.data)
            {
                Debug.Log($"Upgrading {targetUnit.name} using {this.name}!");
                targetUnit.EvolveTo(targetUnit.data.resultItem); 
                Destroy(gameObject); // Consume material
            }
            // B. Upgrade Check: Is the target the "Material" for us?
            else if (this.data.validMergePartner == targetUnit.data)
            {
                Debug.Log($"Upgrading {this.name} using {targetUnit.name}!");
                this.EvolveTo(this.data.resultItem); 
                Destroy(targetUnit.gameObject); // Consume material
            }
            // C. No Match -> Swap Places
            else
            {
                Debug.Log("Swapping Places");
                SwapPlaces(targetUnit);
            }
        }
        // CASE 2: Dropped on an Empty Tile
        else if (targetTile != null)
        {
            transform.position = new Vector3(targetTile.position.x, targetTile.position.y, -1);
        }
        // CASE 3: Dropped in Empty Space
        else
        {
            ReturnToStart(); // Snap back
        }
    }

    public void EvolveTo(ItemData newItemData)
    {
        if (newItemData == null) return;

        // 1. Determine new prefab (Melee or Ranged logic)
        GameObject prefabToSpawn = ShopManager.Instance.unitPrefab;
        // Basic check: if range is short, assume melee prefab
        if (newItemData.range <= 2.5f) prefabToSpawn = ShopManager.Instance.meleePrefab;

        // 2. Spawn new unit
        Vector3 spawnPos = transform.position; 
        spawnPos.z = -1;
        
        GameObject newObj = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        Building newBuilding = newObj.GetComponent<Building>();

        // 3. Setup Data
        if (newBuilding != null)
        {
            newBuilding.data = newItemData;
            newBuilding.isPlaced = true;
            newBuilding.UpdateVisuals();
        }

        // 4. Destroy self (the old T1 unit)
        Destroy(gameObject);
    }

    void SwapPlaces(Building otherUnit)
    {
        Vector3 otherPos = otherUnit.transform.position;
        otherUnit.transform.position = _startDragPos;
        transform.position = otherPos;
    }

    void ReturnToStart()
    {
        transform.position = _startDragPos;
    }

    // --- HELPERS ---

    public void PlaceUnit()
    {
        isPlaced = true;
        if (_myCollider != null) _myCollider.enabled = true;
    }

    public void UpdateVisuals()
    {
        if (data != null && _myRenderer != null)
        {
            _myRenderer.sprite = data.icon;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        p.z = 0;
        return p;
    }
}