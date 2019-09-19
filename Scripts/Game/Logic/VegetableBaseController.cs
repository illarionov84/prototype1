using System.Collections;
using DG.Tweening;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public abstract class VegetableBaseController : MonoBehaviour, IVegetableBaseController
{
    [SerializeField] protected Image _image;
    protected GameConfig _gameConfig;
    protected LocalConfig _localConfig;
    protected VegetableModel _model;
    protected Vector3 StartPosition;
    protected Vector3 DropPosition;
    
    public void Init(GameConfig gameConfig, LocalConfig localConfig,VegetableModel model)
    {
        _gameConfig = gameConfig;
        _localConfig = localConfig;
        _model = model;
        _model.Launched = true;
        var sprite = Resources.Load<Sprite>(_gameConfig.Vegetables[_model.VegetableConfigIndex].RollSprite);
        if (sprite == null)
        {
            throw new UnassignedReferenceException($"{nameof(sprite)} RollSprite: {_gameConfig.Vegetables[_model.VegetableConfigIndex].RollSprite} not found!");
        }
        _image.sprite = sprite;
        _image.SetNativeSize();
        var tmp = transform.localScale;
        tmp.x = Mathf.Abs(tmp.x) * (_model.ShelfIndex < _localConfig.Shelfs.Count / 2 ? 1 : -1);
        transform.localScale = tmp;
        if (_gameConfig.Vegetables[_model.VegetableConfigIndex].IsRotate)
        {
            _image.transform.DOLocalRotate(new Vector3(0, 0, -180), _gameConfig.Vegetables[_model.VegetableConfigIndex].RotateDegreesPerSecond).SetSpeedBased().SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
        }
        StartPosition = new Vector3(
            _localConfig.Shelfs[_model.ShelfIndex].StartPosition.position.x,
            _localConfig.Shelfs[_model.ShelfIndex].StartPosition.position.y +
            _gameConfig.Vegetables[_model.VegetableConfigIndex].HeightAboveShelf,
            _localConfig.Shelfs[_model.ShelfIndex].StartPosition.position.z);
        DropPosition = new Vector3(
            _localConfig.Shelfs[_model.ShelfIndex].DropPosition.position.x,
            _localConfig.Shelfs[_model.ShelfIndex].DropPosition.position.y +
            _gameConfig.Vegetables[_model.VegetableConfigIndex].HeightAboveShelf,
            _localConfig.Shelfs[_model.ShelfIndex].DropPosition.position.z);
        StartCoroutine(Mover());
    }

    protected abstract IEnumerator Mover();
}
