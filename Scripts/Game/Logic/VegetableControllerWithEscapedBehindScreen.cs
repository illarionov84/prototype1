using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class VegetableControllerWithEscapedBehindScreen : VegetableBaseController
{
    protected override IEnumerator Mover()
    {
        yield return transform.position = StartPosition;
        float randomValueForPositionOnShelf = Mathf.Clamp(Random.value, _gameConfig.MinThresholdForPositionOnShelf, _gameConfig.MaxThresholdForPositionOnShelf);
        Vector3 randomPositionOnShelf = Vector3.Lerp(StartPosition, DropPosition, randomValueForPositionOnShelf);
        float randomTimeForPositionOnShelf = Mathf.Lerp(0, _gameConfig.Vegetables[_model.VegetableConfigIndex].DurationOfPath, randomValueForPositionOnShelf);
        _model.VegetableState = VegetableState.roll;
        if (_gameConfig.Vegetables[_model.VegetableConfigIndex].IsRotate)
        {
            _image.transform.DOKill();
        }
        yield return transform.DOMove(DropPosition, _gameConfig.Vegetables[_model.VegetableConfigIndex].DurationOfPath).SetEase(Ease.Linear).WaitForCompletion();
        yield return transform.DOMove(randomPositionOnShelf, _gameConfig.EscapedTime).SetEase(Ease.Linear).WaitForCompletion();
        yield return transform.DOMove(DropPosition, _gameConfig.Vegetables[_model.VegetableConfigIndex].DurationOfPath - randomTimeForPositionOnShelf).SetEase(Ease.Linear).WaitForCompletion();
        yield return transform.DOMove(StartPosition, _gameConfig.EscapedTime).SetEase(Ease.Linear).WaitForCompletion();;
        _model.VegetableState = VegetableState.catchByFeature;
        Destroy(gameObject);
    }
}
