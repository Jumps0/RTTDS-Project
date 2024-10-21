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
        AddController(new UnitController());
    }

    [Header("Controllers")]
    public List<UnitController> controllers = new List<UnitController>();

    #region Utilities
    public void AddController(UnitController c)
    {
        controllers.Add(c);
    }

    public void RemoveController(UnitController c)
    {
        controllers.Remove(c);
    }
    #endregion
}
