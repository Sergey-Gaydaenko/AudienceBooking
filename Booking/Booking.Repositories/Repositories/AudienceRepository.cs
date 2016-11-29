﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Enums;
using Booking.Models;
using Booking.Repositories.Interfaces;

namespace Booking.Repositories.Repositories
{
    public class AudienceRepository : IAudienceRepository
    {
        private readonly BookingDbContext _context;

        public AudienceRepository(BookingDbContext context)
        {
            _context = context;
        }

        public IQueryable<Audience> GetAllAudiences()
        {
            return _context.Audiences;
        }

        public Audience GetAudienceById(AudiencesEnum id)
        {
            return _context.Audiences.Find(id);
        }

        public void UpdateAudience(Audience audience)
        {
            _context.Entry(audience).State = EntityState.Modified;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        private bool _disposed;

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}