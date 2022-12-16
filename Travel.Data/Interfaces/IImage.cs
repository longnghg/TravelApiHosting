using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models;
using Travel.Shared.ViewModels;

namespace Travel.Data.Interfaces
{
    public interface IImage
    {
        Response GetImageByIdTour(string idTour);
        Response GetImageByBanner(Guid idBanner);

        Response CreateImageTourDetail(ICollection<IFormFile> files, string idTour, string emailUser);

        Response DeleteImageTourDetail(ICollection<Image> images, string emailUser);
    }
}
