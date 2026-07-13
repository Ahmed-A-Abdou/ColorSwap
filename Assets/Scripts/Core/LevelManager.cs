using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelManager : MonoBehaviour
{
    #region Inspector Fields
    [SerializeField] private List<LevelData> _levels;
    [SerializeField] private int _currentLevelIndex = 0;
    [SerializeField] private GameObject _nodePrefab;
    [SerializeField] private GameObject _connectionPrefab;
    #endregion

    #region Private Fields
    private List<GameObject> _spawnedNodes = new List<GameObject>();
    private List<GameObject> _spawnedConnections = new List<GameObject>();
    private LevelData _currentLevelData;
    private bool _isAnimating = false;
    #endregion

    #region Unity Actions
    public static event Action OnInvalidSwap;
    public static event Action OnLevelComplete;

    #endregion
    void OnEnable()
    {
        InputHandler.OnSwapAttempt += HandleSwapAttempt;
        UIManager.OnNextLevel += LoadNextLevel;
    }

    void OnDisable()
    {
        InputHandler.OnSwapAttempt -= HandleSwapAttempt;
        UIManager.OnNextLevel -= LoadNextLevel;
    }

    void Start()
    {
        LoadLevel();
        DrawConnections();
    }

    private void LoadLevel()
    {
        _currentLevelData = _levels[_currentLevelIndex];
        int index = 0;
        foreach (var nodeData in _currentLevelData.Nodes)
        {

            Vector3 worldPosition = NormalizedToWorldPosition(nodeData.normalizedPosition);
            GameObject node = Instantiate(_nodePrefab, worldPosition, Quaternion.identity);
            GetChildObject(node).GetComponent<SpriteRenderer>().color = nodeData.color;
            _spawnedNodes.Add(node);
            Node nodeComponent = node.GetComponent<Node>();
            nodeComponent.Index = index;
            nodeComponent.Color = nodeData.color;
            index++;
        }
    }

    GameObject GetChildObject(GameObject node)
    {
        return node.transform.GetChild(1).gameObject;
    }

    Vector3 NormalizedToWorldPosition(Vector2 normalizedPosition)
    {
        // Assuming the world space ranges from (0,0) to (1,1) for normalized positions
        Vector3 normalizedPos = new Vector3(normalizedPosition.x, normalizedPosition.y, 10f);
        Vector3 worldPos = Camera.main.ViewportToWorldPoint(normalizedPos);
        return worldPos;
    }

    void DrawConnections()
    {
        foreach (var connection in _currentLevelData.Connections)
        {
            GameObject node1 = _spawnedNodes[connection.node1];
            GameObject node2 = _spawnedNodes[connection.node2];

            Vector3 startPos = node1.transform.position;
            Vector3 endPos = node2.transform.position;

            GameObject connectionObj = Instantiate(_connectionPrefab);
            _spawnedConnections.Add(connectionObj);
            LineRenderer lineRenderer = connectionObj.GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
        }
    }

    void HandleSwapAttempt(int node1Index, int node2Index)
    {
        if (_isAnimating) return; // Prevent multiple swaps at the same time


        if (IsValidSwap(node1Index, node2Index))
        {
 
            SwapNodes(node1Index, node2Index);
            CheckWinCondition();
        }
        else
        {
 
            OnInvalidSwap?.Invoke();
        }
    }

    bool IsValidSwap(int node1Index, int node2Index)
    {
        foreach (var connection in _currentLevelData.Connections)
        {
            if ((connection.node1 == node1Index && connection.node2 == node2Index) ||
                (connection.node1 == node2Index && connection.node2 == node1Index))
            {
                return true; // Nodes are directly connected, allow swap
            }
        }
        return false; // Default case, disallow swap
    }


    void SwapNodes(int node1Index, int node2Index)
    {
        _isAnimating = true;
        GameObject node1 = _spawnedNodes[node1Index];
        GameObject node2 = _spawnedNodes[node2Index];

        GameObject child1 = node1.transform.GetChild(1).gameObject;
        GameObject child2 = node2.transform.GetChild(1).gameObject;


        // Swap colors
        SpriteRenderer sr1 = child1.GetComponent<SpriteRenderer>();
        SpriteRenderer sr2 = child2.GetComponent<SpriteRenderer>();

        Color tempColor = sr1.color;
        sr1.color = sr2.color;
        sr2.color = tempColor;

        // Update Node components
        Node nodeComp1 = node1.GetComponent<Node>();
        Node nodeComp2 = node2.GetComponent<Node>();

        Color tempNodeColor = nodeComp1.Color;
        nodeComp1.Color = nodeComp2.Color;
        nodeComp2.Color = tempNodeColor;

        TweenHelper.MoveToPosition(child1.transform, node1.transform.position).OnComplete(() => _isAnimating = false);
        TweenHelper.MoveToPosition(child2.transform, node2.transform.position).OnComplete(() => _isAnimating = false);

    }

    bool CheckWinCondition()
    {
        foreach (var connection in _currentLevelData.Connections)
        {
            GameObject node1 = _spawnedNodes[connection.node1];
            GameObject node2 = _spawnedNodes[connection.node2];

            SpriteRenderer sr1 = node1.transform.GetChild(1).GetComponent<SpriteRenderer>();
            SpriteRenderer sr2 = node2.transform.GetChild(1).GetComponent<SpriteRenderer>();

            if (sr1.color == sr2.color)
            {
                return false; // Not a win yet
            }
        }


        OnLevelComplete?.Invoke();
        return true;
    }


    public void LoadNextLevel()
    {
        // Clean up current level
        foreach (var node in _spawnedNodes)
        {
            Destroy(node);
        }
        _spawnedNodes.Clear();

        foreach (var connection in _spawnedConnections)
        {
            Destroy(connection);
        }
        _spawnedConnections.Clear();

        // Load next level

        if (_currentLevelIndex + 1 >= _levels.Count)
        {
            _currentLevelIndex = 0; // Loop back to first level or handle as needed

            LoadLevel();
            DrawConnections();
            return;
        }
        _currentLevelIndex++;
        LoadLevel();
        DrawConnections();
    }
}
