namespace AssimpUtf8.Test;

using AssimpUtf8.Test.Lib;

public class Utf8AssimpDllImportTests
{
    [Theory]
    [InlineData(Filenames.AsciiFilename, 13)]
    [InlineData(Filenames.AsciiFilenameWithTexture, 6)]
    public void AsciiWorksFine(string path, int materialCount)
    {
        var context = new AssimpContext_ImportFile_MarshalAs_LPUTF8Str();
        var scene = context.ImportFile(path);
        Assert.NotNull(scene);
        Assert.Equal(materialCount, scene.MaterialCount);
    }

    // This works fine. it's important for me.
    [Theory]
    [InlineData(Filenames.MultibyteFilename, 13)]
    [InlineData(Filenames.MultibyteFilenameWithTexture, 6)]
    public void ImportWorks_For_Filename_Includes_Multibyte_MarshalAs_LPUTF8Str(string path, int materialCount)
    {
        var context = new AssimpContext_ImportFile_MarshalAs_LPUTF8Str();
        var scene = context.ImportFile(path);
        Assert.NotNull(scene);
        Assert.Equal(materialCount, scene.MaterialCount);
    }

    [Fact]
    public void TestTextureFilename_In_Material_Of_UTF8Encoded_MtlFile()
    {
        var context = new AssimpContext_ImportFile_MarshalAs_LPUTF8Str();
        var scene = context.ImportFile(Filenames.MultibyteFilenameWithTexture);
        Assert.NotNull(scene);

        Assert.Equal(@".\SpiderTex🕸️.jpg", scene.Materials[3].TextureDiffuse.FilePath);
    }

    [Fact]
    public void THIS_TEST_IS_CURRENTLY_FAILURE_because_MaterialName_is_incorrect_even_if_It_uses_MarshalAs_LPUTF8Str_FYI()
    {
        var context = new AssimpContext_ImportFile_MarshalAs_LPUTF8Str();
        var scene = context.ImportFile(Filenames.MultibyteFilename);
        Assert.NotNull(scene);
        Assert.Equal(13, scene.MaterialCount);

        string[]? actualMaterialNames = scene.Materials.Select(static m => m.Name).ToArray();

        foreach (var (expected, actual) in ExpectedSceneInfo.MatrialNames.Zip(actualMaterialNames))
        {
            // success except for @"Terraindæk"
            if (expected == @"Terraindæk") continue;
            Assert.Equal(expected, actual);
        }

        //
        // Strictly every string needs to handle correctly,
        // in this case `Terraindæk` does not match (byte sequence of "æ" looks `mojibake`),
        // but I don't care it at present.
        // [A Field Guide to Japanese Mojibake](https://www.dampfkraft.com/mojibake-field-guide.html)
        //
        // It might be including another problem about code pages.
        //
        Assert.Equal(ExpectedSceneInfo.MatrialNames[9], actualMaterialNames[9]); // fail
    }
}
