﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models
{
    [Table("tbl_CustomerClasses")]
    public class CustomerClass
    {
        public CustomerClass() 
        {
            ID = 1;
            Class = "Wholesale";
            DiscountPercent = 0;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Column("CustomerClass")]
        public string Class { get; set; }
        public decimal DiscountPercent { get; set; }

        public override bool Equals(object? obj)
        {
            if(obj is CustomerClass c)
            {
                return c.ID== ID;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID);
        }

        public override string ToString()
        {
            return Class;
        }
    }
}
