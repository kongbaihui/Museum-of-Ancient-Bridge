using UnityEngine;

public enum ExhibitType
{
    Person,
    Knowledge
}

[System.Serializable]
public class ExhibitInfo
{
    public string colliderName;
    public ExhibitType exhibitType;

    public string title;
    public string subtitle;

    [TextArea(3, 8)] public string section01Title;
    [TextArea(3, 8)] public string section01Body;

    [TextArea(3, 8)] public string section02Title;
    [TextArea(3, 8)] public string section02Body;

    [TextArea(3, 8)] public string section03Title;
    [TextArea(3, 8)] public string section03Body;

    [TextArea(3, 8)] public string section04Title;
    [TextArea(3, 8)] public string section04Body;

    public Sprite mainImage;
    public Sprite detailImage;
    public Sprite[] galleryImages;
}
