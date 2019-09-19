using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarsUI : MonoBehaviour
{
    private ChefModel _chefModel;
    [SerializeField] private Sprite _starOff;
    [SerializeField] private Sprite _starOn;
    [SerializeField] private GameObject _starPrefab;
    [SerializeField] private List<GameObject> _stars;

    public void Init(ChefModel chefModel)
    {
        _chefModel = chefModel;
        for (var i = 0; i < _chefModel.CurrentStars; i++)
        {
            var go = Instantiate(_starPrefab, transform);
            _stars.Add(go);
        }

        StartCoroutine(MovelWatcher());
    }

    private IEnumerator MovelWatcher()
    {
        var StarsCountInView = _chefModel.CurrentStars;
        while (true)
        {
            if (StarsCountInView != _chefModel.CurrentStars)
            {
                for (var i = 0; i < StarsCountInView - _chefModel.CurrentStars; i++)
                {
                    _stars[_chefModel.CurrentStars - i].GetComponent<Image>().sprite = _starOff;
                    _stars[_chefModel.CurrentStars - i].GetComponent<Image>().SetNativeSize();
                }
                StarsCountInView = _chefModel.CurrentStars;
            }

            yield return null;
        }
    }
}