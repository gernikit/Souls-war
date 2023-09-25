using UnityEngine;
using UnityEngine.Events;

namespace YG
{
    [HelpURL("https://www.notion.so/PluginYG-d457b23eee604b7aa6076116aab647ed#178130fecabe4b3f81118dfe0fd88ccf")]
    public class ReviewYG : MonoBehaviour
    {
        [Tooltip("Открывать окно авторизации, если пользователь не авторизован.")]
        public enum ForUnauthorized { OpenAuthDialog, ReviewNotAvailable, Ignore };
        public ForUnauthorized forUnauthorized;

        [Tooltip("Активировать оценку игры на мобильных устройствах?")]
        public bool showOnMobileDevice;

        [Tooltip("Обновлять информацию при каждой активации объекта (в OnEnable)?")]
        public bool updateDataOnEnable;
        [Space(15)]
        public UnityEvent ReviewAvailable;
        public UnityEvent ReviewNotAvailable;
        public UnityEvent LeftReview;
        public UnityEvent NotLeftReview;
        
        private const int levelForReview = 3;

        private void Awake() => ReviewNotAvailable.Invoke();

        private void OnEnable()
        {
            YandexGame.GetDataEvent += UpdateData;
            YandexGame.ReviewSentEvent += ReviewSent;

            if (YandexGame.SDKEnabled) UpdateData();
            if (updateDataOnEnable) UpdateData();
        }
        private void OnDisable()
        {
            YandexGame.GetDataEvent -= UpdateData;
            YandexGame.ReviewSentEvent -= ReviewSent;
        }

        public void UpdateData()
        {
#if UNITY_EDITOR
            YandexGame.EnvironmentData.reviewCanShow = true;
#endif
            if (YandexGame.savesData.gameData.levelsData[LevelType.Lawn] < levelForReview)
                YandexGame.EnvironmentData.reviewCanShow = false;

            if (!showOnMobileDevice && (YandexGame.EnvironmentData.isMobile || YandexGame.EnvironmentData.isTablet))
                YandexGame.EnvironmentData.reviewCanShow = false;

            if (forUnauthorized == ForUnauthorized.ReviewNotAvailable
                 && !YandexGame.auth)
            {
                ReviewNotAvailable.Invoke();
                return;
            }

            if (YandexGame.EnvironmentData.reviewCanShow && YandexGame.savesData.canReviewThisSession)
                ReviewAvailable.Invoke();
            else ReviewNotAvailable.Invoke();
        }

        void ReviewSent(bool sent)
        {
            if (sent) LeftReview.Invoke();
            else NotLeftReview.Invoke();

            ReviewNotAvailable.Invoke();
        }

        public void ReviewShow()
        {
            ReviewNotAvailable.Invoke(); // ?
            YandexGame.savesData.canReviewThisSession = false;
            YandexGame.EnvironmentData.reviewCanShow = false; // ?

            bool authDialog = true;

            if (forUnauthorized == ForUnauthorized.Ignore && !YandexGame.auth)
            {
                return;
            }

            YandexGame.ReviewShow(authDialog);
        }
    }
}
