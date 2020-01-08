using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK.Input;
using OpenGL_Game.Managers;
using OpenGL_Game.Systems;
using OpenGL_Game.Objects;
using OpenGL_Game.Components;

namespace OpenGL_Game.Scenes
{
    class MainMenuScene : Scene
    {
        public MainMenuScene(SceneManager sceneManager) : base(sceneManager)
        {
            // Set the title of the window
            sceneManager.Title = "Main Menu";
            // Set the Render and Update delegates to the Update and Render methods of this class
            sceneManager.renderer = Render;
            sceneManager.updater = Update;

            sceneManager.mouseDelegate += Mouse_BottonPressed;
            sceneManager.keyboardDownDelegate += Keyboard_KeyDown;

        }

        public override void Update(FrameEventArgs e)
        {
        }

        public override void Render(FrameEventArgs e)
        {
            GL.Viewport(0, 0, sceneManager.Width, sceneManager.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, sceneManager.Width, 0, sceneManager.Height, -1, 1);

            //GUI.clearColour = Color.FromArgb(20,20,20);
            GUI.clearColour = Color.Transparent;

            //Display the Title
            float width = sceneManager.Width, height = sceneManager.Height, fontSize = Math.Min(width, height) / 10f;
            GUI.Label(new Rectangle(0, (int)(fontSize / 2f), (int)width, (int)(fontSize * 2f)), "Main Menu", (int)fontSize, StringAlignment.Center);
            GUI.Label(new Rectangle(0, (int)(fontSize*2f), (int)width, (int)(fontSize * 2f)), "Press any key to play.", (int)(fontSize/1.5f), StringAlignment.Center);

            GUI.Render();
        }

        public void Mouse_BottonPressed(MouseButtonEventArgs e)
        {
            sceneManager.ChangeScene(SceneType.SCENE_GAME);
        }
        public void Keyboard_KeyDown(KeyboardKeyEventArgs e)
        {
            sceneManager.ChangeScene(SceneType.SCENE_GAME);
        }
        public override void Close()
        {
            sceneManager.mouseDelegate -= Mouse_BottonPressed;
            sceneManager.keyboardDownDelegate -= Keyboard_KeyDown;
        }
    }
}