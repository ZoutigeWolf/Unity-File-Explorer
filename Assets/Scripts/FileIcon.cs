using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "File Icon", fileName = "New File Icon")]
public class FileIcon : ScriptableObject
{
    public Sprite icon;
    public List<string> fileExtensions = new List<string>();
}
