using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropCategoryManager : MonoBehaviour
{
    public GameObject CurrentCategory;
    public List<GameObject> CategoryButtons;
    private GameObject[] _foundCategoryButtons;

    [SerializeField] private Transform _defaultParent;
    [SerializeField] private RectTransform _foundParent;

    public void ChangeCategory(GameObject nextCategory)
    {
        CurrentCategory.SetActive(false);
        CurrentCategory = nextCategory;
        CurrentCategory.SetActive(true);
    }

    public void FindCategory(String name)
    {
        if (_foundCategoryButtons != null) foreach(GameObject obj in _foundCategoryButtons) obj.transform.parent = _defaultParent;
        if (name != "")
        {
            _foundCategoryButtons = Array.FindAll(CategoryButtons.ToArray(), cat => cat.name.Contains(name));
            _foundParent.sizeDelta = new Vector2(_foundParent.sizeDelta.x, 50 * _foundCategoryButtons.Length);
            foreach (GameObject obj in _foundCategoryButtons) obj.transform.parent = _foundParent;
            _foundParent.gameObject.SetActive(true);
            _defaultParent.gameObject.SetActive(false);
        }
        else
        {
            CategoryButtons = CategoryButtons.OrderBy(c => c.name).ToList();
            foreach (GameObject cat in CategoryButtons)
            {
                cat.transform.parent = null;
                cat.transform.parent = _defaultParent;
            }
            _foundParent.gameObject.SetActive(false);
            _defaultParent.gameObject.SetActive(true);
        }
    }
}
