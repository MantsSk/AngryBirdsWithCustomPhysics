using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : ISystemInterface
{
    int localThrowValue;

    public void Start(World world)
    {
        var entities = world.entities;

        for (var i = 0; i < entities.flags.Count; i++)
        {
            if (entities.flags[i].HasFlag(EntityFlags.kFlagPosition))
            {
                entities.AddComponent(new PlayerEntity(i), EntityFlags.kFlagInput);
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
                if (Input.GetKey(KeyCode.LeftArrow)) 
                {
                    if (entities.inputComponents[i].throwValue < 100) 
                    {
                        entities.positions[i] -= new Vector2(0.1f, 0.0f);
                        localThrowValue += 1;
                        var throwLocal = entities.inputComponents[i];
                        throwLocal.throwValue += localThrowValue;
                        entities.inputComponents[i] = throwLocal;
                    }
                }
                if (Input.GetKeyUp(KeyCode.LeftArrow)) 
                {
                    world.shouldShoot = true;
                }
                if (world.shouldShoot) 
                {
                    entities.positions[i] += entities.moveComponents[i].velocity * new Vector2(30f,0f) * deltaTime +
                    0.5f * entities.moveComponents[i].acceleration * deltaTime * deltaTime;

                    var moveComponent = entities.moveComponents[i];
                    moveComponent.velocity += entities.moveComponents[i].acceleration * deltaTime;
                    entities.moveComponents[i] = moveComponent;
                }
            }
        }
    }
}
