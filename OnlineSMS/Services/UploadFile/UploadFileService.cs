using OnlineSMS.RequestModels;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace OnlineSMS.Services.UploadFile
{
    public class UploadFileService
    {
        private static Cloudinary cloudinary;
        public UploadFileService(IConfiguration configuration)
        {
            string cloudinaryUrl = configuration["CloudinaryUrl"];
            cloudinary = new Cloudinary(cloudinaryUrl);
            cloudinary.Api.Secure = true;
        }

        public RequestResult UploadToPath(string userId, string blobUrl)
        {
            string PublicId = userId + DateTime.Now.ToString("yyyyMMddHHmmss");
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(blobUrl),
                PublicId = PublicId,
                Overwrite = true,
            };

            var result = cloudinary.Upload(uploadParams);

            var url = cloudinary.Api.UrlImgUp.Transform(new Transformation()).BuildUrl(PublicId);

            return new RequestResult
            {
                IsSuccess = true,
                Data = url,
                Message = url
            };
        }
        
    }
}
