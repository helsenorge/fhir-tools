namespace FhirTool.Core.Operations
{
    public enum OperationEnum
    {
        None = 0,
        GenerateResource = 1,
        UploadResource = 2,
        UploadFhirDefinitions = 3,
        BundleResources = 4,
        SplitBundle = 5,
        TransferData = 6,
        VerifyValidation = 7,
        Convert = 8,
        DownloadResources = 9,
    }
}
