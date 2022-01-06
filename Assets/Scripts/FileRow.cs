using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FileRow : MonoBehaviour
{
    public FileExplorer connectedExplorer;

    [field: SerializeField] public Sprite fileIcon { get; set; }    
    [field: SerializeField] public string FileName { get; set; }
    [field: SerializeField] public DateTime lastModified { get; set; }
    [field: SerializeField] public string Path { get; set; }

    [field: SerializeField] public bool IsFolder { get; set; }

    [Space]

    [SerializeField] private Image fileIconImage;
    [SerializeField] private TextMeshProUGUI fileNameText;
    [SerializeField] private TextMeshProUGUI lastModifiedText;
    
    private float _doubleClickTime = 0f;

    private void Start()
    {
        fileIconImage.sprite = fileIcon;
        fileNameText.text = FileName;
        lastModifiedText.text = lastModified.ToString("dd-M-yyyy H:mm");
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
        if (IsFolder)
        {
            connectedExplorer.LoadFolder(Path);
        }
        else
        {
            Application.OpenURL(Path);
        }
    }

    public void Duplicate()
    {
        connectedExplorer.DuplicateFile(Path);
    }

    public void Delete()
    {
        if (connectedExplorer.DeleteFile(Path))
            Destroy(gameObject);
    }
}