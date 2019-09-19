using System;
using System.Collections.Generic;

[Serializable]
public class GameDataModel
{
    public ChefModel Chef;
    public List<VegetableModel> Vegetables;

    public GameDataModel()
    {
        Chef = new ChefModel();
        Vegetables = new List<VegetableModel>();
    }
}

[Serializable]
public class ChefModel
{
    public int CurrentShelf;
    public int CurrentStars;
}

[Serializable]
public class VegetableModel
{
    public float CreateTime;
    public bool Launched;
    public bool CatchedByPaw;
    public bool EscapedAndFallen;
    public bool EscapedBehindScreen;
    public int ShelfIndex;
    public int VegetableConfigIndex;
    public VegetableState VegetableState;
}