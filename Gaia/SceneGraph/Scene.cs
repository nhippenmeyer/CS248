using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Gaia.Rendering.RenderViews;
using Gaia.SceneGraph.GameEntities;

namespace Gaia.SceneGraph
{
    public class Scene
    {
        public List<Entity> Entities = new List<Entity>();
        public List<RenderView> RenderViews = new List<RenderView>();

        public Light MainLight; //Our sunlight

        public Scene()
        {
            Entities.Add(new Player());
            Entities.Add(new Terrain());
            Entities.Add(new Sky());
            MainLight = new Light(LightType.Directional, new Vector3(0.95f, 1.26f, 0.356f), Vector3.Up, true);
            Entities.Add(MainLight);
        }

        public void Initialize()
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                Entities[i].OnAdd(this);
            }
        }

        public void Destroy()
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                Entities[i].OnDestroy();
            }
        }

        public void Update()
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                Entities[i].OnUpdate();
            }
        }

        public void Render()
        {
            for (int i = 0; i < RenderViews.Count; i++)
            {
                for (int j = 0; j < Entities.Count; j++)
                {
                    Entities[j].OnRender(RenderViews[i]);
                }

            }
            for (int i = 0; i < RenderViews.Count; i++)
            {
                RenderViews[i].Render();
            }
        }
    }
}
