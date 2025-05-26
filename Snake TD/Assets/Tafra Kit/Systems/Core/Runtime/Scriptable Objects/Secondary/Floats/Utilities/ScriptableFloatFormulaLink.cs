using System;
using TafraKit;
using TafraKit.Mathematics;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Scriptable Float Formula Link", menuName = "Tafra Kit/Scriptables/Scriptable Float Formula Link")]
public class ScriptableFloatFormulaLink : ScriptableObject, IResettable
{
    #region Private Serialized Fields
    [Tooltip("The scriptable float that holds the level.")]
    [SerializeField] private ScriptableFloat level;
    [Tooltip("The scriptable float that holds the experience.")]
    [SerializeField] private ScriptableFloat experience;
    [Tooltip("When receiving more experience than needed to level up, should the remainder of the experience be moved to next level?")]
    [SerializeField] private bool moveRemainderToNextLvl = true;
    [Space(20)]
    [SerializeField] private FormulasContainer formula;
    #endregion

    #region Events
    [NonSerialized] private UnityEvent onLevelUp = new UnityEvent();
    #endregion

    #region Public Properties
    public ScriptableFloat Level => level;
    public ScriptableFloat Experience => experience;
    public UnityEvent OnLevelUp => onLevelUp;
    #endregion

    #region MonoBehaviour Messages
    void OnEnable()
    {
        #if UNITY_EDITOR
        if(!EditorApplication.isPlayingOrWillChangePlaymode)
            return;
        #endif

        if(experience)
        {
            experience.OnValueChange.AddListener(OnExpValueChange);
            OnExpValueChange(experience.Value);
        }
    }
    void OnDisable()
    {
        if(experience)
            experience.OnValueChange.RemoveListener(OnExpValueChange);
    }
    #endregion

    #region Callbacks
    void OnExpValueChange(float value)
    {
        int requiredExpPoints = GetRequiredExp(); //Equation value at next level
        var exp = experience.Value;
        int lvl = Mathf.RoundToInt(level.Value);
        int lvlsToAdd = 0;
        
        if (moveRemainderToNextLvl)
        {
            while (exp >= requiredExpPoints)
            {
                exp -= requiredExpPoints;
                lvlsToAdd++;
                
                requiredExpPoints = GetRequiredExpAt(lvl + lvlsToAdd + 1); //Get next level requirement
            }
        }
        else if (exp >= requiredExpPoints)
        {
            lvlsToAdd = 1;
            exp = 0;
        }

        if (lvlsToAdd > 0)
        {
            level.Add(lvlsToAdd, false);
            experience.Set(exp);
            
            onLevelUp?.Invoke();
        }
    }
    #endregion

    #region Public Functions
    public int GetRequiredExp()
    {
        return GetRequiredExpAt(Mathf.RoundToInt(level.Value) + 1);
    }
    public int GetRequiredExpAt(int level)
    {
        return Mathf.RoundToInt(formula.Evaluate(level));
    }
    /// <summary>
    /// Gets the total experience required from startLevel to reach targetLevel, regardless of the current exp
    /// </summary>
    /// <param name="startLevel"></param>
    /// <param name="targetLevel"></param>
    /// <returns></returns>
    public int GetTotalRequiredExpTo(int startLevel, int targetLevel)
    {
        if (targetLevel <= startLevel)
            return 0;

        int pointsToReachLevelMax = 0;
        for (int i = startLevel + 1; i <= targetLevel; i++)
        {
            pointsToReachLevelMax += Mathf.RoundToInt(formula.Evaluate(i));
        }
        return pointsToReachLevelMax;
    }
    /// <summary>
    /// Gets the remaining experience to reach targetLevel
    /// </summary>
    /// <param name="targetLevel"></param>
    /// <returns></returns>
    public int GetRequiredExpTo(int targetLevel)
    {
        if (targetLevel <= (int)level.Value)
            return 0;

        int pointsToReachLevelMax = 0;
        pointsToReachLevelMax -= Mathf.RoundToInt(experience.Value);
        for (int i = (int)level.Value + 1; i <= targetLevel; i++)
        {
            pointsToReachLevelMax += Mathf.RoundToInt(formula.Evaluate(i));
        }
        return pointsToReachLevelMax;
    }
    public int PredictLevel(float totalPoints)
    {
        int nextLvl = Mathf.RoundToInt(level.Value + 1);
        float expPoints = totalPoints;
        int predictedLvl = Mathf.RoundToInt(level.Value);

        int requiredExpPoints = GetRequiredExpAt(nextLvl);

        while (expPoints >= requiredExpPoints)
        {
            predictedLvl = nextLvl;

            if (moveRemainderToNextLvl)
            {
                expPoints -= requiredExpPoints;

                nextLvl++;

                requiredExpPoints = GetRequiredExpAt(nextLvl);
            }
            else break;
        }

        return predictedLvl;
    }

    public void ResetSavedData()
    {
        if(experience)
            experience.OnValueChange.RemoveListener(OnExpValueChange);

        if(level)
            level.ResetSavedData();

        if(experience)
            experience.ResetSavedData();

        if(experience)
            experience.OnValueChange.AddListener(OnExpValueChange);
    }
    #endregion
}
