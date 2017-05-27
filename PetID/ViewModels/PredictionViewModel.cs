using System;
using Xamarin.Forms;
namespace PetID.ViewModels
{
    public class PredictionViewModel : BindableObject
    {
        public string Tag { get; set; }
        public double Prediction { get; set; }
    }
}
