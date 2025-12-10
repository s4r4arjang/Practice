using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using RedisSample.Context;
using RedisSample.Service;
using StackExchange.Redis;
using System;
using System.Text.Json;

namespace RedisSample.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public partial class RedisTestController : ControllerBase
    {
        private readonly IDatabase _db;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly UserService _se;

        public RedisTestController ( IConnectionMultiplexer redis , UserService se)
        {
            _db=redis.GetDatabase();
            _jsonOptions=new JsonSerializerOptions
            {
                PropertyNamingPolicy=JsonNamingPolicy.CamelCase,
                WriteIndented=true
            };
            _se = se;
        }

        public IConnectionMultiplexer Redis { get; }

        [HttpGet]
        public IActionResult Get ()
        {
            _db.StringSet("mykey", "salam redis");
            string value = _db.StringGet("mykey");
            return Ok(value);
        }


        [HttpPost("save")]
        public async Task<IActionResult> Save ( Person person )
        {

            string singleKey = $"person:{person.Id}";
            string personJson = JsonSerializer.Serialize(person, _jsonOptions);
            await _db.StringSetAsync(singleKey, personJson);



            string listKey = "person:list";

            // دریافت لیست قدیمی
            string listJson = await _db.StringGetAsync(listKey);

            List<Person> list;

            if (string.IsNullOrEmpty(listJson))
                list=new List<Person>();
            else
                list=JsonSerializer.Deserialize<List<Person>>(listJson, _jsonOptions);


            var existing = list.FirstOrDefault(x => x.Id==person.Id);
            if (existing!=null)
            {
                existing.Name=person.Name;
            }
            else
            {
                list.Add(person);
            }

            // ذخیره لیست جدید
            string updatedListJson = JsonSerializer.Serialize(list, _jsonOptions);
            await _db.StringSetAsync(listKey, updatedListJson);


            return Ok("Saved & added to list.");
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get ( int id )
        {
            string key = $"person:{id}";
            string json = await _db.StringGetAsync(key);

            if (string.IsNullOrEmpty(json))
                return NotFound("Not found");

            var person = JsonSerializer.Deserialize<Person>(json, _jsonOptions);

            return Ok(person);
        }


        [HttpPost("save-list")]
        public async Task<IActionResult> SaveList ( List<Person> people )
        {
            string key = "person:list";

            string json = JsonSerializer.Serialize(people, _jsonOptions);

            await _db.StringSetAsync(key, json);

            return Ok("List Saved");
        }


        [HttpGet("get-list")]
        public async Task<IActionResult> GetList ()
        {
            string key = "person:list";

            string json = await _db.StringGetAsync(key);

            if (string.IsNullOrEmpty(json))
                return Ok(new List<Person>());

            var list = JsonSerializer.Deserialize<List<Person>>(json, _jsonOptions);

            return Ok(list);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete ( int id )
        {
            string key = $"person:{id}";
            await _db.KeyDeleteAsync(key);

            return Ok("Deleted");
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _se.GetUserAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _se.GetAllUsersAsync();

            if (users == null || users.Count == 0)
                return NotFound("هیچ کاربری یافت نشد.");

            return Ok(users);
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            var created = await _se.AddUserAsync(user);
            return Ok(created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            bool removed = await _se.DeleteUserAsync(id);

            if (!removed)
                return NotFound();

            return Ok(true);
        }

    }
}
