using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using PetID.ViewModels;

namespace PetID
{
    public interface IPredictionService
    {
        Task<IList<PredictionViewModel>> Predict(byte[] image);      
    }
}
