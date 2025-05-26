using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TafraKit.Internal
{
    public class MaxUserIDButton : GameSettingsButton
    {
        [SerializeField] private TMP_Text text;

        protected override void Awake()
        {
            base.Awake();

            text.text = $"ID: {SystemInfo.deviceUniqueIdentifier}";
        }
        public override bool AreConditionsSatisfied()
        {
            return true;
        }

        protected override void OnClick()
        {
            ZHelper.CopyToClipboard(SystemInfo.deviceUniqueIdentifier);
        }
    }
}