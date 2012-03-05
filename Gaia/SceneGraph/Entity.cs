using System;
using System.Collections.Generic;

using Gaia.Core;
using Gaia.Rendering.RenderViews;
namespace Gaia.SceneGraph
{
    public abstract class Entity
    {
        protected Scene scene;

        public Transform Transformation = new Transform();

        public virtual void OnAdd(Scene scene) { this.scene = scene; }
        public virtual void OnDestroy() { }

        public virtual void OnUpdate() { }
        public virtual void OnRender(RenderView view) { }
    }
}
