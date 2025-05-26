using TafraKit.AI3;
using TafraKit.AI3.Motor;
using UnityEngine;
using UnityEngine.AI;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Motor/Go To Random Point"), GraphNodeName("Go To Random Point", "Seek Point")]
    public class GoToRandomPointTask : MotorActionTask
    {
        public enum RelativeSpace
        { 
            World,
            Local,
            LocalOnInitialization
        }
        [SerializeField] private BlackboardVector3Getter searchAreaSize = new BlackboardVector3Getter(new Vector3(25, 0, 25));
        [SerializeField] private RelativeSpace areaSpace = RelativeSpace.Local;

        [NavMeshAreaMask()]
        [SerializeField] private int areaMask = 0;

        protected Vector3 areaCenter;

        protected override AIMotor.ActionCategories ActionCategory => AIMotor.ActionCategories.MainMovement;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            searchAreaSize.Initialize(agent.BlackboardCollection);

            if(areaSpace == RelativeSpace.LocalOnInitialization)
                areaCenter = motor.transform.position;
        }
        protected override int StartAction()
        {
            Vector3 size = searchAreaSize.Value;
            Vector3 halfSize = size / 2f;

            Vector3 destination;

            bool foundAPoint;
            int iterations = 10;
            do
            {
                destination = new Vector3(Random.Range(-halfSize.x, halfSize.x), Random.Range(-halfSize.y, halfSize.y), Random.Range(-halfSize.z, halfSize.z));

                switch(areaSpace)
                {
                    case RelativeSpace.Local:
                        destination += motor.transform.position;
                        break;
                    case RelativeSpace.LocalOnInitialization:
                        destination += areaCenter;
                        break;
                }

                foundAPoint = NavMesh.SamplePosition(destination, out NavMeshHit hit, 5, areaMask);

                iterations--;
                if(iterations <= 0)
                    break;

            } while(!foundAPoint);

            if(foundAPoint)
            {
                return motor.GoToPoint(destination, OnActionCompleted, OnActionInterrupted);
            }
            else
            {
                Debug.LogError("Couldn't find a point to go to on a nav mesh.");
                return -1;
            }
        }

        public override void OnDrawGizmosSelected()
        {
            Vector3 center = Vector3.zero;

            switch(areaSpace)
            {
                case RelativeSpace.Local:
                    center += motor.transform.position;
                    break;
                case RelativeSpace.LocalOnInitialization:
                    center += areaCenter;
                    break;
            }

            Gizmos.DrawWireCube(center, searchAreaSize.Value);
        }
    }
}