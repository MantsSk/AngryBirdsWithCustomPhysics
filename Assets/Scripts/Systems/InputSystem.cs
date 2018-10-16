using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : ISystemInterface
{
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
                    Debug.Log("fak");
                    entities.positions[i] += entities.moveComponents[i].velocity * deltaTime +
                        0.5f * entities.moveComponents[i].acceleration * deltaTime * deltaTime;

                    var moveComponent = entities.moveComponents[i];
                    moveComponent.velocity += entities.moveComponents[i].acceleration * deltaTime;
                    entities.moveComponents[i] = moveComponent;
                }
            }
        }
    }
}
