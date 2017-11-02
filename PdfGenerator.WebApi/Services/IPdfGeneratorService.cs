namespace PdfGenerator.WebApi.Services
{
    public interface IPdfGeneratorService
    {
        byte[] GeneratePdf(string documentName, byte[] data);
    }
}
