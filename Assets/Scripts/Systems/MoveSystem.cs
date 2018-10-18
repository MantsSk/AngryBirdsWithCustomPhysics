using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSystem : ISystemInterface 
{    
    public void Start(World world)
    {
        var entities = world.entities;
        var enemyEntities = world.enemyEntities;
        
        // add randomized velocity to all entities that have positions
        for (var i = 0; i < entities.flags.Count; i++)
        {
            if (entities.flags[i].HasFlag(EntityFlags.kFlagPosition))
            {
                entities.AddComponent(new PlayerEntity(i), EntityFlags.kFlagMove);
                
                var moveComponent = entities.moveComponents[i];
                moveComponent.velocity = new Vector2(Random.Range(-3f,3f), Random.Range(-3f, 3f));
                entities.moveComponents[i] = moveComponent;
            }
        }
        
        for (var i = 0; i < enemyEntities.enemyFlags.Count; i++)
        {
            if (enemyEntities.enemyFlags[i].HasFlag(EnemyEntityFlags.kFlagPosition))
            {
                enemyEntities.AddEnemyComponent(new EnemyEntity(i), EnemyEntityFlags.kFlagMove);
                
                var enemyMoveComponent = enemyEntities.enemyMoveComponents[i];
                enemyMoveComponent.velocity = new Vector2(Random.Range(-3f,3f), Random.Range(-3f, 3f));
                enemyEntities.enemyMoveComponents[i] = enemyMoveComponent;
            }
        }
    }

    public void Update(World world, float time = 0, float deltaTime = 0)
    {
        var entities = world.entities;
        var enemyEntities = world.enemyEntities;
        
        for (var i = 0; i < entities.flags.Count; i++)
        {
            if (entities.flags[i].HasFlag(EntityFlags.kFlagMove) && world.shouldShoot)
            {
                //pos = pos + v * dt + a * t^2 / 2
                entities.positions[i] += entities.moveComponents[i].velocity * deltaTime +
                    0.5f * entities.moveComponents[i].acceleration * deltaTime * deltaTime;

                var moveComponent = entities.moveComponents[i];
                moveComponent.velocity += entities.moveComponents[i].acceleration * deltaTime;
                entities.moveComponents[i] = moveComponent;
            }
        }
        for (var i = 0; i < enemyEntities.enemyFlags.Count; i++)
        {
            if (enemyEntities.enemyFlags[i].HasFlag(EnemyEntityFlags.kFlagMove) && world.shouldSmash)
            {
                //pos = pos + v * dt + a * t^2 / 2
                enemyEntities.enemyPositions[i] += enemyEntities.enemyMoveComponents[i].velocity * deltaTime +
                    0.5f * enemyEntities.enemyMoveComponents[i].acceleration * deltaTime * deltaTime;

                var enemyMoveComponent = enemyEntities.enemyMoveComponents[i];
                enemyMoveComponent.velocity += enemyEntities.enemyMoveComponents[i].acceleration * deltaTime;
                enemyEntities.enemyMoveComponents[i] = enemyMoveComponent;
            }
        }
    }
}
