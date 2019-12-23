using OpenGL_Game.Components;
using OpenGL_Game.Managers;
using OpenGL_Game.Objects;
using OpenGL_Game.Systems;
using OpenTK;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;

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

        private NavMesh navMesh;

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
            camera = new Camera(new Vector3(22, 1.8f, 22), new Vector3(0, 1.8f, 0), (float)(sceneManager.Width) / (float)(sceneManager.Height), 0.1f, 100f);
            //camera = new Camera(new Vector3(22, 18.8f, 22), new Vector3(0, 1.8f, 0), (float)(sceneManager.Width) / (float)(sceneManager.Height), 0.1f, 100f);

            //Set collision manager
            collisionManager = new GameSpecific_CollisionManager();

            //Set up navmesh.
            navMesh = new NavMesh(
                new List<NavNode>(){
                    new NavNode("A", new Vector2(-14.5f,-22), new string[]{"B","C","L" }),
                    new NavNode("B", new Vector2(0, -19.5f), new string[]{"M","A","C" }),
                    new NavNode("C", new Vector2(14.5f,-22), new string[]{"B","A","D" }),
                    new NavNode("D", new Vector2(22, -14.5f), new string[]{"C","E","F" }),
                    new NavNode("E", new Vector2(19.5f, 0), new string[]{"D","F","N" }),
                    new NavNode("F", new Vector2(22, 14.5f), new string[]{"D","E","G" }),
                    new NavNode("G", new Vector2(14.5f, 22), new string[]{"F","H","I" }),
                    new NavNode("H", new Vector2(0, 19.5f), new string[]{"G","I","O" }),
                    new NavNode("I", new Vector2(-14.5f, 22), new string[]{"G","J","H" }),
                    new NavNode("J", new Vector2(-22, 14.5f), new string[]{"K","I","L" }),
                    new NavNode("K", new Vector2(-19.5f, 0), new string[]{"J","L","P" }),
                    new NavNode("L", new Vector2(-22, -14.5f), new string[]{"J","K","A" }),
                    new NavNode("M", new Vector2(0, -7.5f), new string[]{"B","P","N","O" }),
                    new NavNode("N", new Vector2(7.5f, 0), new string[]{"E","M","P","O" }),
                    new NavNode("O", new Vector2(0, 7.5f), new string[]{"H","M","N","P" }),
                    new NavNode("P", new Vector2(-7.5f, 0), new string[]{"K","M","N","O" }),
                },
                new List<Cell>()
                {
                    new Cell("C1", new Vector2(-14.5f, -14.5f), new Vector2(-29.5f,-29.5f), new string[]{"A","L" }),
                    new Cell("C2", new Vector2(14.5f, -19.5f), new Vector2(-14.5f,-24.5f), new string[]{"A","B","C" }),
                    new Cell("C3", new Vector2(29.5f, -14.5f), new Vector2(14.5f,-29.5f), new string[]{"D","C" }),
                    new Cell("C4", new Vector2(24.5f, 14.5f), new Vector2(19.5f,-14.5f), new string[]{"D","E","F" }),
                    new Cell("C5", new Vector2(19.5f, 2.5f), new Vector2(7.5f,-2.5f), new string[]{"E","N" }),
                    new Cell("C6", new Vector2(2.5f, -7.5f), new Vector2(-2.5f,-19.5f), new string[]{"B","M" }),
                    new Cell("C7", new Vector2(7.5f, 7.5f), new Vector2(-7.5f,-7.5f), new string[]{"M","N","O","P" }),
                    new Cell("C8", new Vector2(-14.5f, 29.5f), new Vector2(-29.5f,14.5f), new string[]{"I","J" }),
                    new Cell("C9", new Vector2(29.5f, 29.5f), new Vector2(14.5f,14.5f), new string[]{"F","G" }),
                    new Cell("C10", new Vector2(14.5f, 24.5f), new Vector2(-14.5f,19.5f), new string[]{"I","H","G" }),
                    new Cell("C11", new Vector2(2.5f, 19.5f), new Vector2(-2.5f,7.5f), new string[]{"H","O" }),
                    new Cell("C12", new Vector2(-7.5f, 2.5f), new Vector2(-19.5f,-2.5f), new string[]{"P","K" }),
                    new Cell("C13", new Vector2(-19.5f, 14.5f), new Vector2(-24.5f,-14.5f), new string[]{"L","K","J" }),
                }
            );


            CreateEntities();
            CreateSystems();

            // TODO: Add your initialization logic here;
        }



        private void CreateEntities()
        {
            Entity newEntity;

            newEntity = new Entity("SpikyBall_1");
            newEntity.AddComponent(new ComponentTransform(new Vector3(-25, 0.5f, 25)));
            newEntity.AddComponent(new ComponentGeometry("Geometry/SpikyBall/spikyball.obj"));
            newEntity.AddComponent(new ComponentCollisionSphere(0.25f));
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("SpikyBall_2");
            newEntity.AddComponent(new ComponentTransform(new Vector3(-25, 0.5f, -25)));
            newEntity.AddComponent(new ComponentGeometry("Geometry/SpikyBall/spikyball.obj"));
            newEntity.AddComponent(new ComponentCollisionSphere(0.25f));
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("Moon");
            newEntity.AddComponent(new ComponentTransform(new Vector3(-14.5f, 1.8f, -24.5f), new Vector3(0, MathHelper.DegreesToRadians(0), 0), new Vector3(1, 1, 1)));
            newEntity.AddComponent(new ComponentGeometry("Geometry/Moon/moon.obj"));
            newEntity.AddComponent(new ComponentCollisionSphere(2f));
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("Landing_Ship");
            newEntity.AddComponent(new ComponentTransform(new Vector3(24, 0, 24), new Vector3(0, MathHelper.DegreesToRadians(-225), 0), new Vector3(1)));
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

            newEntity = new Entity("Drone");
            newEntity.AddComponent(new ComponentTransform(new Vector3(0), new Vector3(0,0,0), new Vector3(0.75f)));
            newEntity.AddComponent(new ComponentGeometry("Geometry/Drone/drone.obj"));
            newEntity.AddComponent(new ComponentCollisionSphere(1));
            newEntity.AddComponent(new ComponentAI());
            entityManager.AddEntity(newEntity);

            //newEntity = new Entity("Intergalactic_Spaceship");
            //newEntity.AddComponent(new ComponentPosition(0, 0, 0));
            //newEntity.AddComponent(new ComponentGeometry("Geometry/Intergalactic_Spaceship/Intergalactic_Spaceship.obj"));
            //newEntity.AddComponent(new ComponentVelocity(0, 0, 0.2f));
            //newEntity.AddComponent(new ComponentAudio("Audio/buzz.wav"));
            //entityManager.AddEntity(newEntity);


            Vector2[] outsidePoints = new Vector2[]
            {
                new Vector2(14.5f, 24.5f),
                new Vector2(-14.5f, 24.5f),
                new Vector2(-14.5f,29.5f),
                new Vector2(-29.5f,29.5f),
                new Vector2(-29.5f, 14.5f),
                new Vector2(-24.5f, 14.5f),
                new Vector2(-24.5f, -14.5f),
                new Vector2(-29.5f, -14.5f),
                new Vector2(-29.5f, -29.5f),
                new Vector2(-14.5f, -29.5f),
                new Vector2(-14.5f, -24.5f),
                new Vector2(14.5f, -24.5f),
                new Vector2(14.5f, -29.5f),
                new Vector2(29.5f, -29.5f),
                new Vector2(29.5f, -14.5f),
                new Vector2(24.5f, -14.5f),
                new Vector2(24.5f, 14.5f),
                new Vector2(29.5f, 14.5f),
                new Vector2(29.5f, 29.5f),
                new Vector2(14.5f, 29.5f),
                new Vector2(14.5f, 24.5f),
            };
            Vector2[] insidePoints1 = new Vector2[]
            {
                new Vector2(14.5f,19.5f),
                new Vector2(2.5f,19.5f),
                new Vector2(2.5f,7.5f),
                new Vector2(7.5f,7.5f),
                new Vector2(7.5f,2.5f),
                new Vector2(19.5f,2.5f),
                new Vector2(19.5f,14.5f),
                new Vector2(14.5f,14.5f),
                new Vector2(14.5f,19.5f),
            };
            Vector2[] insidePoints2 = new Vector2[]
            {
                new Vector2(-14.5f,19.5f),
                new Vector2(-2.5f,19.5f),
                new Vector2(-2.5f,7.5f),
                new Vector2(-7.5f,7.5f),
                new Vector2(-7.5f,2.5f),
                new Vector2(-19.5f,2.5f),
                new Vector2(-19.5f,14.5f),
                new Vector2(-14.5f,14.5f),
                new Vector2(-14.5f,19.5f),
            };
            Vector2[] insidePoints3 = new Vector2[]
            {
                new Vector2(-14.5f,-19.5f),
                new Vector2(-2.5f,-19.5f),
                new Vector2(-2.5f,-7.5f),
                new Vector2(-7.5f,-7.5f),
                new Vector2(-7.5f,-2.5f),
                new Vector2(-19.5f,-2.5f),
                new Vector2(-19.5f,-14.5f),
                new Vector2(-14.5f,-14.5f),
                new Vector2(-14.5f,-19.5f),
            };
            Vector2[] insidePoints4 = new Vector2[]
            {
                new Vector2(14.5f,-19.5f),
                new Vector2(2.5f,-19.5f),
                new Vector2(2.5f,-7.5f),
                new Vector2(7.5f,-7.5f),
                new Vector2(7.5f,-2.5f),
                new Vector2(19.5f,-2.5f),
                new Vector2(19.5f,-14.5f),
                new Vector2(14.5f,-14.5f),
                new Vector2(14.5f,-19.5f),
            };
            newEntity = new Entity("Maze");
            newEntity.AddComponent(new ComponentTransform(0, 0, 0));
            newEntity.AddComponent(new ComponentGeometry("Geometry/Maze2/maze.obj"));
            newEntity.AddComponent(new ComponentCollisionLine(outsidePoints));
            newEntity.AddComponent(new ComponentCollisionLine(insidePoints1));
            newEntity.AddComponent(new ComponentCollisionLine(insidePoints2));
            newEntity.AddComponent(new ComponentCollisionLine(insidePoints3));
            newEntity.AddComponent(new ComponentCollisionLine(insidePoints4));
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
            newSystem = new SystemCollisionCamera(collisionManager, camera);
            systemManager.AddSystem(newSystem);
            newSystem = new SystemAI(navMesh,camera);
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

            //System.Console.WriteLine("fps=" + (int)(1.0/dt));

            //keyboard stuff:
            if (keysPressed[(char)Key.M])
                sceneManager.ChangeScene(SceneType.SCENE_GAME_OVER);
            if (GamePad.GetState(1).Buttons.Back == ButtonState.Pressed)
                sceneManager.Exit();

            HandleCameraMovement(dt);

            AnimatePickups(dt);

            HandleDoorPortal(dt);

            systemManager.ActionSystems(entityManager);
            collisionManager.ProcessCollisions();

            camera.UpdateView();
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

            List<Cell> cellNames = navMesh.FindCellsForPosition(camera.cameraPosition.Xz);            
            GUI.Label(new Rectangle(0, 40, (int)width, (int)(fontSize * 2f)), $"Current cell: {string.Join(", ",cellNames.ConvertAll(c => c.name))}", 18, StringAlignment.Near, Color.White);

            GUI.Label(new Rectangle(0, 80, (int)width, (int)(fontSize * 2f)), $"Current pos: {camera.cameraPosition.X} {camera.cameraPosition.Z}", 18, StringAlignment.Near, Color.White);
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

            if (e.Key == Key.Q && !keyDown)
            {
                keyDown = true;
                camera.MoveForward(0.1f);
            }
        }
        public void Keyboard_KeyUp(KeyboardKeyEventArgs e)
        {
            keysPressed[(char)e.Key] = false;

            if (e.Key == Key.Q && keyDown)
            {
                keyDown = false;
            }
        }

        int mouseLastX = Mouse.GetState().X;
        private void HandleCameraMovement(float dT)
        {
            float deltaX = Mouse.GetState().X - mouseLastX;
            float camRotation = deltaX * 0.08f;
            camera.RotateY(camRotation * dT);
            mouseLastX = Mouse.GetState().X;

            if (keysPressed[(char)Key.Up] || keysPressed[(char)Key.W])
                camera.MoveForward(6f * dT);
            if (keysPressed[(char)Key.Down] || keysPressed[(char)Key.S])
                camera.MoveForward(-6f * dT);
            if (keysPressed[(char)Key.Left] || keysPressed[(char)Key.A])
                camera.RotateY(-3f * dT);
            if (keysPressed[(char)Key.Right] || keysPressed[(char)Key.D])
                camera.RotateY(3f * dT);

            AL.Listener(ALListener3f.Position, ref camera.cameraPosition);
            AL.Listener(ALListenerfv.Orientation, ref camera.cameraDirection, ref camera.cameraUp);
        }
    }
}
