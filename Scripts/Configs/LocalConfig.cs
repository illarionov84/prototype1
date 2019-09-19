using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LocalConfig
{
    public List<ShelfConfig> Shelfs;
    public Transform DropZone;
    public GameObject VegetablePrefab;
    public GameObject VegetableCatchedByPawPrefab;
    public GameObject VegetableWithEscapedAndFallenPrefab;
    public GameObject VegetableWithEscapedBehindScreenPrefab;
}
