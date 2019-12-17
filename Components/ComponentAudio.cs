using OpenGL_Game.Managers;
using OpenGL_Game.OBJLoader;
using OpenTK;
using OpenTK.Audio.OpenAL;
namespace OpenGL_Game.Components
{
    class ComponentAudio : IComponent
    {
        Vector3 sourcePosition;
        Vector3 sourceVelocity;
        int audioBuffer;
        int audioSource;
        public ComponentAudio(string audioFile)
        {
            this.audioBuffer = ResourceManager.LoadAudio(audioFile);
            audioSource = AL.GenSource();
            AL.Source(audioSource, ALSourcei.Buffer, audioBuffer); // attach the buffer to a source
            AL.Source(audioSource, ALSourceb.Looping, true); // source loops infinitely
            sourcePosition = new Vector3(0.0f, 0.0f, 0.0f); // give the source a position
            sourceVelocity = new Vector3(0, 0, 0);
            AL.Source(audioSource, ALSource3f.Position, ref sourcePosition);
            AL.Source(audioSource, ALSource3f.Velocity, ref sourceVelocity);
            AL.SourcePlay(audioSource); // play the ausio source
        }
        public void Dispose()
        {
            AL.SourceStop(audioSource);
            AL.DeleteSource(audioSource);
            AL.DeleteBuffer(audioBuffer);
        }
        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_AUDIO; }
        }

        public int AudioBuffer()
        {
            return audioBuffer;
        }

        public void Play()
        {
            AL.SourcePlay(audioSource);
        }
        public void Stop()
        {
            AL.SourceStop(audioSource);
        }
        public void Pause()
        {
            AL.SourcePause(audioSource);
        }
        public ALSourceState State 
        {
            get { return AL.GetSourceState(audioSource); }
        }
        public Vector3 SourcePosition
        {
            get { return sourcePosition; }
            set {
                sourcePosition = value;
                AL.Source(audioSource, ALSource3f.Position, ref sourcePosition);
            }
        }

        public Vector3 SourceVelocity
        {
            get { return sourceVelocity; }
            set
            {
                sourceVelocity = value;
                AL.Source(audioSource, ALSource3f.Velocity, ref sourceVelocity);
            }
        }
    }
}
