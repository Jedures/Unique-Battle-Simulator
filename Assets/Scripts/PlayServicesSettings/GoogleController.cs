using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

namespace PlayServicesSettings
{
    public class GoogleController : MonoBehaviour
    {
        private void Awake()
        {
            var config = new PlayGamesClientConfiguration.Builder()
                .RequestServerAuthCode(false)
                .Build();
            
            PlayGamesPlatform.InitializeInstance(config);
            
            PlayGamesPlatform.Activate();
        }

        private void Start()
        {
            Social.localUser.Authenticate((bool success) =>{});
        }

        private void OnApplicationQuit()
        {
            PlayGamesPlatform.Instance.SignOut();
        }
        
    }
}