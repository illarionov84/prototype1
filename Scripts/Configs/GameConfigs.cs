using System;
using System.Collections.Generic;
using Configs;
using UnityEngine;
#if FIREBASE_CONFIGS
using Firebase.RemoteConfig;

#endif

[Serializable]
public class GameConfigs : IRemoteConfig
{
    public GameConfig GameConfig;

    public void UpdateConfigs()
    {
#if FIREBASE_CONFIGS
        try
        {
            GameConfig =
                JsonUtility.FromJson<GameConfig>(FirebaseRemoteConfig.GetValue(nameof(GameConfig)).StringValue);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
#endif
    }

    private float GetFloat(string valName)
    {
#if FIREBASE_CONFIGS
        return float.Parse(FirebaseRemoteConfig.GetValue(valName).StringValue);
#else
        return 0;
#endif
    }

    public void PrintConfigs()
    {
//        Debug.Log($"userSheepStartConfig:{JsonUtility.ToJson(UserSheepStartConfig)}");
    }
}


[Serializable]
public class GameConfig
{
    public string LobbyBackGroundImageName;
    public string GameBackGroundImageName;
    public float DragDistance;
    public float DurationOfSwipe;
    public int FreshVegatablesCount;
    public int NumberOfSwipes;
    public float PercentageOfSpoiledVegetables;
    public float PercentageAppearanceOfPaw;
    public float DurationOfHitPaw;
    public float DurationOfRemovePaw;
    public float PercentageEscaped;
    public float PercentageEscapedAndFallenOrEscapedBehindScreen;
    public float EscapedTime;
    public int LastShelfCountForExclude;
    public int StartStarsCount;
    public float VegetablesCreateRate;
    public float CatchDelay;
    public float FallenDelay;
    public float AfterStartDelay;
    public float DropZoneRangeX;
    public float DropZoneRangeY;
    public float MinThresholdForPositionOnShelf;
    public float MaxThresholdForPositionOnShelf;
    public float PlayerWinDelay;
    public float GameOverDelay;
    public List<VegetableConfig> Vegetables;
}

[Serializable]
public class VegetableConfig
{
    public string Id;
    public float DurationOfPath;
    public float RotateDegreesPerSecond;
    public float HeightAboveShelf;
    public string FallenSprite;
    public string RollSprite;
    public bool IsFresh;
    public bool IsRotate;
    public bool CanCatchedByPaw;
    public bool CanEscaped;
}
