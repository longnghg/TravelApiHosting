using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models;
using Travel.Context.Models.Notification;
using Travel.Context.Models.Travel;
using Travel.Data.Interfaces.INotify;
using Travel.Shared.Ultilities;
using Travel.Shared.ViewModels;

namespace Travel.Data.Repositories.NotifyRes
{
    public class NotificationRes : INotification
    {
        private readonly TravelContext _db;
        private readonly NotificationContext _notifyContext;
        public NotificationRes(NotificationContext notifyContext, TravelContext db)
        {
            _db = db;
            _notifyContext = notifyContext;
        }

        public async Task<Response> Get(string idRole, Guid idEmp, bool IsSeen, int pageSize)
        {
            try
            {
                //var listByRole = (from x in _notifyContext.Notifications
                //            where x.RoleId.Contains(idRole)
                //            select x);
             

                var listByEmp = (from x in _notifyContext.Notifications
                                 where x.ReponseId == idEmp && x.RoleId.Contains(idRole)
                                 select x);
            
                //var list = listByRole.Concat(listByEmp).Distinct();

                var result = (from x in listByEmp
                              orderby x.Time descending
                              select x);

                var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString());

                var usSeen = await (from x in result
                                      where x.IsSeen == false
                                      select x).ToListAsync();

                if(IsSeen)
                {
                    
                    res.Content = usSeen.Take(pageSize).ToList();
                }
                else
                {
                    res.Content = result.Take(pageSize).ToList();
                }
                
                res.TotalResult = usSeen.Count;
                return res;
            }
            catch(Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra!", Enums.TypeCRUD.Error.ToString(), description:e.Message);
            }
        }

        public async Task<Response> UpdateIsSeen(Guid idNotification)
        {
            try
            {
                var  notification = await (from x in _notifyContext.Notifications
                            where   x.IsSeen == false &&
                                    x.IdNotification == idNotification 
                                    
                            select x).FirstOrDefaultAsync();

                if(notification != null)
                {
                    notification.IsSeen = true;
                    _notifyContext.SaveChanges();
                }
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString()); 
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra!", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public void CreateNotification(Guid idEmployee, int Type, string ContentRequest, int[] Roles, string Title)
        {
            try
            {
                var listByRole = (from x in _db.Employees
                                  where x.IsDelete == false && x.IsActive == true
                                  select x);
                var emp = new List<Employee>();
                foreach (var role in Roles)
                {
                    if (emp.Count == 0)
                    {
                        emp = (from x in listByRole
                               where x.RoleId == role
                               select x).ToList();
                    }
                    else
                    {
                        emp = emp.Concat((from x in listByRole
                                         where x.RoleId == role
                                          select x).ToList()).ToList();
                    }
                }

                //foreach (var item in split)
                //{
                //    listByRole = (from x in  _db.Employees
                //                  where x.RoleId = 
                //                  select x);
                //}
                var notifications = new List<Notifications>();
                foreach (var item in emp)
                {
                    if (idEmployee != item.IdEmployee)
                    {
                        Notifications notification = new Notifications();
                        notification.IdNotification = Guid.NewGuid();
                        notification.Time = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                        notification.IsSeen = false;
                        notification.Title = Title;
                        notification.Content = ContentRequest;
                        notification.Type = Type;
                        notification.RoleId = Ultility.ConvertListInt(Roles);
                        notification.RequestId = idEmployee;
                        notification.ReponseId = item.IdEmployee;
                        notifications.Add(notification);
                    }
                }

                _notifyContext.AddRange(notifications);
                _notifyContext.SaveChanges();
                
            }
            catch (Exception e)
            {
                
            }
        }

        public async Task<Response> Delete(Guid idNotification)
        {
            try
            {
                var notification = await (from x in _notifyContext.Notifications
                                          where x.IdNotification == idNotification
                                          select x).FirstOrDefaultAsync();

                if (notification != null)
                {
                    _notifyContext.Remove(notification);
                    _notifyContext.SaveChanges();
                }
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString());
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra!", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
    }
}
