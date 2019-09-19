using System;
using UnityEngine;

public class SwipeController
{
    private readonly GameConfig _remoteConfig;
    private readonly Action<Vector2> OnSwipeDetected;
    private LocalConfig _localConfig;
    private Vector3 firstPosition;
    private Vector3 lastPosition;
    private Vector2 startPoint;

    public SwipeController(Action<Vector2> onSwipeDetected, LocalConfig localConfig, GameConfig remoteConfig)
    {
        OnSwipeDetected = onSwipeDetected;
        _localConfig = localConfig;
        _remoteConfig = remoteConfig;
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPoint = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            firstPosition = startPoint;
            lastPosition = Input.mousePosition;

            if (Mathf.Abs(lastPosition.x - firstPosition.x) > _remoteConfig.DragDistance ||
                Mathf.Abs(lastPosition.y - firstPosition.y) > _remoteConfig.DragDistance)
            {
                if (Mathf.Abs(lastPosition.x - firstPosition.x) > Mathf.Abs(lastPosition.y - firstPosition.y))
                {
                    if (lastPosition.x > firstPosition.x)
                    {
                        OnSwipeDetected?.Invoke(Vector2.right);
                    }
                    else
                    {
                        OnSwipeDetected?.Invoke(Vector2.left);
                    }
                }
                else
                {
                    if (lastPosition.y > firstPosition.y)
                    {
                        OnSwipeDetected?.Invoke(Vector2.up);
                    }
                    else
                    {
                        OnSwipeDetected?.Invoke(Vector2.down);
                    }
                }
            }
        }
    }
}