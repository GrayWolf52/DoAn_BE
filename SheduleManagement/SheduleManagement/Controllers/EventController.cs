﻿using Microsoft.AspNetCore.Mvc;
using SheduleManagement.Data;
using SheduleManagement.Data.Services;
using SheduleManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SheduleManagement.Controllers
{
    [Route("api/Event")]
    public class EventController : Controller
    {
        private readonly ScheduleManagementDbContext _dbContext;
        public EventController(ScheduleManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("{eventId}")]
        public IActionResult Index(int eventId)
        {
            try
            {
                var eventService = new EventService(_dbContext);
                var (msg, ev) = eventService.Get(eventId);
                if (msg.Length > 0) return BadRequest(msg);
                return Ok(new
                {
                    Id = ev.Id,
                    Title = ev.Title,
                    Description = ev.Description,
                    Place = ev.Place,
                    StartTime = ev.StartTime,
                    EndTime = ev.StartTime,
                    RecurrenceType = ev.RecurrenceType,
                    GroupId = ev.GroupId,
                    StatusEvent = ev.StatusEvent,
                    Participants = ev.EventUsers.Select(x => new
                    {
                        id = x.UserId,
                        username = x.Users.UserName,
                        firstname = x.Users.FirstName,
                        lastname = x.Users.LastName,
                        status = x.Status
                    }).ToList()
                }); ;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetByMonth")]
        public IActionResult GetByMonth(int userId, int groupId, int month, int year)
        {
            try
            {
                var eventService = new EventService(_dbContext);
                var (msg, events) = eventService.GetByMonth(userId, groupId, month, year);
                if (msg.Length > 0) return BadRequest(msg);
                return Ok(events.Select(x => new
                {
                    Id = x.Id,
                    Title = x.Title,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Description = x.Description,
                    Place = x.Place,
                    Creator = new
                    {
                        Id = x.Creator.Id,
                        UserName = x.Creator.UserName
                    },
                    RecurrenceType = x.RecurrenceType,
                    GropuId = x.GroupId,
                    StatusEvent = x.StatusEvent,

                    Status = (x.EventUsers == null || x.EventUsers.Count == 0) ? 2 : x.EventUsers[0].Status,

                    Participants = x.EventUsers.Select(y => new
                    {
                        id = y.UserId,
                        username = y.Users.UserName,
                        firstname = y.Users.FirstName,
                        lastname = y.Users.LastName,
                        status = y.Status
                    }).ToList()

                }).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate([FromBody]EventInfos model)
        {
            try
            {
                var eventService = new EventService(_dbContext);
                var (msg, eventId) = eventService.Update(model.id, model.title, model.description, model.place, model.startTime, model.endTime, model.recurrenceType, model.statusEvent, model.groupId, model.participants.Select(x => x.Id).ToList(), model.creator.Id);
                if (msg.Length > 0) return BadRequest(msg);
                return Ok(eventId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{eventId}")]
        public IActionResult Delete(int eventId)
        {
            try
            {
                var eventService = new EventService(_dbContext);
                var msg = eventService.Delete(eventId);
                if (msg.Length > 0) return BadRequest(msg);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("DeleteInvitaion/{eventId}")]
        public IActionResult DeleteInvitation(int eventId, int userId)
        {
            try
            {
                var eventUserService = new EventUserService(_dbContext);
                string msg = eventUserService.Delete(eventId, userId);
                if (msg.Length > 0) return BadRequest(msg);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("ReplyInvitation")]
        public IActionResult ReplyInvitation([FromBody]ReplyInvitationModel model)
        {
            try
            {
                var eventUserService = new EventUserService(_dbContext);
                string msg = eventUserService.ReplyInvitation(model.UserId, model.EventId, model.IsAccepted);
                if (string.IsNullOrEmpty(msg)) return Ok();
                else return BadRequest(msg);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                var eventService = new EventService(_dbContext);
                var (msg, events) = eventService.GetAll();
                if (msg.Length > 0) return BadRequest(msg);
                return Ok(events.Select(x => new
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    Place = x.Place,
                    CreateTime = x.CreatedTime,
                    CreateId = x.CreatorId,
                    StatusEvent = x.StatusEvent,
                }).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //[HttpGet("GetAllEventGroup")]
        //public IActionResult GetAllEventGroup(int groupId)
        //{
        //    try
        //    {
        //        var eventService = new EventService(_dbContext);
        //        var (msg, events) = eventService.GetEventGroup(groupId);
        //        if (msg.Length > 0) return BadRequest(msg);
        //        return Ok(events.Select(x => new
        //        {
        //            Id = x.Id,
        //            Title = x.Title,
        //            Description = x.Description,
        //            CreateTime = x.CreatedTime,
        //            CreateId = x.CreatorId,
        //        }).ToList());
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
    }
}
