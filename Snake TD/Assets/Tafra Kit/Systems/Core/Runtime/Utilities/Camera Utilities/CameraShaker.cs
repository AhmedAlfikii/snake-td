#if TAFRA_CINEMACHINE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

namespace TafraKit
{
    public class CameraShaker
    {
        private static MonoBehaviour coroutinePlayer;
        private static IEnumerator shakingEnum;

        private static Dictionary<CinemachineCamera, CinemachineCamera> camMimics = new Dictionary<CinemachineCamera, CinemachineCamera>();
        private static Dictionary<CinemachineCamera, GameObject> camMimicHolders = new Dictionary<CinemachineCamera, GameObject>();
        /// <summary>
        /// The camera being shaked (mimiced to shake).
        /// </summary>
        private static CinemachineCamera currentOriginalCam;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            GameObject cp = new GameObject("CameraShakerCoroutinePlayer");
            coroutinePlayer = cp.AddComponent<EmptyMonoBehaviour>();
            GameObject.DontDestroyOnLoad(cp);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            camMimics.Clear();
        }

        public static void Shake(CameraShakeProperties shakeProperties)
        {
            Shake(shakeProperties.PositionDisplacement, shakeProperties.RotationDisplacement, shakeProperties.Duration);
        }
        public static void Shake(Vector3 positionDisplacement, Vector3 rotationDisplacement, float duration)
        {
            if (shakingEnum != null)
            {
                coroutinePlayer.StopCoroutine(shakingEnum);

                //if (currentOriginalVCam != null)
                //    currentOriginalVCam.MoveToTopOfPrioritySubqueue();
            }

            CinemachineCamera originalVCam = currentOriginalCam ? currentOriginalCam : (CinemachineCamera)CinemachineBrain.GetActiveBrain(0).ActiveVirtualCamera;

            CinemachineCamera mimicVCam = null;
            GameObject mimicHolder = null;

            if (camMimics.ContainsKey(originalVCam))
            {
                mimicVCam = camMimics[originalVCam];
                mimicHolder = camMimicHolders[originalVCam];
            }
            else
            {
                mimicHolder = new GameObject($"{originalVCam.name}_MimicHolder");
                mimicHolder.transform.SetParent(originalVCam.transform);

                mimicHolder.transform.localPosition = Vector3.zero;
                mimicHolder.transform.localRotation = Quaternion.identity;
                mimicHolder.transform.localScale = Vector3.one;

                GameObject mimicVCamGO = GameObject.Instantiate(originalVCam.gameObject, mimicHolder.transform.position, mimicHolder.transform.rotation, mimicHolder.transform);
                mimicVCam = mimicVCamGO.GetComponent<CinemachineCamera>();
               
                camMimics.Add(originalVCam, mimicVCam);
                camMimicHolders.Add(originalVCam, mimicHolder);
            }

            shakingEnum = Shaking(mimicVCam, originalVCam, positionDisplacement, rotationDisplacement, duration);

            coroutinePlayer.StartCoroutine(shakingEnum);
        }

        private static IEnumerator Shaking(CinemachineCamera mimicCam, CinemachineCamera originalCam, Vector3 positionDisplacement, Vector3 rotationDisplacement, float duration)
        {
            currentOriginalCam = originalCam;

            if (!mimicCam.gameObject.activeSelf)
                mimicCam.gameObject.SetActive(true);
            
            mimicCam.Prioritize();
            
            float startTime = Time.time;
            float endTime = startTime + duration;

            while (Time.time < endTime)
            {
                mimicCam.transform.localPosition = new Vector3(Random.Range(-positionDisplacement.x, positionDisplacement.x),
                    Random.Range(-positionDisplacement.y, positionDisplacement.y),
                    Random.Range(-positionDisplacement.z, positionDisplacement.z));

                mimicCam.transform.localRotation = Quaternion.Euler(new Vector3(Random.Range(-rotationDisplacement.x, rotationDisplacement.x),
                    Random.Range(-rotationDisplacement.y, rotationDisplacement.y),
                    Random.Range(-rotationDisplacement.z, rotationDisplacement.z)));

                yield return null;
            }
            mimicCam.transform.localPosition = Vector3.zero;
            mimicCam.transform.localRotation = Quaternion.identity;

            mimicCam.gameObject.SetActive(false);

            originalCam.Prioritize();
            
            currentOriginalCam = null;

            shakingEnum = null;
            yield break;
        }
    }
}
#endif