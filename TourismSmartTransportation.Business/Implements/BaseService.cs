﻿using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces;
using TourismSmartTransportation.Data.Interfaces;

namespace TourismSmartTransportation.Business.Implements
{
    public class BaseService : IBaseService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly BlobServiceClient _blobServiceClient;
        private static readonly string[] _container = { "admin", "partner", "customer", "driver", "upload-file" };
        public BaseService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient)
        {
            _unitOfWork = unitOfWork;
            _blobServiceClient = blobServiceClient;
        }

        public static T UpdateTypeOfNullAbleObject<T>(T oldValue, T newValue)
        {
            return newValue != null ? newValue : oldValue;
        }
        public static T UpdateTypeOfNotNullAbleObject<T>(T oldValue, T? newValue) where T : struct
        {
            return newValue != null ? newValue.Value : oldValue;
        }

        public static T? UpdateTypeOfNotNullAbleObject<T>(T? oldValue, T? newValue) where T : struct
        {
            return newValue != null ? newValue.Value : oldValue != null ? oldValue.Value : null;
        }

        public static int SkipItemsOfPagingFunc(int itemPerPage, int totalRecord, int pageIndex)
        {
            return itemPerPage < totalRecord ? itemPerPage * Math.Max(pageIndex - 1, 0) : 0;
        }

        public static int TakeItemsOfPagingFunc(int itemPerPage, int totalRecord)
        {
            return itemPerPage < totalRecord && itemPerPage > 0 ? itemPerPage : totalRecord;
        }

        // Get number of pages need to show on UI
        public static int GetPageSize(int itemPerPage, int totalRecord)
        {
            return totalRecord != 0 ? itemPerPage == 0 ? 1 : (totalRecord / itemPerPage) + (totalRecord % itemPerPage > 0 ? 1 : 0) : 0;
        }

        // Function using for method OrderBySingleField
        public static string SortBy(string sortByField, string defaultField)
        {
            if (sortByField == null || sortByField.Trim() == "")
                sortByField = defaultField;

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sortByField.ToLower()); ;
        }


        // Return list after sorting
        public static List<T> GetListAfterSorting<T>(List<T> list, string sortBy) where T : class
        {
            var result = string.IsNullOrWhiteSpace(sortBy)
                            ? list.AsQueryable().ToList()
                            : list.AsQueryable().OrderByMultipleFields(sortBy).ToList();
            return result;
        }

        // Return total records from list
        public static int GetTotalRecord<T>(List<T> list,
                                                int itemsPerPage, int pageIndex) where T : class
        {
            var totalRecord = list.Count();
            if (GetPageSize(itemsPerPage, totalRecord) < pageIndex)
                totalRecord = 0;

            return totalRecord;
        }

        // Return list after paging
        public static List<T> GetListAfterPaging<T>(List<T> list, int itemsPerPage,
                                    int pageIndex, int totalRecord) where T : class
        {
            var result = list.AsQueryable()
                                        .Skip(SkipItemsOfPagingFunc(itemsPerPage, totalRecord, pageIndex))
                                        .Take(TakeItemsOfPagingFunc(itemsPerPage, totalRecord))
                                        .ToList();

            return result;
        }

        // Upload files to azure blob
        public async Task<string> UploadFile(IFormFile[] files, Container index)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(_container[(int)index]);
            if (blobContainer == null || files == null)
            {
                return null;
            }
            string result = string.Empty;
            foreach (IFormFile file in files)
            {
                string fileName = Guid.NewGuid().ToString() + "." + file.ContentType.Substring(6);
                var blobClient = blobContainer.GetBlobClient(fileName);
                await blobClient.UploadAsync(file.OpenReadStream());
                result += fileName + " ";
            }
            return result;
        }

        // Upload file to azure blob
        public async Task<string> UploadFile(IFormFile file, Container index)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(_container[(int)index]);
            if (blobContainer == null || file == null)
            {
                return null;
            }
            string fileName = Guid.NewGuid().ToString() + "." + file.ContentType.Substring(6);
            var blobClient = blobContainer.GetBlobClient(fileName);
            await blobClient.UploadAsync(file.OpenReadStream());
            return fileName + " ";
        }

        // Delete files from azure blob
        public async Task<string> DeleteFile(string[] fileNames, Container index, string photoUrl)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(_container[(int)index]);
            if (blobContainer == null || fileNames == null)
            {
                return photoUrl;
            }

            foreach (string fileName in fileNames)
            {
                if (photoUrl.Contains(fileName))
                {
                    try
                    {
                        var blobClient = blobContainer.GetBlobClient(fileName);
                        await blobClient.DeleteAsync();
                        photoUrl = photoUrl.Replace(fileName + " ", "");
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            return photoUrl;
        }

        // Delete file from azure blob
        public async Task<string> DeleteFile(string fileName, Container index, string photoUrl)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(_container[(int)index]);
            if (blobContainer == null || string.IsNullOrEmpty(fileName))
            {
                return photoUrl;
            }
            if (photoUrl.Contains(fileName))
            {
                try
                {
                    var blobClient = blobContainer.GetBlobClient(fileName);
                    await blobClient.DeleteAsync();
                    photoUrl = photoUrl.Replace(fileName + " ", "");
                }
                catch
                {
                    return photoUrl;
                }
            }
            return photoUrl;
        }

        public static string convertVietnameseStringToEnglish(string str)
        {
            string[] vietnameseSigns = new String[]
            {
                "aAeEoOuUiIdDyY",

                "áàạảãâấầậẩẫăắằặẳẵ",

                "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

                "éèẹẻẽêếềệểễ",

                "ÉÈẸẺẼÊẾỀỆỂỄ",

                "óòọỏõôốồộổỗơớờợởỡ",

                "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

                "úùụủũưứừựửữ",

                "ÚÙỤỦŨƯỨỪỰỬỮ",

                "íìịỉĩ",

                "ÍÌỊỈĨ",

                "đ",

                "Đ",

                "ýỳỵỷỹ",

                "ÝỲỴỶỸ"
            };
            for (int i = 1; i < vietnameseSigns.Length; i++)
            {
                for (int j = 0; j < vietnameseSigns[i].Length; j++)
                {
                    str = str.Replace(vietnameseSigns[i][j], vietnameseSigns[0][i - 1]);
                }
            }
            return str;
        }

        /// <summary>
        /// return username by firstname and lastname given.
        /// </summary>
        /// <returns></returns>
        public static string GenerateUserNameAuto(string firstname, string lastname)
        {
            string[] lastnameArr = Regex.Split(lastname.Trim(), @"\s+");
            string firstLetter = "";
            for (int i = 0; i < lastnameArr.Length; i++)
            {
                firstLetter += lastnameArr[i][0];
            }
            string username = firstname.Trim() + firstLetter;
            return convertVietnameseStringToEnglish(username);
        }

        /// <summary>
        /// return a random string
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateTextAuto(int lengthOfString, bool specialText = true)
        {
            var random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
            if (!specialText)
            {
                chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            }
            return new string(Enumerable.Repeat(chars, lengthOfString).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// return a random string
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GeneratePinCodeAuto(int lengthOfString)
        {
            var random = new Random();
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, lengthOfString).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static double ConvertDateTimeToTimestamp(DateTime value)
        {
            TimeSpan timeSpan = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
            return (double)timeSpan.TotalSeconds; // seconds 
        }
    }
}