using System.Collections.Generic;

namespace TestApp
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<StudyGroup> StudyGroups { get; set; } = new List<StudyGroup>();
    }
}