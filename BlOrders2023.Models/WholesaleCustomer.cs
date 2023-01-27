﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models
{
    [Table("tblCustomerWholesale")]
    public class WholesaleCustomer
    {
        #region Properties
        [Key]
        public int CustID { get; set; }
        public string CustomerName { get; set; } = "";
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string Phone { get; set; }
        public string? PhoneExt { get; set; }
        public string? Phone_2 { get; set; }
        public string? Phone2Ext { get; set; }
        public string? Buyer { get; set; }
        public string? LastPurchase { get; set; }
        public string? Fax { get; set; }
        // -1 is grocer 0 is gift
        //public short Grocer { get; set; }
        public bool Inactive { get; set; }
        public string? Email { get; set; }
        //[Column("Gift-Grocer")]
        //public string? Gift_Grocer { get; set; }
        public bool? SingleProdPerPallet { get; set; }
        public int? CustomerClassID { get; set; }
        public bool? isGrocer { get; set; }
        
        [ForeignKey("CustomerClassID")]
        public virtual CustomerClass CustomerClass
        {
            get { return _customerClass; }
            set
            {
                CustomerClassID = value.ID;
                _customerClass = value;
            }
        }

        public virtual List<Order> orders { get; set; } = new();
        #endregion Properties

        #region Fields
        private CustomerClass _customerClass;
        #endregion Fields

        #region Constructors
        public WholesaleCustomer()
        {
            CustomerName = "";
            Phone = "";
            isGrocer = true;
            Inactive = false;
            SingleProdPerPallet = false;
            CustomerClass = new();
        }
        #endregion Constructors

        #region Methods
        public string CityStateZip()
        {
            return String.Format("{0} {1}  {2}", City, State, ZipCode);
        }

        public string PhoneString()
        {
            if (PhoneExt != null)
                return String.Format("{0} x.{1}", Phone, PhoneExt);
            else
                return Phone;
        }

        public override string ToString()
        {
            return CustomerName;
        }
        #endregion Methods
    }
}
