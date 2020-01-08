using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK.Input;
using OpenGL_Game.Managers;

namespace OpenGL_Game.Scenes
{
    class GameOverScene : Scene
    {
        public GameOverScene(SceneManager sceneManager) : base(sceneManager)
        {
            // Set the title of the window
            sceneManager.Title = "Game Over";
            // Set the Render and Update delegates to the Update and Render methods of this class
            sceneManager.renderer = Render;
            sceneManager.updater = Update;

            sceneManager.mouseDelegate += Mouse_BottonPressed;
            sceneManager.keyboardDownDelegate += Keyboard_KeyDown;
        }


        public override void Update(FrameEventArgs e)
        {
            if (GameScene.gameInstance.Score >= 3)
                win = true;
        }

        bool win = false;
        public override void Render(FrameEventArgs e)
        {
            GL.Viewport(0, 0, sceneManager.Width, sceneManager.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, sceneManager.Width, 0, sceneManager.Height, -1, 1);

            GUI.clearColour = Color.Transparent;

            //Display the Title
            float width = sceneManager.Width, height = sceneManager.Height, fontSize = Math.Min(width, height) / 10f;
            GUI.Label(new Rectangle(0, (int)(fontSize / 2f), (int)width, (int)(fontSize * 2f)), "Game Over", (int)fontSize, StringAlignment.Center);

            GUI.Label(new Rectangle(0, (int)(fontSize * 2f), (int)width, (int)(fontSize * 2f)), $"Keys Collected: {GameScene.gameInstance.Score}", (int)(fontSize / 4) * 3, StringAlignment.Center);
            if (win)
                GUI.Label(new Rectangle(0, (int)(fontSize * 3f), (int)width, (int)(fontSize * 2f)), $"Congratulations!", (int)(fontSize/2), StringAlignment.Center);
            else
                GUI.Label(new Rectangle(0, (int)(fontSize * 4f), (int)width, (int)(fontSize * 2f)), $"You lost", (int)(fontSize / 2), StringAlignment.Center);

            GUI.Label(new Rectangle(0, (int)(fontSize * 5f), (int)width, (int)(fontSize * 2f)), $"Press any key to return to main menu.", (int)(fontSize / 4), StringAlignment.Center);
            GUI.Render();
        }

        public void Mouse_BottonPressed(MouseButtonEventArgs e)
        {
            sceneManager.ChangeScene(SceneType.SCENE_MAIN_MENU);
        }

        private void Keyboard_KeyDown(KeyboardKeyEventArgs e)
        {
            sceneManager.ChangeScene(SceneType.SCENE_MAIN_MENU);
        }
        public override void Close()
        {
            sceneManager.mouseDelegate -= Mouse_BottonPressed;
            sceneManager.keyboardDownDelegate -= Keyboard_KeyDown;
        }
    }
}