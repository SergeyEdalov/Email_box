using AutoMapper;
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
        string? _message;
        Guid _fromUserId;
        Guid _targetUserId;
        Mock<IMapper> _messageMockMapper;
        MessageService _messageService;

        [OneTimeSetUp]
        public void Init()
        {
            _fromUserId = new Guid("e6ad89aa-8885-4a7b-acf5-c3356ab09ea5");
            _targetUserId = new Guid("4530fc35-b32f-4da0-84a5-1baaf8f7fb38");
            _messageMockMapper = new Mock<IMapper>();
            _messageContext = getMContext();

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

            _messageService = new MessageService(_messageMockMapper.Object, _messageContext);
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            Destroy(_messageContext);
        }

        [Test]
        public async Task SendMessage_DefaultSuccsess()
        {
            // arrage
            _message = "Very well. Test is completed";
            //act

            await _messageService.SendMessageAsync(_message, _fromUserId, _targetUserId);
            var actual = _messageContext.Messages.Select(x => x.Body).LastOrDefault();

            //assert
            Assert.That(_message, Is.EqualTo(actual));
        }

        [Test]
        public void SendMessage_AwaitException_MessageIsNull()
        {
            // arrage
            _message = null;

            //act
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => 
                await _messageService.SendMessageAsync(_message, _fromUserId, _targetUserId));

            //assert
            Assert.That(ex.ParamName, Is.EqualTo("Message is empty"));
        }

        [Test]
        public async Task GetMessage_DefaultSuccsess()
        {
            // arrage
            var listMessage = _messageContext.Messages
                .Where(x => x.TargetUserId == _fromUserId && x.IsDelivery == true)
                .ToList();
            foreach (var item in listMessage)
            {
                item.IsDelivery = false;
                _messageContext.Update(item);
            }
            _messageContext.SaveChanges();

            var expected = new List<string> { "Hello", "How are you?" };

            //act
            var actual = await _messageService.GetMessageAsync(_fromUserId);

            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetMessage_AwaitException_ListMessageIsNull()
        {
            // arrage
            var listMessage = _messageContext.Messages
                .Where(x => x.TargetUserId == _fromUserId && x.IsDelivery == false)
                .ToList();
            foreach (var item in listMessage)
            {
                item.IsDelivery = true;
                _messageContext.Update(item);
            }
            _messageContext.SaveChanges();

            //act
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _messageService.GetMessageAsync(_fromUserId));

            //assert
            Assert.That(ex.Message, Is.EqualTo("There is no new message"));
        }
    }
}