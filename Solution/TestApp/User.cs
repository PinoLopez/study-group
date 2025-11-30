using System.Collections.Generic;

namespace TestApp
{
    public class User
    {
        public User()
        {
            // Parameterless constructor for EF
            StudyGroups = new List<StudyGroup>();
        }

        public User(int userId, string name) : this()
        {
            Id = userId;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<StudyGroup> StudyGroups { get; set; }

        // Override Equals and GetHashCode for proper comparison
        public override bool Equals(object? obj)
        {
            return obj is User user && Id == user.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}