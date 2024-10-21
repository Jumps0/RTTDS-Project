using UnityEngine;

/// <summary>
/// A generic class used to represent any actionable entity and store its key information.
/// </summary>
public class Entity : MonoBehaviour
{
    [Header("Values")]
    public string uniqueName;
    // ...


    public void AddToGameManager()
    {
        GameManager.inst.Entities.Add(this);
    }
}
