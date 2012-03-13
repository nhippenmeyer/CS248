using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gaia.Rendering.Geometry
{
    public class GrassGeometry
    {
        VertexBuffer grassGeometryHigh;
        IndexBuffer grassIndicesHigh;

        VertexBuffer grassGeometryMedium;
        VertexBuffer grassIndicesMedium;

        VertexBuffer grassGeometryLow;
        VertexBuffer grassIndicesLow;

        public GrassGeometry()
        {

        }


        void CreateGrass(int numSubdivisions, VertexBuffer vb, IndexBuffer ib)
        {
            for (int i = 0; i <= numSubdivisions; i++)
            {

            }
        }
    }
}
