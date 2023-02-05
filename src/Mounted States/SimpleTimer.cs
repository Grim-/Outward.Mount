using System;

public class SimpleTimer
{
    private float TimerDuration;
    private bool ResetTimerOnComplete = false;

    private float CurrentTimer = 0;
    private bool CanUpdate = true;

    public Action OnComplete;

    public SimpleTimer(float timerDuration, bool resetTimerOnComplete = false, Action onComplete = null)
    {
        TimerDuration = timerDuration;
        ResetTimerOnComplete = resetTimerOnComplete;
        OnComplete = onComplete;
    }

    public void UpdateTimer(float deltaTime)
    {
        if (CanUpdate)
        {
            CurrentTimer += deltaTime;

            if (CurrentTimer >= TimerDuration)
            {
                OnComplete?.Invoke();

                if (ResetTimerOnComplete)
                {
                    ResetTimer();
                }
                else
                {
                    CanUpdate = false;
                }

            }
        }
    }

    public void ResetTimer()
    {
        CurrentTimer = 0;
    }
}
