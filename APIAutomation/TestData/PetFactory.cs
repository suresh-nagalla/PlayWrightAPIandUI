namespace APIAutomation.TestData
{
    using APIAutomation.Models;
    using System;
    using System.Collections.Generic;

    public static class PetFactory
    {
        private static readonly Random _random = new Random();

        public static Pet CreateRandomPet(string petName="")
        {
            return new Pet
            {
                Id = _random.Next(1, 1000000),
                Name = !string.IsNullOrEmpty(petName) ? petName : $"TestPet_{Guid.NewGuid():N}",
                Category = new Category
                {
                    Id = _random.Next(1, 100),
                    Name = GetRandomCategory()
                },
                PhotoUrls = new List<string>
                {
                    "https://example.com/photo1.jpg",
                    "https://example.com/photo2.jpg"
                },
                Tags = new List<Tag>
                {
                    new Tag { Id = _random.Next(1, 50), Name = "Tag1" },
                    new Tag { Id = _random.Next(51, 100), Name = "Tag2" }
                },
                Status = GetRandomStatus()
            };
        }

        public static Pet CreatePetWithStatus(string status)
        {
            var pet = CreateRandomPet();
            pet.Status = status;
            return pet;
        }

        private static string GetRandomCategory()
        {
            var categories = new[]
            {
                "Dog", "Cat", "Bird", "Fish", "Reptile"
            };

            return categories[_random.Next(categories.Length)];
        }

        public static string GetRandomStatus()
        {
            var statuses = new[]
            {
                "available", "pending", "sold"
            };

            return statuses[_random.Next(statuses.Length)];
        }
    }
}