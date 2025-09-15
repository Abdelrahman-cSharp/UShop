﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UShop.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be 0 or greater")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock must be 0 or greater")]
        public int StockQuantity { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public int SellerId { get; set; }
        [ValidateNever]
        public Seller? Seller { get; set; }

        [Required]
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category? Category { get; set; }

        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
