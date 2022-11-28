using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models;
using Travel.Shared.ViewModels;

namespace Travel.Data.Interfaces
{
    public interface IAuthentication
    {
        Employee EmpLogin(string email);
        Employee EmpLogin(string email, string password);
        bool EmpAddToken(string token, Guid idEmp);
        bool EmpActive(string email);
        bool EmpIsNew(string email);
        Response EmpDeleteToken(Guid idEmp);
        Customer CusLogin(string email);
        Customer CusLogin(string email, string password);
        Response CusDeleteToken(Guid idCus);
        bool CreateAccountGoogle(Customer cus);
        bool CusAddToken(string token, Guid idCus);
        bool CusAddTokenGoogle(string token, Guid idCus);

        Response CusChangePassword(Guid idCus, string password, string newPassword);
        Response CusForgotPassword(string email, string password);
        string Encryption(string password);

        Response EmpUnBlock(string email);
        Employee EmpCheckBlock(string email);
        Response EmpBlock(string email);

        Response CusUnBlock(string email);
        Customer CusCheckBlock(string email);
        Response CusBlock(string email);
        string RoleName(int roleId);

        Response EmpChangePassword(Guid idEmp, string password, string newPassword);
        Response EmpForgotPassword(string email, string password);
    }
}
