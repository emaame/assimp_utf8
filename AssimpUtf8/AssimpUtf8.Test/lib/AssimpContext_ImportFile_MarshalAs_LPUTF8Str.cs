namespace AssimpUtf8.Test.Lib;

using Assimp;
using Assimp.Unmanaged;
using System.Runtime.InteropServices;

public class AssimpContext_ImportFile_MarshalAs_LPUTF8Str
{
    [DllImport("assimp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "aiImportFileExWithProperties")]
    private static extern IntPtr AiImportFileExWithProperties_MarshalAsLPUTF8Str([In, MarshalAs(UnmanagedType.LPUTF8Str)] string file, uint flag, IntPtr fileIO, IntPtr propStore);

    public Scene ImportFile(string file)
    {
        return ImportFile(file, PostProcessSteps.None);
    }

    // comment out prepare and cleanup codes
    public Scene ImportFile(string file, PostProcessSteps postProcessFlags)
    {
#if false
        CheckDisposed();
#endif

        var ptr = IntPtr.Zero;
        var fileIO = IntPtr.Zero;
#if false
        //Only do file checks if not using a custom IOSystem
        if (UsingCustomIOSystem)
        {
            fileIO = m_ioSystem?.AiFileIO;
        }
        else if (String.IsNullOrEmpty(file) || !File.Exists(file))
        {
            throw new FileNotFoundException("Filename was null or could not be found", file);
        }

        PrepareImport();
#endif
        try
        {
            ptr = AiImportFileExWithProperties_MarshalAsLPUTF8Str(file, (uint)PostProcessSteps.None, fileIO, IntPtr.Zero);

            if (ptr == IntPtr.Zero)
                throw new AssimpException("Error importing file: " + AssimpLibrary.Instance.GetErrorString());

            TransformScene(ptr);

            if (postProcessFlags != PostProcessSteps.None)
                ptr = AssimpLibrary.Instance.ApplyPostProcessing(ptr, postProcessFlags);

            return Scene.FromUnmanagedScene(ptr);
        }
        finally
        {
#if false
            CleanupImport();
#endif

            if (ptr != IntPtr.Zero)
            {
                AssimpLibrary.Instance.ReleaseImport(ptr);
            }
        }
    }

    // copied necessary private instances / methods from AssimpContext
    // https://bitbucket.org/Starnick/assimpnet/src/master/AssimpNet/AssimpContext.cs

    private float m_scale = 1.0f;
    private float m_xAxisRotation = 0.0f;
    private float m_yAxisRotation = 0.0f;
    private float m_zAxisRotation = 0.0f;
    private bool m_buildMatrix = false;
    private Matrix4x4 m_scaleRot = Matrix4x4.Identity;
    //Build import transformation matrix

    private void BuildMatrix()
    {

        if (m_buildMatrix)
        {
            var scale = Matrix4x4.FromScaling(new Vector3D(m_scale, m_scale, m_scale));
            var xRot = Matrix4x4.FromRotationX(m_xAxisRotation * (float)(Math.PI / 180.0d));
            var yRot = Matrix4x4.FromRotationY(m_yAxisRotation * (float)(Math.PI / 180.0d));
            var zRot = Matrix4x4.FromRotationZ(m_zAxisRotation * (float)(Math.PI / 180.0d));
            m_scaleRot = scale * (xRot * yRot * zRot);
        }

        m_buildMatrix = false;
    }
    //Transforms the root node of the scene and writes it back to the native structure
    private bool TransformScene(IntPtr scene)
    {
        BuildMatrix();

        try
        {
            if (!m_scaleRot.IsIdentity)
            {
                var aiScene = MemoryHelper.MarshalStructure<AiScene>(scene);
                if (aiScene.RootNode == IntPtr.Zero)
                    return false;

                var matrixPtr = MemoryHelper.AddIntPtr(aiScene.RootNode, MemoryHelper.SizeOf<AiString>()); //Skip over Node Name

                var matrix = MemoryHelper.Read<Matrix4x4>(matrixPtr); //Get the root transform
                matrix = matrix * m_scaleRot; //Transform

                //Write back to unmanaged mem
                MemoryHelper.Write(matrixPtr, matrix);

                return true;
            }
        }
        catch (Exception)
        {

        }

        return false;
    }
}
