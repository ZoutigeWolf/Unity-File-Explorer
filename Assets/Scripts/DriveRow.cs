using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class DriveRow : MonoBehaviour
{
    public FileExplorer connectedExplorer;

    [field: SerializeField] public DriveInfo Info { get; set; }

    [SerializeField] private TextMeshProUGUI driveNameText;

    private float _doubleClickTime = 0f;

    private void Start()
    {
        if (Info != null)
            driveNameText.text = $"{Info.Name} {Info.VolumeLabel}";
    }

    private void Update()
    {
        _doubleClickTime = _doubleClickTime <= 0f ? 0f : _doubleClickTime - Time.deltaTime;
    }

    public void OnClick()
    {
        if (_doubleClickTime > 0f)
        {
            _doubleClickTime = 0f;
            OnDoubleClick();
        }
        else
        {
            _doubleClickTime = 0.25f;
        }
    }

    public void OnDoubleClick()
    {
        connectedExplorer.LoadFolder(Info.Name);
    }
}
