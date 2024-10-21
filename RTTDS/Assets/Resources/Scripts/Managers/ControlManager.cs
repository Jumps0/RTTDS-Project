using UnityEngine;
using System.Resources;
using System.Collections.Generic;

/// <summary>
/// A broad script for managing player control. NOTE: Can be expanded later for multiplayer
/// </summary>
public class ControlManager : MonoBehaviour
{
    public static ControlManager inst;

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // DEBUG for singleplayer
        CreateNewController();
    }

    [Header("Controllers")]
    public List<UnitController> controllers = new List<UnitController>();
    [SerializeField] private GameObject prefab_controller;

    #region Utilities
    public void CreateNewController()
    {
        GameObject c = Instantiate(prefab_controller, Vector3.zero, Quaternion.identity); // Create a new controller based on the prefab
        c.transform.parent = this.transform; // Assign parent to this so we can track it better

        AddController(c.GetComponent<UnitController>());
    }

    public void AddController(UnitController c)
    {
        controllers.Add(c);
    }

    public void RemoveController(UnitController c)
    {
        controllers.Remove(c);
        Destroy(c.gameObject);
    }
    #endregion
}
