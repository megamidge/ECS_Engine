using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenGL_Game.Scenes;

using OpenTK.Audio;

namespace OpenGL_Game.Managers
{
    class SceneManager : GameWindow
    {
        Scene scene;
        public static int width = 800, height = 450;
        public static float deltaTime;

        public delegate void SceneDelegate(FrameEventArgs e);
        public SceneDelegate renderer;
        public SceneDelegate updater;

        public delegate void KeyboardDelegate(KeyboardKeyEventArgs e);
        public KeyboardDelegate keyboardDownDelegate;
        public KeyboardDelegate keyboardUpDelegate;

        public delegate void MouseDelegate(MouseButtonEventArgs e);
        public MouseDelegate mouseDelegate;

        public delegate void MouseMoveDelegate(MouseMoveEventArgs e);
        public MouseMoveDelegate mouseMoveDelegate;

        AudioContext audioContext;
        public SceneManager() : base(width, height, new OpenTK.Graphics.GraphicsMode(new OpenTK.Graphics.ColorFormat(8, 8, 8, 8), 16))
        {
            audioContext = new AudioContext();
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape) Exit();
            if (keyboardDownDelegate != null) keyboardDownDelegate.Invoke(e);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (keyboardUpDelegate != null) keyboardUpDelegate.Invoke(e);

            if (e.Key == Key.Number1)
                Systems.SystemAI.Toggle();
            if (e.Key == Key.Number2)
                Systems.SystemCollisionCamera.Toggle();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if(mouseDelegate != null) mouseDelegate.Invoke(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            if (mouseMoveDelegate != null) mouseMoveDelegate.Invoke(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            //Load the GUI
            GUI.SetUpGUI(width, height);


            StartMenu();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            deltaTime = (float)e.Time;
            
            updater(e);
        }

        public double time = 0;
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            time += e.Time;
            base.OnRenderFrame(e);
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1);

            renderer(e);

            GL.Flush();
            SwapBuffers();
        }
        int LoadShaders(string vertex, string fragment)
        {
            int address = GL.CreateProgram();
            string vertStr = System.IO.File.ReadAllText(vertex);
            string fragStr = System.IO.File.ReadAllText(fragment);
            int vertID = GL.CreateShader(ShaderType.VertexShader);
            int fragID = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(vertID, vertStr);
            GL.CompileShader(vertID);
            GL.AttachShader(address, vertID);
            Console.WriteLine(GL.GetShaderInfoLog(vertID));
            GL.ShaderSource(fragID, fragStr);
            GL.CompileShader(fragID);
            GL.AttachShader(address, fragID);
            Console.WriteLine(GL.GetShaderInfoLog(fragID));
            Console.WriteLine(GL.GetShaderInfoLog(address));
            return address;
        }


        public void ChangeScene(SceneType sceneType)
        {
            //Not a fan of this, would prefer a method that handles it dynamically or something. This seems very limited, ie only set scene types can exist, adding extra scenes means changing lots of code.
            if (scene != null) scene.Close();
            switch (sceneType)
            {
                case SceneType.SCENE_GAME:
                    scene = new GameScene(this);
                    break;
                case SceneType.SCENE_MAIN_MENU:
                    scene = new MainMenuScene(this);
                    break;
                case SceneType.SCENE_GAME_OVER:
                    scene = new GameOverScene(this);
                    break;
                case SceneType.SCENE_NONE:
                    scene = null;
                    break;
            }
        }

        public void StartNewGame()
        {
            if(scene != null) scene.Close();
            scene = new GameScene(this);
        }

        public void StartMenu()
        {
            if (scene != null) scene.Close();
            scene = new MainMenuScene(this);
        }

        public static int WindowWidth
        {
            get { return width; }
        }

        public static int WindowHeight
        {
            get { return height; }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            SceneManager.width = Width;
            SceneManager.height = Height;

            //Load the GUI
            GUI.SetUpGUI(Width, Height);
        }
    }

}

