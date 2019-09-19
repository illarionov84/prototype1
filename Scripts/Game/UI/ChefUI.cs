using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChefUI : MonoBehaviour
{
    [SerializeField] private Image _chefImage;
    private ChefModel _chefModel;
    private LocalConfig _localConfig;
    [SerializeField] private List<Sprite> _spriteByState;

    public void Init(LocalConfig localConfig, ChefModel chefModel)
    {
        _localConfig = localConfig;
        _chefModel = chefModel;

        StartCoroutine(ModelWatcher());
    }

    private IEnumerator ModelWatcher()
    {
        var ShelfInView = _chefModel.CurrentShelf;
        while (true)
        {
            if (ShelfInView != _chefModel.CurrentShelf)
            {
                ShelfInView = _chefModel.CurrentShelf;
                _chefImage.sprite = _spriteByState[_chefModel.CurrentShelf];
                var tmp = transform.localPosition;
                tmp.x = Mathf.Abs(tmp.x) * (_chefModel.CurrentShelf < _localConfig.Shelfs.Count / 2 ? -1 : 1);
                transform.localPosition = tmp;
            }

            yield return null;
        }
    }
}