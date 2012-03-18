using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;

namespace Gaia.Physics
{
    public static class PhysicsHelper
    {
        public static float GravityEarth = -9.80665f;

        public static Body[] PhysicsBodiesVolume(BoundingBox bounds)
        {
            List<Body> BodiesInVolume = new List<Body>();
            for (int i = 0; i < PhysicsSystem.CurrentPhysicsSystem.Bodies.Count; i++)
            {
                if (bounds.Contains(PhysicsSystem.CurrentPhysicsSystem.Bodies[i].Position) != ContainmentType.Disjoint)
                    BodiesInVolume.Add(PhysicsSystem.CurrentPhysicsSystem.Bodies[i]);
            }
            return BodiesInVolume.ToArray();
        }

        public static Vector3 SetMass(float mass, Body body, CollisionSkin collision)
        {
            PrimitiveProperties primitiveProperties =
                new PrimitiveProperties(PrimitiveProperties.MassDistributionEnum.Solid, PrimitiveProperties.MassTypeEnum.Density, mass);

            float junk;
            Vector3 com;
            Matrix it, itCoM;

            collision.GetMassProperties(primitiveProperties, out junk, out com, out it, out itCoM);
            body.BodyInertia = itCoM;
            body.Mass = junk;

            return com;
        }
    }
}
