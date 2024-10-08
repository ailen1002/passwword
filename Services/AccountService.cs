﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using password.Data;
using password.Interfaces;
using password.Models;

namespace password.Services
{
    public class AccountService(MainDbContext context) : IAccountService
    {
        // 通过构造函数注入数据库上下文，并确保数据库已创建

        // 添加新账户
        public void AddAccount(AccountInfo accountInfo)
        {
            ArgumentNullException.ThrowIfNull(accountInfo);

            context.AccountInfo.Add(accountInfo);
            context.SaveChanges();
        }
        // excel导入数据
        public void AddAccounts(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                var account = new AccountInfo
                {
                    AccountName = row["AccountName"].ToString() ?? throw new InvalidOperationException(),
                    Account = row["Account"].ToString() ?? throw new InvalidOperationException(),
                    Password = row["Password"].ToString() ?? throw new InvalidOperationException(),
                };

                context.AccountInfo.Add(account);
            }

            context.SaveChanges();
        }
        // 查询所有账户
        public List<AccountInfo> LoadAccounts()
        {
            return context.AccountInfo.ToList();
        }

        // 根据ID查询账户
        public AccountInfo GetAccountById(int accountId)
        {
            return context.AccountInfo.AsNoTracking().FirstOrDefault(a => a.Id == accountId) ?? throw new InvalidOperationException();
        }

        // 根据关键字查询账户（按账户名或账户字段查找）
        public List<AccountInfo> SearchAccounts(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) 
                return new List<AccountInfo>();

            return context.AccountInfo.AsNoTracking()
                .Where(a => EF.Functions.Like(a.AccountName, $"%{keyword}%") 
                            || EF.Functions.Like(a.Account, $"%{keyword}%"))
                .ToList();
        }

        // 更新现有账户
        public void UpdateAccount(AccountInfo? accountInfo)
        {
            ArgumentNullException.ThrowIfNull(accountInfo);

            var existingAccount = context.AccountInfo.Find(accountInfo.Id);
            // 更新账户信息
            if (existingAccount != null)
            {
                existingAccount.AccountName = accountInfo.AccountName;
                existingAccount.Account = accountInfo.Account;
                existingAccount.Password = accountInfo.Password;
            }
            else
            {
                MessageBoxManager.GetMessageBoxStandard("删除账户", "请选择一个账号", ButtonEnum.YesNo, Icon.Info);
            }

            context.SaveChanges();
        }

        // 删除账户
        public void DeleteAccount(int accountId)
        {
            var account = context.AccountInfo.Find(accountId);
            if (account != null)
            {
                context.AccountInfo.Remove(account);
            }
            else
            {
                MessageBoxManager.GetMessageBoxStandard("删除账户", "请选择一个账号", ButtonEnum.YesNo, Icon.Info);
            }
            context.SaveChanges();
        }
    }
}
