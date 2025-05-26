using UnityEngine;

namespace TafraKit.Internal
{
    public class GlobalManager : MonoBehaviour
    {
        [SerializeField] private string id;

        public string ID => id;

        private void Awake()
        {
            bool registered = GlobalManagersHandler.RegisterManager(this);

            if(!registered)
                Destroy(gameObject);
        }
    }
}