using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Inspector Fields
    [SerializeField] private RectTransform _successPanel;
    #endregion

    #region Unity Actions
    public static event Action OnNextLevel;
    #endregion

    void OnEnable()
    {
        LevelManager.OnLevelComplete += ShowSuccessPanel;
    }

    void OnDisable()
    {
        LevelManager.OnLevelComplete -= ShowSuccessPanel;
    }

    void ShowSuccessPanel()
    {

        TweenHelper.MoveUIToPosition(_successPanel, Vector2.zero, 0.5f);
    }


    public void OnNextLevelButtonPressed()
    {
        OnNextLevel?.Invoke();
        HideSuccessPanel();

    }

    void HideSuccessPanel()
    {
        TweenHelper.MoveUIToPosition(_successPanel, new Vector2(0, -1500), 0.5f);

    }



}
