using System;
using System.Collections.Generic;
using PetID.ViewModels;
using Xamarin.Forms;
using System.Linq;
using System.Text;

namespace PetID.Pages
{
    public partial class ResultPage : ContentPage
    {
        private ResultViewModel model;

        public ResultPage(byte[] image)
        {
            InitializeComponent();
            model = new ResultViewModel { StatusText = "Uploading...", Image = image };
            this.BindingContext = model;

            btnTryAgain.Clicked += (sender, e) => {App.Current.MainPage.Navigation.PopModalAsync();};
        }

        protected override void OnAppearing()
        {
            Predict();
            base.OnAppearing();
        }

        private async void Predict()
        {
            model.StatusText = "Predicting...";
            var predictionService = DependencyService.Get<IPredictionService>();
            var predictions = await predictionService.Predict(model.Image);
            predictions = predictions.OrderByDescending(p => p.Prediction).ToList();

            var builder = new StringBuilder();
            foreach(var p in predictions)
            {
                builder.AppendLine($"{p.Tag}: {p.Prediction}");
            }

            var top = predictions.First().Tag;
            model.PredictionText = $"This is {top}.";
            model.StatusText = builder.ToString();
            btnTryAgain.IsVisible = true;
        }
    }
}
