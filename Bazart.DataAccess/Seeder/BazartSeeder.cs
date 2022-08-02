﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bazart.DataAccess.Data;
using Bazart.Models;

namespace Bazart.DataAccess.Seeder
{
    public class BazartSeeder
    {
        private readonly BazartDbContext _dbContext;

        public BazartSeeder(BazartDbContext dbContext)
        {
            _dbContext = dbContext;

        }

        public void Seeder()
        {
            if (_dbContext.Database.CanConnect())
            {
                if (!_dbContext.Products.Any())
                {
                    var products = CreateRandomProducts();
                    _dbContext.Products.AddRange(products);
                }
            }
        }

        private IEnumerable<Product> CreateRandomProducts()
        {
            var products = new List<Product>()
            {
                new Product()
                {
                    Name = Faker.Name.FullName(),
                    Description = Faker.Lorem.Sentence(),
                    Price = Faker.Finance.Coupon(),
                    Quantity = Faker.RandomNumber.Next(1, 10),
                    isForSale = Faker.Boolean.Random(),
                    ImageUrl = Faker.Name.First(),
                    CategoryId = Faker.RandomNumber.Next(1, 4),
                    Category = new Category()
                },
                new Product()
                {
                    Name = Faker.Name.FullName(),
                    Description = Faker.Lorem.Sentence(),
                    Price = Faker.Finance.Coupon(),
                    Quantity = Faker.RandomNumber.Next(1, 10),
                    isForSale = Faker.Boolean.Random(),
                    ImageUrl = Faker.Name.First(),
                    CategoryId = Faker.RandomNumber.Next(1, 4),
                    Category = new Category()
                }
            };
            return products;
        }
    }
}
