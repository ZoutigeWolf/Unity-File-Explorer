using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FileExplorer : MonoBehaviour
{
    [Header("File List")]
    [SerializeField] private ScrollRect filesScrollRect;
    [SerializeField] private Transform fileListParent;
    [SerializeField] private GameObject fileRowPrefab;
    [SerializeField] private Slider filesProgressBar;
    [SerializeField] private TMP_InputField dirInput;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;

    [Header("Drive List")]
    [SerializeField] private ScrollRect drivesScrollRect;
    [SerializeField] private Transform driveListParent;
    [SerializeField] private GameObject driveRowPrefab;

    [field: SerializeField] public string CurrentPath { get; private set; }

    private List<string> history = new List<string>();
    private int currentHistoryIndex = 0;

    public List<FileIcon> FileIcons = new List<FileIcon>();

    public Sprite folderIcon;
    public Sprite unknownIcon;

    [SerializeField] private float _loadingProgress = 0f;

    private void Start()
    {
        filesProgressBar.gameObject.SetActive(false);

        LoadDrives();
    }

    private void Update()
    {
        previousButton.interactable = currentHistoryIndex > 0;
        nextButton.interactable = currentHistoryIndex < history.Count - 1;

        filesProgressBar.value = _loadingProgress;
    }

    private IEnumerator LoadDirectory(string dir)
    {
        if (!Directory.Exists(dir))
        {
            dirInput.text = CurrentPath;
            yield break;
        }

        foreach (Transform row in fileListParent)
        {
            Destroy(row.gameObject);
        }

        List<string> files = new List<string>();

        files.AddRange(Directory.GetDirectories(dir));
        files.AddRange(Directory.GetFiles(dir));

        filesScrollRect.verticalScrollbar.value = 0f;

        filesProgressBar.gameObject.SetActive(true);

        _loadingProgress = 0f;
        int loadedFiles = 0;

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

            loadedFiles++;

            _loadingProgress = ((float)loadedFiles / files.Count);

            yield return null;
        }

        CurrentPath = dir;
        dirInput.text = CurrentPath;

        filesProgressBar.gameObject.SetActive(false);
    }

    private void LoadDrives()
    {
        foreach (Transform row in driveListParent)
        {
            Destroy(row.gameObject);
        }

        DriveInfo[] drives = DriveInfo.GetDrives();

        foreach(DriveInfo drive in drives)
        {
            DriveRow driveRow = Instantiate(driveRowPrefab, driveListParent).GetComponent<DriveRow>();
            driveRow.connectedExplorer = this;
            driveRow.Info = drive;
        }

        drivesScrollRect.verticalScrollbar.value = 0f;
    }

    public void LoadFolder(string folder)
    {
        if (currentHistoryIndex != history.Count - 1 && history.Count > 0)
        {
            history.RemoveRange(currentHistoryIndex + 1, history.Count - (currentHistoryIndex + 1));
        }

        StopAllCoroutines();
        StartCoroutine(LoadDirectory(folder));

        history.Add(folder);
        currentHistoryIndex = history.Count == 1 ? 0 : currentHistoryIndex + 1;
    }

    public void PreviousDir()
    {
        if (currentHistoryIndex <= 0)
            return;

        currentHistoryIndex--;
        StopAllCoroutines();
        StartCoroutine(LoadDirectory(history[currentHistoryIndex]));
    }

    public void NextDir()
    {
        if (currentHistoryIndex >= history.Count - 1)
            return;

        currentHistoryIndex++;
        StopAllCoroutines();
        StartCoroutine(LoadDirectory(history[currentHistoryIndex]));
    }

    public void RefreshDirectory()
    {
        LoadDrives();

        StopAllCoroutines();
        StartCoroutine(LoadDirectory(CurrentPath));
    }

    private Sprite GetFileIcon(string file)
    {
        if (Directory.Exists(file))
            return folderIcon;

        string extension;

        try
        {
            extension = Path.GetExtension(file).Substring(1);
        }
        catch
        {
            return unknownIcon;
        }

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
