using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;



using Gaia.Core;
using Gaia.Input;
using Gaia.Rendering;
using Gaia.Resources;
using Gaia.SceneGraph;

namespace Gaia
{
    public class GameCore : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        Scene mainScene; //Our default level

        public GameCore()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            new GFX(this.GraphicsDevice);
            new InputManager();
            new ResourceManager();
            ResourceManager.Inst.LoadResources();

            mainScene = new Scene();
            mainScene.Initialize();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Time.GameTime.Elapse(gameTime.ElapsedGameTime.Milliseconds);
            Time.GameTime.DT = (float)gameTime.ElapsedGameTime.Ticks / (float)TimeSpan.TicksPerSecond;
            //Update functions here
            InputManager.Inst.Update();
            if(InputManager.Inst.IsKeyDown(GameKey.Pause))
                this.Exit();

            mainScene.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GFX.Inst.AdvanceSimulations((float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
            mainScene.Render();

            base.Draw(gameTime);
        }
    }
}
