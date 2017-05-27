using System;
using System.Threading.Tasks;
using Microsoft.Cognitive.CustomVision;
using System.Collections.Generic;
using PetID.ViewModels;

[assembly: Xamarin.Forms.Dependency(typeof(PetID.iOS.PredictionService))]
namespace PetID.iOS
{
    public class PredictionService : IPredictionService
    {
        public PredictionService()
        {
        }

        public async Task<IList<PredictionViewModel>> Predict(byte[] image)
        {
            var trainingCredentials = new TrainingApiCredentials(APIKeys.TrainingAPIKey);
            var trainingApi = new TrainingApi(trainingCredentials);

            var predictionEndpointCredentials = new PredictionEndpointCredentials(APIKeys.PredictionAPIKey);
            var predictionEndpoint = new PredictionEndpoint(predictionEndpointCredentials);

            var projects = await trainingApi.GetProjectsAsync();

            var stream = new System.IO.MemoryStream(image);
            var results = await predictionEndpoint.PredictImageAsync(projects[0].Id, stream);

            var predictions = new List<PredictionViewModel>();
            foreach(var result in results.Predictions)
            {
                predictions.Add(new PredictionViewModel() { Tag = result.Tag, Prediction = result.Probability });  
            }

            return predictions;
        }
    }
}
