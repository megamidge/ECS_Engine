using System;
using System.Collections.Generic;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenGL_Game.Components;
using OpenGL_Game.OBJLoader;
using OpenGL_Game.Objects;
using OpenGL_Game.Scenes;

namespace OpenGL_Game.Systems
{
    class SystemRender : ISystem
    {
        const ComponentTypes MASK = (ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_GEOMETRY);

        protected int pgmID;
        protected int vsID;
        protected int fsID;
        protected int uniform_stex;
        protected int uniform_mmodelviewproj;
        protected int uniform_mmodel;
        protected int uniform_mview;
        protected int uniform_lightpos;

        public SystemRender()
        {
            pgmID = GL.CreateProgram();
            LoadShader("Engine/Shaders/vs.glsl", ShaderType.VertexShader, pgmID, out vsID);
            LoadShader("Engine/Shaders/fs.glsl", ShaderType.FragmentShader, pgmID, out fsID);
            GL.LinkProgram(pgmID);
            Console.WriteLine(GL.GetProgramInfoLog(pgmID));

            uniform_stex = GL.GetUniformLocation(pgmID, "s_texture");
            uniform_mmodelviewproj = GL.GetUniformLocation(pgmID, "ModelViewProjMat");
            uniform_mmodel = GL.GetUniformLocation(pgmID, "ModelMat");
            uniform_mview = GL.GetUniformLocation(pgmID, "ViewMat");
            uniform_lightpos = GL.GetUniformLocation(pgmID, "lightPos");
        }

        void LoadShader(String filename, ShaderType type, int program, out int address)
        {
            address = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(filename))
            {
                GL.ShaderSource(address, sr.ReadToEnd());
            }
            GL.CompileShader(address);
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }

        public string Name
        {
            get { return "SystemRender"; }
        }

        public void OnAction(Entity entity)
        {
            if ((entity.Mask & MASK) == MASK)
            {
                List<IComponent> components = entity.Components;

                IComponent geometryComponent = components.Find(delegate(IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_GEOMETRY;
                });
                Geometry geometry = ((ComponentGeometry)geometryComponent).Geometry();

                IComponent transformComponent = components.Find(delegate(IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_TRANSFORM;
                });
                //Get transformations
                Vector3 position = ((ComponentTransform)transformComponent).Position;
                Vector3 scale = ((ComponentTransform)transformComponent).Scale;
                Vector3 rotation = ((ComponentTransform)transformComponent).Rotation;
                //apply transformations                
                Matrix4 model =
                    Matrix4.CreateRotationZ(rotation.Z) *
                    Matrix4.CreateRotationX(rotation.X) *
                    Matrix4.CreateRotationY(rotation.Y) *
                    Matrix4.CreateScale(scale) *
                    Matrix4.CreateTranslation(position);

                Draw(model, geometry);
            }
        }

        public void Draw(Matrix4 model, Geometry geometry)
        {
            GL.UseProgram(pgmID);

            GL.Uniform1(uniform_stex, 0);
            GL.ActiveTexture(TextureUnit.Texture0);

            for(int i = 0; i < GameScene.gameInstance.lights.Length; i++)
            {
                int uniform_lightPos = GL.GetUniformLocation(pgmID, $"lights[{i}].Position");
                Vector4 lightPosition = Vector4.Transform(GameScene.gameInstance.lights[i].Position, GameScene.gameInstance.camera.view);
                GL.Uniform4(uniform_lightPos, ref lightPosition);
                int uniform_lightColour = GL.GetUniformLocation(pgmID, $"lights[{i}].Colour");
                GL.Uniform3(uniform_lightColour, ref GameScene.gameInstance.lights[i].Colour);
            }
            int uniform_lightCount = GL.GetUniformLocation(pgmID, "lightCount");
            GL.Uniform1(uniform_lightCount, GameScene.gameInstance.lights.Length);

            GL.UniformMatrix4(uniform_mview, true, ref GameScene.gameInstance.camera.view);
            //Vector4 lightPosition = Vector4.Transform(new Vector4(0, 20, 0,1),GameScene.gameInstance.camera.view);
            //GL.Uniform4(uniform_lightpos, ref lightPosition);
            GL.UniformMatrix4(uniform_mmodel, true, ref model);
            Matrix4 modelViewProjection = model * GameScene.gameInstance.camera.view * GameScene.gameInstance.camera.projection;
            GL.UniformMatrix4(uniform_mmodelviewproj, false, ref modelViewProjection);



            geometry.Render();

            GL.UseProgram(0);
        }
    }
}
