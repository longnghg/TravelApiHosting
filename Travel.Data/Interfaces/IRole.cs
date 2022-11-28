using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel;

namespace Travel.Data.Interfaces
{
    public interface IRole
    {
        string CheckBeforSave(JObject frmData, ref Notification _message, bool isUpdate);
        Response GetsRole(bool isDelete);
        Response SearchRole(JObject frmData);
        Response CreateRole(CreateRoleViewModel input);
        Response UpdateRole(UpdateRoleViewModel input);
        Response DeleteRole(int idRole);
        Response RestoreRole(int idRole);
    }
}
