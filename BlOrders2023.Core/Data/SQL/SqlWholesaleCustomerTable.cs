﻿using BlOrders2023.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Core.Data.SQL;

internal class SqlWholesaleCustomerTable : IWholesaleCustomerTable
{
    private readonly SqlBLOrdersDBContext _db;

    public SqlWholesaleCustomerTable(SqlBLOrdersDBContext dB)
    {
        _db = dB;
    }

    public Task DeleteAsync(WholesaleCustomer order)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<WholesaleCustomer>> GetIncludeInavtiveAsync(string query = null)
    {
        if (query.IsNullOrEmpty())
        {
            return await _db.Customers
                .ToListAsync();
        }
        else
        {
            return await _db.Customers
                .Where(c => c.CustomerName.Contains(query) ||
                            c.CustID.ToString().Contains(query))
                .ToListAsync();
        }
    }

    public IEnumerable<WholesaleCustomer> Get(string query = null)
    {
        if (query.IsNullOrEmpty())
        {
            return  _db.Customers.Where(c => c.Inactive != true)
                .ToList();
        }
        else
        {
            return  _db.Customers
                .Where(c => c.Inactive != true &&
                            (c.CustomerName.Contains(query) ||
                            c.CustID.ToString().Contains(query)))
                .ToList();
        }
    }

    public IEnumerable<WholesaleCustomer> Get(int customerID, bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return _db.Customers
                      .Where(c => c.CustID == customerID)
                      .AsNoTrackingWithIdentityResolution()
                      .ToList();
        }
        else
        {
            return _db.Customers
                      .Where(c => c.CustID == customerID)
                      .ToList();
        }

    }

    public async Task<IEnumerable<WholesaleCustomer>> GetAsync(string query = null)
    {
        if (query.IsNullOrEmpty())
        {
            return await _db.Customers.Where(c => c.Inactive != true).OrderBy(c => c.CustomerName)
                .ToListAsync();
        }
        else
        {
            return await _db.Customers
                .Where(c => c.Inactive != true && 
                            (c.CustomerName.Contains(query) || 
                            c.CustID.ToString().Contains(query)))
                .OrderBy(c => c.CustomerName)
                .ToListAsync();
        }
    }

    public async Task<IEnumerable<WholesaleCustomer>> GetAsync(int customerID, bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await _db.Customers.Include(c => c.Orders).Where(c => c.CustID == customerID).AsNoTrackingWithIdentityResolution().ToListAsync();
        }
        else
        {
            return await _db.Customers.Include(c => c.Orders).Where(c => c.CustID == customerID).ToListAsync();
        }
    }
        

    public async Task<CustomerClass> GetDefaultCustomerClassAsync() =>
       await _db.CustomerClasses.FirstAsync();

    public WholesaleCustomer Upsert(WholesaleCustomer customer, bool overwrite = false)
    {
        if(customer.Note != null)
        {
            if (_db.CustomerNotes.AsNoTracking().Where(n => n.CustId == customer.Note.CustId) == null)
            {
                _db.Add(customer.Note);
            }
            //else
            //{
            //    _db.Update(customer.Note);
            //}
        }

        _db.Update(customer);

        //TODO: may be a cleaner way of doing this but it works for now
        if (overwrite)
        {
            foreach (var entry in _db.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged))
            {
                entry.OriginalValues.SetValues(entry.GetDatabaseValues());
            }
        }

        
        var res = _db.SaveChanges();
        return customer;
    }

    public Task<WholesaleCustomer> UpsertAsync(WholesaleCustomer order)
    {
        throw new NotImplementedException();
    }
    public void Reload()
    {
        _db.ChangeTracker.Clear();
    }
}
