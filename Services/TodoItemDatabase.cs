using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chat.Constants;
using Chat.Models;


namespace Chat.Services
{
  public  class TodoItemDatabase
    {
        // 定义数据库连接对象
        private SQLiteAsyncConnection? Database;

        // 初始化数据库连接
        async Task Init()
        {
            if (Database is not null)
                return;
            else
            // 创建一个新的 SQLiteAsyncConnection 连接到指定的数据库文件
            {
                Database = new SQLiteAsyncConnection(Constants.Constants.DatabasePath, Constants.Constants.Flags);
                // 创建 User 表格，如果表格不存在的话
                var result = await Database.CreateTableAsync<User>();
            }
        }
        // 获取所有用户列表
        public async Task<List<User>> GetItemsAsync()
        {
            await Init();
            if (Database is not null)
            {
                
                // 从数据库中获取所有 User 记录，并转换为 List 返回
            
                return await Database.Table<User>().ToListAsync();
            }
            return new List<User>();
            
        }
        // 根据电话号码获取单个用户
        public async Task<User> GetItemAsync(string? phone)
        {
            await Init();
            //if (Database is not null)
            //{
            //    // 从数据库中查询电话号码匹配的用户记录，并返回第一条记录
            //    return null;
            //}
            return await Database.Table<User>().Where(i => i.Phone == phone).FirstOrDefaultAsync();
        }
        // 保存或更新用户信息
        public async Task<int> SaveItemAsync(User item)
        {
            await Init();
            if (Database == null)
            {
                return -1;

            }
            User existingUser = await GetItemAsync(item.Phone);

            if (existingUser !=null)
            {
                return await Database.UpdateAsync(item);
            }
            else
            {
                return await Database.InsertAsync(item);
            }


        }
        // 删除用户信息
        public async Task<int> DeleteItemAsync(User item)
        {
            await Init();
            if (Database is not null)
            {
                // 从数据库中删除指定的用户记录
                return await Database.DeleteAsync(item);
            }
            else
            {
                return -1;
            }
        }

     
    }
}
