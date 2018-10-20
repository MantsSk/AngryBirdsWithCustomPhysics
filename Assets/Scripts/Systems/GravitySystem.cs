using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySystem : ISystemInterface 
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
                entities.AddComponent(new PlayerEntity(i), EntityFlags.kFlagGravity);
            }
        }

        for (var i = 0; i < enemyEntities.enemyFlags.Count; i++)
        {
            if (enemyEntities.enemyFlags[i].HasFlag(EnemyEntityFlags.kFlagPosition))
            {
                enemyEntities.AddEnemyComponent(new EnemyEntity(i), EnemyEntityFlags.kFlagGravity);
            }
        }
    }

    public void Update(World world, float time = 0, float deltaTime = 0)
    {
        var entities = world.entities;
        var enemyEntities = world.enemyEntities;
        var gravity = world.gravity;
        
        for (var i = 0; i < entities.flags.Count; i++)
        {
            if (entities.flags[i].HasFlag(EntityFlags.kFlagGravity) && 
                entities.flags[i].HasFlag(EntityFlags.kFlagForce))
            {
                var forceComponent = entities.forceComponents[i];
                
                // F = m * g
                if (forceComponent.massInverse > 1e-6f)
                    forceComponent.force += gravity / forceComponent.massInverse;
                
                entities.forceComponents[i] = forceComponent;
            }
        }
 
        for (var i = 0; i < enemyEntities.enemyFlags.Count; i++)
        {
            if (enemyEntities.enemyFlags[i].HasFlag(EnemyEntityFlags.kFlagGravity) && 
                enemyEntities.enemyFlags[i].HasFlag(EnemyEntityFlags.kFlagForce))
            {
                var enemyForceComponent = enemyEntities.enemyForceComponents[i];
                // F = m * g
                if (enemyForceComponent.massInverse > 1e-6f)
                    enemyForceComponent.force += gravity / enemyForceComponent.massInverse;
                
                enemyEntities.enemyForceComponents[i] = enemyForceComponent;
            }
        }   
    }
}
