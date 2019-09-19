using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameLogic : MonoBehaviour
{
    [SerializeField] private Image _backGround;
    [SerializeField] private Button _btnBack;
    [SerializeField] private Button _btnRestart;
    [SerializeField] private ChefUI _chefUI;
    [SerializeField] private Text _gameResult;
    [SerializeField] private StarsUI _starsUI;
    [SerializeField] private Transform _vegetablesRootParent;
    [SerializeField] private LocalConfig _localConfig;
    [SerializeField] private GameDataModel _gameDataModel;
    private SwipeController _inputController;
    private Queue<int> _queueSwipes;
    private List<int> shelfList;
    private RemoteConfigsService _remoteConfigsService;

#if UNITY_EDITOR
    [Header("Debug info")]
    [SerializeField] private AnimationCurve _dropTimeCurve;
    [SerializeField] private AnimationCurve _createTimeCurve;
#endif

    private void Awake()
    {
        if (_backGround == null) 
        {
            throw new ArgumentNullException(nameof(_backGround));
        }
        if (_btnBack == null) 
        {
            throw new ArgumentNullException(nameof(_btnBack));
        }
        if (_btnRestart == null) 
        {
            throw new ArgumentNullException(nameof(_btnRestart));
        }
        if (_chefUI == null) 
        {
            throw new ArgumentNullException(nameof(_chefUI));
        }
        if (_gameResult == null) 
        {
            throw new ArgumentNullException(nameof(_gameResult));
        }
        if (_starsUI == null) 
        {
            throw new ArgumentNullException(nameof(_starsUI));
        }
        if (_vegetablesRootParent == null) 
        {
            throw new ArgumentNullException(nameof(_vegetablesRootParent));
        }

        _remoteConfigsService = FindObjectOfType<RemoteConfigsService>();
        if (_remoteConfigsService == null)
        {
            StartCoroutine(InitInEditor());
        }
        else
        {
            StartCoroutine(Prepare());
        }
    }
    
    private IEnumerator InitInEditor()
    {
        var newRemConf = new GameObject(nameof(RemoteConfigsService));
        _remoteConfigsService = newRemConf.AddComponent<RemoteConfigsService>();
        _remoteConfigsService.UpdateConfigs();

        while (_remoteConfigsService.IsFetching)
        {
            yield return null;
        }

        Debug.LogWarning("Configs UPDATED SUCCESS");
        StartCoroutine(Prepare());
    }

    private VegetableModel CreateVegetableModel(bool isFresh, int shelfIndex)
    {
        var NewVegetable = new VegetableModel();
        var filteredVegetables = _remoteConfigsService.Configs.GameConfig.Vegetables.FindAll(item => item.IsFresh == isFresh);
        var selectedVegetable = filteredVegetables[Random.Range(0, filteredVegetables.Count)];
        NewVegetable.VegetableConfigIndex = _remoteConfigsService.Configs.GameConfig.Vegetables.IndexOf(selectedVegetable);
        NewVegetable.ShelfIndex = shelfIndex;
        if (_remoteConfigsService.Configs.GameConfig.Vegetables[NewVegetable.VegetableConfigIndex].CanCatchedByPaw && Random.value < _remoteConfigsService.Configs.GameConfig.PercentageAppearanceOfPaw)
        {
            NewVegetable.CatchedByPaw = true;
        }
        if (_remoteConfigsService.Configs.GameConfig.Vegetables[NewVegetable.VegetableConfigIndex].CanEscaped && Random.value < _remoteConfigsService.Configs.GameConfig.PercentageEscaped)
        {
            if (Random.value < _remoteConfigsService.Configs.GameConfig.PercentageEscapedAndFallenOrEscapedBehindScreen)
            {
                NewVegetable.EscapedAndFallen = true;
            }
            else
            {
                NewVegetable.EscapedBehindScreen = true;
            }
        }
        return NewVegetable;
    }
    
    private int GetIndexShelf(int shelf)
    {
        if (shelfList.Count < _remoteConfigsService.Configs.GameConfig.LastShelfCountForExclude)
        {
            shelfList.Add(shelf);
        }

        for (int j = shelfList.Count - 1; j > 0; j--)
        {
            shelfList[j] = shelfList[j - 1];
        }

        shelfList[0] = Utils.RandomExclude(_localConfig.Shelfs.Count, shelfList.ToArray());
        return shelfList[0];
    }

    private void OnSwipeDetected(Vector2 direction)
    {
        if (_queueSwipes.Count >= _remoteConfigsService.Configs.GameConfig.NumberOfSwipes)
        {
            return;
        }
        
        var currentSwipe = Mathf.Clamp(_gameDataModel.Chef.CurrentShelf +
                                       Mathf.FloorToInt(direction.x) * (_localConfig.Shelfs.Count / 2) +
                                       Mathf.FloorToInt(direction.y) * -1, 0, _localConfig.Shelfs.Count - 1);

        if ((_gameDataModel.Chef.CurrentShelf == _localConfig.Shelfs.Count / 2 && currentSwipe == _localConfig.Shelfs.Count / 2 - 1) || 
            (_gameDataModel.Chef.CurrentShelf == _localConfig.Shelfs.Count / 2 - 1 && currentSwipe == _localConfig.Shelfs.Count / 2))
        {
            currentSwipe = _gameDataModel.Chef.CurrentShelf;
        }
            
        _queueSwipes.Enqueue(currentSwipe);
    }

    private void BackButtonPressedHandler()
    {
        LoadingLobbyScene();
    }
    
    private void RestartButtonPressedHandler()
    {
        RestartLevel();
    }
    
    private void LoadingLobbyScene()
    {
        SceneManager.LoadScene(SceneName.Lobby_Scene.ToString());
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneName.Game_Scene.ToString());
    }

#if UNITY_EDITOR
    private void DebugInfo()
    {
        _createTimeCurve = new AnimationCurve();
        _dropTimeCurve = new AnimationCurve();

        var dropTimes = new List<float>();

        for (var i = 0; i < _gameDataModel.Vegetables.Count; i++)
        {
            var createTime = _gameDataModel.Vegetables[i].CreateTime;
            var dropTime = _gameDataModel.Vegetables[i].CreateTime + _remoteConfigsService.Configs.GameConfig
                               .Vegetables[_gameDataModel.Vegetables[i].VegetableConfigIndex]
                               .DurationOfPath;
            _createTimeCurve.AddKey(i, createTime);
            _dropTimeCurve.AddKey(i, dropTime);
            Debug.Log("Vegetable[" + i + "]: createTime:" + createTime + ", dropTime:" + dropTime);
            dropTimes.Add(dropTime);
        }

        var message = "";
        dropTimes.Sort();
        foreach (var t in dropTimes)
        {
            message += t + " ";
        }

        Debug.Log("dropTimes: " + message);
    }
#endif
    
    private IEnumerator Prepare()
    {
        _btnBack.onClick.AddListener(BackButtonPressedHandler);
        _btnRestart.onClick.AddListener(RestartButtonPressedHandler);
        _gameDataModel = new GameDataModel();
        _gameDataModel.Chef.CurrentStars = _remoteConfigsService.Configs.GameConfig.StartStarsCount;
        var background = Resources.Load<Sprite>(_remoteConfigsService.Configs.GameConfig.GameBackGroundImageName);
        if (background == null)
        {
            throw new UnassignedReferenceException($"{nameof(background)}: {_remoteConfigsService.Configs.GameConfig.GameBackGroundImageName} not found!");
        }
        _backGround.sprite = background;
        _chefUI.Init(_localConfig, _gameDataModel.Chef);
        _starsUI.Init(_gameDataModel.Chef);
        _queueSwipes = new Queue<int>();
        _inputController = new SwipeController(OnSwipeDetected, _localConfig, _remoteConfigsService.Configs.GameConfig);

        float createStartTime = 0;
        var shelf = Random.Range(0, _localConfig.Shelfs.Count);
        shelfList = new List<int>();

        for (var i = 0; i < _remoteConfigsService.Configs.GameConfig.FreshVegatablesCount; i++)
        {
            shelf = GetIndexShelf(shelf);
            VegetableModel newVegetable;
            newVegetable = CreateVegetableModel(true, shelf);
            newVegetable.CreateTime = createStartTime - _remoteConfigsService.Configs.GameConfig.Vegetables[newVegetable.VegetableConfigIndex].DurationOfPath;
            _gameDataModel.Vegetables.Add(newVegetable);

            if (Random.value <= _remoteConfigsService.Configs.GameConfig.PercentageOfSpoiledVegetables)
            {
                
                shelf = GetIndexShelf(shelf);
                newVegetable = CreateVegetableModel(false, Utils.RandomExclude(_localConfig.Shelfs.Count, shelf));
                newVegetable.CreateTime = createStartTime - _remoteConfigsService.Configs.GameConfig.Vegetables[newVegetable.VegetableConfigIndex].DurationOfPath -
                                          _remoteConfigsService.Configs.GameConfig.VegetablesCreateRate / 2;
                _gameDataModel.Vegetables.Add(newVegetable);
            }

            createStartTime += _remoteConfigsService.Configs.GameConfig.VegetablesCreateRate;
        }
        
        _gameDataModel.Vegetables.Sort((a, b) =>
            {
                return a.CreateTime.CompareTo(b.CreateTime);
            }
        );

        for (int i = _gameDataModel.Vegetables.Count; i > 0; i--)
        {
            _gameDataModel.Vegetables[i-1].CreateTime -= (_gameDataModel.Vegetables[0].CreateTime - _remoteConfigsService.Configs.GameConfig.AfterStartDelay);
        }
        
#if UNITY_EDITOR
        DebugInfo();
#endif

        StartCoroutine(GameProcess());

        yield return null;
    }

    private IEnumerator GameProcess()
    {
        var startTime = Time.time;
        var respawnTime = startTime + _remoteConfigsService.Configs.GameConfig.AfterStartDelay;
        var swipeTime = Time.time;
        var indexOfVegetables = 0;
        Dictionary<VegetableModel, IVegetableControllerWithBadAndGoodScenarios> vegetableControllers = new Dictionary<VegetableModel, IVegetableControllerWithBadAndGoodScenarios>();
        while (true)
        {
            //Initialized Vegetables
            if (indexOfVegetables < _gameDataModel.Vegetables.Count && Time.time >= respawnTime)
            {
                GameObject prefab = _localConfig.VegetablePrefab;
                if (_gameDataModel.Vegetables[indexOfVegetables].CatchedByPaw)
                {
                    prefab = _localConfig.VegetableCatchedByPawPrefab;
                }
                if (_gameDataModel.Vegetables[indexOfVegetables].EscapedAndFallen)
                {
                    prefab = _localConfig.VegetableWithEscapedAndFallenPrefab;
                }
                if (_gameDataModel.Vegetables[indexOfVegetables].EscapedBehindScreen)
                {
                    prefab = _localConfig.VegetableWithEscapedBehindScreenPrefab;
                }
                GameObject go = Instantiate(prefab, _vegetablesRootParent);
                var vegetableController = go.GetComponent<IVegetableBaseController>();
                vegetableController.Init(_remoteConfigsService.Configs.GameConfig, _localConfig, _gameDataModel.Vegetables[indexOfVegetables]);
                if (vegetableController is IVegetableControllerWithBadAndGoodScenarios)
                {
                    vegetableControllers.Add(_gameDataModel.Vegetables[indexOfVegetables], (IVegetableControllerWithBadAndGoodScenarios) vegetableController);
                }
                indexOfVegetables++;
                if (indexOfVegetables < _gameDataModel.Vegetables.Count)
                {
                    respawnTime = startTime + _gameDataModel.Vegetables[indexOfVegetables].CreateTime;
                }
            }
            
            //Swipes
            if (Time.time >= swipeTime)
            {
                if (_queueSwipes.Count > 0)
                {
                    _gameDataModel.Chef.CurrentShelf = _queueSwipes.Dequeue();
                    swipeTime = Time.time + _remoteConfigsService.Configs.GameConfig.DurationOfSwipe;
                }
            }
            
            //Check states of Vegetables models
            int failures = 0;
            int total = 0;
            foreach (var vegetable in _gameDataModel.Vegetables)
            {
                if (!vegetable.Launched)
                {
                    continue;
                }
                
                if (vegetable.VegetableState == VegetableState.canСatch)
                {
                    vegetable.VegetableState = VegetableState.waitResult;
                    if (vegetable.ShelfIndex == _gameDataModel.Chef.CurrentShelf)
                    {
                        vegetableControllers[vegetable].RunGoodScenario();
                    }
                    else 
                    { 
                        vegetableControllers[vegetable].RunBadScenario();
                    }
                    
                }

                if ((vegetable.VegetableState == VegetableState.caught &&
                     _remoteConfigsService.Configs.GameConfig.Vegetables[vegetable.VegetableConfigIndex].IsFresh == false) || 
                    (vegetable.VegetableState == VegetableState.fallen &&
                     _remoteConfigsService.Configs.GameConfig.Vegetables[vegetable.VegetableConfigIndex].IsFresh == true))
                {
                    failures++;
                }

                if (vegetable.VegetableState == VegetableState.caught ||
                    vegetable.VegetableState == VegetableState.fallen ||
                    vegetable.VegetableState == VegetableState.catchByFeature)
                {
                    total++;
                }
            }

            _inputController.Update();
            
            _gameDataModel.Chef.CurrentStars = _remoteConfigsService.Configs.GameConfig.StartStarsCount - failures;
            
            if (total == _gameDataModel.Vegetables.Count)
            {
                _gameResult.text = "Молодец!";
                yield return new WaitForSeconds(_remoteConfigsService.Configs.GameConfig.PlayerWinDelay);
                RestartLevel();
                yield break;
            }

            if (_gameDataModel.Chef.CurrentStars <= 0)
            {
                _gameResult.text = "Игра окончена";
                yield return new WaitForSeconds(_remoteConfigsService.Configs.GameConfig.GameOverDelay);
                RestartLevel();
                yield break;
            }

            yield return null;
        }
    }
}
