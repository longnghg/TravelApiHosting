using Newtonsoft.Json.Linq;
using PrUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models.Travel;
using Travel.Data.Interfaces;
using Travel.Shared.Ultilities;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel;
using Travel.Context.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using static Travel.Shared.ViewModels.Travel.CreateTimeLineViewModel;

namespace Travel.Data.Repositories
{
    public class TimeLineRes : ITimeLine
    {
        private readonly TravelContext _db;
        private Notification message;
        private Response res;

        public TimeLineRes(TravelContext db)
        {
            _db = db;
            message = new Notification();
            res = new Response();
        }

        public Response Create(ICollection<CreateTimeLineViewModel> input)
        {
            try
             {
                ICollection<Timeline> timeline = Mapper.MapCreateTimeline(input);
                _db.Timelines.AddRange(timeline.AsEnumerable());
                _db.SaveChanges();
                return Ultility.Responses("Tạo mới thành công !", Enums.TypeCRUD.Success.ToString());
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response Update(ICollection<UpdateTimeLineViewModel> input)
        {
            try
            {
                ICollection<Timeline> timeline = Mapper.MapUpdateTimeline(input);
 
                _db.Timelines.UpdateRange(timeline.AsEnumerable());
                _db.SaveChanges();
                return Ultility.Responses("Chỉnh sửa thành công !", Enums.TypeCRUD.Success.ToString());
            }
            catch(Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response Delete(ICollection<Timeline> timelines)
        {
            try
            {
                _db.Timelines.RemoveRange(timelines.AsEnumerable());
                _db.SaveChanges();
                return Ultility.Responses($"Xóa thành công !", Enums.TypeCRUD.Success.ToString());
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response Get()
        {
            try
            {
                var list = (from x in _db.Timelines where x.IsDelete == false select x).ToList();
                var result = Mapper.MapTimeLine(list);
                if (list.Count() > 0)
                {
                    res.Content = result;
                }
 
                return res;
            }
            catch (Exception e)
            {
                res.Notification.DateTime = DateTime.Now;
                res.Notification.Description = e.Message;
                res.Notification.Messenge = "Có lỗi xảy ra !";
                res.Notification.Type = "Error";
                return res;
            }
        }

        public Response GetTimelineByIdSchedule(string IdSchedule)
        {
            try
            {
                var timeline = (from x in _db.Timelines where x.IdSchedule == IdSchedule 
                                orderby x.FromTime
                                select x).ToList();
                var result = Mapper.MapTimeLine(timeline);
                if (result != null)
                {
                    res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                }
                return res;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        string ITimeLine.CheckBeforSave(JObject frmData, ref Notification _message, bool isUpdate)
        {
            var d = PrCommon.GetString("timeline", frmData);

            var timelines = JsonSerializer.Deserialize<List<Timeline>>(d);

            CreateTimeLineViewModel timeline = new CreateTimeLineViewModel();
            try
            {
                var description = PrCommon.GetString("description", frmData);
                if (string.IsNullOrEmpty(description))
                {
                }
                //fromTime ToTime idShecdule
                var fromtime = PrCommon.GetString("fromTime", frmData);
                if (string.IsNullOrEmpty(fromtime))
                {
                }

                var totime = PrCommon.GetString("toTime", frmData);
                if (string.IsNullOrEmpty(totime))
                {
                }
                var idschedule = PrCommon.GetString("idSchedule", frmData);
                if (string.IsNullOrEmpty(idschedule))
                {
                }
                if (isUpdate)
                {

                }

                CreateTimeLineViewModel obj = new CreateTimeLineViewModel();
                obj.Description = description;
                obj.FromTime = long.Parse(fromtime);
                obj.ToTime = long.Parse(totime);
                obj.IdSchedule = idschedule;
                return JsonSerializer.Serialize(obj);
            }
            catch (Exception e)
            {
                message.DateTime = DateTime.Now;
                message.Description = e.Message;
                message.Messenge = "Có lỗi xảy ra !";
                message.Type = "Error";
                _message = message;
                return null;
            }
        }

    }
}
