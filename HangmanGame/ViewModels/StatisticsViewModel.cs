using HangmanGame.Data;
using HangmanGame.Models;
using System.Collections.ObjectModel;

namespace HangmanGame.ViewModels
{
    public class StatisticsViewModel : ViewModelBase
    {
        public ObservableCollection<Statistics> AllStatistics { get; set; }

        public StatisticsViewModel()
        {
            var statisticsData = new StatisticsData();
            var allStats = statisticsData.LoadAll();
            AllStatistics = new ObservableCollection<Statistics>(allStats);
        }
    }
}