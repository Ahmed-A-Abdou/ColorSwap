using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class NodeData
{
    public Vector2 normalizedPosition;
    public Color color;
}

[System.Serializable]
public class NodeConnection
{
    public int node1;
    public int node2;
}

[CreateAssetMenu(fileName = "LevelData", menuName = "ColorSwap/LevelData")]
public class LevelData : ScriptableObject
{
    public List<NodeData> Nodes = new List<NodeData>();
    public List<NodeConnection> Connections = new List<NodeConnection>();
}