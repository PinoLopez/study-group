using System.Collections.Generic;

namespace TestApp
{
    public class User
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public List<StudyGroup> StudyGroups { get; private set; } = new List<StudyGroup>();

        // Parameterless for EF
        private User() { }

        public User(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}