﻿using SheduleManagement.Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SheduleManagement.Data.Services
{
    public class GroupService
    {
        private readonly ScheduleManagementDbContext _dbContext;
        public GroupService(ScheduleManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public (string, int) Add(int userId, string name , string description)
        {
            try
            {
                using (var transac = _dbContext.Database.BeginTransaction())
                {
                    var group = new Groups
                    {
                        GroupName = name,
                        GroupDescription = description,
                        CreatorId = userId,
                        CreatedTime = DateTime.Now
                    };
                    _dbContext.Groups.Add(group);
                    _dbContext.SaveChanges();
                    var userGroupService = new UserGroupService(_dbContext);
                    string msg = userGroupService.Add(group.Id, new List<KeyValuePair<int, int>> { new KeyValuePair<int, int>(userId, 1) });
                    if (msg.Length > 0) return (msg, 0);
                    transac.Commit();

                    return (String.Empty, group.Id);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, 0);
            }
        }
        public string Delete(int groupId)
        {
            try
            {
                var group = _dbContext.Groups.Find(groupId);
                if (group == null)
                    return "Không tìm thấy nhóm tương ứng";
                _dbContext.Groups.Remove(group);
                _dbContext.SaveChanges();
                return String.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string ChangeName(int groupId, string name)
        {
            try
            {
                var group = _dbContext.Groups.Find(groupId);
                if (group == null)
                    return "Không tìm thấy nhóm tương ứng";
                group.GroupName = name;
                _dbContext.SaveChanges();
                return String.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public (string, Groups) GetGroupById(int groupId)
        {
            try
            {
                var gr = _dbContext.Groups.Find(groupId);
                if (gr == null) return ("Không tồn tại nhóm tương ứng.", null);
                else
                {
                    return ("", gr); 
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }
        public (string, List<Groups>) GetAll()
        {
            try
            {
                return (String.Empty, _dbContext.Groups.ToList());
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }
        public (string, Groups) GetById(int id)
        {
            try
            {
                return (String.Empty, _dbContext.Groups.Find(id));
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

    }
}
