using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBoundsSystem : ISystemInterface 
{
    public void Start(World world)
    {
        var entities = world.entities;
        var enemyEntities = world.enemyEntities;
        
        // add world bounds to all entities
        for (var i = 0; i < entities.flags.Count; i++)
        {
            if (entities.flags[i].HasFlag(EntityFlags.kFlagPosition))
            {
                entities.AddComponent(new PlayerEntity(i), EntityFlags.kFlagWorldBounds);
            }
        }

        for (var i = 0; i < enemyEntities.enemyFlags.Count; i++)
        {
            if (enemyEntities.enemyFlags[i].HasFlag(EnemyEntityFlags.kFlagPosition))
            {
                enemyEntities.AddEnemyComponent(new EnemyEntity(i), EnemyEntityFlags.kFlagWorldBounds);
            }
        }
    }

    public void Update(World world, float time = 0, float deltaTime = 0)
    {
        var entities = world.entities;
        var enemyEntities = world.enemyEntities;
        var bounds = world.worldBounds;
        
        for (var i = 0; i < entities.flags.Count; i++)
        {
            if (entities.flags[i].HasFlag(EntityFlags.kFlagWorldBounds) && 
                entities.flags[i].HasFlag(EntityFlags.kFlagMove))
            {
                var radius = 0f;
                var coeffOfRestitution = 1.0f;
                var pos = entities.positions[i];
                var moveComponent = entities.moveComponents[i];

                if (entities.flags[i].HasFlag(EntityFlags.kFlagCollision))
                {
                    radius = entities.collisionComponents[i].radius;
                    coeffOfRestitution = entities.collisionComponents[i].coeffOfRestitution;
                }

                // Apply only if object is leaving horizontal boundaries
                if ((pos.x - radius < bounds.xMin) && (moveComponent.velocity.x < 0f) || 
                    (pos.x + radius > bounds.xMax) && (moveComponent.velocity.x > 0f))
                {
                    moveComponent.velocity.x = -coeffOfRestitution * moveComponent.velocity.x;
                    pos.x = Mathf.Clamp(pos.x, bounds.xMin + radius, bounds.xMax + radius);
                }

                // Apply only if object is leaving vertical boundaries
                if ((pos.y - radius < bounds.yMin) && (moveComponent.velocity.y < 0f) || 
                    (pos.y + radius > bounds.yMax) && (moveComponent.velocity.y > 0f))
                {
                    moveComponent.velocity.y = -coeffOfRestitution * moveComponent.velocity.y;
                    pos.y = Mathf.Clamp(pos.y, bounds.yMin + radius, bounds.yMax - radius);
                }

                entities.moveComponents[i] = moveComponent;
                entities.positions[i] = pos;
            }
        }

        for (var i = 0; i < enemyEntities.enemyFlags.Count; i++)
        {
            if (enemyEntities.enemyFlags[i].HasFlag(EnemyEntityFlags.kFlagWorldBounds) && 
                enemyEntities.enemyFlags[i].HasFlag(EnemyEntityFlags.kFlagMove))
            {
                var enemyRadius = 0f;
                var enemyCoeffOffRestitution = 1.0f;
                var enemyPos = enemyEntities.enemyPositions[i];
                var enemyMoveComponent = enemyEntities.enemyMoveComponents[i];

                if (enemyEntities.enemyFlags[i].HasFlag(EnemyEntityFlags.kFlagCollision))
                {
                    enemyRadius = enemyEntities.enemyCollisionComponents[i].radius;
                    enemyCoeffOffRestitution = enemyEntities.enemyCollisionComponents[i].coeffOfRestitution;
                }

                // Apply only if object is leaving horizontal boundaries
                if ((enemyPos.x - enemyRadius < bounds.xMin) && (enemyMoveComponent.velocity.x < 0f) || 
                    (enemyPos.x + enemyRadius > bounds.xMax) && (enemyMoveComponent.velocity.x > 0f))
                {
                    enemyMoveComponent.velocity.x = -enemyCoeffOffRestitution * enemyMoveComponent.velocity.x;
                    enemyPos.x = Mathf.Clamp(enemyPos.x, bounds.xMin + enemyRadius, bounds.xMax + enemyRadius);
                }

                // Apply only if object is leaving vertical boundaries
                if ((enemyPos.y - enemyRadius < bounds.yMin) && (enemyMoveComponent.velocity.y < 0f) || 
                    (enemyPos.y + enemyRadius > bounds.yMax) && (enemyMoveComponent.velocity.y > 0f))
                {
                    enemyMoveComponent.velocity.y = -enemyCoeffOffRestitution * enemyMoveComponent.velocity.y;
                    enemyPos.y = Mathf.Clamp(enemyPos.y, bounds.yMin + enemyRadius, bounds.yMax - enemyRadius);
                }

                enemyEntities.enemyMoveComponents[i] = enemyMoveComponent;
                enemyEntities.enemyPositions[i] = enemyPos;
            }
        }
    }
}
