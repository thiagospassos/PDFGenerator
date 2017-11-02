using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;

namespace PdfGenerator
{
    public partial class PdfGeneratorClient
    {
        public static PdfGeneratorClient CreateClient(string baseUrl, NetworkCredential networkCredential = null)
        {
            var baseUri = new Uri(baseUrl);
            var handler = new HttpClientHandler();
            if (networkCredential != null)
            {
                handler.Credentials = networkCredential;
            }
            else
            {
                handler.UseDefaultCredentials = true;
            }

            return new PdfGeneratorClient(baseUri, handler);
        }
    }
}
