using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;

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
    }

    [Header("Entities")]
    public List<Entity> entities = new List<Entity>();
    public List<Entity> Entities { get => entities; }

    [Header("Prefabs")]
    public GameObject prefab_vision;
}
