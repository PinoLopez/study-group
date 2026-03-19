using System;
using System.Collections.Generic;

namespace TestApp
{
    public class User
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public List<StudyGroup> StudyGroups { get; private set; } = new();

        private User() { } // EF

        public User(int id, string name)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}