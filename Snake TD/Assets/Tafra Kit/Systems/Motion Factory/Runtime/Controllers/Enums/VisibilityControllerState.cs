namespace TafraKit.MotionFactory
{ 
    public enum VisibilityControllerState
    {
        /// <summary>
        /// The element is fully visible.
        /// </summary>
        Shown,
        /// <summary>
        /// The element is currently animating towards being fully visible.
        /// </summary>
        Showing,
        /// <summary>
        /// The element is fully hidden.
        /// </summary>
        Hidden,
        /// <summary>
        /// The element is currently animating towards being fully hidden.
        /// </summary>
        Hiding,
        /// <summary>
        /// The element was interrupted while animating and it's currently in an unknown state.
        /// </summary>
        Interrupted,
        None
    }
}