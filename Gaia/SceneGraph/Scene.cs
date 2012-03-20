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
        public List<Actor> Actors = new List<Actor>();
        PriorityQueue<int, RenderView> RenderViews = new PriorityQueue<int, RenderView>();

        public Light MainLight; //Our sunlight
        
        public RenderView MainCamera;

        public Player MainPlayer;

        public Terrain MainTerrain;

        BoundingBox sceneDimensions;
       
        public Scene()
        {
            InitializeScene();
        }

        public BoundingBox GetSceneDimensions()
        {
            return sceneDimensions;
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

        void DetermineSceneDimensions()
        {
            sceneDimensions.Max = Vector3.One * float.NegativeInfinity;
            sceneDimensions.Min = Vector3.One * float.PositiveInfinity;
            for (int i = 0; i < Entities.Count; i++)
            {
                BoundingBox bounds = Entities[i].Transformation.GetBounds();
                sceneDimensions.Min = Vector3.Min(sceneDimensions.Min, bounds.Min);
                sceneDimensions.Max = Vector3.Max(sceneDimensions.Max, bounds.Max);
            }
        }

        void InitializeScene()
        {
            Vector3[] directions = new Vector3[] { Vector3.Left, Vector3.Right, Vector3.Forward, Vector3.Backward, Vector3.Up };
            foreach (Vector3 dir in directions)
            {
                Entities.Add(new Opponent(dir * 0.5f));
            }
            Entities.Add(new Sky());
            MainLight = new Sunlight();
            MainTerrain = new Terrain();
            MainPlayer = new Player(Vector3.Up * 0.25f);

            Entities.Add(MainPlayer);
            Entities.Add(MainTerrain);
            Entities.Add(MainLight);
            for (int i = 0; i < 100; i++)
            {
                Tree tree = new Tree();
                tree.setNum(i % 8);
                Entities.Add(tree);
            }

            Random randomHelper = new Random();
            for (int i = 0; i < 20; i++)
            {     
                Entities.Add(new Gem(randomHelper));
                Entities.Add(new Health(randomHelper));
            }

            Entities.Add(new Light(LightType.Directional, new Vector3(0.1797f, 0.744f, 1.12f), Vector3.Right, false));
        }

        public void Update()
        {
            DetermineSceneDimensions();

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
