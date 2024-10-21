using UnityEngine;

public class Actor : Entity
{
    [Header("Control")]
    public UnitController controller;
    public GameObject vision_component;

    [Header("Visuals")]
    public SpriteRenderer sprite_propulsion;
    public SpriteRenderer sprite_body;
    public SpriteRenderer sprite_mount;

    // ...

    private void Start()
    {
        AddToGameManager(); // Add to entities list
    }
}
