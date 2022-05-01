using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreloadPropCategories : MonoBehaviour
{
    [SerializeField] private string[] _categories;
    [SerializeField] private GameObject _buttonPrefab, _categoryPrefab;
    [SerializeField] private RectTransform _buttonsParent, _categoriesParent;
    [SerializeField] private PropCategoryManager _propCategoryManager;
    [SerializeField] private Spawner _spawner;

    private void Start()
    {
        for (int i = 0; i < _categories.Length; i++)
        {
            _buttonsParent.sizeDelta += new Vector2(0, 50 * i);

            GameObject buttonObj = Instantiate(_buttonPrefab);
            buttonObj.transform.parent = _buttonsParent;
            buttonObj.name = _categories[i].Split('/')[0];
            buttonObj.GetComponentInChildren<Text>().text = buttonObj.name;
            _propCategoryManager.CategoryButtons.Add(buttonObj);

            GameObject categoryObj = Instantiate(_categoryPrefab);
            categoryObj.transform.parent = _categoriesParent;
            categoryObj.GetComponent<PreloadPropButtons>().Spawner = _spawner;
            categoryObj.GetComponent<PreloadPropButtons>().Folder = _categories[i];
            categoryObj.GetComponent<PreloadPropButtons>().CanPreload = true;
            categoryObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -50);
            if (i == 0) _propCategoryManager.CurrentCategory = categoryObj;
            else categoryObj.SetActive(false);

            buttonObj.GetComponent<Button>().onClick.AddListener(delegate { _propCategoryManager.ChangeCategory(categoryObj); });
        }
    }
}
