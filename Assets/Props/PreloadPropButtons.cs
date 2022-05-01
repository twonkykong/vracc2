using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreloadPropButtons : MonoBehaviour
{
    [SerializeField] private GameObject _buttonPrefab;

    public RectTransform ButtonsRectTransform;
    public Spawner Spawner;
    public string Folder;

    private GameObject[] _prefabs;
    private int _amount = 0;

    public bool CanPreload = false;

    public void Start()
    {
        StartCoroutine(Generate());
    }

    private IEnumerator Generate()
    {
        yield return new WaitUntil(() => CanPreload == true);
        _prefabs = Resources.LoadAll<GameObject>(Folder);

        foreach (GameObject obj in _prefabs)
        {
            yield return new WaitForEndOfFrame();
            GameObject button = Instantiate(_buttonPrefab);
            button.GetComponent<RectTransform>().SetParent(ButtonsRectTransform.transform);
            button.GetComponent<PropButton>().Prefab = obj;
            button.GetComponent<PropButton>().Spawner = Spawner;
            button.GetComponent<PropButton>().GeneratePreview();

            _amount += 1;
            if (_amount % 4 == 0) ButtonsRectTransform.sizeDelta += new Vector2(0, 198);
        }
    }
}
