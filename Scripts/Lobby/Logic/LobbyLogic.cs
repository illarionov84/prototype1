using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyLogic : MonoBehaviour
{
    [SerializeField] private Image _backGround;
    [SerializeField] private Image _rollinPin;
    [SerializeField] private Image _btnArrowLeft;
    [SerializeField] private Image _btnArrowRight;
    [SerializeField] private Image _splashScreen;
    [SerializeField] private Button _btnPlay;
    [SerializeField] private LocalConfig _localConfig;

    private RemoteConfigsService _remoteConfigsService;

    private void Awake()
    {
        if (_backGround == null) 
        {
            throw new ArgumentNullException(nameof(_backGround));
        }
        if (_btnPlay == null) 
        {
            throw new ArgumentNullException(nameof(_btnPlay));
        }
        
        _remoteConfigsService = FindObjectOfType<RemoteConfigsService>();

        if (_remoteConfigsService == null)
        {
            var newRemConf = new GameObject(nameof(RemoteConfigsService));
            _remoteConfigsService = newRemConf.AddComponent<RemoteConfigsService>();
        }

        StartCoroutine(ShowSplashScreen());
        StartCoroutine(UpdateConfigsProcess());
    }

    private IEnumerator UpdateConfigsProcess()
    {
#if FIREBASE_CONFIGS

        _remoteConfigsService.UpdateConfigs();

        yield return new WaitForSeconds(1f);

        while (_remoteConfigsService.IsFetching)
        {
            yield return null;
        }
#else
        yield return null;
#endif
        
        var background = Resources.Load<Sprite>(_remoteConfigsService.Configs.GameConfig.LobbyBackGroundImageName);
        if (background == null)
        {
            throw new UnassignedReferenceException($"{nameof(background)}: {_remoteConfigsService.Configs.GameConfig.GameBackGroundImageName} not found!");
        }
        _backGround.sprite = background;
        _btnPlay.onClick.AddListener(
            () =>
            {
                _btnPlay.onClick.RemoveAllListeners();
                SceneManager.LoadScene(SceneName.Game_Scene.ToString());
            }
        );
    }

    private IEnumerator ShowSplashScreen()
    {
        _rollinPin.transform.position = _btnPlay.transform.position;
        yield return _rollinPin.DOFillAmount(1, 2).SetEase(Ease.Linear).WaitForCompletion();
        yield return _rollinPin.transform.DOLocalMove(Vector3.zero, 1);
        yield return _splashScreen.DOFade(0, 1).SetEase(Ease.Linear).WaitForCompletion();
        _btnPlay.gameObject.SetActive(true);
        _btnArrowLeft.gameObject.SetActive(true);
        _btnArrowRight.gameObject.SetActive(true);
    }
}
