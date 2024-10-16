using System;

namespace S2dio.Utils
{
    public class CountdownTimer
    {
        private float duration;
        private float timeRemaining;
        private bool isRunning;

        public Action OnTimerStart = delegate { };
        public Action OnTimerStop = delegate { };

        public CountdownTimer(float duration)
        {
            this.duration = duration;
            timeRemaining = 0f;
            isRunning = false;
        }

        public bool IsRunning => isRunning;

        public void Start()
        {
            if (!isRunning)
            {
                timeRemaining = duration;
                isRunning = true;
                OnTimerStart.Invoke();
            }
        }

        public void Stop()
        {
            isRunning = false;
            timeRemaining = 0f;
            OnTimerStop.Invoke();
        }

        public void Tick(float deltaTime)
        {
            if (isRunning)
            {
                timeRemaining -= deltaTime;
                if (timeRemaining <= 0f)
                {
                    Stop();
                }
            }
        }
    }
}