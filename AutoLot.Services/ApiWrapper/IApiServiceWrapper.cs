using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoLot.Model.Entities;


namespace AutoLot.Services.ApiWrapper
{
    public interface IApiServiceWrapper
    {
        Task<IList<Car>> GetCarsAsync();
        Task<IList<Car>> GetCarsByMakeAsync(int id);
        Task<Car> GetCarAsync(int id);
        Task<Car> AddCarAsync(Car entity);
        Task<Car> UpdateCarAsync(int id, Car entity);
        Task DeleteCarAsync(int id, Car entity);
        Task<IList<Make>> GetMakesAsync();
    }
}
