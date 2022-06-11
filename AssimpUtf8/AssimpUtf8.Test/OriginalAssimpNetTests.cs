namespace AssimpUtf8.Test;

using Assimp;

public class OriginalAssimpNetTests
{
    [Fact]
    public void AsciiWorksFine()
    {
        var context = new AssimpContext();
        var scene = context.ImportFile(Filenames.AsciiFilename);
        Assert.NotNull(scene);
        Assert.Equal(13, scene.MaterialCount);
    }
    [Fact]
    public void ImportFileFromStreamIsNotAlsoAbleToHandleMultibyteCharacters()
    {
        var context = new AssimpContext();

        using var stream = new FileStream(Filenames.MultibyteFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        // no exception
        var scene = context.ImportFileFromStream(stream);

        Assert.NotNull(scene);
        // fail ... failed to read a MTL file for the same reason to an OBJ file in AssimpContextCouldNotImportFileWithMultibyteCharacters
        Assert.Equal(13, scene.MaterialCount);
    }
    [Fact]
    public void AssimpContextCouldNotImportFileWithMultibyteCharacters()
    {
        var context = new AssimpContext();

        // Assimp.AssimpException : Error importing file: Unable to open file ".\TestFiles\models\OBJ\box_mat_with_spaces??.obj".
        // because 📦 is not passed correctly to assimp
        var scene = context.ImportFile(Filenames.MultibyteFilename);

        Assert.NotNull(scene);
        Assert.Equal(13, scene.MaterialCount);
    }
}
