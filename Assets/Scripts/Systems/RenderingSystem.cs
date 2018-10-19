using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderingSystem : ISystemInterface
{
	private const int BATCH_SIZE = 1000;
	Mesh [] enemyMesh;
	Material [] enemyMaterial; 
	Mesh [] mesh;
	Material [] material;
	
	public void Start(World world)
	{
		//do nothing
		var entities = world.entities;
		var enemyEntities = world.enemyEntities;

		enemyMesh = new Mesh [world.enemyEntities.enemyFlags.Count];
		enemyMaterial = new Material [world.enemyEntities.enemyFlags.Count];
		mesh = new Mesh [world.entities.flags.Count];
		material = new Material [world.entities.flags.Count];
		
		for (var i = 0; i < mesh.Length; i++)
		{
			mesh[i] = world.templateObject.GetComponent<MeshFilter>().sharedMesh;
			material[i] = world.templateObject.GetComponent<Renderer>().sharedMaterial;
		}
		
		for (var i = 0; i < enemyMesh.Length; i++)
		{
			enemyMesh[i] = world.enemyTemplateObject.GetComponent<MeshFilter>().sharedMesh;
			enemyMaterial[i] = world.enemyTemplateObject.GetComponent<Renderer>().sharedMaterial;
		}
	}

	public void Update(World world, float time = 0, float deltaTime = 0)
	{
		var entities = world.entities;
		var enemyEntities = world.enemyEntities;
	
		List<Matrix4x4> transformList = new List<Matrix4x4>();
		List<Matrix4x4> enemyTransformList = new List<Matrix4x4>();

		for (var i = 0; i < mesh.Length; i++)
		{
			var pos = entities.positions[i];
			var mtrx = new Matrix4x4();
			var scale = 2.0f * Vector2.one;

			if (entities.flags[i].HasFlag(EntityFlags.kFlagCollision))
				scale *= entities.collisionComponents[i].radius;
		
			mtrx.SetTRS(pos, Quaternion.Euler(Vector3.zero), scale);
				
			transformList.Add(mtrx);

			// DrawMeshInstanced has limitation of up to 1023(?) items per single call
			if (transformList.Count >= BATCH_SIZE)
			{
				Graphics.DrawMeshInstanced(mesh[i], 0, material[i], transformList);
				transformList.Clear();
			}
			if (transformList.Count > 0) 
			{
				Graphics.DrawMeshInstanced(mesh[i], 0, material[i], transformList);
			}
		}	

		for (var i = 0; i < enemyMesh.Length; i++)
		{
			if (enemyEntities.enemyCollisionComponents[i].isDamaged == true) 
			{
				enemyMesh[i] = world.enemyTemplateSmashedObj.GetComponent<MeshFilter>().sharedMesh;
				enemyMaterial[i] = world.enemyTemplateSmashedObj.GetComponent<Renderer>().sharedMaterial;
			}

			var enemyScale = 2.0f * Vector2.one;
			var enemyPos = enemyEntities.enemyPositions[i];
			var enemyMtrx = new Matrix4x4();
			enemyMtrx.SetTRS(enemyPos, Quaternion.Euler(Vector3.zero), enemyScale);
			enemyTransformList.Add(enemyMtrx);

			if (enemyTransformList.Count >= BATCH_SIZE)
			{
				Graphics.DrawMeshInstanced(enemyMesh[i], 0, enemyMaterial[i], enemyTransformList);
				enemyTransformList.Clear();
			}
			// Remaining objects
			if (enemyTransformList.Count > 0) 
			{
				Graphics.DrawMeshInstanced(enemyMesh[i], 0, enemyMaterial[i], enemyTransformList);
			}
		}
	}
}
