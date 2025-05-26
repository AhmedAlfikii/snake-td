using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TAFRA_FIREBASE_AUTH
using Firebase.Auth;
using LionStudios.Suite.Core;
#endif
using System.Threading.Tasks;
using UnityEngine.Events;
#if UNITY_ANDROID && TAFRA_GOOGLE_PLAY
using GooglePlayGames.BasicApi;
using GooglePlayGames;
#endif

namespace TafraKit.ExternalSDKs.Firebase
{
#if TAFRA_FIREBASE_AUTH
    public static class FirebaseAuthenticator
    {
        private static FirebaseAuth auth;
        private static FirebaseUser currentUser;
        private static UnityEvent onInitialized = new UnityEvent();

        private static bool isInitialized;
        private static bool debugLogs = true;

        public static bool IsInitialized => isInitialized;
        public static FirebaseUser CurrentUser => currentUser;
        public static UnityEvent OnInitialized => onInitialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if(LionCore.IsInitialized)
                OnFirebaseInitialized();
            else
                LionCore.OnInitialized += OnFirebaseInitialized;
        }
        private static void OnFirebaseInitialized()
        {
            auth = FirebaseAuth.DefaultInstance;

#if UNITY_ANDROID
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                .RequestServerAuthCode(false /* Don't force refresh */)
                .Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.Activate();
#endif

            if (auth.CurrentUser != null)
                currentUser = auth.CurrentUser;

            isInitialized = true;
            onInitialized?.Invoke();
        }

        public static async Task<FirebaseUser> SignInWithPlatformDefault()
        {
            FirebaseUser resultUser = null;

            if(Application.platform == RuntimePlatform.Android)
                resultUser = await SignInWithPlayGames();
            else if(Application.platform == RuntimePlatform.IPhonePlayer)
                resultUser = await SignInWithGameCenter();

            if(resultUser != null)
            {
                if(debugLogs)
                    TafraDebugger.Log("Firebase Authenticator", $"Successfully signed in with platform default. User ID: {resultUser.UserId}.", TafraDebugger.LogType.Info);

                currentUser = resultUser;
            }
            //else
            //{
            //    if(debugLogs)
            //        TafraDebugger.Log("Firebase Authenticator", "Failed to sign in with platform default. Will attempt to sign in annonymously", TafraDebugger.LogType.Error);
            //}

            return resultUser;
        }
        private static async Task<FirebaseUser> SignInWithPlayGames()
        {
#if UNITY_ANDROID
            if (debugLogs)
                TafraDebugger.Log("Firebase Authenticator", $"Attempting to sign in with Play Games.", TafraDebugger.LogType.Info);

            bool operationFinished = false;
            bool operationSuccess = false;
            PlayGamesPlatform.Instance.Authenticate((success) =>
            {
                operationSuccess = success;
                operationFinished = true;
            }, true);

            while(!operationFinished)
                await Task.Yield();

            if(operationSuccess)
            {
                if(debugLogs)
                    TafraDebugger.Log("Firebase Authenticator", "Play Games login successful. Linking to Firebase...", TafraDebugger.LogType.Info);

                var code = PlayGamesPlatform.Instance.GetServerAuthCode();
                Credential credential = PlayGamesAuthProvider.GetCredential(code);

                //No user is signed in, sign in using the fetched credentials.
                if(auth.CurrentUser == null)
                {
                    FirebaseUser signInResult = await auth.SignInWithCredentialAsync(credential);

                    return signInResult;
                }
                //A user is signed in, link the fetched credentials to it.
                else
                {
                    AuthResult linkResult = await auth.CurrentUser.LinkWithCredentialAsync(credential);

                    if(linkResult == null || linkResult.User == null)
                    {
                        if(debugLogs)
                            TafraDebugger.Log("Firebase Authenticator", "Linking Play Games to existing user failed.", TafraDebugger.LogType.Error);

                        return null;
                    }
                    else
                    {
                        if(debugLogs)
                            TafraDebugger.Log("Firebase Authenticator", "Successfully linked Play Games to existing user.", TafraDebugger.LogType.Info);

                        return linkResult.User;
                    }
                }
            }
            else
            {
                if(debugLogs)
                    TafraDebugger.Log("Firebase Authenticator", "Play Games authentication failed.", TafraDebugger.LogType.Error);

                return null;
            }
#else
            return null;
#endif
        }
        private static async Task<FirebaseUser> SignInWithGameCenter()
        {
            //TODO: do.

            if(debugLogs)
                TafraDebugger.Log("Firebase Authenticator", $"Attempting to sign in with Apple.", TafraDebugger.LogType.Info);

            await Task.Yield();
            return null;
        }
        private static async Task<FirebaseUser> SignInAnonymously()
        {
            if(debugLogs)
                TafraDebugger.Log("Firebase Authenticator", $"Attempting to sign in anonymously.", TafraDebugger.LogType.Info);

            AuthResult authResult = await auth.SignInAnonymouslyAsync();

            if(authResult == null || authResult.User == null)
            {
                if (debugLogs)
                    TafraDebugger.Log("Firebase Authenticator", "Failed to authenticate anonymously.", TafraDebugger.LogType.Error);

                return null;
            }

            if(debugLogs)
                TafraDebugger.Log("Firebase Authenticator", $"User successfully signed in anonymously. User ID: {authResult.User.UserId}.", TafraDebugger.LogType.Info);

            return authResult.User;
        }
    }
#endif
        }