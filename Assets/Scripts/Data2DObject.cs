using UnityEngine;

[System.Serializable]
public class RowData
{
    public int[] rowValues;
}

[CreateAssetMenu(fileName = "2DData", menuName = "ScriptableObjects/Data2DObject")]
public class Data2DObject : ScriptableObject
{
    public RowData[] Columns;
}