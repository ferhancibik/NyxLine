using NyxLine.MAUI.Models;

namespace NyxLine.MAUI.ViewModels
{
    [QueryProperty(nameof(Star), "Star")]
    public class StarDetailViewModel : BindableObject
    {
        private Star _star;
        public Star Star
        {
            get => _star;
            set
            {
                _star = value;
                OnPropertyChanged();
                LoadStarProperties();
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        private string _mythologicalStory;
        public string MythologicalStory
        {
            get => _mythologicalStory;
            set
            {
                _mythologicalStory = value;
                OnPropertyChanged();
            }
        }

        private string _howToFind;
        public string HowToFind
        {
            get => _howToFind;
            set
            {
                _howToFind = value;
                OnPropertyChanged();
            }
        }

        private List<string> _interestingFacts;
        public List<string> InterestingFacts
        {
            get => _interestingFacts;
            set
            {
                _interestingFacts = value;
                OnPropertyChanged();
            }
        }

        private string _animatedImageUrl;
        public string AnimatedImageUrl
        {
            get => _animatedImageUrl;
            set
            {
                _animatedImageUrl = value;
                OnPropertyChanged();
            }
        }

        private void LoadStarProperties()
        {
            if (Star != null)
            {
                Name = Star.Name;
                Description = Star.Description;
                MythologicalStory = Star.MythologicalStory;
                HowToFind = Star.HowToFind;
                InterestingFacts = Star.InterestingFacts;
                AnimatedImageUrl = Star.AnimatedImageUrl;
            }
        }
    }
} 