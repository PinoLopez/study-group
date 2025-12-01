using System.Collections.Generic;

namespace TestApp
{
    public class User
    {
        protected User() { }

        public User(int id, string name)
        {
            Id = id;
            Name = name;
            StudyGroups = new List<StudyGroup>();
        }

        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public List<StudyGroup> StudyGroups { get; private set; } = new List<StudyGroup>();
    }
}