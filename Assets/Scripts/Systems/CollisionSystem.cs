using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSystem : ISystemInterface
{
    private Vector2[] velocityCache;
    private Vector2[] enemyVelocityCache;
    private bool[] enemyRenderCache;
    private bool[] enemyMoveCache;
     
    public void Start(World world)
    {
        var entities = world.entities;
        var enemyEntities = world.enemyEntities;
        velocityCache = new Vector2[entities.flags.Count];
        enemyVelocityCache = new Vector2[enemyEntities.enemyFlags.Count];
        enemyRenderCache = new bool [enemyEntities.enemyFlags.Count];
        enemyMoveCache = new bool [enemyEntities.enemyFlags.Count];
                
        // add randomized collision radius (derived from mass) and coefficient of restitution
        for (var i = 0; i < entities.flags.Count; i++)
        {
            if (entities.flags[i].HasFlag(EntityFlags.kFlagPosition) &&
                entities.flags[i].HasFlag(EntityFlags.kFlagForce))
            {
                entities.AddComponent(new PlayerEntity(i), EntityFlags.kFlagCollision);
                var collisionComponent = new CollisionComponent();

                if (entities.forceComponents[i].massInverse > 1e-6f)
                    collisionComponent.radius = 0.6f;

                collisionComponent.coeffOfRestitution = Random.Range(0.1f, 0.9f);

                entities.collisionComponents[i] = collisionComponent;
            }
        }

        for (var i = 0; i < enemyEntities.enemyFlags.Count; i++)
        {
            if (enemyEntities.enemyFlags[i].HasFlag(EnemyEntityFlags.kFlagPosition) &&
                enemyEntities.enemyFlags[i].HasFlag(EnemyEntityFlags.kFlagForce))
            {
                enemyEntities.AddEnemyComponent(new EnemyEntity(i), EnemyEntityFlags.kFlagCollision);
                var enemyCollisionComponent = new EnemyCollisionComponent();

                if (enemyEntities.enemyForceComponents[i].massInverse > 1e-6f)
                    enemyCollisionComponent.radius = Random.Range(0.5f, 0.7f);

                enemyCollisionComponent.coeffOfRestitution = Random.Range(0.1f, 0.9f);

                enemyEntities.enemyCollisionComponents[i] = enemyCollisionComponent;
            }
        }

    }

    public static bool CirclesCollide(Vector2 pos1, float r1, Vector2 pos2, float r2)
    {
        // |pos1 - pos2| <= |r1+r2| is the same as
        // (pos1 - pos2)^2 <= (r1+r2)^2
        return (pos2 - pos1).sqrMagnitude <= (r2 + r1) * (r2 + r1);
    }

    // Impulse resolution inspired by:
    // https://gamedevelopment.tutsplus.com/tutorials/how-to-create-a-custom-2d-physics-engine-the-basics-and-impulse-resolution--gamedev-6331
    public void Update(World world, float time = 0, float deltaTime = 0)
    {
        var entities = world.entities;
        var enemyEntities = world.enemyEntities;
        
        // Init velocity cache
        for (var i = 0; i < entities.flags.Count; i++)
        {            
            velocityCache[i] = entities.moveComponents[i].velocity;
        }
            
        for (var i = 0; i < entities.flags.Count; i++)
        {
            // Check all pairs only once
            for (var j = 0; j < enemyEntities.enemyFlags.Count; j++)
            {
                if (entities.flags[i].HasFlag(EntityFlags.kFlagCollision) && 
                    enemyEntities.enemyFlags[j].HasFlag(EnemyEntityFlags.kFlagCollision))
                {
                    var col1 = entities.collisionComponents[i];
                    var col2 = enemyEntities.enemyCollisionComponents[j];

                    var pos1 = entities.positions[i];
                    var pos2 = enemyEntities.enemyPositions[j];

                    if (CirclesCollide(pos1, col1.radius, pos2, col2.radius))
                    {
                        var move1 = entities.moveComponents[i];
                        var move2 = enemyEntities.enemyMoveComponents[j];

                        col2.isDamaged = true;
                        col2.shouldSmash = true;

                        // Relative velocity
                        Vector2 relVel = move2.velocity - move1.velocity;
                        // Collision normal
                        Vector2 normal = (pos2 - pos1).normalized;

                        float velocityProjection = Vector2.Dot(relVel, normal);


                        // Process only if objects are not separating
                        if (velocityProjection < 0)
                        {
                            var force1 = entities.forceComponents[i];
                            var force2 = enemyEntities.enemyForceComponents[j];
                            
                            float cr = Mathf.Min(col1.coeffOfRestitution, col2.coeffOfRestitution);
                            
                            // Impulse scale
                            float impScale = -(1f + cr) * velocityProjection /
                                             (force1.massInverse + force2.massInverse);

                            Vector2 impulse = impScale * normal;

                            velocityCache[i] -= force1.massInverse * (impulse*0.90f); // i need to make seperate velocity caches
                            enemyVelocityCache[j] += force2.massInverse * impulse;
                            enemyRenderCache[j] = col2.isDamaged;
                            enemyMoveCache[j] = col2.shouldSmash;
                        }        
                    } 
                }
            }
            
        }

        // Apply cached velocities and renders
        for (var i = 0; i < entities.flags.Count; i++)
        {
            var move1 = entities.moveComponents[i];
            move1.velocity = velocityCache[i];
            entities.moveComponents[i] = move1;
            velocityCache[i] = Vector2.zero;
        }

        for (var i = 0; i < enemyEntities.enemyFlags.Count; i++)
        {    
            var move2 = enemyEntities.enemyMoveComponents[i];
            move2.velocity = enemyVelocityCache[i];
            enemyEntities.enemyMoveComponents[i] = move2;
            enemyVelocityCache[i] = Vector2.zero;

            Debug.Log(i + "enemy render cache: " + enemyEntities.enemyCollisionComponents[i].shouldSmash);

            var col = enemyEntities.enemyCollisionComponents[i];
            col.isDamaged = enemyRenderCache[i];
            enemyEntities.enemyCollisionComponents[i] = col;
            enemyRenderCache[i] = false;

            col.shouldSmash = enemyMoveCache[i];
            enemyEntities.enemyCollisionComponents[i] = col;
            enemyMoveCache[i] = false;
        }
    }
}

/*
        //enemy code
        for (var i = 0; i < enemyEntities.enemyFlags.Count; i++)
        {            
          //  enemyVelocityCache[i] = enemyEntities.enemyMoveComponents[i].velocity;
        }
            
        for (var i = 0; i < enemyEntities.enemyFlags.Count; i++)
        {
            // Check all pairs only once
            for (var j = i + 1; j < enemyEntities.enemyFlags.Count; j++)
            {
                if (enemyEntities.enemyFlags[i].HasFlag(EnemyEntityFlags.kFlagCollision) && 
                    enemyEntities.enemyFlags[j].HasFlag(EnemyEntityFlags.kFlagCollision))
                {
                    var enemyCol1 = enemyEntities.enemyCollisionComponents[i];
                    var enemyCol2 = enemyEntities.enemyCollisionComponents[j];

                    var enemyPos1 = enemyEntities.enemyPositions[i];
                    var enemyPos2 = enemyEntities.enemyPositions[j];

                    if (CirclesCollide(enemyPos1, enemyCol1.radius, enemyPos2, enemyCol2.radius))
                    {
                        var enemyMove1 = enemyEntities.enemyMoveComponents[i];
                        var enemyMove2 = enemyEntities.enemyMoveComponents[j];

                        // Relative velocity
                        Vector2 relVel = enemyMove2.velocity - enemyMove1.velocity;
                        // Collision normal
                        Vector2 normal = (enemyPos2 - enemyPos1).normalized;

                        float velocityProjection = Vector2.Dot(relVel, normal);

                        // Process only if objects are not separating
                        if (velocityProjection < 0)
                        {
                            var force1 = enemyEntities.enemyForceComponents[i];
                            var force2 = enemyEntities.enemyForceComponents[j];
                            
                            float cr = Mathf.Min(enemyCol1.coeffOfRestitution, enemyCol1.coeffOfRestitution);
                            
                            // Impulse scale
                            float impScale = -(1f + cr) * velocityProjection /
                                             (force1.massInverse + force2.massInverse);

                            Vector2 impulse = impScale * normal;

                         //   enemyVelocityCache[i] -= force1.massInverse * impulse;
                         //   enemyVelocityCache[j] += force2.massInverse * impulse;
                        }        
                    } 
                }
            }
            
        }

        // Apply cached velocities
        for (var i = 0; i < enemyEntities.enemyFlags.Count; i++)
        {
            var enemyMove1 = enemyEntities.enemyMoveComponents[i];
          //  enemyMove1.velocity = enemyVelocityCache[i];
            enemyEntities.enemyMoveComponents[i] = enemyMove1;
            
           // enemyVelocityCache[i] = Vector2.zero;
        } */