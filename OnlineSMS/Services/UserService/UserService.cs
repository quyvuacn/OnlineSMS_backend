using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineSMS.Data;
using OnlineSMS.Models;
using OnlineSMS.RequestModels;
using OnlineSMS.Services.UploadFile;
using System.Text.Json;

namespace OnlineSMS.Services.UserService
{
    public class UserService
    {
        private readonly UserManager<User> userManager;
        private readonly OnlineSMSContext context;
        private readonly UploadFileService uploadFile;

        public UserService(UserManager<User> userManager, OnlineSMSContext context,UploadFileService uploadFile)
        {
            this.userManager = userManager;
            this.context = context;
            this.uploadFile = uploadFile;
        }

        public async Task<RequestResult> GetOverviewProfile(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            var overview = await context.UserProfile.FirstOrDefaultAsync(x => x.UserId == userId);
            bool isExist = overview != null;
            string message = isExist ? "OK!" : "Yêu cầu cập nhật profile overview";

            return new RequestResult
            {
                IsSuccess = isExist,
                Message= message,
                Data = new {
                    UserId = user.Id,
                    user.Email,
                    user.PhoneNumber,
                    overview?.FullName,
                    overview?.Avatar,
                    overview?.MaritalStatus,
                    overview?.Address,
                    overview?.Gender,
                    DateOfBirth = overview?.DateOfBirth.ToString("yyyy-MM-dd")
                }
            };
        }

        public async Task<RequestResult> GetMoreProfile(string userId)
        {

            var hobbies = await context.UserHobbie.Where(x => x.UserId == userId).ToListAsync();
            var cuisines = await context.UserCuisine.Where(x => x.UserId == userId).ToListAsync();

            return new RequestResult
            {
                IsSuccess = true,
                Message = "Ok!",
                Data = new
                {
                    hobbies,
                    cuisines
                }
            };

        }

        public async Task<RequestResult> CheckProfile(string userId)
        {
            var overview = context.UserProfile.FirstOrDefault(u => u.UserId == userId);

            bool overviewExist = overview != null;

            return new RequestResult
            {
                IsSuccess = overviewExist,
                Message = overviewExist ? "Ok!" : "Yêu cầu cập nhật profile"
            };
        }

        public async Task<RequestResult> UpdateOverview(OvervieProfile overvieProfile,string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user== null)
            {
                return new RequestResult
                {
                    Message = "Tài khoản không tồn tại",
                };
            }

            var overview = context.UserProfile.FirstOrDefault(u=>u.UserId == userId);

            var newUserProfile = new UserProfile
            {
                UserId = userId,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = overvieProfile.FullName,
                Address = overvieProfile.Address,
                DateOfBirth = overvieProfile.DateOfBirth,
                Gender = overvieProfile.Gender,
                MaritalStatus = overvieProfile.MaritalStatus
            };

            if (overview==null)
            {
                overview = newUserProfile;
                context.UserProfile.Add(overview);
            }
            else
            {
                overview.Email = newUserProfile.Email ?? overview.Email;
                overview.PhoneNumber = newUserProfile.PhoneNumber ?? overview.PhoneNumber;
                overview.FullName = newUserProfile.FullName ?? overview.FullName;
                overview.Address = newUserProfile.Address ?? overview.Address;
                overview.DateOfBirth = newUserProfile.DateOfBirth != DateTime.MinValue ? newUserProfile.DateOfBirth : overview.DateOfBirth;
                overview.Gender = newUserProfile.Gender ?? overview.Gender;
                overview.MaritalStatus = newUserProfile.MaritalStatus ?? overview.MaritalStatus;

                context.Entry(overview).State = EntityState.Modified;
            }

            await context.SaveChangesAsync();

            return new RequestResult
            {
                IsSuccess = true,
                Message = "Cập nhật thành công"
            };

        }

        public async Task<RequestResult> CreateHobbie(string userId,string hobbieName)
        {
            var newHobbie = new UserHobbie
            {
                UserId = userId,
                Name = hobbieName
            };

            context.UserHobbie.Add(newHobbie);
            await context.SaveChangesAsync();

            return new RequestResult
            {
                IsSuccess = true,
                Message = "Ok!",
                Data = new {
                   hobbie = newHobbie
                }
            };
        }

        public async Task<RequestResult> CreateCuisine(string userId, string cuisineName)
        {
            var newCuisine = new UserCuisine
            {
                UserId = userId,
                Name = cuisineName
            };

            context.UserCuisine.Add(newCuisine);
            await context.SaveChangesAsync();

            return new RequestResult
            {
                IsSuccess = true,
                Message = "Ok!",
                Data = new
                {
                    cuisine = newCuisine
                }
            };
        }

        public async Task<RequestResult> DeleteCuisine(string userId, string cuisineId)
        {
            var cuisine = await context.UserCuisine.FindAsync(cuisineId);

            context.UserCuisine.Remove(cuisine);

            return new RequestResult
            {
                IsSuccess = true,
                Message = "Ok!",
            };
        }

        public async Task<RequestResult> DeleteHobbie(string userId, string hobbieId)
        {
            var hobbie = await context.UserHobbie.FindAsync(hobbieId);

            context.UserHobbie.Remove(hobbie);

            return new RequestResult
            {
                IsSuccess = true,
                Message = "Ok!",
            };
        }


        public async Task<RequestResult> UpdateAvatar(string userId,string blobUrl)
        {
            var result = uploadFile.UploadToPath(userId,blobUrl);

            if (result.IsSuccess)
            {
                var userProfile = await context.UserProfile.Where(u => u.UserId == userId).FirstOrDefaultAsync();
                userProfile.Avatar = result.Message;

                context.UserProfile.Update(userProfile);
                await context.SaveChangesAsync();
            }

            return result;
        }

    }
}
