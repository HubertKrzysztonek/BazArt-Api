﻿using Bazart.API.DTO;
using Bazart.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bazart.API.Services
{
    public interface IEventService
    {
        IEnumerable<EventDto> GetAllEvents();
        EventDto GetEventById([FromRoute] int id);
        int CreateNewEvent(CreateEventDto create);
        void RemoveEvent(int id);
        void UpdateEvent(int id, UpdateEventDto update);
    }
}