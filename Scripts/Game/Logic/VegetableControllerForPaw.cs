using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class VegetableControllerForPaw : VegetableBaseController
{
    [SerializeField] private Transform _paw;
    
    protected override IEnumerator Mover()
    {
        yield return transform.position = StartPosition;
        float randomValueForPositionOnShelf = Mathf.Clamp(Random.value, _gameConfig.MinThresholdForPositionOnShelf, _gameConfig.MaxThresholdForPositionOnShelf);
        Vector3 randomPositionOnShelf = Vector3.Lerp(StartPosition, DropPosition, randomValueForPositionOnShelf);
        float randomTimeForPositionOnShelf = Mathf.Lerp(0, _gameConfig.Vegetables[_model.VegetableConfigIndex].DurationOfPath, randomValueForPositionOnShelf);
        _model.VegetableState = VegetableState.roll;
        _image.transform.DOMove(randomPositionOnShelf, randomTimeForPositionOnShelf).SetEase(Ease.Linear);
        yield return new WaitForSeconds(Mathf.Clamp(randomTimeForPositionOnShelf - _gameConfig.DurationOfHitPaw, 0, randomTimeForPositionOnShelf));
        yield return _paw.transform.DOMove(randomPositionOnShelf, _gameConfig.DurationOfHitPaw).SetEase(Ease.Linear).WaitForCompletion();
        if (_gameConfig.Vegetables[_model.VegetableConfigIndex].IsRotate)
        {
            _image.transform.DOKill();
        }
        _image.transform.DOLocalMove(Vector3.zero, _gameConfig.DurationOfRemovePaw).SetEase(Ease.Linear);
        yield return _paw.transform.DOLocalMove(Vector3.zero, _gameConfig.DurationOfRemovePaw).SetEase(Ease.Linear).WaitForCompletion();
        _model.VegetableState = VegetableState.catchByFeature;
        Destroy(gameObject);
    }
}
