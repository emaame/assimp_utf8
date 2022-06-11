namespace AssimpUtf8.Test;

public static class Filenames
{
    private const string _path = @".\TestFiles\models\OBJ\";
    private const string _asciiFilename = @"box_mat_with_spaces.obj";

    // use `emoji` as a popular multibyte character for English people
    private const string _multibyteFilename = @"box_mat_with_spaces📦.obj";
    private const string _asciiFilenameWithTexture = @"spider.obj";

    // we usually aviod to use multibyte characters to filenames,
    // but it is slipped in to folder names unintentionally (or as copied filename's postfix at local language).
    private const string _multibyteFilenameWithTexture = @"📁\Spider🕸️.obj";

    public const string AsciiFilename = _path + _asciiFilename;
    public const string MultibyteFilename = _path + _multibyteFilename;
    public const string AsciiFilenameWithTexture = _path + _asciiFilenameWithTexture;
    public const string MultibyteFilenameWithTexture = _path + _multibyteFilenameWithTexture;
}

public static class ExpectedSceneInfo
{
    public static readonly string[] MatrialNames = new[] {
        "DefaultMaterial",
        "Material name with many, many spaces",
        "Door",
        "Floor",
        "Rafter",
        "Ridging",
        "Sill",
        "Site",
        "Roof",
        "Terraindæk",
        "Wall-inner",
        "Wall-out",
        "Windows",
    };
}
