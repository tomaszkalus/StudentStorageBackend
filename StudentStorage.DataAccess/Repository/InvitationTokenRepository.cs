using Microsoft.EntityFrameworkCore;
using StudentStorage.DataAccess.Data;
using StudentStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentStorage.DataAccess.Repository
{
    public class InvitationTokenRepository : Repository<InvitationToken>
    {
        private readonly ApplicationDbContext _db;
        public InvitationTokenRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<List<InvitationToken>> GetByEmail(string email)
        {
            return await _db.InvitationTokens.Where(t => t.Email == email).OrderBy(e => e.CreatedAt).ToListAsync();
        }

    }
}
