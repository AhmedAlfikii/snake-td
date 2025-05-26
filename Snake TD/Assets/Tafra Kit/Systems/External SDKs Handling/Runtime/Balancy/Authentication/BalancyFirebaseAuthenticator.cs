using System.Collections;
using System.Collections.Generic;
using TafraKit.ExternalSDKs.Firebase;
using UnityEngine;
#if TAFRA_BALANCY && TAFRA_FIREBASE_AUTH
using Firebase.Auth;
using Balancy;
#endif
using System.Threading.Tasks;

namespace TafraKit.ExternalSDKs.BalancySDK
{
#if TAFRA_BALANCY && TAFRA_FIREBASE_AUTH
    public static class BalancyFirebaseAuthenticator
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (FirebaseAuthenticator.IsInitialized)
                OnFirebaseAuthInitialized();
            else
                FirebaseAuthenticator.OnInitialized.AddListener(OnFirebaseAuthInitialized);
        }

        private static async void OnFirebaseAuthInitialized()
        {
            while(!TafraBalancy.IsInitialized || !FirebaseAuthenticator.IsInitialized)
            {
                await Task.Yield();
            }

            if(TafraBalancy.InitializationState == InitializationState.Failed)
                return;

            FirebaseUser user;

            if (FirebaseAuthenticator.CurrentUser != null)
                user = FirebaseAuthenticator.CurrentUser;
            else
                user = await FirebaseAuthenticator.SignInWithPlatformDefault();

            if(user == null)
                return;

            TafraBalancy.AuthWithName(user.UserId, user.UserId);
        }
    }
#endif
}