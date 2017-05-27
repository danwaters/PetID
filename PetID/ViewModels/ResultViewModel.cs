using System;
using Xamarin.Forms;
using System.Collections.Generic;

namespace PetID.ViewModels
{
    public class ResultViewModel : BindableObject
    {
        public ResultViewModel()
        {
        }

        public static BindableProperty StatusTextProperty = BindableProperty.Create("StatusText", typeof(string), typeof(ResultViewModel), "");
        public static BindableProperty PredictionTextProperty = BindableProperty.Create("PredictionText", typeof(string), typeof(ResultViewModel), "");

        public IList<PredictionViewModel> Predictions = new List<PredictionViewModel>();

        public byte[] Image
        {
            get; set;
        }

        public string StatusText 
        { 
            get
            {
                return (string)GetValue(StatusTextProperty);
            }

            set
            {
                SetValue(StatusTextProperty, value);
            } 
        }

        public string PredictionText
        {
            get
            {
                return (string)GetValue(PredictionTextProperty);
            }
            set
            {
                SetValue(PredictionTextProperty, value);
            }
        }
    }
}
