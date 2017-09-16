using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using FileManagement;
using Frontend.Authorization;
using Journalist;
using UserManagement.Domain;

namespace Frontend.Controllers
{
    public class FileController : ApiController
    {
        private readonly IFileManager _fileManager;

        public FileController(IFileManager fileManager)
        {
            Require.NotNull(fileManager, nameof(fileManager));

            _fileManager = fileManager;
        }

        [HttpGet]
        [Route("images/{imageName}")]
        public IHttpActionResult GetImage(string imageName)
        {
            try
            {
                return Ok(GetAnyFile(() => _fileManager.GetImage(imageName)));
            }
            catch (FileNotFoundException ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        [Authorization(AccountRoles.User)]
        [HttpPost]
        [Route("file")]
        public async Task<string> UploadImage()
        {
            try
            {
                var image = await _fileManager.UploadImageAsync(Request.Content);

                return Path.GetFileName(image.BigImage.LocalPath);
            }
            catch (NotSupportedException)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            catch (InvalidDataException)
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable);
            }
        }

        private HttpResponseMessage GetAnyFile(Func<Stream> getStream)
        {
            Stream stream;
            try
            {
                stream = getStream();
            }
            catch (FileNotFoundException)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(stream)
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
            return response;
        }
    }
}
