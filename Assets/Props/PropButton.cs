using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropButton : MonoBehaviour
{
    public Spawner Spawner;
    public RawImage Preview;
    public Text PropName;
    public GameObject Prefab;

    public void GeneratePreview()
    {
        PropName.text = Prefab.name;
        Preview.texture = RuntimePreviewGenerator.GenerateModelPreview(Prefab.transform, 128, 128);
    }

    public void Click()
    {
        Spawner.Spawn(Prefab);
    }
}
