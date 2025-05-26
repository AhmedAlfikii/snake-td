using System;
using System.Collections;
using TafraKit.Ads;
using TafraKit.Healthies;
using TafraKit.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZUI;

namespace TafraKit.Internal
{
    [SearchMenuItem("Revive")]
    public class LevelEndReviveModule : LevelEndModule
    {
        [SerializeField] private float delay = 1.5f;
        [SerializeField] private UIElementsGroup uieg;
        [SerializeField] private RVButton reviveBTN;
        [SerializeField] private string reviveRVSource = "revive";
        [SerializeField] private ZButton declineBTN;
        [SerializeField] private TextMeshProUGUI remainingRevivesCount;
        [SerializeField] private GraphicRaycaster graphicsRaycaster;

        [Header("Revive Properties")]
        [SerializeField] private TafraFloat revivesCount;
        [SerializeField] private TafraFloat reviveHealth;
        [SerializeField] private TafraFloat immunityDuration = new TafraFloat(2f);

        private int remainingRevives;

        protected override void OnInitialize()
        {
            remainingRevives = TafraSaveSystem.LoadInt("LEVEL_END_REVIVE_REMAINING_REVIVES", revivesCount.ValueInt);
            reviveBTN.Initialize(reviveRVSource, OnReviveButtonClicked);
        }
        protected override void OnStart()
        {
            handler.StartCoroutine(Starting());
           
            if(remainingRevives <= 0)
            {
                return;
            }

            graphicsRaycaster.enabled = true;

            remainingRevivesCount.text = remainingRevives.ToString();
            remainingRevivesCount.text += " ";
            remainingRevivesCount.text += remainingRevives < 2 ? " Use Remaining" : "Uses Remaining";

            declineBTN.onClick.AddListener(OnDeclineButtonClicked);
        }
        protected override void OnEnd()
        {
            TimeScaler.RemoveTimeScaleControl("leve_end_revive_module");

            uieg.ChangeVisibility(false);
            graphicsRaycaster.enabled = false;

            declineBTN.onClick.RemoveListener(OnDeclineButtonClicked);
        }
        protected override void OnResetSavedData()
        {
            TafraSaveSystem.DeleteKey("LEVEL_END_REVIVE_REMAINING_REVIVES");
        }

        private IEnumerator Starting()
        {
            yield return Yielders.GetWaitForSecondsRealtime(delay);

            if (remainingRevives <= 0)
            {
                End();
                yield break;
            }

            TimeScaler.SetTimeScale("leve_end_revive_module", 0);

            uieg.ChangeVisibility(true);
        }

        private void OnReviveButtonClicked()
        {
            handler.InterruptLevelFail();

            Healthy playerHealthy = SceneReferences.PlayerHealthy;

            playerHealthy.Revive(reviveHealth.Value);

            float immunityDurationValue = immunityDuration.Value;
            if(immunityDurationValue > 0.001f)
            {
                ImmunityModule immunityModule = playerHealthy.GetModule<ImmunityModule>();

                if(immunityModule != null)
                {
                    immunityModule.AddImmunityActivator("level_end_screen_revive");
                    CompactCouroutines.StartCompactCoroutine(0, immunityDurationValue, false, null, null, () =>
                    {
                        immunityModule.RemoveImmunityActivator("level_end_screen_revive");
                    });
                }
                else
                    TafraDebugger.Log("Level End Module", "The player's healthy component doesn't have an immunity module.", TafraDebugger.LogType.Error);
            }

            remainingRevives--;
            TafraSaveSystem.SaveInt("LEVEL_END_REVIVE_REMAINING_REVIVES", remainingRevives);

            End();
        }
        private void OnDeclineButtonClicked()
        {
            End();
        }
    }
}