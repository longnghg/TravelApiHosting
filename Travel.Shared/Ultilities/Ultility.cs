using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Travel.Context.Models;
using Travel.Shared.ViewModels;

namespace Travel.Shared.Ultilities
{

    public static class Ultility
    {
        private static Notification message = new Notification();
        private static Image image = new Image();
        private static List<Image> imageList = new List<Image>();
        private const string CLOUD_NAME = "ddv2idi9d";
        private const string API_KEY = "687389283419199";
        private const string API_SECRET = "BOCNwD1_s-DwP67WIkwNkuURBtE";
        private static Cloudinary cloudinary;
        private static string publicId;
        private static string link;
        public static List<T> Shuffle<T>(this List<T> list, Random rnd)
        {
            for (var i = list.Count; i > 0; i--)
                list.Swap(0, rnd.Next(0, i));

            return list.Take(4).ToList();
        }
        public static void Swap<T>(this List<T> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
        public static T DeepCopy<T>(this T self)
        {
            var serialized = JsonSerializer.Serialize(self, new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return JsonSerializer.Deserialize<T>(serialized, new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve
            });
        }
        public static Response Responses(string message, string type, object content = null, string description = null)
        {
            Response res = new Response();
            res.Notification.DateTime = DateTime.Now;
            res.Notification.Description = description;
            res.Content = content;
            res.Notification.Messenge = message;
            res.Notification.Type = type;
            return res;
        }
        public static string GenerateRandomCode()
        {
            Random random = new Random();
            string s = "";
            for (int i = 0; i < 6; i++)
                s = String.Concat(s, random.Next(10).ToString());
            return s;
        }



        #region String Handle
        public static string SEOUrl(string url)
        {
            url = url.ToLower();
            url = Regex.Replace(url, @"[áàạảãâấầậẩẫăắằặẳẵ]", "a");
            url = Regex.Replace(url, @"[éèẹẻẽêếềệểễ]", "e");
            url = Regex.Replace(url, @"[óòọỏõôốồộổỗơớờợởỡ]", "o");
            url = Regex.Replace(url, @"[íìịỉĩ]", "i");
            url = Regex.Replace(url, @"[ýỳỵỉỹ]", "y");
            url = Regex.Replace(url, @"[úùụủũưứừựửữ]", "u");
            url = Regex.Replace(url, @"[đ]", "d");

            //2. Chỉ cho phép nhận:[0-9a-z-\s]
            url = Regex.Replace(url.Trim(), @"[^0-9a-z-\s]", "").Trim();
            //xử lý nhiều hơn 1 khoảng trắng --> 1 kt
            url = Regex.Replace(url.Trim(), @"\s+", "-");
            //thay khoảng trắng bằng -
            url = Regex.Replace(url, @"\s", "-");
            while (true)
            {
                if (url.IndexOf("--") != -1)
                {
                    url = url.Replace("--", "-");
                }
                else
                {
                    break;
                }
            }
            return url;
        }
        public static string GenerateId(string phraseName)
        {
            phraseName = Regex.Replace(phraseName, @"\s+", " ");
            string[] words = phraseName.Split(' ');
            var resultString = new StringBuilder();
            foreach (var word in words)
            {
                resultString.Append(word.Substring(0, 1).ToUpper());
            }
            resultString.Append("-");
            resultString.Append(ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now));
            return resultString.ToString();
        }
        public static string removeVietnameseSign(string content)
        {
            string temp = "";

            if (!string.IsNullOrEmpty(content))
            {
                try
                {
                    Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
                    temp = content.Normalize(NormalizationForm.FormD).Trim();
                    Array BadCommands = ";,--,create,drop,select,insert,delete,update,union,sp_,xp_".Split(new Char[] { ',' });
                    temp = (regex.Replace(temp, String.Empty)
                        .Replace('\u0111', 'd')
                        .Replace('\u0110', 'D')
                        .Replace(",", " ")
                        .Replace(".", " ")
                        .Replace(":", " ")
                        .Replace("!", " ")
                        .Replace(";", " ")
                        .Replace("/", " ")
                        .Replace("&", " ")
                        .Replace("%", " ")
                        .Replace("*", " ")
                        .Replace("?", " "));
                }
                catch { temp = ""; }
            }
            return temp;
        }

        public static string RandomPassword()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(1, true));
            builder.Append(RandomNumber(0, 9));
            builder.Append(RandomNumber(0, 9));
            builder.Append(RandomString(2, true));
            builder.Append(RandomNumber(0, 9));
            builder.Append(RandomString(1, true));
            return builder.ToString();
        }

        public static string RandomString(int Size, bool LowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < Size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
                Thread.Sleep(10);
            }
            if (LowerCase)
            {
                return builder.ToString().ToLower();
            }
            return builder.ToString();
        }
        #endregion


        public static string ConvertListInt(int[] list)
        {
            Random random = new Random();
            string s = "";
            for (int i = 0; i < list.Length; i++)
                if (i > 0)
                {
                    if (i < list.Length)
                    {
                        s += ",";
                    }

                    s += list[i].ToString();
                }
                else
                {
                    s = list[i].ToString();
                }
            return s;
        }

        #region Date Handle
        public static string CountDay(DateTime fromdate, DateTime todate)
        {
            string result = string.Empty;
            if (fromdate == DateTime.MinValue || todate == DateTime.MinValue || fromdate > todate)
            {
                result = "N/A";
            }
            else
            {
                if (fromdate.ToString("yyyyMMdd") == todate.ToString("yyyyMMdd"))
                {
                    result = "1 ngày";
                }
                else
                {
                    var day = (todate.Date - fromdate.Date).TotalDays;
                    result = string.Format("{0}N{1}Đ", day + 1, day);

                    //DateTime fDate = fromdate;
                    //while (fDate.ToString("yyyyMMdd") != todate.ToString("yyyyMMdd"))
                    //{
                    //    day++;
                    //    fDate = fromdate.AddDays(day);
                    //}
                    //result = string.Format("{0}N{1}Đ", day + 1, day);
                }
            }
            return result.Substring(0, 1);
        }
        public static DateTime GetDateZeroTime(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 1);
        }
        public static long ConvertDatetimeToUnixTimeStampMiliSecond(DateTime date)
        {
            try
            {
                var dateTimeOffset = new DateTimeOffset(date);
                var unixDateTime = dateTimeOffset.ToUnixTimeMilliseconds();
                return unixDateTime;
            }
            catch
            {
                return 0;
            }

        }
        #endregion


        public static void uploadQR(Stream stream, string name, string folderPath)
        {
            try
            {
                var uploadParams = new ImageUploadParams()
                {
                    Folder = folderPath,
                    File = new FileDescription(name,stream),
                    UseFilename = true
                };

                var uploadResult = cloudinary.Upload(uploadParams);
                publicId = $"lia/Folder/{uploadResult.PublicId}";
                link = uploadResult.Uri.ToString();
            }
            catch (Exception e)
            {

                Ultility.Responses("lỗi khi thêm vào cloud !", Enums.TypeCRUD.Success.ToString());
            }
        }

        public static string UploadQR(Stream stream, string idService, ref Notification _message)
        {
            try
            {
                var d = DateTime.Now;
                string dateTimeString = $"{d.Year}{d.Month}{d.Day}{d.Hour}{d.Minute}{d.Second}";
                var nameFile = $"QR{dateTimeString}";
                var folderPath = "/Upload/QR";

                #region upload cloud
                //up lên cloud
                Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
                cloudinary = new Cloudinary(account);
                uploadQR(stream, nameFile, folderPath);
                #endregion
                return link;
            }
            catch (Exception e)
            {
                message.Messenge = "Có lỗi xảy ra khi lưu file !";
                message.Type = "Error";
                message.Description = e.Message;
                _message = message;

                return null;
            }
        }


        public static void uploadImage(string filePath, string folderPath)
        {
            try
            {
                var uploadParams = new ImageUploadParams()
                {
                    Folder = folderPath,
                    File = new FileDescription(filePath),
                    UseFilename = true
                };

                var uploadResult = cloudinary.Upload(uploadParams);
                publicId = $"lia/Folder/{uploadResult.PublicId}";
                link = uploadResult.Uri.ToString();
            }
            catch (Exception e)
            {

                Ultility.Responses("lỗi khi thêm vào cloud !", Enums.TypeCRUD.Success.ToString());
            }
        }





        public static Image WriteFile(IFormFile file, string type, string idService, ref Notification _message, int orderby = 0)
        {
            try
            {
                var folder = Directory.GetCurrentDirectory() + @"\wwwroot";
                string path = Path.Combine(folder, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string pathType = Path.Combine(path, type);

                if (!Directory.Exists(pathType))
                {
                    Directory.CreateDirectory(pathType);
                }

                string pathId = Path.Combine(pathType, idService.ToString());

                if (!Directory.Exists(pathId))
                {
                    Directory.CreateDirectory(pathId);
                }
                var date = Ultility.FormatDateToInt(DateTime.Now, "DDMMYYYY").ToString();

                string pathDate = Path.Combine(pathId, date);
                if (!Directory.Exists(pathDate))
                {
                    Directory.CreateDirectory(pathDate);
                }
                //get file extension
                //string[] str = file.FileName.Split('.');
                string fileName = "";
                if (orderby > 0)
                {
                    fileName = orderby.ToString() + "_" + Ultility.FormatDateToInt(DateTime.Now, "DDMMYYYYHHMMSS").ToString() + Path.GetExtension(file.FileName);
                }
                else
                {
                    fileName = Ultility.FormatDateToInt(DateTime.Now, "DDMMYYYYHHMMSS").ToString() + Path.GetExtension(file.FileName);
                }
                string fullpath = Path.Combine(pathDate, fileName);
                string folderPath = "/Uploads/" + type + "/" + idService + "/" + date;
                string serverPath = "/Uploads/" + type + "/" + idService + "/" + date + "/" + fileName;
                if (Directory.Exists(fullpath))
                {
                    System.IO.File.Delete(fullpath);

                }
                using (var stream = new FileStream(fullpath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                //up lên cloud
                Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
                cloudinary = new Cloudinary(account);
                string imagePath = Path.GetFullPath(fullpath);
                uploadImage(imagePath, folderPath);

                image.IdImage = Guid.NewGuid();
                image.NameImage = fileName;
                image.Extension = Path.GetExtension(file.FileName).Replace(".", "");
                image.IdService = idService;
                image.Size = file.Length;
                image.FilePath = link;
                image.IsDelete = false;
                return image;
            }
            catch (Exception e)
            {
                message.Messenge = "Có lỗi xảy ra khi lưu file !";
                message.Type = "Error";
                message.Description = e.Message;
                _message = message;

                return image;
            }
        }

        public static List<Image> WriteFiles(ICollection<IFormFile> files, string type, string idService, ref Notification _message, int orderby = 0)
        {
            try
            {
                imageList = new List<Image>();
                if (files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        Image imageDetail = new Image();

                        var folder = Directory.GetCurrentDirectory() + @"\wwwroot";
                        string path = Path.Combine(folder, "Uploads");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        string pathType = Path.Combine(path, type);

                        if (!Directory.Exists(pathType))
                        {
                            Directory.CreateDirectory(pathType);
                        }

                        string pathId = Path.Combine(pathType, idService.ToString());

                        if (!Directory.Exists(pathId))
                        {
                            Directory.CreateDirectory(pathId);
                        }
                        var date = Ultility.FormatDateToInt(DateTime.Now, "DDMMYYYY").ToString();

                        string pathDate = Path.Combine(pathId, date);
                        if (!Directory.Exists(pathDate))
                        {
                            Directory.CreateDirectory(pathDate);
                        }
                        //get file extension
                        //string[] str = file.FileName.Split('.');
                        string fileName = "";
                        if (orderby > 0)
                        {
                            fileName = orderby.ToString() + "_" + Ultility.FormatDateToInt(DateTime.Now, "DDMMYYYYHHMMSS").ToString() + Path.GetExtension(file.FileName);
                        }
                        else
                        {
                            fileName = Ultility.FormatDateToInt(DateTime.Now, "DDMMYYYYHHMMSS").ToString() + Path.GetExtension(file.FileName);
                        }
                        string fullpath = Path.Combine(pathDate, fileName);
                        string folderPath = "/Uploads/" + type + "/" + idService + "/" + date;
                        string serverPath = "/Uploads/" + type + "/" + idService + "/" + date + "/" + fileName;
                        if (Directory.Exists(fullpath))
                        {
                            System.IO.File.Delete(fullpath);

                        }
                        using (var stream = new FileStream(fullpath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
                        cloudinary = new Cloudinary(account);
                        string imagePath = Path.GetFullPath(fullpath);
                        uploadImage(imagePath, folderPath);

                        imageDetail.IdImage = Guid.NewGuid();
                        imageDetail.NameImage = fileName;
                        imageDetail.Extension = Path.GetExtension(file.FileName).Replace(".", "");
                        imageDetail.IdService = idService;
                        imageDetail.Size = file.Length;
                        imageDetail.FilePath = link;
                        imageDetail.IsDelete = false;
                        imageList.Add(imageDetail);
                    }
                }
                return imageList;
            }
            catch (Exception e)
            {
                message.Messenge = "Có lỗi xảy ra khi lưu file !";
                message.Type = "Error";
                message.Description = e.Message;
                _message = message;

                return imageList;
            }
        }

        public static string getHtmlBookingSuccess(string pincode,string fullname, string phone, string totalamount, string qr, string statuspayment)
        {
            try
            {
                string body = @"<!DOCTYPE html PUBLIC ' -//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'><html xmlns='http://www.w3.org/1999/xhtml'><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8' /> <link href='https://fonts.googleapis.com/css?family=Roboto' rel='stylesheet' /> <title>Optional Tour</title><style type='text/css'> body {font-family: 'Roboto', sans-serif;font-size: 14px;min-width: 760px;}.f-left {float: left;}.mg-left5 {margin-left: 5px;}.td-left { background: #f4f5f6; border-bottom: 1px solid #fff;margin: 0; padding: 5px 10px;text-align: right;vertical-align: middle;width: 150px;}.td-right {border-bottom: 1px solid #f4f5f6;font-size: 14px;line-height: 20px;margin: 0;padding: 5px 10px;vertical-align: middle;}.chitietbooking table, .thongtinlienlac table {border: 1px solid #f4f5f6;border-collapse: collapse;margin: 0;padding: 0;width: 100%;}</style></head><body><div class='container'style='    background: linear-gradient(-180deg, #1ae28a, #209077 150%);margin:auto;border: 1px solid #505050;  border-radius: 10px;     width: 60%;  height: 500px;'><table cellpadding='0' cellspacing='0' width='760' border='0'style='margin: auto;top: 10px;' ><tr><td colspan='2'><table width='100%'><tr><td style='width: 100px;'><a href='https://travel.com.vn/Content/Theme/images/logo.png'><img src='https://res.cloudinary.com/ddv2idi9d/image/upload/v1670731543/Upload/Logo/Logoaiu_ntvora.png' width='208px' height='70px' alt='logo' /></a></td><td align='right'><div style='text-align: right;margin-right: 2.5rem;'><a href='tel:19001839'><span style='color: #c50000; font-weight: bold; font-size: 20px;'>1900 1839</span></a><br /><span style='color: #333; font-size: 11px;'>cước gọi 1000đ/phút</span></div></td></tr></table></td></tr><tr><td colspan='2'><div style='text-align: center; font-weight: bold; text-transform: uppercase; color: #000; font-size: 24px; padding-top: 20px; padding-bottom: 20px;border-bottom: 1px dotted #ccc; border-top: 1px dotted #ccc; margin-bottom: 30px;'>BOOKING THANH TOÁN</div></td></tr><tr><td colspan='2'><div style='font-weight: bold; text-transform: uppercase; color: #c50000; margin-bottom: 10px; font-size: 16px'>A. THÔNG TIN TOUR BOOKING :</div><div class='col-sm-3 ' style='position: relative' ><img src='{qr}' width='20%' ><div class='col-sm-5' style='position: absolute; width: 80%;'><table width='100%' style='font-size: 18px;margin-left: 170px;margin-bottom: 20px;margin-top: -135px;position: absolute'><tr><td style='font-weight:bold'>Mã Pincode:</td><td colspan='3'><a style='color: black;margin-right: 80px;'></a></td></tr><tr><td style='font-weight:bold'>Họ tên:</td><td>{fullname}</td></tr><tr><td style='font-weight:bold'>Số điện thoại:</td><td>{phone}</td></tr><tr><td style='font-weight:bold'>Tình trạng thanh toán:</td><td>{statuspayment}</td></tr><tr><td style='font-weight:bold'>Tổng tiền thanh toán:</td><td>{totalamount}</td></tr></table></div></div><hr></td></tr><tr><td colspan='2'>&nbsp;</td></tr><tr><td colspan='2' style='font-weight:bold;font-style: italic;text-align:center;color: #c50000'><div style='margin-bottom: 15px;color: rgb(201, 5, 5);'>Đây là email tự động Quý khách vui lòng không phản hồi vào email này</div></td></tr><tr><td colspan='2' style='height: 30px; text-align: center; padding-top: 15px; padding-bottom: 15px; color: #fff;'><div style='margin-bottom: 5px;'> 19 To ky, District 12, Ho Chi Minh City, Viet Nam</div><div><span>Điện thoại:</span> (+84 28) 38 22 8898 - <span>Fax:</span> (+84 28) 3829 9142 - <span>Email:</span> <a href='mailto:info@travelRover.com' style='color: white;'>info@travelRover.com</a></div></td></tr></table></div></body></html>";
                body = body.Replace("{pincode}", pincode);
                body = body.Replace("{fullname}", fullname);
                body =  body.Replace("{phone}", phone);
                body =  body.Replace("{statuspayment}", statuspayment);
                body =  body.Replace("{totalamount}", totalamount);
                body =  body.Replace("{qr}", qr);
                return (body);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string getHtmtFile()
        {
            try
            {

                string body = $@"<div style='max-width: 60vw; min-height:50%; background-color: whitesmoke; padding: 50px; border-radius:20px; margin: auto'><h1>EMAIL FROM TRAVELROVER</h1><h4>TravelRover Xin chào quý khách. Cảm ơn đã sử dụng dịch vụ của chúng tôi</h4><h5></h5><span>:</span>  <span style='font-size:32px ;' ><b></b></span></div>";

                return (body);


            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static void sendEmailUploadFile(string htmlString, string toEmail, string mailSubject, string emailSend, string keySecurity)
        {
            try
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(emailSend);
                    message.To.Add(new MailAddress(toEmail));
                    message.Subject = mailSubject;
                    message.IsBodyHtml = true; //to make message body as html  
                    message.Body = htmlString;
                    var attachment = new System.Net.Mail.Attachment("d:/Travel.txt");
                    message.Attachments.Add(attachment);
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Port = 587;
                        smtp.Host = "smtp.gmail.com"; //for gmail host  
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("professional8778781@gmail.com", keySecurity);
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Send(message);
                    }

                }


            }
            catch (Exception e)
            {

            }
        }
        public static string getHtmlBookingSuccess(string content, string subjectBody, string textHead)
        {
            try
            {

                string body = $@"<div style='max-width: 60vw; min-height:50%; background-color: whitesmoke; padding: 50px; border-radius:20px; margin: auto'><h1>EMAIL FROM TRAVELROVER</h1><h4>TravelRover. Xin chào quý khách. Cảm ơn đã sử dụng dịch vụ của chúng tôi</h4><h5>{subjectBody}</h5><hr><span>{textHead}:</span> <span style='font-size:32px ;' ><b>{content}</b></span></div>";
                return (body);


            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string getHtmlBookingCancel(string subjectBody, string textHead)
        {
            try
            {
                string body = $"{subjectBody} {textHead}";
                return (body);


            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string getHtmlBookingTicket(string content, string subjectBody, string textHead)
        {
            try
            {

                string body = $@"<div style='max-width: 60vw; min-height:50%; background-color: whitesmoke; padding: 50px; border-radius:20px; margin: auto'><h1>EMAIL FROM TRAVELROVER</h1><h4>TravelRover. Xin chào quý khách. Cảm ơn đã sử dụng dịch vụ của chúng tôi</h4><h5>{subjectBody}</h5><hr><span>{textHead}:</span> <span style='font-size:32px ;' ><b>{content}</b></span></div>";
                return (body);


            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string getHtmlApologize()
        {
            try
            {
                string body = $@"<div style='max-width: 60vw; min-height:50%; background-color: whitesmoke; padding: 50px; border-radius:20px; margin: auto'>Chúng tôi rất tiếc vì tour này đang gặp vấn đề! <br/> Nếu bạn đã thanh toán bạn vui lòng liên hệ số 0901306661 để thực hiện chức năng hoàn tiền </div>";
                return (body);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string getHtml(string content, string subjectBody, string textHead)
        {
            try
            {

                string body = $@"<div style='max-width: 60vw; min-height:50%; background-color: whitesmoke; padding: 50px; border-radius:20px; margin: auto'><h1>EMAIL FROM TRAVELROVER</h1><h4>TravelRover. Xin chào quý khách. Cảm ơn đã sử dụng dịch vụ của chúng tôi</h4><h5>{subjectBody}</h5><hr><span>{textHead}:</span> <span style='font-size:32px ;' ><b>{content}</b></span></div>";
                return (body);


            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static void sendEmail(string htmlString, string toEmail, string mailSubject, string emailSend, string keySecurity)
        {
            try
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(emailSend);
                    message.To.Add(new MailAddress(toEmail));
                    message.Subject = mailSubject;
                    message.IsBodyHtml = true; //to make message body as html  
                    message.Body = htmlString;

                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Port = 587;
                        smtp.Host = "smtp.gmail.com"; //for gmail host  
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("professional8778781@gmail.com", keySecurity);
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Send(message);
                    }

                }


            }
            catch (Exception e)
            {

            }
        }
        //public static void SendMail(UserModel userModel, string password, string url, ref NotificationModel _message)
        //{
        //    try
        //    {
        //        //var url = "http://localhost:4200/#/login";
        //        SmtpClient Client = new SmtpClient("smtp.gmail.com", 587);
        //        Client.UseDefaultCredentials = false;
        //        Client.EnableSsl = true;

        //        Client.Credentials = new NetworkCredential("phuongkiet850@gmail.com", "qrnqqbrjxedkblie");
        //        MailMessage Msg = new MailMessage();
        //        Msg.From = new MailAddress("phuongkiet850@gmail.com");
        //        Msg.To.Add(userModel.UserEmail);
        //        //Msg.Subject = "Mã xác nhận";
        //        Msg.IsBodyHtml = true;
        //        string html = "";
        //        if (!string.IsNullOrEmpty(password))
        //        {
        //            html = @"<!DOCTYPE html>
        //                            <html>
        //                            <meta name='viewport' content='width=device-width, initial-scale=1'>
        //                            <body>
        //                            <div class='w3-container'>
        //                              <div class='w3-card-4' style='width:70%'>
        //                                <header class='w3-container w3-light-grey'>
        //                                 <img src='https://localhost:44304//Uploads/image.png'alt='Avatar' class='w3-left w3-circle w3-margin-right' style='width:60px'> <h3>Highlands Coffee</h3>
        //                                </header>
        //                                <div class='w3-container'>
        //                                  <p>Xin chào " + userModel.UserFullName + " !</p>" +
        //                                 "<hr> <p>Đây là tin nhắn tự động mật khẩu của bạn là: " + password + " </p><br></div>" +
        //                                                       "<a href='" + url + "' class='w3-button w3-block w3-dark-grey'>Kích hoạt</a> </div></div></body>";
        //        }
        //        else if (string.IsNullOrEmpty(url))
        //        {
        //            html = @"<!DOCTYPE html>
        //                            <html>
        //                            <meta name='viewport' content='width=device-width, initial-scale=1'>
        //                            <body>
        //                            <div class='w3-container'>
        //                              <div class='w3-card-4' style='width:70%'>
        //                                <header class='w3-container w3-light-grey'>
        //                                  <img src='https://localhost:44304//Uploads/image.png' alt='Avatar' class='w3-left w3-circle w3-margin-right' style='width:60px'><h3>Highlands Coffee</h3>
        //                                </header>
        //                                <div class='w3-container'>
        //                                  <p>Xin chào !</p>""<hr> <p>Đây là tin nhắn tự động mật khẩu của bạn là: " + password + " </p><br></div></div></div></body>";
        //        }
        //        else
        //        {
        //            html = @"<!DOCTYPE html>
        //                            <html>
        //                            <meta name='viewport' content='width=device-width, initial-scale=1'>
        //                            <body>
        //                            <div class='w3-container'>
        //                              <div class='w3-card-4' style='width:70%'>
        //                                <header class='w3-container w3-light-grey'>
        //                                  <img src='https://localhost:44304//Uploads/image.png' alt='Avatar' class='w3-left w3-circle w3-margin-right' style='width:60px'><h3>Highlands Coffee</h3>
        //                                </header>
        //                                <div class='w3-container'>
        //                                  <p>Xin chào " + userModel.UserFullName + " !</p>" +
        //                                 "<hr> <a href='" + url + "' class='w3-button w3-block w3-dark-grey'>Kích hoạt</a> </div></div></body>";
        //        }
        //        Msg.Body = html;
        //        Client.Send(Msg);
        //    }
        //    catch (Exception e)
        //    {
        //        message.Messenge = "Có lỗi xảy ra khi gửi email !";
        //        message.Type = "Error";
        //        message.Description = e.Message;
        //        _message = message;
        //    }
        //}
        public static string Encryption(string password)
        {
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
            byte[] Encrypt;
            UTF8Encoding Encode = new UTF8Encoding();
            Encrypt = MD5.ComputeHash(Encode.GetBytes(password));
            StringBuilder Encryptdata = new StringBuilder();
            for (int i = 0; i < Encrypt.Length; i++)
            {
                Encryptdata.Append(Encrypt[i].ToString());
            }
            return Encryptdata.ToString();
        }



        #region Number Handle
        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
        public static bool IsNumber(this string Strnumber)
        {
            double n;
            return double.TryParse(Strnumber, out n);
        }
        #endregion
        public static long FormatDateToInt(DateTime datetime, string type)
        {
            // Type DDMMYYYY - day month year
            // Type YYYYMMDD - year month day
            // Type DDMMYYYYHHMM - day month year hour min
            // Type YYYYMMDDHHMM - year month day hour min
            // Type YYYYMMDDHHMMSS - year month day hour min sec
            // Type DDMMYYYYHHMMSS - day month year hour min sec
            var dateTimeNow = datetime;
            string year = dateTimeNow.Year.ToString();
            string month = dateTimeNow.Month.ToString();
            string day = dateTimeNow.Day.ToString();
            string hour = dateTimeNow.Hour.ToString();
            string mi = dateTimeNow.Minute.ToString();
            string sec = dateTimeNow.Second.ToString();
            try
            {
                if (type == "DDMMYYYY")
                {
                    //29/01/2001 
                    return int.Parse(year + month + day);
                }
                else if (type == "YYYYMMDD")
                {
                    //2001/01/29
                    return int.Parse(year + month + day);
                }
                else if (type == "YYYYMMDDHHMM")
                {
                    ////2001/01/29 23:12
                    return int.Parse(year + month + day + hour + mi);
                }
                else if (type == "DDMMYYYYHHMM")
                {
                    ////29/01/2001 23:12
                    return int.Parse(year + month + day + hour + mi);
                }
                else if (type == "YYYYMMDDHHMMSS")
                {
                    ////2001/01/29 23:12:12
                    return Int64.Parse(year + month + day + hour + mi + sec);
                }
                else
                {
                    ////29/01/2001 23:12:12
                    return Int64.Parse(year + month + day + hour + mi + sec);
                }
            }
            catch (Exception)
            {
                return 1;
            }
        }

        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

    }
}
