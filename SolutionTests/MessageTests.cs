using AutoMapper;
using Azure.Messaging;
using Message.Database.Context;
using Message.Database.DTO;
using Message.Database.Entity;
using Message.Services;
using Moq;
using SolutionTests.TestDbContext;

namespace SolutionTests
{
    [TestFixture]
    class MessageTests : TestCommandBase
    {
        string _message;
        Guid _fromUserId;
        Guid _targetUserId;
        Mock<IMapper> _messageMockMapper;

        [SetUp]
        public void SetUp()
        {
            _message = "AAAAAAA";
            _fromUserId = new Guid("e6ad89aa-8885-4a7b-acf5-c3356ab09ea5");
            _targetUserId = new Guid("4530fc35-b32f-4da0-84a5-1baaf8f7fb38");

            _messageContext = getMContext();

            _messageMockMapper = new Mock<IMapper>();

            _messageMockMapper.Setup(x => x.Map<MessageEntity>(It.IsAny<MessageDto>()))
                .Returns((MessageDto src) =>
                new MessageEntity()
                {
                    Id = src.Id,
                    Body = src.Body,
                    FromUserId = src.FromUserId,
                    TargetUserId = src.TargetUserId,
                    IsDelivery = src.IsDelivery
                });
            _messageMockMapper.Setup(x => x.Map<MessageDto>(It.IsAny<MessageEntity>()))
                .Returns((MessageEntity src) =>
                new MessageDto()
                {
                    Id = src.Id,
                    Body = src.Body,
                    FromUserId = src.FromUserId,
                    TargetUserId = src.TargetUserId,
                    IsDelivery = src.IsDelivery
                });
        }

        [Test]
        public async Task TestSendMessage()
        {
            // arrage
            var messageService = new MessageService(_messageMockMapper.Object, _messageContext);

            //act
            
            await messageService.SendMessageAsync(_message, _fromUserId, _targetUserId);

            //assert
            Assert.IsTrue(_messageContext.SaveChangesAsync().Result == 0);

            Destroy(_messageContext);

            //messageMock.Setup((x) => x.SendMessageAsync(It.IsAny<string>(), fromUserId, targetUserId))
            //    .Callback<string>(mess => captureMessage = mess);
            //Assert.That(captureMessage, Is.EqualTo("Hello"));    
            //s = messageMock.Setup((x) => x.SendMessageAsync(message, fromUserId, targetUserId));
            //Assert.IsNull(s);
        }

        [Test]
        public async Task TestGetMessage()
        {
            // arrage
            var messageService = new MessageService(_messageMockMapper.Object, _messageContext);
            var expected = new List<string> { "I`m fine" };
            //act
            var actual = await messageService.GetMessageAsync(_targetUserId);
            //assert
            Assert.IsNotNull(actual);
            Assert.That(actual, Is.EqualTo(expected));

            Destroy(_messageContext);
        }
    }
}