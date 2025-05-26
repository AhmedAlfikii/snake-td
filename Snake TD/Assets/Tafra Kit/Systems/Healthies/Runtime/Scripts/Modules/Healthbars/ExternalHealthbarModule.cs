using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Healthies
{
    [SearchMenuItem("Healthbars/External Healthbar")]
    public class ExternalHealthbarModule : HealthyModule
    {
        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        public override bool DisableOnDeath => false;
    }
}