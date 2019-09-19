using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class VegetableController : VegetableBaseController, IVegetableControllerWithBadAndGoodScenarios
{
    public void RunGoodScenario()
    {
        StartCoroutine(CaughtProcess());
    }
    
    public void RunBadScenario()
    {
        StartCoroutine(FallenProcess());
    }

    protected override IEnumerator Mover()
    {
        yield return transform.position = StartPosition;
        _model.VegetableState = VegetableState.roll;
        yield return transform.DOMove(DropPosition, _gameConfig.Vegetables[_model.VegetableConfigIndex].DurationOfPath).SetEase(Ease.Linear).WaitForCompletion();
        yield return transform.DOMove(_localConfig.Shelfs[_model.ShelfIndex].EndPosition.position, _gameConfig.CatchDelay)
            .SetEase(Ease.Linear).WaitForCompletion();
        _model.VegetableState = VegetableState.canСatch;
    }

    private IEnumerator CaughtProcess()
    {
        _model.VegetableState = VegetableState.caught;
        Destroy(gameObject);
        yield return null;
    }
    
    private IEnumerator FallenProcess()
    {
        var RandomDropZone = new Vector3(_localConfig.DropZone.position.x + Random.Range(-_gameConfig.DropZoneRangeX, _gameConfig.DropZoneRangeX),
            _localConfig.DropZone.position.y + Random.Range(-_gameConfig.DropZoneRangeY, _gameConfig.DropZoneRangeY), 0);
        yield return transform.DOMove(RandomDropZone, _gameConfig.FallenDelay).SetEase(Ease.Linear).WaitForCompletion();
        if (_gameConfig.Vegetables[_model.VegetableConfigIndex].IsRotate)
        {
            _image.transform.DOKill();
        }
        var sprite = Resources.Load<Sprite>(_gameConfig.Vegetables[_model.VegetableConfigIndex].FallenSprite);
        if (sprite == null)
        {
            throw new UnassignedReferenceException($"{nameof(sprite)} FallenSprite: {_gameConfig.Vegetables[_model.VegetableConfigIndex].FallenSprite} not found!");
        }
        _image.sprite = sprite;
        _model.VegetableState = VegetableState.fallen;
    }
}
