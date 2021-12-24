using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FileExplorer : MonoBehaviour
{
    [SerializeField] private Transform fileListParent;
    [SerializeField] private GameObject fileRowPrefab;

    [SerializeField] private TMP_InputField dirInput;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;

    [field: SerializeField] public string CurrentPath { get; private set; }

    private List<string> history = new List<string>();
    private int currentHistoryIndex = 0;

    public List<FileIcon> FileIcons = new List<FileIcon>();

    public Sprite folderIcon;
    public Sprite unknownIcon;

    private void Update()
    {
        previousButton.interactable = currentHistoryIndex > 0;
        nextButton.interactable = currentHistoryIndex < history.Count - 1;
    }

    private void LoadDirectory(string dir)
    {
        if (!Directory.Exists(dir))
        {
            dirInput.text = CurrentPath;
            return;
        }

        foreach (Transform row in fileListParent.transform)
        {
            Destroy(row.gameObject);
        }

        List<string> files = new List<string>();

        files.AddRange(Directory.GetDirectories(dir));
        files.AddRange(Directory.GetFiles(dir));

        foreach (string file in files)
        {
            FileInfo fileInfo = new FileInfo(file);

            if (fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                continue;

            FileRow fileRow = Instantiate(fileRowPrefab, fileListParent).GetComponent<FileRow>();
            fileRow.connectedExplorer = this;
            fileRow.fileIcon = GetFileIcon(file);
            fileRow.FileName = Path.GetFileName(file);
            fileRow.Path = file;
            fileRow.lastModified = fileInfo.LastWriteTime;
            fileRow.IsFolder = Directory.Exists(file);
        }

        CurrentPath = dir;
        dirInput.text = CurrentPath;
    }

    public void LoadFolder(string folder)
    {
        if (currentHistoryIndex != history.Count - 1 && history.Count > 0)
        {
            history.RemoveRange(currentHistoryIndex + 1, history.Count - (currentHistoryIndex + 1));
        }

        LoadDirectory(folder);
        history.Add(folder);
        currentHistoryIndex = history.Count == 1 ? 0 : currentHistoryIndex + 1;
    }

    public void PreviousDir()
    {
        if (currentHistoryIndex <= 0)
            return;

        currentHistoryIndex--;
        LoadDirectory(history[currentHistoryIndex]);
    }

    public void NextDir()
    {
        if (currentHistoryIndex >= history.Count - 1)
            return;

        currentHistoryIndex++;
        LoadDirectory(history[currentHistoryIndex]);
    }

    public void RefreshDirectory()
    {
        LoadDirectory(CurrentPath);
    }

    private Sprite GetFileIcon(string file)
    {
        if (Directory.Exists(file))
            return folderIcon;

        string extension = Path.GetExtension(file).Substring(1);

        foreach (FileIcon fileIcon in FileIcons)
        {
            if (fileIcon.fileExtensions.Contains(extension))
            {
                return fileIcon.icon;
            }
        }

        return unknownIcon;
    }
}
