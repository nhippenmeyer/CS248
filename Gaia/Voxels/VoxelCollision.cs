using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;

using Gaia.Core;
using Gaia.Physics;
using Gaia.Rendering;

namespace Gaia.Voxels
{
    public class VoxelCollision
    {
        TriangleMesh CollisionMesh = null;
        CollisionSkin Collision = null;

        const float CollisionDeleteTimeS = 10; //Ten seconds of idle collision before we delete the mesh
        float CollisionDeleteTime = CollisionDeleteTimeS;

        BoundingBox boundsWorldSpaceCollision;
        Transform transformation;
        VoxelGeometry geometry;

        public VoxelCollision(VoxelGeometry voxel, Transform transform, BoundingBox bounds)
        {
            geometry = voxel;
            transformation = transform;

            boundsWorldSpaceCollision = bounds;
            boundsWorldSpaceCollision.Min = bounds.Min * 1.5f;
            boundsWorldSpaceCollision.Max = bounds.Max * 1.5f;
        }

        public void UpdateCollision()
        {
            if (geometry.CanRender)
            {
                Body[] bodies = PhysicsHelper.PhysicsBodiesVolume(boundsWorldSpaceCollision);
                if(CollisionMesh == null && bodies.Length > 0)
                {
                    GenerateCollisionMesh();
                    CollisionDeleteTime = CollisionDeleteTimeS;
                }
                else if (CollisionMesh != null)
                {
                    if (bodies.Length < 1)
                    {
                        CollisionDeleteTime -= Time.GameTime.ElapsedTime;
                        if (CollisionDeleteTime <= 0)
                        {
                            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.RemoveCollisionSkin(Collision);
                            Collision = null;
                            CollisionMesh = null;
                        }
                    }
                    else
                    {
                        CollisionDeleteTime = CollisionDeleteTimeS;
                    }
                }
            }
        }

        void GenerateCollisionMesh()
        {
            List<Vector3> vertColl = new List<Vector3>();
            
            for (int i = 0; i < geometry.verts.Length; i++)
            {
                vertColl.Add(Vector3.Transform(new Vector3(geometry.verts[i].Position.X, geometry.verts[i].Position.Y, geometry.verts[i].Position.Z), transformation.GetTransform()));
                //vertColl[i] = Vector3.Transform(new Vector3(geometry.verts[i].Position.X, geometry.verts[i].Position.Y, geometry.verts[i].Position.Z), transformation.GetTransform());
            }
            
            int triCount = 0;
            TriangleVertexIndices triIdx = new TriangleVertexIndices(0, 0, 0);
            List<TriangleVertexIndices> triColl = new List<TriangleVertexIndices>();
            for (int i = 0; i < geometry.ib.Length; i++)
            {
                //int index = geometry.ib[i];
                
                //vertColl.Add(Vector3.Transform(new Vector3(geometry.verts[index].Position.X, geometry.verts[index].Position.Y, geometry.verts[index].Position.Z), transformation.GetTransform()));
                switch (triCount)
                {
                    case 0:
                        triIdx.I2 = geometry.ib[i];
                        break;
                    case 1:
                        triIdx.I1 = geometry.ib[i];
                        break;
                    case 2:
                        triIdx.I0 = geometry.ib[i];
                        triCount = -1;
                        triColl.Add(triIdx);
                        triIdx = new TriangleVertexIndices(0, 0, 0);
                        break;
                }
                triCount++;
            }

            CollisionMesh = new TriangleMesh();
            CollisionMesh.CreateMesh(vertColl.ToArray(), triColl.ToArray(), 1500, 0.01f);
            Collision = new CollisionSkin(null);
            Collision.AddPrimitive(CollisionMesh, (int)MaterialTable.MaterialID.NotBouncyRough);
            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(Collision);
        }
    }
}
