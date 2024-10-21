using RichTap.Common;

namespace RichTap.Types
{

    public enum State
    {
        Idle,
        Stop,
        Play,
        Update,
    }

    public struct CurrentEffect
    {
        public State state { get; private set; }
        public RichtapEffect effect { get; private set; }

        public CurrentEffect(State state = State.Idle, RichtapEffect effect = null)
        {
            this.state = state;
            this.effect = effect;
        }

        public void Idle()
        {
            state = State.Idle;
        }

        public void Play()
        {
            state = State.Play;
        }

        public void Update()
        {
            state = State.Update;
        }

        public void Stop()
        {
            state = State.Stop;
        }

        public void Reset()
        {
            state = State.Idle;
            effect = null;
        }

        public void Effect(RichtapEffect effect)
        {
            this.effect = effect;
        }
    }

    public struct UpdateInfo
    {
        public int amplitude { get; private set; }
        public int frequency { get; private set; }
        public int loopInterval { get; private set; }

        public void Amplitude(int amplitude)
        {
            this.amplitude = amplitude;
        }

        public void LoopInterval(int loopInterval)
        {
            this.loopInterval = loopInterval;
        }

        public void Frequency(int frequency)
        {
            this.frequency = frequency;
        }
    }

}
