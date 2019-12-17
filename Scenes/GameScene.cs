using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenGL_Game.Components;
using OpenGL_Game.Systems;
using OpenGL_Game.Managers;
using OpenGL_Game.Objects;
using System.Drawing;
using System;
using OpenTK.Audio.OpenAL;

namespace OpenGL_Game.Scenes
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    partial class GameScene : Scene
    {
        public static float dt = 0;
        EntityManager entityManager;
        SystemManager systemManager;

        public Camera camera;
        public GameSpecific_CollisionManager collisionManager;

        public static GameScene gameInstance;

        public GameScene(SceneManager sceneManager) : base(sceneManager)
        {
            sceneManager.CursorVisible = false;
            sceneManager.CursorGrabbed = true;
            gameInstance = this;
            entityManager = new EntityManager();
            systemManager = new SystemManager();

            // Set the title of the window
            sceneManager.Title = "Game";
            // Set the Render and Update delegates to the Update and Render methods of this class
            sceneManager.renderer = Render;
            sceneManager.updater = Update;
            // Set Keyboard events to go to a method in this class
            sceneManager.keyboardDownDelegate += Keyboard_KeyDown;
            sceneManager.keyboardUpDelegate += Keyboard_KeyUp;
            // Set mouse events methods
            //sceneManager.mouseMoveDelegate += MouseMove;


            // Enable Depth Testing
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            // Set Camera
            camera = new Camera(new Vector3(22, 1.8f, 22), new Vector3(0, 1.8f,0), (float)(sceneManager.Width) / (float)(sceneManager.Height), 0.1f, 100f);

            //Set collision manager
            collisionManager = new GameSpecific_CollisionManager();

            CreateEntities();
            CreateSystems();

            // TODO: Add your initialization logic here

            //mousePosInit
            //mouseLastX = Mouse.GetState().X;
        }

        

        private void CreateEntities()
        {
            Entity newEntity;

            newEntity = new Entity("Moon");
            newEntity.AddComponent(new ComponentTransform(new Vector3(0,0,7), new Vector3(0,MathHelper.DegreesToRadians(0),0), new Vector3(1,1,1)));
            newEntity.AddComponent(new ComponentGeometry("Geometry/Moon/moon.obj"));
            newEntity.AddComponent(new ComponentCollisionSphere(2f));
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("Landing_Ship");
            newEntity.AddComponent(new ComponentTransform(new Vector3(24,0,24), new Vector3(0,MathHelper.DegreesToRadians(-225),0), new Vector3(1)));
            newEntity.AddComponent(new ComponentGeometry("Geometry/LandingShip/spaceship.obj"));
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("Landing_Ship_Door");
            newEntity.AddComponent(new ComponentTransform(new Vector3(24, 4.32f, 24), new Vector3(0, MathHelper.DegreesToRadians(-225), 0.6f), new Vector3(1)));
            newEntity.AddComponent(new ComponentGeometry("Geometry/LandingShip/spaceship_door.obj"));
            newEntity.AddComponent(new ComponentCollisionSphere(2f));
            ComponentAudio audioComp = new ComponentAudio("Audio/drill_looped_mono.wav");
            audioComp.Stop();
            newEntity.AddComponent(audioComp);
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("Pickup_1");
            newEntity.AddComponent(new ComponentTransform(new Vector3(-22, 1, 22), new Vector3(0, 0, MathHelper.DegreesToRadians(15)), new Vector3(1, 1, 1)));
            newEntity.AddComponent(new ComponentGeometry("Geometry/Pickup/pickup.obj"));
            newEntity.AddComponent(new ComponentCollisionSphere(1));
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("Pickup_2");
            newEntity.AddComponent(new ComponentTransform(new Vector3(22, 1, -22), new Vector3(0, 0, MathHelper.DegreesToRadians(15)), new Vector3(1, 1, 1)));
            newEntity.AddComponent(new ComponentGeometry("Geometry/Pickup/pickup.obj"));
            newEntity.AddComponent(new ComponentCollisionSphere(1));
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("Pickup_3");
            newEntity.AddComponent(new ComponentTransform(new Vector3(-22, 1, -22), new Vector3(0, 0, MathHelper.DegreesToRadians(15)), new Vector3(1, 1, 1)));
            newEntity.AddComponent(new ComponentGeometry("Geometry/Pickup/pickup.obj"));
            newEntity.AddComponent(new ComponentCollisionSphere(1));
            entityManager.AddEntity(newEntity);

            //newEntity = new Entity("Intergalactic_Spaceship");
            //newEntity.AddComponent(new ComponentPosition(0, 0, 0));
            //newEntity.AddComponent(new ComponentGeometry("Geometry/Intergalactic_Spaceship/Intergalactic_Spaceship.obj"));
            //newEntity.AddComponent(new ComponentVelocity(0, 0, 0.2f));
            //newEntity.AddComponent(new ComponentAudio("Audio/buzz.wav"));
            //entityManager.AddEntity(newEntity);


            Vector2 colliderStartPos = new Vector2(14.5f, 24.5f);
            Vector2 colliderEndPos = new Vector2(-14.5f, 24.5f);

            newEntity = new Entity("Maze");
            newEntity.AddComponent(new ComponentTransform(0, 0, 0));
            newEntity.AddComponent(new ComponentGeometry("Geometry/Maze2/maze.obj"));
            newEntity.AddComponent(new ComponentCollisionLine(new Vector2(colliderStartPos.X, colliderStartPos.Y), new Vector2(colliderEndPos.X, colliderEndPos.Y)));//Across the middle of the maze.
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("StartPosition");
            newEntity.AddComponent(new ComponentTransform(new Vector3(colliderStartPos.X, 1f, colliderStartPos.Y), new Vector3(0), new Vector3(0.05f)));
            newEntity.AddComponent(new ComponentGeometry("Geometry/Moon/moon.obj"));
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("EndPosition");
            newEntity.AddComponent(new ComponentTransform(new Vector3(colliderEndPos.X, 1f, colliderEndPos.Y), new Vector3(0), new Vector3(0.05f)));
            newEntity.AddComponent(new ComponentGeometry("Geometry/Moon/moon.obj"));
            entityManager.AddEntity(newEntity);

        }

        private void CreateSystems()
        {
            ISystem newSystem;

            newSystem = new SystemRender();
            systemManager.AddSystem(newSystem);
            newSystem = new SystemPhysics();
            systemManager.AddSystem(newSystem);
            newSystem = new SystemAudio();
            systemManager.AddSystem(newSystem);
            newSystem = new SystemCollisionCameraSphere(collisionManager, camera);
            systemManager.AddSystem(newSystem);
        }

        //Persistent variables (cross-update variables):
        bool[] keysPressed = new bool[255];
        
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="e">Provides a snapshot of timing values.</param>
        public override void Update(FrameEventArgs e)
        {
            dt = (float)e.Time;
            collisionManager.ProcessCollisions();

            systemManager.ActionSystems(entityManager);
            //System.Console.WriteLine("fps=" + (int)(1.0/dt));

            //keyboard stuff:
            if(keysPressed[(char)Key.M])
                sceneManager.ChangeScene(SceneType.SCENE_GAME_OVER);
            if (GamePad.GetState(1).Buttons.Back == ButtonState.Pressed)
                sceneManager.Exit();

            HandleCameraMovement();



            AL.Listener(ALListener3f.Position, ref camera.cameraPosition);
            AL.Listener(ALListenerfv.Orientation, ref camera.cameraDirection, ref camera.cameraUp);

            AnimatePickups();

            HandleDoorPortal();            

        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="e">Provides a snapshot of timing values.</param>
        public override void Render(FrameEventArgs e)
        {
            GL.Viewport(0, 0, sceneManager.Width, sceneManager.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            // Action ALL systems
            systemManager.ActionRenderSystems(entityManager);

            // Render score
            float width = sceneManager.Width, height = sceneManager.Height, fontSize = Math.Min(width, height) / 10f;
            GUI.clearColour = Color.Transparent;
            GUI.Label(new Rectangle(0, 0, (int)width, (int)(fontSize * 2f)), $"Keys Collected: {pickupsCollected}/3  {doorState}", 18, StringAlignment.Near, Color.White);
            GUI.Render();
        }

        /// <summary>
        /// This is called when the game exits.
        /// </summary>
        public override void Close()
        {
            sceneManager.keyboardDownDelegate -= Keyboard_KeyDown;
            sceneManager.keyboardUpDelegate -= Keyboard_KeyUp;
            
            ResourceManager.RemoveAllAssets();
            entityManager.DisposeEntities();


            sceneManager.CursorVisible = true;
            sceneManager.CursorGrabbed = false;
        }

        private bool keyDown = false;
        public void Keyboard_KeyDown(KeyboardKeyEventArgs e)
        {
            keysPressed[(char)e.Key] = true;

            if (e.Key == Key.F)
            {
                pickupsCollected++;
            }
            if(e.Key == Key.P)
            {
                Console.WriteLine($"Pos: {camera.cameraPosition.X} {camera.cameraPosition.Z}");
            }

            if(e.Key == Key.Q && !keyDown)
            {
                keyDown = true;
                camera.MoveForward(0.1f);
            }
        }
        public void Keyboard_KeyUp(KeyboardKeyEventArgs e)
        {
            keysPressed[(char)e.Key] = false;

            if(e.Key == Key.Q && keyDown)
            {
                keyDown = false;
            }
        }

        int mouseLastX = Mouse.GetState().X;
        private void HandleCameraMovement()
        {
            float deltaX = Mouse.GetState().X - mouseLastX;
            float camRotation = deltaX * 0.001f;
            camera.RotateY(camRotation);
            mouseLastX = Mouse.GetState().X;

            if (keysPressed[(char)Key.Up] || keysPressed[(char)Key.W])
                camera.MoveForward(0.1f);
            if (keysPressed[(char)Key.Down] || keysPressed[(char)Key.S])
                camera.MoveForward(-0.1f);
            if (keysPressed[(char)Key.Left] || keysPressed[(char)Key.A])
                camera.RotateY(-0.01f);
            if (keysPressed[(char)Key.Right] || keysPressed[(char)Key.D])
                camera.RotateY(0.01f);
        }
    }
}
