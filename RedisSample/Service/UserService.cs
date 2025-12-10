using RedisSample.Context;
using RedisSample.Provider;
using Microsoft.EntityFrameworkCore;
using System;

namespace RedisSample.Service
{
    public class UserService
    {
        private readonly IRedisCacheProvider _cache;
        private readonly DataBaseContext _db;

        public UserService(IRedisCacheProvider cache, DataBaseContext db)
        {
            _cache = cache;
            _db = db;
        }
        private string Key(int id) => $"user:{id}";
        private string AllKey() => "users:all";

        public async Task<User?> GetUserAsync(int id)
        {
            string key = Key(id);

 
            var cached = await _cache.GetAsync<User>(key);
            if (cached != null)
                return cached;

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return null;

 
            await _cache.SetAsync(key, user, absoluteExpiration: TimeSpan.FromMinutes(10));

            return user;
        }

        public async Task<User> AddUserAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            await _cache.SetAsync(Key(user.Id), user, TimeSpan.FromMinutes(10));

            var allUsers = await _db.Users.ToListAsync();
            await _cache.SetAsync("users:all", allUsers, TimeSpan.FromMinutes(10));

            return user;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            string key = AllKey();

            var cached = await _cache.GetAsync<List<User>>(key);
            if (cached != null)
                return cached;

            var users = await _db.Users.ToListAsync();

            await _cache.SetAsync(key, users, absoluteExpiration: TimeSpan.FromMinutes(10));

            return users;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return false;

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();


            await _cache.RemoveAsync(Key(id));

         
            var allUsers = await _db.Users.ToListAsync();
            await _cache.SetAsync("users:all", allUsers, TimeSpan.FromMinutes(10));

            return true;
        }
    }
}
