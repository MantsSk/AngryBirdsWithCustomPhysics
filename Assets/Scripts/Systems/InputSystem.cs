using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : ISystemInterface
{
    bool shouldShoot = false;

    public void Start(World world)
    {
        var entities = world.entities;

        for (var i = 0; i < entities.flags.Count; i++)
        {
            if (entities.flags[i].HasFlag(EntityFlags.kFlagPosition))
            {
                Debug.Log("hohoho");
                entities.AddComponent(new Entity(i), EntityFlags.kFlagInput);
            }
        }
    }

    public void Update(World world, float time = 0, float deltaTime = 0)
    {
        var entities = world.entities;
        
        for (var i = 0; i < entities.flags.Count; i++)
        {
            if (entities.flags[i].HasFlag(EntityFlags.kFlagInput))
            {
                if (Input.GetKey(KeyCode.UpArrow)) 
                {
                    entities.positions[i] += new Vector2(0f,0.1f);
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow)) 
                {
                    entities.positions[i] += new Vector2(-0.1f, 0.0f);
                    Debug.Log("key down");
                }
                else if (Input.GetKey(KeyCode.RightArrow)) 
                {
                    entities.positions[i] += new Vector2(0.1f,0.0f);
                }
                else if (Input.GetKey(KeyCode.DownArrow)) 
                {
                    entities.positions[i] += new Vector2(0.0f, -0.1f);
                }
                else if (Input.GetKeyUp(KeyCode.LeftArrow)) 
                {
                    shouldShoot = true;
                }
                if (shouldShoot) {
                    entities.positions[i] += entities.moveComponents[i].velocity * new Vector2(30f,0f) * deltaTime +
                    0.5f * entities.moveComponents[i].acceleration * deltaTime * deltaTime;

                var moveComponent = entities.moveComponents[i];
                moveComponent.velocity += entities.moveComponents[i].acceleration * deltaTime;
                entities.moveComponents[i] = moveComponent;
                }
            }
        }
    }

    public void OnMouseDrag (World world) 
    {
        var entities = world.entities;

        for (var i = 0; i < entities.flags.Count; i++)
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100f);
            Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            entities.positions[i] = objPosition;
        }
    }
}
