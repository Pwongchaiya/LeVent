﻿// -------------------------------------------------
// Copyright (c) PiorSoft, LLC. All rights reserved.
// -------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KellermanSoftware.CompareNetObjects;
using LeVent.Models.Foundations.EventHandlerRegistrations;
using LeVent.Models.Foundations.EventHandlerRegistrations.Exceptions;
using LeVent.Models.Foundations.Events.Exceptions;
using LeVent.Services.Foundations.EventRegistrations;
using LeVent.Services.Foundations.Events;
using LeVent.Services.Processings.Events;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace LeVent.Tests.Unit.Services.Foundations.Events
{
    public partial class EventProcessingServiceTests
    {
        private readonly Mock<IEventService<object>> eventServiceMock;
        private readonly Mock<IEventHandlerRegistrationService<object>> eventHandlerRegistrationServiceMock;
        private readonly ICompareLogic compareLogic;
        private readonly IEventProcessingService<object> eventProcessingService;

        public EventProcessingServiceTests()
        {
            this.eventServiceMock =
                new Mock<IEventService<object>>();
            
            this.eventHandlerRegistrationServiceMock =
                new Mock<IEventHandlerRegistrationService<object>>();
            
            this.compareLogic = new CompareLogic();

            this.eventProcessingService = 
                new EventProcessingService<object>(
                    eventService: this.eventServiceMock.Object,
                    eventHandlerRegistrationService: this.eventHandlerRegistrationServiceMock.Object);
        }

        public static TheoryData DependencyValidationExceptions()
        {
            var nullEventHandlerRegistrationException =
                new NullEventHandlerRegistrationException();

            return new TheoryData<Exception>
            {
                new EventHandlerRegistrationValidationException(
                    nullEventHandlerRegistrationException)
            };
        }

        public static TheoryData DependencyExceptions()
        {
            var someInnerException =
                new Xeption();

            return new TheoryData<Exception>
            {
                new EventHandlerRegistrationServiceException(someInnerException)
            };
        }

        private Expression<Func<EventHandlerRegistration<object>, bool>> SameEventHandlerRegistrationAs(
            EventHandlerRegistration<object> expectedEventHandlerRegistration)
        {
            return actualEventHandlerRegistration =>
                this.compareLogic.Compare(expectedEventHandlerRegistration, actualEventHandlerRegistration)
                    .AreEqual;
        }

        private static List<Mock<Func<object, ValueTask>>> CreateRandomEventHandlerMocks()
        {
            int randomCount = GetRandomNumber();

            return Enumerable.Range(start: 0, count: randomCount)
                .Select(_ => new Mock<Func<object, ValueTask>>())
                    .ToList();
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();
    }
}
