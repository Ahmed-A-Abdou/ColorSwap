using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputHandler : MonoBehaviour
{
    #region Private Fields
    private int _currentNodeIndex = -1;
    private bool _isDragging = false;
    private Node _draggedNode = null;
    private bool _isGameActive = true;

    private Vector3 _originalNodePosition;
    #endregion

    #region Unity Actions
    public static event Action<int, int> OnSwapAttempt;
    #endregion


    void OnEnable()
    {
        LevelManager.OnInvalidSwap += ReturnNode;
        LevelManager.OnLevelComplete += () => _isGameActive = false;
        UIManager.OnNextLevel += () => _isGameActive = true;
    }

    void OnDisable()
    {
        LevelManager.OnInvalidSwap -= ReturnNode;
        LevelManager.OnLevelComplete -= () => _isGameActive = false;
        UIManager.OnNextLevel -= () => _isGameActive = true;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!_isGameActive) return;
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = -Camera.main.transform.position.z;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(worldPosition);

            if (hit != null)
            {
                Node node = hit.transform.parent.GetComponent<Node>();

                if (node != null)
                {

                    _currentNodeIndex = node.Index;
                    _isDragging = true;
                    _draggedNode = node;
                    _originalNodePosition = node.transform.position;
                    _draggedNode.GetComponentInChildren<Collider2D>().enabled = false;
                }
            }
        }



        if (Input.GetMouseButton(0))
        {
            DragNode();
        }


        if (Input.GetMouseButtonUp(0) && _isDragging)
        {
            //Mouse position in world space
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = -Camera.main.transform.position.z;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);


            //Check if we hit another node
            Collider2D hit = Physics2D.OverlapPoint(worldPosition);
            _draggedNode.GetComponentInChildren<Collider2D>().enabled = true;



            Node node = hit != null ? hit.GetComponentInParent<Node>() : null;

            if (node != null && node.Index != _currentNodeIndex)
            {

                OnSwapAttempt?.Invoke(_currentNodeIndex, node.Index);
            }
            else
            {
                ReturnNode();
            }

            _isDragging = false;
            _draggedNode = null;

        }


    }

    void ReturnNode()
    {
        if (_draggedNode == null) return;
        TweenHelper.MoveToPosition(_draggedNode.transform.GetChild(1), _originalNodePosition);
        _draggedNode.GetComponentInChildren<Collider2D>().enabled = true;
    }

    void DragNode()
    {
        if (_isDragging && _draggedNode != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = -Camera.main.transform.position.z;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            _draggedNode.transform.GetChild(1).gameObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, _draggedNode.transform.position.z);
        }
    }
}