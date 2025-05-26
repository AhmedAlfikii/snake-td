using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Ads
{
    public struct TafraAdInfo
    {
        public string AdUnitIdentifier;
        public string Placement;
        public string AdFormat;
        public string NetworkName;
        public string NetworkPlacement;
        public double Revenue;
        public string RevenuePrecision;

        public TafraAdInfo(string adUnitIdentifier, string placement, string adFormat = "", string networkName = "", string networkPlacement = "", double revenue = 0, string revenuePrecision = "")
        {
            AdUnitIdentifier = adUnitIdentifier;
            Placement = placement;
            AdFormat = adFormat;
            NetworkName = networkName;
            NetworkPlacement = networkPlacement;
            Revenue = revenue;
            RevenuePrecision = revenuePrecision;
        }
    }
}