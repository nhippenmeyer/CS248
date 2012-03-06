using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Gaia.Core;
using Gaia.Rendering.RenderViews;
using Gaia.SceneGraph.GameEntities;

namespace Gaia.SceneGraph
{
    public class Scene
    {
        public List<Entity> Entities = new List<Entity>();
        PriorityQueue<int, RenderView> RenderViews = new PriorityQueue<int, RenderView>();

        public Light MainLight; //Our sunlight
        
        public RenderView MainCamera;
       
        public Scene()
        {
            Entities.Add(new Player());
            Entities.Add(new Terrain());
            Entities.Add(new Sky());
            MainLight = new Sunlight();
            Entities.Add(MainLight);
            Entities.Add(new Light(LightType.Directional, new Vector3(0.1797f, 0.744f, 1.12f), Vector3.Right, false));
        }

        public void AddRenderView(RenderView view)
        {
            RenderViews.Enqueue(view, (int)view.GetRenderType());
        }

        public void RemoveRenderView(RenderView view)
        {
            RenderViews.RemoveAt((int)view.GetRenderType(), view);
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
            int renderViewCount = RenderViews.Count;
            RenderView[] views = new RenderView[renderViewCount];
            
            for (int i = 0; i < renderViewCount; i++)
            {
                RenderView renderView = RenderViews.ExtractMin();
                for (int j = 0; j < Entities.Count; j++)
                {
                    Entities[j].OnRender(renderView);
                }
                views[i] = renderView;
            }
            for (int i = 0; i < views.Length; i++)
            {
                views[i].Render();
                RenderViews.Enqueue(views[i], (int)views[i].GetRenderType());
            }
        }
    }
}
