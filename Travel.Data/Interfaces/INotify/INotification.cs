﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Shared.ViewModels;

namespace Travel.Data.Interfaces.INotify
{
    public interface INotification
    {
        Task<Response> Get(string idRole, Guid idEmp, bool IsSeen, int pageSize);
        Task<Response> UpdateIsSeen(Guid idNotification);
        void CreateNotification(Guid idEmployee, int Type, string Content, int[] RoleId, string Name);
        Task<Response> Delete(Guid idNotification);
    }
}
