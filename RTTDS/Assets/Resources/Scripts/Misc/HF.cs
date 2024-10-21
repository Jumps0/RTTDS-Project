using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Contains helper functions to be used globally.
/// </summary>
public static class HF
{
    #region Vector Conversions
    public static Vector2Int V3_to_V2I(Vector3 v3)
    {
        return new Vector2Int(Mathf.RoundToInt(v3.x), Mathf.RoundToInt(v3.y));
    }
    #endregion
}
