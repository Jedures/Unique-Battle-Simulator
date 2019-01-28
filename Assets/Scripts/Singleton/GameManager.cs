using UnityEngine;

namespace Singleton
{
    public class GameManager : MonoBehaviour {

        public static GameManager gameManager = null;
 
        public float audioLevel = 100.0f;

        public void Awake()
        {
            if (gameManager == null) 
                gameManager = this;
            else 
                Destroy(gameManager);

            DontDestroyOnLoad(this);
        }
    

    

    }
}
