namespace NyxLine.MAUI.Models
{
    public class Star
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string MythologicalStory { get; set; }
        public string HowToFind { get; set; }
        public List<string> InterestingFacts { get; set; }
        public string ImageUrl { get; set; }
        public string AnimatedImageUrl { get; set; }
        public bool IsConstellation { get; set; }
        public double DistanceFromEarth { get; set; }
        public double Magnitude { get; set; }
        public string Constellation { get; set; }
        public string BestViewingSeason { get; set; }
        public string Icon { get; set; }
        public string Visibility { get; set; }
    }
} 