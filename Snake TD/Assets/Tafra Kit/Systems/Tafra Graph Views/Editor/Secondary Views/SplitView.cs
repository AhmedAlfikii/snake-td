using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor.GraphViews
{
    public class SplitView : TwoPaneSplitView
    {
        public class UxmlFactory : UxmlFactory<SplitView, TwoPaneSplitView.UxmlTraits> { }

        public SplitView() : base() { }

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="fixedPaneIndex">0 for setting first child as the fixed pane, 1 for the second child element.</param>
        /// <param name="fixedPaneStartDimension">Set an inital width or height for the fixed pane.</param>
        /// <param name="orientation">Orientation of the split view.</param>
        public SplitView(int fixedPaneIndex, float fixedPaneStartDimension, TwoPaneSplitViewOrientation orientation, string viewDataKey = null) : base(fixedPaneIndex, fixedPaneStartDimension, orientation)
        {
            base.viewDataKey = viewDataKey;
        }
    }
}